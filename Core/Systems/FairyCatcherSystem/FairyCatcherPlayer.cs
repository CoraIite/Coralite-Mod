using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader.IO;
using static System.Net.WebRequestMethods;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public class FairyCatcherPlayer : ModPlayer
    {
        public struct FairyIVRandom
        {
            /// <summary> 基础值，可以说是中心点 </summary>
            public float BaseValue;
            /// <summary> 基础值向下浮动的范围 </summary>
            public float Sub;
            /// <summary> 基础值向上浮动的范围 </summary>
            public float Add;

            /// <summary> 获取随机的值 </summary>
            public readonly float RandValue => Main.rand.NextFloat(Math.Clamp(BaseValue - Sub, 0, float.MaxValue), BaseValue + Add);

            public readonly float MinValue => BaseValue - Sub;
            public readonly float MaxValue => BaseValue + Add;

            public void Reset()
            {
                BaseValue= FairyIVLevelID.Common;
                Sub = 2;//默认能降到0也就是弱小
                Add = 1;//默认增加到3也就是不寻常
            }
        }

        /// <summary> 仙灵捕捉力增幅 </summary>
        public StatModifier fairyCatchPowerBonus;
        /// <summary>
        /// 仙灵复活时间的增幅（应该减少数值）
        /// </summary>
        public StatModifier fairyResurrectionTimeBous;

        /// <summary>
        /// 当前的仙灵索引，仅用于部分武器的绘制仙灵
        /// </summary>
        public int CurrentFairyIndex { get; set; } = -1;

        private SkillModifyer[] _skillBonus;

        /// <summary>
        /// 仙灵技能增幅
        /// </summary>
        public SkillModifyer[] FairySkillBonus
        {
            get
            {
                _skillBonus ??= new SkillModifyer[FairyLoader.FairySkillTagCount];
                return _skillBonus;
            }
        }

        public struct SkillModifyer
        {
            /// <summary>
            /// 最小等级
            /// </summary>
            public byte minLevel;

            /// <summary>
            /// 增加的等级
            /// </summary>
            public byte addLevel;

            public readonly int ModifyLevel(int level)
            {
                if (level < minLevel)
                    level = minLevel;

                level += addLevel;

                return level;
            }
        }

        #region 捕捉环相关数值

        /// <summary> 基础环大小 </summary>
        private const int FairyCatcherBaseRadius = 16 * 6;

        /// <summary> 仙灵捕捉环的加成 </summary>
        public float FairyCatcherRadiusBonus { get; private set; }

        /// <summary> 加成后的仙灵捕捉器的半径 </summary>
        public float FairyCatcherRadius { get => FairyCatcherBaseRadius + FairyCatcherRadiusBonus; }

        /// <summary> 核心的类型，使用<see cref="CoraliteContent.FairyCircleCoreType"/>设置 </summary>
        public int FairyCircleCoreType { get; set; }

        /// <summary> 玩家当前持有的仙灵捕捉环的弹幕索引 </summary>
        public int FairyCircleProj { get; set; }

        /// <summary> 仙灵尘消耗概率 </summary>
        public int FairyPowderCostProb { get; set; } = 100;

        #endregion

        #region 仙灵个体值增幅相关数值

        private FairyIVRandom lifeMaxRand;
        /// <summary> 仙灵生命值随机量 </summary>
        public ref FairyIVRandom LifeMaxRand => ref lifeMaxRand;

        private FairyIVRandom damageRand;
        /// <summary> 仙灵伤害随机量 </summary>
        public ref FairyIVRandom DamageRand => ref damageRand;

        private FairyIVRandom defenceRand;
        /// <summary> 仙灵防御随机量 </summary>
        public ref FairyIVRandom DefenceRand => ref defenceRand;

        private FairyIVRandom speedRand;
        /// <summary> 仙灵速度随机量 </summary>
        public ref FairyIVRandom SpeedRand => ref speedRand;

        private FairyIVRandom skillLevelRand;
        /// <summary> 仙灵技能等级随机量 </summary>
        public ref FairyIVRandom SkillLevelRand => ref skillLevelRand;

        private FairyIVRandom staminaRand;
        /// <summary> 仙灵速度随机量 </summary>
        public ref FairyIVRandom StaminaRand => ref staminaRand;

        private FairyIVRandom scaleRand;
        /// <summary> 仙灵速度随机量 </summary>
        public ref FairyIVRandom ScaleRand => ref scaleRand;

        #endregion

        private List<IFairyAccessory> fairyAccessories;
        public List<IFairyAccessory> FairyAccessories
        {
            get
            {
                fairyAccessories ??= new List<IFairyAccessory>();
                return fairyAccessories;
            }
        }

        private Item _bottleItem;

        /// <summary>
        /// 存储了仙灵瓶物品
        /// </summary>
        public Item BottleItem
        {
            get
            {
                _bottleItem ??= new Item();
                return _bottleItem;
            }
            set { _bottleItem = value; }
        }

        /// <summary>
        /// 自动丢出罐子
        /// </summary>
        public bool AutoShootJar;

        /// <summary>
        /// 绘制罐子瞄准线
        /// </summary>
        public bool DrawJarAimLine;


        public bool TryGetFairyBottle(out BaseFairyBottle bottle)
        {
            bottle = null;
            if (!BottleItem.IsAir && BottleItem.ModItem is BaseFairyBottle b2)
            {
                bottle = b2;
                return true;
            }

            return false;
        }

        public override void ResetEffects()
        {
            FairyCircleCoreType = -1;
            FairyCircleCoreType = -1;
            FairyPowderCostProb = 100;
            fairyAccessories?.Clear();

            fairyCatchPowerBonus = new StatModifier();
            fairyResurrectionTimeBous = new StatModifier();

            LifeMaxRand.Reset();
            DamageRand.Reset();
            DefenceRand.Reset();
            SpeedRand.Reset();
            SkillLevelRand.Reset();
            StaminaRand.Reset();
            ScaleRand.Reset();

            FairyCatcherRadiusBonus = 0;
            if (_skillBonus != null)
                Array.Fill(_skillBonus, default);
        }

        public override void PostUpdateEquips()
        {
            if (TryGetFairyBottle(out BaseFairyBottle bottle))
                bottle.UpdateBottle(Player);
        }

        /// <summary>
        /// 遍历玩家背包获取仙灵饵料
        /// </summary>
        /// <param name="bait"></param>
        public void FairyCatch_GetPowder(out Item bait)
        {
            bait = null;

            for (int j = 0; j < 50; j++)
            {
                if (Player.inventory[j].stack > 0 && CoraliteSets.Items.IsFairyPowder[Player.inventory[j].type])
                {
                    bait = Player.inventory[j];
                    break;
                }
            }
        }

        /// <summary>
        /// 根据技能的标签获取技能等级增幅
        /// </summary>
        /// <param name="baseLevel"></param>
        /// <param name="skillType"></param>
        /// <returns></returns>
        public int GetFairySkillBonus(int skillType,int baseLevel)
        {
            if (FairySkill.SkillWithTags.TryGetValue(skillType, out int[] tags))
                foreach (var tag in tags)
                    baseLevel = FairySkillBonus[tag].ModifyLevel(baseLevel);

            return baseLevel;
        }

        /// <summary>
        /// 增加捕捉环半径，单位：像素<br></br>
        /// 该加成不叠加，取最高效果
        /// </summary>
        /// <param name="howMany"></param>
        public void AddCircleRadius(int howMany)
        {
            if (FairyCatcherRadiusBonus < howMany)
                FairyCatcherRadiusBonus = howMany;
        }

        /// <summary>
        /// 获取仙灵捕捉力，默认使用玩家手持物品进行计算，如果手持物品不是仙灵捕手则返回一个增幅倍率
        /// </summary>
        /// <returns></returns>
        public int GetCatchPowerByHeldItem()
        {
            int basePower = 1;
            float exBonus = 1f;
            if (Player.HeldItem.ModItem is BaseFairyCatcher baseFairyCatcher)
            {
                basePower = baseFairyCatcher.CatchPower;
                exBonus = baseFairyCatcher.CatchPowerMult;
            }

            return GetBonusedCatchPower(basePower, exBonus);
        }

        public int GetBonusedCatchPower(int baseCatchPower, float mult = 1)
        {
            StatModifier modifyer = fairyCatchPowerBonus;
            modifyer += mult - 1;

            return (int)modifyer.ApplyTo(baseCatchPower);
        }

        /// <summary>
        /// 获取总的捕捉力加成幅度，物品为带有捕捉器前缀的物品
        /// </summary>
        /// <param name="base"></param>
        /// <param name="catcherItem"></param>
        /// <returns></returns>
        public void TotalCatchPowerBonus(ref float @base, Item catcherItem)
        {
            StatModifier modifyer = fairyCatchPowerBonus;
            float exBonus = 0f;

            if (catcherItem.ModItem is BaseFairyCatcher baseFairyCatcher)
                exBonus = baseFairyCatcher.CatchPowerMult - 1f;

            modifyer += exBonus;

            @base = modifyer.ApplyTo(@base);
        }

        /// <summary>
        /// 将仙灵饰品添加到列表中
        /// </summary>
        /// <param name="acc"></param>
        public void AddFairyAccessory(IFairyAccessory acc)
        {
            fairyAccessories ??= [];
            fairyAccessories.Add(acc);
        }

        public override void SaveData(TagCompound tag)
        {
            if (!BottleItem.IsAir)
                tag.Add(nameof(BottleItem), BottleItem);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet(nameof(BottleItem), out Item i))
                BottleItem = i;
        }
    }
}
