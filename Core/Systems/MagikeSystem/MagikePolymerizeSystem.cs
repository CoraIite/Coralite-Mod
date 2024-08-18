using Coralite.Content.Raritys;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader.Core;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem : ModSystem, ILocalizedModType
    {
        internal Dictionary<int, List<MagikeCraftRecipe>> magikeCraftRecipes;
        internal static FrozenDictionary<int, List<MagikeCraftRecipe>> MagikeCraftRecipes;

        private void RegisterPolymerize()
        {
            Mod Mod = Coralite.Instance;

            magikeCraftRecipes = [];

            foreach (var magPolymerize in from Type t in AssemblyManager.GetLoadableTypes(Mod.Code)//添加魔能聚合合成表
                                          where !t.IsAbstract && t.GetInterfaces().Contains(typeof(IMagikePolymerizable))
                                          let magPolymerize = Activator.CreateInstance(t) as IMagikePolymerizable
                                          select magPolymerize)
            {
                magPolymerize.AddMagikePolymerizeRecipe();
            }

            foreach (var magPolymerize in from mod in ModLoader.Mods
                                          where mod is ICoralite
                                          from Type t in AssemblyManager.GetLoadableTypes(mod.Code)//添加魔能聚合合成表
                                          where !t.IsAbstract && t.GetInterfaces().Contains(typeof(IMagikePolymerizable))
                                          let magPolymerize = Activator.CreateInstance(t) as IMagikePolymerizable
                                          select magPolymerize)
            {
                magPolymerize.AddMagikePolymerizeRecipe();
            }

            foreach (var recipes in magikeCraftRecipes)     //只是简单整理一下
            {
                recipes.Value.Sort((r1, r2) => r1.magikeCost.CompareTo(r2.magikeCost));
            }

            //使用性能更高的冻结字典
            MagikeCraftRecipes = magikeCraftRecipes.ToFrozenDictionary();
            magikeCraftRecipes = null;
        }

        public static bool TryGetMagikeCraftRecipes(int selfType, out List<MagikeCraftRecipe> recipes)
        {
            if (MagikeCraftRecipes != null && MagikeCraftRecipes.TryGetValue(selfType, out List<MagikeCraftRecipe> value))
            {
                recipes = value;
                return true;
            }

            recipes = null;
            return false;
        }
    }

    public class MagikeCraftRecipe
    {
        /// <summary>
        /// 主物品，以此物品为基础进行合成
        /// </summary>
        public required Item MainItem { get; set; }
        /// <summary>
        /// 结果物品，最后产出的物品
        /// </summary>
        public required Item ResultItem { get; set; }

        public int magikeCost;
        public int antiMagikeCost;

        private List<Condition> _conditions;
        public List<Condition> Conditions
        {
            get
            {
                _conditions ??= [];
                return _conditions;
            }
        }

        private List<Item> _items;
        public List<Item> RequiredItems
        {
            get
            {
                _items ??= [];
                return _items;
            }
        }

        public Action<Item, Item> onPolymerize;

        /// <summary>
        /// 检测是否能合成
        /// </summary>
        /// <returns></returns>
        public bool CanCraft()
        {
            if (_conditions == null)
                return true;

            foreach (var condition in Conditions)
                if (!condition.Predicate())
                    return false;

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="magikeCost"></param>
        /// <param name="resultItemStack"></param>
        /// <returns></returns>
        public static MagikeCraftRecipe CreateRecipe<TMainItem, TResultItem>(int magikeCost, int MainItenStack = 1, int resultItemStack = 1)
            where TMainItem : ModItem where TResultItem : ModItem
        {
            return new MagikeCraftRecipe()
            {
                MainItem = new(ModContent.ItemType<TMainItem>(), MainItenStack),
                ResultItem = new(ModContent.ItemType<TResultItem>(), resultItemStack),
                magikeCost = magikeCost
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="magikeCost"></param>
        /// <param name="resultItemStack"></param>
        /// <returns></returns>
        public static MagikeCraftRecipe CreateRecipe(int mainItemType, int resultItemType, int magikeCost, int MainItenStack = 1, int resultItemStack = 1)
        {
            return new MagikeCraftRecipe()
            {
                MainItem = new(mainItemType, MainItenStack),
                ResultItem = new(resultItemType, resultItemStack),
                magikeCost = magikeCost
            };
        }

        public MagikeCraftRecipe SetAntiMagikeCost(int antiMagikeCost)
        {
            magikeCost = 0;
            this.antiMagikeCost = antiMagikeCost;
            return this;
        }

        public MagikeCraftRecipe SetMagikeCost(int MagikeCost)
        {
            antiMagikeCost = 0;
            this.magikeCost = MagikeCost;
            return this;
        }

        public MagikeCraftRecipe AddIngredient(int itemID, int stack = 1)
        {
            RequiredItems.Add(new Item(itemID, stack));
            return this;
        }

        public MagikeCraftRecipe AddIngredient<T>(int stack = 1) where T : ModItem
        {
            RequiredItems.Add(new Item(ModContent.ItemType<T>(), stack));
            return this;
        }

        public MagikeCraftRecipe AddCondition(Condition condition)
        {
            Conditions.Add(condition);
            return this;
        }

        public void Register()
        {
            Dictionary<int, List<MagikeCraftRecipe>> MagikeCraftRecipes = ModContent.GetInstance<MagikeSystem>().magikeCraftRecipes;
            if (MagikeCraftRecipes == null)
                return;

            if (MagikeCraftRecipes.TryGetValue(MainItem.type, out List<MagikeCraftRecipe> value))
                value.Add(this);
            else
                MagikeCraftRecipes.Add(MainItem.type, [this]);

            AddVanillaRecipe();
        }

        private void AddVanillaRecipe()
        {
            Recipe recipe = Recipe.Create(ResultItem.type, ResultItem.stack);
            if (magikeCost > 0)
                recipe.AddIngredient<MagikePolySymbol>(magikeCost);

            recipe.AddIngredient(MainItem.type, MainItem.stack);

            if (_items != null)
                foreach (var item in RequiredItems)
                    recipe.AddIngredient(item.type, item.stack);
            if (_conditions != null)
                recipe.AddCondition(Conditions);

            recipe.DisableDecraft();
            recipe.Register();
        }
    }

    public class MagikePolySymbol : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + "MagikeSymbol";

        public override void SetDefaults()
        {
            Item.maxStack = int.MaxValue;
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
        }

        public override bool OnPickup(Player player) => false;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new(Mod, "Coralite: MagikeNeed", "魔能需求量：" + Item.stack)
            {
                OverrideColor = Coralite.MagicCrystalPink
            };

            tooltips.Add(line);
        }
    }
}
