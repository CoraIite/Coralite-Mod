using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class BaseFairyBottle : ModItem//, IFairyBottle
    {
        public override string Texture => AssetDirectory.FairyBottleItems + Name;

        /// <summary> 战斗仙灵物品数组 </summary>
        private Item[] fightFairies;
        /// <summary> 存储仙灵物品数组 </summary>
        private Item[] containFairies;

        /// <summary> 战斗仙灵的容量，默认3 </summary>
        public virtual int FightCapacity => 3;
        /// <summary> 捕捉仙灵的容量，默认30 </summary>
        public virtual int ContainCapacity => 30;

        /// <summary> 战斗仙灵，用于出战 </summary>
        public Item[] FightFairies { get => fightFairies; set => fightFairies = value; }
        /// <summary> 容纳的仙灵，捕捉后会抓到这里 </summary>
        public Item[] ContainFairies { get => containFairies; set => containFairies = value; }

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
            fightFairies = new Item[FightCapacity];
            for (int i = 0; i < FightCapacity; i++)
                fightFairies[i] = new Item();

            containFairies = new Item[ContainCapacity];
            for (int i = 0; i < ContainCapacity; i++)
                containFairies[i] = new Item();
        }

        //public override void UpdateInventory(Player player)
        //{
        //    int lifeRegan = 0;
        //    if (++lifeReganTime > FairyLifeReganSpacing)
        //    {
        //        lifeReganTime = 0;
        //        lifeRegan = FairyLifeRegan;
        //    }

        //    foreach (var fairy in fightFairies)
        //    {
        //        if (fairy.ModItem is BaseFairyItem fairyItem)
        //            fairyItem.LifeRegan(lifeRegan);
        //    }
        //}

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new(Mod, "FightCapacity",
                FairySystem.BottleFightCapacity.Format(fightFairies.Count(i => !i.IsAir), FightCapacity));
            tooltips.Add(line);
            
           line = new(Mod, "FightCapacity",
                FairySystem.BottleFightCapacity.Format(containFairies.Count(i => !i.IsAir), ContainCapacity));
            tooltips.Add(line);
        }

        public override void SaveData(TagCompound tag)
        {
            //存储战斗仙灵
            for (int i = 0; i < FightCapacity; i++)
                if (!fightFairies[i].IsAir)
                tag.Add("FightFairies" + i, fightFairies[i]);

            //存储仙灵
            for (int i = 0; i < ContainCapacity; i++)
                if (!containFairies[i].IsAir)
                    tag.Add("ContainFairies" + i, containFairies[i]);
        }

        public override void LoadData(TagCompound tag)
        {
            for (int i = 0; i < FightCapacity; i++)
            {
                if (tag.TryGet("FightFairies" + i, out Item fairy))
                    fightFairies[i] = fairy;
            }

            for (int i = 0; i < ContainCapacity; i++)
            {
                if (tag.TryGet("ContainFairies" + i, out Item fairy))
                    containFairies[i] = fairy;
            }
        }

        public override ModItem Clone(Item newEntity)
        {
            ModItem item = base.Clone(newEntity);

            if (item is BaseFairyBottle newBottle)
            {
                for (int i = 0; i < FightCapacity; i++)
                    newBottle.fightFairies[i] = fightFairies[i].Clone();
                for (int i = 0; i < ContainCapacity; i++)
                    newBottle.containFairies[i] = containFairies[i].Clone();
            }

            return item;
        }

        //public bool GetNextFairy

        //public bool ShootFairy(int index, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int damage, float knockback)
        //{
        //    return (fairies[index].ModItem as IFairyItem).ShootFairy(player, source, position, velocity, damage, knockback);
        //}
    }
}
