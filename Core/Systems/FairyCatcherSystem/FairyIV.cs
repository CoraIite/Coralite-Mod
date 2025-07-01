using Coralite.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    /// <summary>
    /// 仙灵的个体值，内部仅存储各种具体数值
    /// </summary>
    public struct FairyIV
    {
        /// <summary> 生命值上限 </summary>
        public int LifeMax { get; set; }
        /// <summary> 攻击 </summary>
        public int Damage { get; set; }
        /// <summary> 防御 </summary>
        public int Defence { get; set; }
        /// <summary> 速度 </summary>
        public float Speed { get; set; }
        /// <summary> 技能等级 </summary>
        public int SkillLevel { get; set; }
        /// <summary> 耐力，决定单次射出能够使用多少次技能 </summary>
        public int Stamina { get; set; }
        /// <summary> 没什么大用的尺寸属性 </summary>
        public int Scale { get; set; }

        /// <summary> 
        /// 生命值上限等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public float LifeMaxLevel { get; set; }

        /// <summary> 
        /// 伤害等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public float DamageLevel { get; set; }

        /// <summary> 
        /// 防御等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public float DefenceLevel { get; set; }

        /// <summary> 
        /// 速度等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public float SpeedLevel { get; set; }

        /// <summary> 
        /// 技能等级的等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public float SkillLevelLevel { get; set; }

        /// <summary> 
        /// 耐力的等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public float StaminaLevel { get; set; }

        /// <summary> 
        /// 尺寸等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public float ScaleLevel { get; set; }


        private const int EternalToOver = FairyIVLevelID.Over - FairyIVLevelID.Eternal;

        /// <summary>
        /// 根据仙灵类型和玩家的加成随机一个仙灵的六维个体值
        /// </summary>
        /// <param name="fairyType"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static FairyIV GetFairyIV(Fairy fairy, FairyCatcherPlayer player)
        {
            if (!FairySystem.fairyDatas.TryGetValue(fairy.Type, out FairyData data))
                return default(FairyIV);

            FairyIV iv = new FairyIV();

            SetLevels(player, ref iv);

            fairy.ModifyIVLevel(ref iv, player);

            //生命值上限
            GetLifeMaxIV(data, ref iv);
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

            fairy.PostModifyIV(ref iv, player);
            return iv;
        }

        private static void SetLevels(FairyCatcherPlayer player, ref FairyIV iv)
        {
            iv.LifeMaxLevel = player.LifeMaxRand.RandValue;
            iv.DamageLevel = player.DamageRand.RandValue;
            iv.DefenceLevel = player.DefenceRand.RandValue;
            iv.SkillLevelLevel = player.SkillLevelRand.RandValue;
            iv.SpeedLevel = player.SpeedRand.RandValue;
            iv.StaminaLevel = player.StaminaRand.RandValue;
            iv.ScaleLevel = player.ScaleRand.RandValue;
        }



        /// <summary>
        /// 存储仙灵个体值
        /// </summary>
        /// <param name="tag"></param>
        public readonly void Save(TagCompound tag)
        {
            const string preName = "IV_";

            tag.Add(preName + nameof(LifeMax),LifeMax);
            tag.Add(preName + nameof(Damage), Damage);
            tag.Add(preName + nameof(Defence), Defence);
            tag.Add(preName + nameof(Speed), Speed);
            tag.Add(preName + nameof(SkillLevel), SkillLevel);
            tag.Add(preName + nameof(Stamina), Stamina);
            tag.Add(preName + nameof(Scale), Scale);

            tag.Add(preName + nameof(LifeMaxLevel), LifeMaxLevel);
            tag.Add(preName + nameof(DamageLevel), DamageLevel);
            tag.Add(preName + nameof(DefenceLevel), DefenceLevel);
            tag.Add(preName + nameof(SpeedLevel), SpeedLevel);
            tag.Add(preName + nameof(SkillLevelLevel), SkillLevelLevel);
            tag.Add(preName + nameof(StaminaLevel), StaminaLevel);
            tag.Add(preName + nameof(ScaleLevel), ScaleLevel);
        }

        public static FairyIV Load(Item item, TagCompound tag)
        {
            const string preName = "IV_";
            if (tag.TryGet(preName + nameof(LifeMax), out int lifemax)
                && tag.TryGet(preName + nameof(Damage), out int damage)
                && tag.TryGet(preName + nameof(Defence), out int defence)
                && tag.TryGet(preName + nameof(Speed), out float speed)
                && tag.TryGet(preName + nameof(SkillLevel), out int skillLevel)
                && tag.TryGet(preName + nameof(Stamina), out int stamina)
                && tag.TryGet(preName + nameof(Scale), out int scale)
                && tag.TryGet(preName + nameof(LifeMaxLevel), out float lifeMaxLevel)
                && tag.TryGet(preName + nameof(DamageLevel), out float damageLevel)
                && tag.TryGet(preName + nameof(DefenceLevel), out float defenceLevel)
                && tag.TryGet(preName + nameof(SpeedLevel), out float speedLevel)
                && tag.TryGet(preName + nameof(SkillLevelLevel), out float skillLevelLevel)
                && tag.TryGet(preName + nameof(StaminaLevel), out float staminaLevel)
                && tag.TryGet(preName + nameof(ScaleLevel), out float scaleLevel)
                )
            {
                return new FairyIV
                {
                    LifeMax = lifemax,
                    Damage = damage,
                    Defence = defence,
                    Speed = speed,
                    SkillLevel = skillLevel,
                    Stamina = stamina,
                    Scale = scale,
                    LifeMaxLevel = lifeMaxLevel,
                    DamageLevel = damageLevel,
                    DefenceLevel = defenceLevel,
                    SpeedLevel = speedLevel,
                    SkillLevelLevel = skillLevelLevel,
                    StaminaLevel = staminaLevel,
                    ScaleLevel = scaleLevel,
                };
            }
            else
            {
                $"数据读取异常，物品名{item.Name}".Dump();
                return default;
            }
        }


        public static void GetLifeMaxIV(FairyData data, ref FairyIV iv)
        {
            //数值高于永恒，从永恒到最大值之间缩放
            if ((int)iv.LifeMaxLevel > data.LifeMaxData.Count - 1)
                iv.LifeMax = (int)Helper.Lerp(
                    data.LifeMaxData[^1],
                    data.OverLifeMax,
                    Math.Clamp((iv.LifeMaxLevel - FairyIVLevelID.Eternal), 0, EternalToOver) / EternalToOver);
            else
            {
                //在二者间使用X2插值(四舍五入)
                iv.LifeMax = GetLerpIVValue(data.LifeMaxData, iv.LifeMaxLevel);
            }
        }

        public static void GetDamageIV(FairyCatcherPlayer player, FairyData data, ref FairyIV iv)
        {
            //数值高于永恒，从永恒到最大值之间缩放
            if ((int)iv.DamageLevel > data.DamageData.Count - 1)
                iv.Damage = (int)Helper.Lerp(
                    data.DamageData[^1],
                    data.OverDamage,
                    Math.Clamp((iv.DamageLevel - FairyIVLevelID.Eternal), 0, EternalToOver) / EternalToOver);
            else
            {
                //在二者间使用X2插值(四舍五入)
                iv.Damage = GetLerpIVValue(data.DamageData, iv.DamageLevel);
            }
        }

        public static void GetDefenceIV(FairyCatcherPlayer player, FairyData data, ref FairyIV iv)
        {
            //数值高于永恒，从永恒到最大值之间缩放
            if ((int)iv.DefenceLevel > data.DefenceData.Count - 1)
                iv.Defence = (int)Helper.Lerp(
                    data.DefenceData[^1],
                    data.OverDefence,
                    Math.Clamp((iv.DefenceLevel - FairyIVLevelID.Eternal), 0, EternalToOver) / EternalToOver);
            else
            {
                //在二者间使用X2插值(四舍五入)
                iv.Defence = GetLerpIVValue(data.DefenceData, iv.DefenceLevel);
            }
        }

        public static void GetSpeedIV(FairyCatcherPlayer player, FairyData data, ref FairyIV iv)
        {
            //数值高于永恒，从永恒到最大值之间缩放
            if ((int)iv.SpeedLevel > data.SpeedData.Count - 1)
                iv.Speed = MathF.Round(Helper.Lerp(
                    data.SpeedData[^1],
                    data.OverSpeed,
                    Math.Clamp((iv.SpeedLevel - FairyIVLevelID.Eternal), 0, EternalToOver) / EternalToOver), 1, MidpointRounding.AwayFromZero);
            else
            {
                //在二者间使用X2插值(四舍五入)
                //小一个等级的值
                float less = data.SpeedData[(int)iv.SpeedLevel];
                //大一个等级的值
                float more = data.SpeedData[(int)iv.SpeedLevel + 1];

                iv.Speed = MathF.Round(Helper.Lerp(less, more, Math.Clamp(Helper.X2Ease(iv.SpeedLevel - (int)iv.SpeedLevel), 0, 1)), 1, MidpointRounding.AwayFromZero);
            }
        }

        public static void GetSkillLevelIV(FairyCatcherPlayer player, FairyData data, ref FairyIV iv)
        {
            //数值高于永恒，从永恒到最大值之间缩放
            if ((int)iv.SkillLevelLevel > data.SkillLevelData.Count - 1)
                iv.SkillLevel = (int)Helper.Lerp(
                    data.SkillLevelData[^1],
                    data.OverSkillLevel,
                    Math.Clamp((iv.SkillLevelLevel - FairyIVLevelID.Eternal), 0, EternalToOver) / EternalToOver);
            else
            {
                //在二者间使用X2插值(四舍五入)
                iv.SkillLevel = GetLerpIVValue(data.SkillLevelData, iv.SkillLevelLevel);
            }
        }

        public static void GetStaminaIV(FairyCatcherPlayer player, FairyData data, ref FairyIV iv)
        {
            //数值高于永恒，从永恒到最大值之间缩放
            if ((int)iv.StaminaLevel > data.StaminaData.Count - 1)
                iv.Stamina = (int)Helper.Lerp(
                    data.StaminaData[^1],
                    data.OverStamina,
                    Math.Clamp((iv.StaminaLevel - FairyIVLevelID.Eternal), 0, EternalToOver) / EternalToOver);
            else
            {
                //在二者间使用X2插值(四舍五入)
                iv.Stamina = GetLerpIVValue(data.StaminaData, iv.StaminaLevel);
            }
        }

        private static void GetScaleIV(FairyCatcherPlayer player, ref FairyIV iv)
        {
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

        public static (Color, LocalizedText) GetFairyLocalize(float level)
        {
            return level switch
            {
                > FairyIVLevelID.Weak and < FairyIVLevelID.WeakCommon
                    => (Color.Gray, FairySystem.WeakLevel),
                >= FairyIVLevelID.WeakCommon and < FairyIVLevelID.Common
                    => (FairySystem.VeryCommonLevel_Brown, FairySystem.WeakCommonLevel),
                >= FairyIVLevelID.Common and < FairyIVLevelID.Uncommon
                    => (Color.White, FairySystem.CommonLevel),
                >= FairyIVLevelID.Uncommon and < FairyIVLevelID.Rare
                    => (Color.LawnGreen, FairySystem.UncommonLevel),
                >= FairyIVLevelID.Rare and < FairyIVLevelID.Epic
                    => (Color.DodgerBlue, FairySystem.RareLevel),
                >= FairyIVLevelID.Epic and < FairyIVLevelID.Legendary
                    => (Color.Yellow, FairySystem.EpicLevel),
                >= FairyIVLevelID.Legendary and < FairyIVLevelID.Eternal
                    => (Color.HotPink, FairySystem.LegendaryLevel),
                >= FairyIVLevelID.Eternal and < FairyIVLevelID.Over
                    => (Color.Orange, FairySystem.EternalLevel),
                _ => (Color.Coral, FairySystem.OverLevel)
            };
        }
    }
}
