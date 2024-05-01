using Coralite.Content.UI;
using Coralite.Core.Loaders;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class BaseFairyBottle : ModItem, IFairyBottle
    {
        public override string Texture => AssetDirectory.FairyBottleItems + Name;

        /// <summary>
        /// 仙灵物品数组
        /// </summary>
        private readonly Item[] fairies;

        /// <summary>
        /// 仙灵瓶的容量，默认10
        /// </summary>
        public virtual int Capacity => 10;

        public Item[] Fairies => fairies;

        /// <summary>
        /// 每次回血回多少，默认1
        /// </summary>
        public virtual int FairyLifeRegan => 1;
        /// <summary>
        /// 多少帧回一次血，默认1秒
        /// </summary>
        public virtual int FairyLifeReganSpacing => 60;

        private int lifeReganTime;

        public BaseFairyBottle()
        {
            fairies = new Item[Capacity];
            for (int i = 0; i < Capacity; i++)
                fairies[i] = new Item();
        }

        public override void UpdateInventory(Player player)
        {
            int lifeRegan = 0;
            if (++lifeReganTime > FairyLifeReganSpacing)
            {
                lifeReganTime = 0;
                lifeRegan = FairyLifeRegan;
            }

            foreach (var fairy in fairies)
            {
                if (fairy.ModItem is BaseFairyItem fairyItem)
                    fairyItem.LifeRegan(lifeRegan);
            }
        }

        public override bool CanRightClick() => true;
        public override bool ConsumeItem(Player player) => false;

        public override void RightClick(Player player)
        {
            UILoader.GetUIState<FairyBottleUI>().ShowUI(this);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(Mod, "Capacity",
                FairySystem.BottleCapacity.Format(fairies.Count(i => !i.IsAir), Capacity));
        }

        public override void SaveData(TagCompound tag)
        {
            for (int i = 0; i < Capacity; i++)
                tag.Add("Fairies" + i, fairies[i]);
        }

        public override void LoadData(TagCompound tag)
        {
            for (int i = 0; i < Capacity; i++)
            {
                if (tag.TryGet("Fairies" + i, out Item fairy))
                    fairies[i] = fairy;
                else
                    fairies[i] = new Item();
            }
        }
    }

    public interface IFairyBottle
    {
        int Capacity { get; }

        public Item[] Fairies { get; }
    }
}
