using Coralite.Content.UI;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class MagikeFactory_RemodelPool : MagikeFactory, ISingleItemContainer
    {
        public float itemScale = 1;
        public float itemAlpha = 1;
        public Item containsItem = new Item();
        public RemodelRecipe chooseRecipe;

        public abstract Color MainColor { get; }
        public Item ContainsItem { get => containsItem; set => containsItem = value; }

        public MagikeFactory_RemodelPool(int magikeMax, int workTimeMax) : base(magikeMax, workTimeMax) { }

        public override bool StartWork()
        {
            if (containsItem is not null && !containsItem.IsAir &&
                chooseRecipe is not null && chooseRecipe.CanRemodel(containsItem,magike, containsItem.type, containsItem.stack))
                return base.StartWork();

            return false;
        }

        public override void DuringWork()
        {
            float factor = workTimer / (float)workTimeMax;

            itemScale = Helper.Lerp(1, 0.6f, factor);
            itemAlpha = Helper.Lerp(1, 0, factor);

            Vector2 center = Position.ToWorldCoordinates(24, -16);
            if (workTimer % 5 == 0)
            {
                Dust dust = Dust.NewDustPerfect(center + new Vector2(Main.rand.Next(-16, 16), 32), DustID.FireworksRGB, -Vector2.UnitY * Main.rand.NextFloat(0.8f, 3f), newColor: MainColor);
                dust.noGravity = true;

                //CrossLight.Spawn(center + new Vector2(Main.rand.Next(-16, 16), 32), -Vector2.UnitY * Main.rand.NextFloat(0.8f, 1.1f), 0, 30, new Vector2(0.5f, 1.5f), MainColor);
            }

            float width = 24 - factor * 22;
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustPerfect(center + Main.rand.NextVector2CircularEdge(width, width), DustID.LastPrism, Vector2.Zero, newColor: MainColor);
                dust.noGravity = true;
            }
        }

        public override void WorkFinish()
        {
            itemScale = 1;
            itemAlpha = 1;
            if (containsItem is not null && !containsItem.IsAir &&
                chooseRecipe is not null && chooseRecipe.CanRemodel(containsItem,magike, containsItem.type, containsItem.stack))
            {
                Charge(-chooseRecipe.magikeCost);

                Vector2 position = Position.ToWorldCoordinates(24, -8);

                Item item = chooseRecipe.resultItem.Clone();
                if (item.TryGetGlobalItem(out MagikeItem magikeItem))   //把不必要的东西删掉
                {
                    magikeItem.magike_CraftRequired = -1;
                    magikeItem.stack_CraftRequired = 0;
                    magikeItem.condition = null;
                }

                int index=  Item.NewItem(new EntitySource_TileEntity(this), position, item);    //生成掉落物
                chooseRecipe.onRemodel?.Invoke(containsItem, Main.item[index]); //触发OnRemodel

                containsItem.stack -= chooseRecipe.selfRequiredNumber;  //消耗原物品
                if (containsItem.stack < 1)
                    containsItem.TurnToAir();

                SoundEngine.PlaySound(CoraliteSoundID.ManaCrystal_Item29, position);
                MagikeHelper.SpawnDustOnGenerate(3, 2, Position+new Point16(0,-2), MainColor);
            }
        }

        public override void OnKill()
        {
            MagikeRemodelUI.visible = false;
            UILoader.GetUIState<MagikeRemodelUI>().Recalculate();

            if (!containsItem.IsAir)
                Item.NewItem(new EntitySource_TileEntity(this), Position.ToWorldCoordinates(24, -8), containsItem);
        }

        public virtual bool CanInsertItem(Item item)
        {
            return true;
        }

        public virtual Item GetItem()
        {
            return containsItem;
        }

        public virtual bool InsertItem(Item item)
        {
            containsItem = item;
            return true;
        }

        public bool CanGetItem()
        {
            return workTimer == -1;
        }

        bool ISingleItemContainer.TryOutputItem(Func<bool> rule, out Item item)
        {
            throw new NotImplementedException();
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            if (!containsItem.IsAir)
                tag.Add("containsItem", containsItem);

            if (chooseRecipe != null)
            {
                tag.Add("RecipeSelfItem", chooseRecipe.selfItem);
                tag.Add("RecipeResultItem", chooseRecipe.resultItem);
            }
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            if (tag.TryGet("containsItem", out Item item))
                containsItem = item;

            if (tag.TryGet("RecipeSelfItem", out Item mainItem) && tag.TryGet("RecipeResultItem", out Item resultItem))
                if ( MagikeSystem.TryGetRemodelRecipes(mainItem.type, out List<RemodelRecipe> recipes))
                    chooseRecipe = recipes.FirstOrDefault(p => p.resultItem.type == resultItem.type && p.resultItem.stack == resultItem.stack, null);
        }
    }
}
