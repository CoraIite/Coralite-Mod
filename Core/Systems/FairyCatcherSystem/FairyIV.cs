using Coralite.Helpers;
using System;
using System.Collections.Generic;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    /// <summary>
    /// 仙灵的个体值，内部仅存储各种具体数值
    /// </summary>
    public struct FairyIV
    {
        /// <summary> 生命值上限 </summary>
        public int LifeMax { get; private set; }
        /// <summary> 攻击 </summary>
        public int Damage { get; private set; }
        /// <summary> 防御 </summary>
        public int Defence { get; private set; }
        /// <summary> 速度 </summary>
        public float Speed { get; private set; }
        /// <summary> 技能等级 </summary>
        public int SkillLevel { get; private set; }
        /// <summary> 耐力，决定单次射出能够使用多少次技能 </summary>
        public int Stamina { get; private set; }
        /// <summary> 没什么大用的尺寸属性 </summary>
        public int Scale { get; private set; }

        /// <summary> 
        /// 生命值上限等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public short LifeMaxLevel { get; private set; }

        /// <summary> 
        /// 伤害等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public short DamageLevel { get; private set; }

        /// <summary> 
        /// 防御等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public short DefenceLevel { get; private set; }

        /// <summary> 
        /// 速度等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public short SpeedLevel { get; private set; }

        /// <summary> 
        /// 技能等级的等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public short SkillLevelLevel { get; private set; }

        /// <summary> 
        /// 耐力的等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public short StaminaLevel { get; private set; }

        /// <summary> 
        /// 尺寸等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public short ScaleLevel { get; private set; }

        /// <summary>
        /// 根据仙灵类型和玩家的加成随机一个仙灵的六维个体值
        /// </summary>
        /// <param name="fairyType"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static FairyIV GetFairyIV(int fairyType, FairyCatcherPlayer player)
        {
            if (!FairySystem.fairyDatas.TryGetValue(fairyType, out FairyData data))
                return default(FairyIV);

            FairyIV iv = new FairyIV();

            //生命值上限
            GetLifeMaxIV(player, data, ref iv);
            //伤害
            GetDamageIV(player, data, ref iv);
            //防御
            GetDefenceIV(player, data, ref iv);
            //速度
            GetSpeedIV(player, data, ref iv);
            //技能等级
            GetSkillLevelIV(player, data, ref iv);
            //耐力
            GetStaminaIV(player, data, ref iv);
            //大小
            GetScaleIV(player, ref iv);


            return iv;
        }

        public static void GetLifeMaxIV(FairyCatcherPlayer player, FairyData data, ref FairyIV iv)
        {
            float level = player.LifeMaxRand.RandValue;
            iv.LifeMaxLevel = (short)level;

            //数值高于永恒，从永恒到最大值之间缩放
            if ((int)level > data.LifeMaxData.Count - 1)
                iv.LifeMax = (int)Helper.Lerp(
                    data.LifeMaxData[^1],
                    data.OverLifeMax,
                    Math.Clamp((level - data.LifeMaxData.Count - 1), 0, 100) / 100);
            else
            {
                //在二者间使用X2插值(四舍五入)
                iv.LifeMax = GetLerpIVValue(data.LifeMaxData, level);
            }
        }

        public static void GetDamageIV(FairyCatcherPlayer player, FairyData data, ref FairyIV iv)
        {
            float level = player.DamageRand.RandValue;
            iv.DamageLevel = (short)level;
            //数值高于永恒，从永恒到最大值之间缩放
            if ((int)level > data.DamageData.Count - 1)
                iv.Damage = (int)Helper.Lerp(
                    data.DamageData[^1],
                    data.OverDamage,
                    Math.Clamp((level - data.DamageData.Count - 1), 0, 100) / 100);
            else
            {
                //在二者间使用X2插值(四舍五入)
                iv.Damage = GetLerpIVValue(data.DamageData, level);
            }
        }

        public static void GetDefenceIV(FairyCatcherPlayer player, FairyData data, ref FairyIV iv)
        {
            float level = player.DefenceRand.RandValue;
            iv.DefenceLevel = (short)level;
            //数值高于永恒，从永恒到最大值之间缩放
            if ((int)level > data.DefenceData.Count - 1)
                iv.Defence = (int)Helper.Lerp(
                    data.DefenceData[^1],
                    data.OverDefence,
                    Math.Clamp((level - data.DefenceData.Count - 1), 0, 100) / 100);
            else
            {
                //在二者间使用X2插值(四舍五入)
                iv.Defence = GetLerpIVValue(data.DefenceData, level);
            }
        }

        public static void GetSpeedIV(FairyCatcherPlayer player, FairyData data, ref FairyIV iv)
        {
            float level = player.SpeedRand.RandValue;
            iv.SpeedLevel = (short)level;
            //数值高于永恒，从永恒到最大值之间缩放
            if ((int)level > data.SpeedData.Count - 1)
                iv.Speed = Helper.Lerp(
                    data.SpeedData[^1],
                    data.OverSpeed,
                    Math.Clamp((level - data.SpeedData.Count - 1), 0, 100) / 100);
            else
            {
                //在二者间使用X2插值(四舍五入)
                //小一个等级的值
                float less = data.SpeedData[(int)level];
                //大一个等级的值
                float more = data.SpeedData[(int)level + 1];

                iv.Speed= MathF.Round(Helper.Lerp(less, more, Math.Clamp(Helper.X2Ease(level - (int)level), 0, 1)), MidpointRounding.AwayFromZero);
            }
        }

        public static void GetSkillLevelIV(FairyCatcherPlayer player, FairyData data, ref FairyIV iv)
        {
            float level = player.SkillLevelRand.RandValue;
            iv.SkillLevelLevel = (short)level;
            //数值高于永恒，从永恒到最大值之间缩放
            if ((int)level > data.SkillLevelData.Count - 1)
                iv.SkillLevel = (int)Helper.Lerp(
                    data.SkillLevelData[^1],
                    data.OverSkillLevel,
                    Math.Clamp((level - data.SkillLevelData.Count - 1), 0, 100) / 100);
            else
            {
                //在二者间使用X2插值(四舍五入)
                iv.SkillLevel = GetLerpIVValue(data.SkillLevelData, level);
            }
        }

        public static void GetStaminaIV(FairyCatcherPlayer player, FairyData data, ref FairyIV iv)
        {
            float level = player.StaminaRand.RandValue;
            iv.StaminaLevel = (short)level;
            //数值高于永恒，从永恒到最大值之间缩放
            if ((int)level > data.StaminaData.Count - 1)
                iv.Stamina = (int)Helper.Lerp(
                    data.StaminaData[^1],
                    data.OverStamina,
                    Math.Clamp((level - data.StaminaData.Count - 1), 0, 100) / 100);
            else
            {
                //在二者间使用X2插值(四舍五入)
                iv.Stamina = GetLerpIVValue(data.StaminaData, level);
            }
        }

        private static void GetScaleIV(FairyCatcherPlayer player, ref FairyIV iv)
        {
            float level = player.ScaleRand.RandValue;
            iv.ScaleLevel = (short)level;
            //数值高于永恒，从永恒到最大值之间缩放

        }

        private static int GetLerpIVValue(List<int> dataList, float level)
        {
            //小一个等级的值
            int less = dataList[(int)level];
            //大一个等级的值
            int more = dataList[(int)level + 1];

            return (int)MathF.Round(Helper.Lerp(less, more, Math.Clamp(Helper.X2Ease(level - (int)level), 0, 1)), MidpointRounding.AwayFromZero);
        }
    }
}
