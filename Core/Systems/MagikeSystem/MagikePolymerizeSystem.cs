using Coralite.Content.Raritys;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem : ModSystem, ILocalizedModType
    {
        internal static Dictionary<int, List<PolymerizeRecipe>> polymerizeRecipes = new Dictionary<int, List<PolymerizeRecipe>>();

        private void RegisterPolymerize()
        {
            Mod Mod = Coralite.Instance;

            polymerizeRecipes=new Dictionary<int, List<PolymerizeRecipe>>();

            foreach (Type t in AssemblyManager.GetLoadableTypes(Mod.Code))  //添加魔能重塑合成表
            {
                if (!t.IsAbstract && t.GetInterfaces().Contains(typeof(IMagikePolymerizable)))
                {
                    IMagikePolymerizable magPolymerize = Activator.CreateInstance(t) as IMagikePolymerizable;
                    magPolymerize.AddMagikePolymerizeRecipe();
                }
            }

            foreach (var recipes in polymerizeRecipes)     //只是简单整理一下
            {
                recipes.Value.Sort((r1, r2) => r1.magikeCost.CompareTo(r2.magikeCost));
            }

            foreach (var list in polymerizeRecipes)
            {
                foreach (var polymerize in list.Value)
                {
                    Recipe recipe = Recipe.Create(polymerize.ResultItem.type, polymerize.ResultItem.stack);
                    recipe.AddIngredient<MagikePolySymbol>(polymerize.magikeCost);
                    recipe.AddIngredient(polymerize.selfType, polymerize.selfRequiredNumber);
                    foreach (var item in polymerize.RequiredItems)
                        recipe.AddIngredient(item.type, item.stack);

                    recipe.AddDecraftCondition(this.GetLocalization("DecraftCondition"), () => false);
                    recipe.Register();
                }
            }
        }

        public static void AddPolymerizeRecipes(PolymerizeRecipe recipe)
        {
            if (recipe.ResultItem != null && recipe.ResultItem.TryGetGlobalItem(out MagikeItem magikeItem))
            {
                magikeItem.magike_CraftRequired = recipe.magikeCost;
                magikeItem.stack_CraftRequired = recipe.selfRequiredNumber;
                magikeItem.condition = recipe.condition;
            }

            if (polymerizeRecipes == null)
                throw new Exception("合成表为null!");

            if (polymerizeRecipes.ContainsKey(recipe.selfType))
                polymerizeRecipes[recipe.selfType].Add(recipe);
            else
                polymerizeRecipes.Add(recipe.selfType, new List<PolymerizeRecipe> { recipe });
        }

        public static bool TryGetPolymerizeRecipes(int selfType, out List<PolymerizeRecipe> recipes)
        {
            if (polymerizeRecipes != null && polymerizeRecipes.ContainsKey(selfType))
            {
                recipes = polymerizeRecipes[selfType];
                return true;
            }

            recipes = null;
            return false;
        }
    }

    public class PolymerizeRecipe
    {
        public int selfType;
        public int selfRequiredNumber;
        public int magikeCost;
        public IMagikeCraftCondition condition;

        public Item MainItem;
        public Item ResultItem;

        public List<Item> RequiredItems;

        public Action<Item, Item> onPolymerize;

        /// <summary>
        /// 在之后请一定要调用<see cref="SetMainItem"/>
        /// </summary>
        /// <param name="resultItemType"></param>
        /// <param name="magikeCost"></param>
        /// <param name="resultItemStack"></param>
        /// <returns></returns>
        public static PolymerizeRecipe CreateRecipe(int resultItemType, int magikeCost, int resultItemStack = 1)
        {
            return new PolymerizeRecipe()
            {
                RequiredItems = new List<Item>(),
                ResultItem = new Item(resultItemType, resultItemStack),
                magikeCost = magikeCost
            };
        }

        /// <summary>
        /// 在之后请一定要调用<see cref="SetMainItem"/>
        /// </summary>
        /// <param name="magikeCost"></param>
        /// <param name="resultItemStack"></param>
        /// <returns></returns>
        public static PolymerizeRecipe CreateRecipe<T>( int magikeCost, int resultItemStack = 1)where T :ModItem
        {
            return new PolymerizeRecipe()
            {
                RequiredItems = new List<Item>(),
                ResultItem = new Item(ModContent.ItemType<T>(), resultItemStack),
                magikeCost = magikeCost
            };
        }

        public PolymerizeRecipe SetMainItem(int itemID, int stack = 1)
        {
            MainItem = new Item(itemID, stack);
            selfType = itemID;
            selfRequiredNumber = stack;
            return this;
        }

        public PolymerizeRecipe SetMainItem<T>( int stack = 1) where T : ModItem
        {
            MainItem = new Item(ModContent.ItemType<T>(), stack);
            selfType = ModContent.ItemType<T>();
            selfRequiredNumber = stack;
            return this;
        }

        public PolymerizeRecipe AddIngredient(int itemID, int stack = 1)
        {
            RequiredItems ??= new List<Item>();
            RequiredItems.Add(new Item(itemID, stack));
            return this;
        }

        public PolymerizeRecipe AddIngredient<T>( int stack = 1) where T : ModItem
        {
            RequiredItems ??= new List<Item>();
            RequiredItems.Add(new Item(ModContent.ItemType<T>(), stack));
            return this;
        }
        public PolymerizeRecipe AddCondition(IMagikeCraftCondition condition)
        {
            this.condition = condition;
            return this;
        }

        public void Register()
        {
            MagikeSystem.AddPolymerizeRecipes(this);
        }
    }

    public class MagikePolySymbol : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + "MagikeSymbol";

        public override void SetDefaults()
        {
            Item.maxStack = 999999;
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
        }

        public override bool OnPickup(Player player) => false;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(Mod, "Coralite: MagikeNeed", "魔能需求量：" + Item.stack);

            if (Item.stack < 300)
                line.OverrideColor = Coralite.Instance.MagicCrystalPink;
            else if (Item.stack < 1000)
                line.OverrideColor = Coralite.Instance.CrystallineMagikePurple;
            else if (Item.stack < 2_0000)
                line.OverrideColor = Coralite.Instance.SplendorMagicoreLightBlue;
            else
                line.OverrideColor = Color.Orange;

            tooltips.Add(line);
        }
    }

}
