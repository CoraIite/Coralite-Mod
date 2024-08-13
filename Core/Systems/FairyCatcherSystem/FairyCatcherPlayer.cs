﻿using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public class FairyCatcherPlayer : ModPlayer
    {
        public struct FairyIVRandomModifyer
        {
            public float base_Min;
            public float base_Max;
            public float Flat_Min;
            public float Flat_Max;
            public float additive_Min;
            public float additive_Max;
            public float multiplicative_Min;
            public float multiplicative_Max;
        }

        /// <summary>
        /// 仙灵捕捉器指针的大小，用于对于其进行缩放<br></br>
        /// 捕捉器默认大小由贴图决定，查看<see cref="BaseFairyCatcherProj.GetCursorBox"/>以了解更多
        /// </summary>
        public StatModifier cursorSizeBonus;
        /// <summary>
        /// 仙灵捕捉器的捕捉力增幅
        /// </summary>
        public StatModifier fairyCatchPowerBonus;
        /// <summary>
        /// 仙灵复活时间的增幅（应该减少数值）
        /// </summary>
        public StatModifier fairyResurrectionTimeBous;
        public float fairyCatcherRadius;

        /// <summary>
        /// 每次生成仙灵时会生成多少个
        /// </summary>
        public int spawnFairyCount;

        public FairyIVRandomModifyer damageRamdom;
        public FairyIVRandomModifyer defenceRamdom;
        public FairyIVRandomModifyer lifeMaxRamdom;
        public (float, float) ScaleRange;

        public Color CatcherCircleColor;
        public Color CatcherBackColor;

        public override void ResetEffects()
        {
            cursorSizeBonus = new StatModifier();
            fairyCatchPowerBonus = new StatModifier();
            fairyResurrectionTimeBous = new StatModifier();

            //默认伤害区间为0.8-1.2
            damageRamdom = new FairyIVRandomModifyer()
            {
                additive_Min = 0.8f,
                additive_Max = 1.2f,
                multiplicative_Min = 1,
                multiplicative_Max = 1.01f,
            };

            //默认防御区间为0.8-1.15
            defenceRamdom = new FairyIVRandomModifyer()
            {
                additive_Min = 0.9f,
                additive_Max = 1.15f,
                multiplicative_Min = 1,
                multiplicative_Max = 1.01f,
            };

            //默认血量区间为0.8-1.3
            lifeMaxRamdom = new FairyIVRandomModifyer()
            {
                additive_Min = 0.8f,
                additive_Max = 1.3f,
                multiplicative_Min = 1,
                multiplicative_Max = 1.01f,
            };

            //默认大小区间0.9-1.1
            ScaleRange = (0.9f, 1.1f);

            spawnFairyCount = 1;
            fairyCatcherRadius = 7 * 16;

            CatcherCircleColor = Color.White;
            CatcherBackColor = Color.DarkSlateBlue * 0.5f;
        }

        /// <summary>
        /// 遍历玩家背包获取仙灵饵料
        /// </summary>
        /// <param name="bait"></param>
        public void FairyCatch_GetBait(out Item bait)
        {
            bait = null;
            for (int i = 54; i < 58; i++)
            {
                if (Player.inventory[i].stack > 0 && Player.inventory[i].bait > 0)
                {
                    bait = Player.inventory[i];
                    break;
                }
            }

            if (bait != null)
                return;

            for (int j = 0; j < 50; j++)
            {
                if (Player.inventory[j].stack > 0 && Player.inventory[j].bait > 0)
                {
                    bait = Player.inventory[j];
                    break;
                }
            }
        }

        public bool FairyShoot_GetFairyBottle(out IFairyBottle bottle)
        {
            bottle = null;

            for (int j = 0; j < 50; j++)
            {
                if (Player.inventory[j].stack > 0 && Player.inventory[j].ModItem is IFairyBottle fairyBottle)
                {
                    bottle = fairyBottle;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取仙灵捕捉力，默认使用玩家手持物品进行计算，如果手持物品不是仙灵捕手则返回一个增幅倍率
        /// </summary>
        /// <returns></returns>
        public int FairyCatch_GetCatchPower()
        {
            int basePower = 1;
            float exBonus = 0f;
            if (Player.HeldItem.TryGetGlobalItem(out FairyGlobalItem fgi))
            {
                basePower = fgi.CatchPower;
                exBonus = fgi.CatchPowerMult - 1f;
            }
            StatModifier modifyer = fairyCatchPowerBonus;

            modifyer += exBonus;

            return (int)modifyer.ApplyTo(basePower);
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

            if (catcherItem.TryGetGlobalItem(out FairyGlobalItem fgi))
                exBonus = fgi.CatchPowerMult - 1f;

            modifyer += exBonus;

            @base = modifyer.ApplyTo(@base);
        }

        /// <summary>
        /// 仙灵伤害加成，使用仙灵捕获力增强幅度来增幅伤害
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        public float FairyDamageBonus(Item item, float damage)
        {
            StatModifier modifyer = fairyCatchPowerBonus;
            float exBonus = 0f;

            if (item.TryGetGlobalItem(out FairyGlobalItem fgi))
                exBonus = fgi.CatchPowerMult - 1f;

            modifyer += exBonus;

            return modifyer.ApplyTo(damage);
        }

        public bool FairyCatch_GetEmptyFairyBottle(out IFairyBottle fairyBottle, out int emptySlot)
        {
            fairyBottle = null;
            emptySlot = -1;

            for (int j = 0; j < 50; j++)
            {
                if (Player.inventory[j].stack > 0 && Player.inventory[j].ModItem is IFairyBottle)
                {
                    fairyBottle = Player.inventory[j].ModItem as IFairyBottle;

                    for (int i = 0; i < fairyBottle.Fairies.Length; i++)
                    {
                        Item item = fairyBottle.Fairies[i];
                        if (item.IsAir)
                        {
                            emptySlot = i;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 随机仙灵的个体值
        /// </summary>
        /// <param name="faityItem"></param>
        /// <returns></returns>
        public FairyData RollFairyIndividualValues(BaseFairyItem faityItem)
        {
            FairyData data = new();

            data.damageBonus = new StatModifier(
                Main.rand.NextFloat(damageRamdom.additive_Min, damageRamdom.additive_Max),
                Main.rand.NextFloat(damageRamdom.multiplicative_Min, damageRamdom.multiplicative_Max),
                Main.rand.NextFloat(damageRamdom.Flat_Min, damageRamdom.Flat_Max),
                Main.rand.NextFloat(damageRamdom.base_Min, damageRamdom.base_Max)
                );
            data.defenceBonus = new StatModifier(
                Main.rand.NextFloat(defenceRamdom.additive_Min, defenceRamdom.additive_Max),
                Main.rand.NextFloat(defenceRamdom.multiplicative_Min, defenceRamdom.multiplicative_Max),
                Main.rand.NextFloat(defenceRamdom.Flat_Min, defenceRamdom.Flat_Max),
                Main.rand.NextFloat(defenceRamdom.base_Min, defenceRamdom.base_Max)
                );
            data.lifeMaxBonus = new StatModifier(
                Main.rand.NextFloat(lifeMaxRamdom.additive_Min, lifeMaxRamdom.additive_Max),
                Main.rand.NextFloat(lifeMaxRamdom.multiplicative_Min, lifeMaxRamdom.multiplicative_Max),
                Main.rand.NextFloat(lifeMaxRamdom.Flat_Min, lifeMaxRamdom.Flat_Max),
                Main.rand.NextFloat(lifeMaxRamdom.base_Min, lifeMaxRamdom.base_Max)
                );

            data.scaleBonus = Main.rand.NextFloat(ScaleRange.Item1, ScaleRange.Item2);

            return data;
        }
    }
}
