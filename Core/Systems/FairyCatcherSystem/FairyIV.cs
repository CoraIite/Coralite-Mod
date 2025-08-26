using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    /// <summary>
    /// 仙灵的个体值，内部仅存储各种具体数值
    /// </summary>
    public class FairyIV
    {
        public const float EVMax = 42;
        public const int TotalEVMax = 42 * 3;

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
        public float Scale { get; set; }

        /// <summary> 血量努力值 </summary>
        public int LifeMaxEV { get; set; }
        /// <summary> 攻击努力值 </summary>
        public int DamageEV { get; set; }
        /// <summary> 防御努力值 </summary>
        public int DefenceEV { get; set; }
        /// <summary> 速度努力值 </summary>
        public int SpeedEV { get; set; }
        /// <summary> 技能等级努力值 </summary>
        public int SkillLevelEV { get; set; }
        /// <summary> 耐力努力值 </summary>
        public int StaminaEV { get; set; }

        /// <summary>
        /// 总努力值
        /// </summary>
        public int TotalEV => LifeMaxEV + DamageEV + DefenceEV + SpeedEV + SkillLevelEV + StaminaEV;


        /// <summary> 
        /// 生命值上限等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public float LifeMaxLV { get; set; }

        /// <summary> 
        /// 伤害等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public float DamageLV { get; set; }

        /// <summary> 
        /// 防御等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public float DefenceLV { get; set; }

        /// <summary> 
        /// 速度等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public float SpeedLV { get; set; }

        /// <summary> 
        /// 技能等级的等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public float SkillLevelLV { get; set; }

        /// <summary> 
        /// 耐力的等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public float StaminaLV { get; set; }

        /// <summary> 
        /// 尺寸等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public float ScaleLV { get; set; }

        public enum IVType : byte
        {
            LifeMax,
            Damage,
            Defence,
            Speed,
            SkillLevel,
            Stamina,
        }

        private const int EternalToOver = FairyIVLevelID.Over - FairyIVLevelID.Eternal - 1;

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

            SetLevels(player, iv);
            //仙灵自身的随机量
            fairy.ModifyIVLevel(iv, player);

            //计算各种数值
            CalculateAllIV(data, iv);
            //大小
            GetScaleIV(iv);

            fairy.PostModifyIV(iv, player);
            return iv;
        }

        /// <summary>
        /// 获取最大与最小值
        /// </summary>
        /// <param name="fairy"></param>
        /// <param name="player"></param>
        /// <param name="ivMin"></param>
        /// <param name="ivMax"></param>
        public static void GetIVForUI(Fairy fairy, FairyCatcherPlayer player, out FairyIV ivMin, out FairyIV ivMax)
        {
            ivMin = new FairyIV();
            ivMax = new FairyIV();
            if (!FairySystem.fairyDatas.TryGetValue(fairy.Type, out FairyData data))
                return;

            ivMin.LifeMaxLV = player.LifeMaxRand.MinValue;
            ivMin.DamageLV = player.DamageRand.MinValue;
            ivMin.DefenceLV = player.DefenceRand.MinValue;
            ivMin.SkillLevelLV = player.SkillLevelRand.MinValue;
            ivMin.SpeedLV = player.SpeedRand.MinValue;
            ivMin.StaminaLV = player.StaminaRand.MinValue;
            ivMin.ScaleLV = player.ScaleRand.MinValue;

            ivMin = SetIV(fairy, player, ivMin, data);

            ivMax.LifeMaxLV = player.LifeMaxRand.MaxValue;
            ivMax.DamageLV = player.DamageRand.MaxValue;
            ivMax.DefenceLV = player.DefenceRand.MaxValue;
            ivMax.SkillLevelLV = player.SkillLevelRand.MaxValue;
            ivMax.SpeedLV = player.SpeedRand.MaxValue;
            ivMax.StaminaLV = player.StaminaRand.MaxValue;
            ivMax.ScaleLV = player.ScaleRand.MaxValue;

            ivMax = SetIV(fairy, player, ivMax, data);

            static FairyIV SetIV(Fairy fairy, FairyCatcherPlayer player, FairyIV iv, FairyData data)
            {
                fairy.ModifyIVLevel(iv, player);

                CalculateAllIV(data, iv);

                fairy.PostModifyIV(iv, player);
                return iv;
            }
        }

        private static void SetLevels(FairyCatcherPlayer player, FairyIV iv)
        {
            iv.LifeMaxLV = player.LifeMaxRand.RandValue;
            iv.DamageLV = player.DamageRand.RandValue;
            iv.DefenceLV = player.DefenceRand.RandValue;
            iv.SkillLevelLV = player.SkillLevelRand.RandValue;
            iv.SpeedLV = player.SpeedRand.RandValue;
            iv.StaminaLV = player.StaminaRand.RandValue;
            iv.ScaleLV = player.ScaleRand.RandValue;
        }

        #region IO

        /// <summary>
        /// 存储仙灵个体值
        /// </summary>
        /// <param name="tag"></param>
        public void Save(TagCompound tag)
        {
            const string preName = "IV_";

            tag.Add(preName + nameof(LifeMaxEV), LifeMaxEV);
            tag.Add(preName + nameof(DamageEV), DamageEV);
            tag.Add(preName + nameof(DefenceEV), DefenceEV);
            tag.Add(preName + nameof(SpeedEV), SpeedEV);
            tag.Add(preName + nameof(SkillLevelEV), SkillLevelEV);
            tag.Add(preName + nameof(StaminaEV), StaminaEV);
            tag.Add(preName + nameof(Scale), Scale);

            tag.Add(preName + nameof(LifeMaxLV), LifeMaxLV);
            tag.Add(preName + nameof(DamageLV), DamageLV);
            tag.Add(preName + nameof(DefenceLV), DefenceLV);
            tag.Add(preName + nameof(SpeedLV), SpeedLV);
            tag.Add(preName + nameof(SkillLevelLV), SkillLevelLV);
            tag.Add(preName + nameof(StaminaLV), StaminaLV);
            tag.Add(preName + nameof(ScaleLV), ScaleLV);
        }

        public static FairyIV Load(Item item, int fairyType, TagCompound tag)
        {
            const string preName = "IV_";
            if (tag.TryGet(preName + nameof(LifeMaxEV), out int lifemaxEV)
                && tag.TryGet(preName + nameof(DamageEV), out int damageEV)
                && tag.TryGet(preName + nameof(DefenceEV), out int defenceEV)
                && tag.TryGet(preName + nameof(SpeedEV), out int speedEV)
                && tag.TryGet(preName + nameof(SkillLevelEV), out int skillLevelEV)
                && tag.TryGet(preName + nameof(StaminaEV), out int staminaEV)
                && tag.TryGet(preName + nameof(Scale), out float scale)
                && tag.TryGet(preName + nameof(LifeMaxLV), out float lifeMaxLevel)
                && tag.TryGet(preName + nameof(DamageLV), out float damageLevel)
                && tag.TryGet(preName + nameof(DefenceLV), out float defenceLevel)
                && tag.TryGet(preName + nameof(SpeedLV), out float speedLevel)
                && tag.TryGet(preName + nameof(SkillLevelLV), out float skillLevelLevel)
                && tag.TryGet(preName + nameof(StaminaLV), out float staminaLevel)
                && tag.TryGet(preName + nameof(ScaleLV), out float scaleLevel)
                )
            {
                var iv = new FairyIV
                {
                    LifeMaxEV = lifemaxEV,
                    DamageEV = damageEV,
                    DefenceEV = defenceEV,
                    SpeedEV = speedEV,
                    SkillLevelEV = skillLevelEV,
                    StaminaEV = staminaEV,
                    Scale = scale,
                    LifeMaxLV = lifeMaxLevel,
                    DamageLV = damageLevel,
                    DefenceLV = defenceLevel,
                    SpeedLV = speedLevel,
                    SkillLevelLV = skillLevelLevel,
                    StaminaLV = staminaLevel,
                    ScaleLV = scaleLevel,
                };

                if (!FairySystem.fairyDatas.TryGetValue(fairyType, out FairyData data))
                    return default;

                CalculateAllIV(data, iv);
                return iv;
            }
            else
            {
                $"数据读取异常，物品名{item.Name}".Dump();
                return default;
            }
        }

        #endregion

        /// <summary>
        /// 尝试增加EV
        /// </summary>
        /// <param name="type"></param>
        /// <param name="addValue"></param>
        /// <returns></returns>
        public bool TryAddEV(IVType type, int addValue, int fairyType)
        {
            int total = TotalEV;
            if (total + addValue > TotalEVMax)
                addValue = TotalEVMax - total;

            if (addValue < 1)
                return false;


            switch (type)
            {
                case IVType.LifeMax:
                    if (LifeMaxEV + addValue > EVMax)
                        addValue = (int)EVMax - LifeMaxEV;

                    if (addValue < 1)
                        return false;

                    LifeMaxEV += addValue;
                    goto over;
                case IVType.Damage:
                    if (DamageEV + addValue > EVMax)
                        addValue = (int)EVMax - DamageEV;

                    if (addValue < 1)
                        return false;

                    DamageEV += addValue;
                    goto over;
                case IVType.Defence:
                    if (DefenceEV + addValue > EVMax)
                        addValue = (int)EVMax - DefenceEV;

                    if (addValue < 1)
                        return false;

                    DefenceEV += addValue;
                    goto over;
                case IVType.Speed:
                    if (SpeedEV + addValue > EVMax)
                        addValue = (int)EVMax - SpeedEV;

                    if (addValue < 1)
                        return false;

                    SpeedEV += addValue;
                    goto over;
                case IVType.SkillLevel:
                    if (SkillLevelEV + addValue > EVMax)
                        addValue = (int)EVMax - SkillLevelEV;

                    if (addValue < 1)
                        return false;

                    SkillLevelEV += addValue;
                    goto over;
                case IVType.Stamina:
                    if (StaminaEV + addValue > EVMax)
                        addValue = (int)EVMax - StaminaEV;

                    if (addValue < 1)
                        return false;

                    StaminaEV += addValue;
                    goto over;
                default:
                    return false;
            }

        over:
            if (!FairySystem.fairyDatas.TryGetValue(fairyType, out FairyData data))
                return false;

            CalculateAllIV(data, this);

            return true;
        }

        /// <summary>
        /// 根据种族值，个体值和努力值计算具体的数值
        /// </summary>
        /// <param name="OffsetValue">特殊调整值</param>
        /// <param name="LV">个体值</param>
        /// <param name="speciesStrength">种族值</param>
        /// <param name="EV">努力值</param>
        public static float CalculateIV(float OffsetValue, short speciesStrength, float LV, int EV, float minValue = 1)
        {
            //计算缩放后的种族值效应
            float speciesStrengthScaled = speciesStrength;
            if (speciesStrengthScaled > 300)
                speciesStrengthScaled = 1 + (speciesStrengthScaled - 300) / (300 * 3);//大于255的种族值中只有三分之一的作用
            else
                speciesStrengthScaled /= 300f;

            return minValue + (speciesStrengthScaled + 0.1f * EV / EVMax) * LV * OffsetValue;
        }

        public static int CalculateIVRound(float OffsetValue, short speciesStrength, float LV, int EV, float minValue = 1)
        {
            return (int)MathF.Round(CalculateIV(OffsetValue, speciesStrength, LV, EV, minValue)
                , MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 计算除大小以外的所有数值
        /// </summary>
        /// <param name="SSVData"></param>
        /// <param name="iv"></param>
        public static void CalculateAllIV(FairyData SSVData, FairyIV iv)
        {
            iv.LifeMax = CalculateIVRound(20, SSVData.LifeMaxSSV, iv.LifeMaxLV, iv.LifeMaxEV, 20);
            iv.Damage = CalculateIVRound(4f, SSVData.DamageSSV, iv.DamageLV, iv.DamageEV);
            iv.Defence = CalculateIVRound(2.5f, SSVData.DefenceSSV, iv.DefenceLV, iv.DefenceEV, 4);
            iv.Speed = MathF.Round(CalculateIV(0.15f, SSVData.SpeedSSV, iv.SpeedLV, iv.SpeedEV, 5), 1);
            iv.SkillLevel = CalculateIVRound(1, SSVData.SkillLevelSSV, iv.SkillLevelLV, iv.SkillLevelEV);
            iv.Stamina = CalculateIVRound(1, SSVData.StaminaSSV, iv.StaminaLV, iv.StaminaEV);
        }

        private static void GetScaleIV(FairyIV iv)
        {
            //数值高于永恒，从永恒到最大值之间缩放
            if ((int)iv.ScaleLV >= 8 - 1)
                iv.Scale = Helper.Lerp(
                    1.5f,
                    2,
                    Math.Clamp((iv.ScaleLV - FairyIVLevelID.Eternal), 0, EternalToOver) / EternalToOver);
            else
            {
                //在二者间使用X2插值(四舍五入)
                iv.Scale = MathF.Round(Helper.Lerp(0.8f, 1.5f, Math.Clamp(iv.ScaleLV / 7, 0, 1)), 1, MidpointRounding.AwayFromZero);
            }
        }

        public static (Color, LocalizedText) GetFairyIVColorAndText(float level)
        {
            return level switch
            {
                >= FairyIVLevelID.Weak and < FairyIVLevelID.WeakCommon
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
