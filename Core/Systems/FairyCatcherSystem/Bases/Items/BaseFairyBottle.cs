using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases.Items
{
    public abstract class BaseFairyBottle : ModItem
    {
        public override string Texture => AssetDirectory.FairyBottleItems + Name;

        /// <summary> 战斗仙灵物品数组 </summary>
        private Item[] fightFairies;
        /// <summary> 存储仙灵物品数组 </summary>
        private Item[] containFairies;

        /// <summary> 战斗仙灵的容量，默认3 </summary>
        public virtual int FightCapacity => 3;
        /// <summary> 捕捉仙灵的容量，默认30 </summary>
        public virtual int ContainCapacity => 20;

        /// <summary> 战斗仙灵，用于出战 </summary>
        public Item[] FightFairies { get => fightFairies; set => fightFairies = value; }
        /// <summary> 容纳的仙灵，捕捉后会抓到这里 </summary>
        public Item[] ContainFairies { get => containFairies; set => containFairies = value; }

        /// <summary>
        /// 每次回血回多少，默认1
        /// </summary>
        public virtual (float, int) FairyHeal => (0.00_5f, 5);
        /// <summary>
        /// 多少帧回一次血，默认1秒
        /// </summary>
        public virtual int FairyHealSpacing => 60 * 3;

        private int _healTime;

        public BaseFairyBottle()
        {
            fightFairies = new Item[FightCapacity];
            for (int i = 0; i < FightCapacity; i++)
                fightFairies[i] = new Item();

            containFairies = new Item[ContainCapacity];
            for (int i = 0; i < ContainCapacity; i++)
                containFairies[i] = new Item();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new(Mod, "HealValue",
                FairySystem.BottleHeal.Format(Math.Round(FairyHealSpacing / 60f, 1), FairyHeal.Item1 * 100, FairyHeal.Item2));
            tooltips.Add(line);

            line = new(Mod, "FightCapacity",
                FairySystem.BottleFightCapacity.Format(fightFairies.Count(i => !i.IsAir), FightCapacity));
            tooltips.Add(line);

            line = new(Mod, "ContainCapacity",
                 FairySystem.BottleContainCapacity.Format(containFairies.Count(i => !i.IsAir), ContainCapacity));
            tooltips.Add(line);
        }

        /// <summary>
        /// 每帧更新仙灵瓶中的仙灵
        /// </summary>
        public virtual void UpdateBottle(Player player)
        {
            bool heal = false;
            if (++_healTime > FairyHealSpacing)
            {
                heal = true;
                _healTime = 0;
            }

            foreach (var item in fightFairies)
            {
                if (!item.IsAir && item.ModItem is BaseFairyItem bfi)
                {
                    bfi.UpdateInBottle_Inner(this, player);
                    if (heal)
                    {
                        bfi.HealFairy(FairyHeal.Item1, FairyHeal.Item2);
                    }
                }
            }
        }

        public virtual void OnFairyBack(Player player, BaseFairyItem fairyItem) { }

        public sealed override void UpdateInventory(Player player)
        {
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
            {
                if (!fcp.UnlockFairyCatcherUse)
                {
                    fcp.UnlockFairyCatcherUse = true;
                    //TODO: 解锁珊瑚笔记内容
                }
            }
        }

        #region 存入取出部分

        /// <summary>
        /// 在仙灵瓶放入UI的时候调用，初始化仙灵物品内的值
        /// </summary>
        public virtual void OnBottleActive()
        {
            foreach (var item in fightFairies)
                if (!item.IsAir && item.ModItem is BaseFairyItem bfi)
                    bfi.OnBottleActive();
        }

        /// <summary>
        /// 在仙灵瓶取出UI的时候调用，清除弹幕
        /// </summary>
        /// <param name="player"></param>
        public virtual void OnBottleInactive(Player player)
        {
            foreach (var item in fightFairies)
                if (!item.IsAir && item.ModItem is BaseFairyItem bfi)
                    bfi.OnBottleInactive(player);
        }


        #endregion

        #region 获取瓶中仙灵部分

        /// <summary>
        /// 遍历仙灵瓶，设置<see cref="FairyCatcherPlayer.CurrentFairyIndex"/>
        /// </summary>
        /// <param name="p"></param>
        public void SetIndex(Player p)
        {
            if (!p.TryGetModPlayer(out FairyCatcherPlayer fcp))
                return;

            int currentIndex = fcp.CurrentFairyIndex;
            if (currentIndex == -1)//索引-1直接从头遍历到尾
            {
                //遍历，找到下一个存活且未出动的仙灵
                for (int i = 0; i < FightCapacity; i++)
                {
                    Item item = fightFairies[i];
                    if (!item.IsAir && item.ModItem is BaseFairyItem fairyItem && !fairyItem.IsDead && !fairyItem.IsOut)
                    {
                        currentIndex = i;
                        break;
                    }
                }

                fcp.CurrentFairyIndex = currentIndex;
            }
            else
            {
                bool find = false;
                for (int i = 0; i < FightCapacity; i++)
                {
                    Item item = fightFairies[currentIndex];
                    if (!item.IsAir && item.ModItem is BaseFairyItem fairyItem && !fairyItem.IsDead && !fairyItem.IsOut)
                    {
                        find = true;
                        break;
                    }

                    currentIndex++;
                    if (currentIndex > FightCapacity - 1)
                        currentIndex = 0;
                }
                if (find)
                    fcp.CurrentFairyIndex = currentIndex;
                else
                    fcp.CurrentFairyIndex = -1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="bfi"></param>
        /// <returns></returns>
        public bool GetShootableFairy(Player p, out BaseFairyItem bfi)
        {
            bfi = null;
            if (!p.TryGetModPlayer(out FairyCatcherPlayer fcp))
                return false;

            SetIndex(p);
            int currentIndex = fcp.CurrentFairyIndex;

            //遍历并返回仙灵
            for (int i = 0; i < FightCapacity; i++)
            {
                Item item = fightFairies[currentIndex];
                if (!item.IsAir && item.ModItem is BaseFairyItem fairyItem && !fairyItem.IsDead && !fairyItem.IsOut)
                {
                    bfi = fairyItem;
                    return true;
                }

                currentIndex++;
                if (currentIndex > FightCapacity - 1)
                    currentIndex = 0;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public IEnumerable<BaseFairyItem> GetShootableFairy(Player p)
        {
            if (!p.TryGetModPlayer(out FairyCatcherPlayer fcp))
                yield break;

            SetIndex(p);

            //索引-1说明没一个可用的仙灵，直接返回
            if (fcp.CurrentFairyIndex == -1)
                yield break;

            int currentIndex = fcp.CurrentFairyIndex;

            //遍历并返回仙灵
            for (int i = 0; i < FightCapacity; i++)
            {
                Item item = fightFairies[currentIndex];
                if (!item.IsAir && item.ModItem is BaseFairyItem fairyItem && !fairyItem.IsDead && !fairyItem.IsOut)
                    yield return fairyItem;

                currentIndex++;
                if (currentIndex > FightCapacity - 1)
                    currentIndex = 0;
            }
        }

        /// <summary>
        /// 向仙灵瓶内加入物品，用于自动拾取
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool AddItem(Item item)
        {
            for (int i = 0; i < ContainCapacity; i++)
            {
                Item cItem = containFairies[i];
                if (cItem.IsAir)
                {
                    containFairies[i] = item.Clone();
                    item.TurnToAir();
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region 整理部分

        public enum SortStyle
        {
            /// <summary> 根据稀有度排序 </summary>
            ByRarity,
            /// <summary> 根据最大生命值等级排序 </summary>
            ByLifeMaxLevel,
            /// <summary> 根据伤害等级排序 </summary>
            ByDamageLevel,
            /// <summary> 根据防御力等级排序 </summary>
            ByDefenceLevel,
            /// <summary> 根据速度等级排序 </summary>
            BySpeedLevel,
            /// <summary> 根据技能等级排序 </summary>
            BySkillLevelLevel,
            /// <summary> 根据耐力等级排序 </summary>
            ByStaminaLevel,
        }

        public void Sort(SortStyle style)
        {
            //拿出来
            List<Item> temp = new List<Item>();
            for (int i = 0; i < ContainCapacity; i++)
            {
                Item item = containFairies[i];
                if (!item.IsAir)
                    temp.Add(item);
            }

            if (temp.Count == 0)
                return;

            //整理
            Comparison<Item> compare = style switch
            {
                SortStyle.ByLifeMaxLevel => LifeMaxSort,
                SortStyle.ByDamageLevel => DamageSort,
                SortStyle.ByDefenceLevel => DefenceSort,
                SortStyle.BySpeedLevel => SpeedSort,
                SortStyle.BySkillLevelLevel => SkillLevelSort,
                SortStyle.ByStaminaLevel => StaminaSort,
                _ => RaritySort,
            };

            temp.Sort(compare);

            //重新存回去
            int count = 0;
            for (int i = temp.Count - 1; i > -1; i--)
            {
                containFairies[count] = temp[i];
                count++;
            }

            for (int i = count; i < ContainCapacity - 1; i++)
            {
                containFairies[i] = new Item();
            }
        }

        private int RaritySort(Item i1, Item i2)
        {
            if (i1.ModItem is BaseFairyItem f1 && i2.ModItem is BaseFairyItem f2)
            {
                return f1.Rarity.CompareTo(f2.Rarity);
            }

            return 0;
        }

        private int LifeMaxSort(Item i1, Item i2)
        {
            if (i1.ModItem is BaseFairyItem f1 && i2.ModItem is BaseFairyItem f2)
            {
                return f1.FairyIV.LifeMaxLV.CompareTo(f2.FairyIV.LifeMaxLV);
            }

            return 0;
        }

        private int DamageSort(Item i1, Item i2)
        {
            if (i1.ModItem is BaseFairyItem f1 && i2.ModItem is BaseFairyItem f2)
            {
                return f1.FairyIV.DamageLV.CompareTo(f2.FairyIV.DamageLV);
            }

            return 0;
        }

        private int DefenceSort(Item i1, Item i2)
        {
            if (i1.ModItem is BaseFairyItem f1 && i2.ModItem is BaseFairyItem f2)
            {
                return f1.FairyIV.DefenceLV.CompareTo(f2.FairyIV.DefenceLV);
            }

            return 0;
        }

        private int SpeedSort(Item i1, Item i2)
        {
            if (i1.ModItem is BaseFairyItem f1 && i2.ModItem is BaseFairyItem f2)
            {
                return f1.FairyIV.SpeedLV.CompareTo(f2.FairyIV.SpeedLV);
            }

            return 0;
        }

        private int SkillLevelSort(Item i1, Item i2)
        {
            if (i1.ModItem is BaseFairyItem f1 && i2.ModItem is BaseFairyItem f2)
            {
                return f1.FairyIV.SkillLevelLV.CompareTo(f2.FairyIV.SkillLevelLV);
            }

            return 0;
        }

        private int StaminaSort(Item i1, Item i2)
        {
            if (i1.ModItem is BaseFairyItem f1 && i2.ModItem is BaseFairyItem f2)
            {
                return f1.FairyIV.StaminaLV.CompareTo(f2.FairyIV.StaminaLV);
            }

            return 0;
        }

        #endregion

        #region IO

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

        #endregion
    }
}
