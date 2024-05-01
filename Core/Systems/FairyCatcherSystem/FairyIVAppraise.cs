using Terraria.Localization;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public struct FairyIVAppraise((float, Color, LocalizedText)[] appraiseLevels)
    {
        public readonly (float, Color, LocalizedText)[] AppraiseLevels = appraiseLevels;

        public static FairyIVAppraise FairyDamageAppraise = new FairyIVAppraise
            (
                new (float, Color, LocalizedText)[]
                {
                    (0.9f,Color.Gray,FairySystem.WeakLevel),
                    (1,FairySystem.VeryCommonLevel_Brown,FairySystem.VeryCommonLevel),
                    (1.15f,Color.White,FairySystem.CommonLevel),
                    (1.3f,Color.LawnGreen,FairySystem.UncommonLevel),
                    (1.45f,Color.DodgerBlue,FairySystem.RareLevel),
                    (1.6f,Color.Yellow,FairySystem.SpecialLevel),
                    (1.75f,Color.HotPink,FairySystem.UniqueLevel),
                    (float.MaxValue,Color.Orange,FairySystem.EternalLevel),
                }
            );

        public static FairyIVAppraise FairyDefenceAppraise = new FairyIVAppraise
            (
                new (float, Color, LocalizedText)[]
                {
                    (0.9f,Color.Gray,FairySystem.WeakLevel),
                    (1,FairySystem.VeryCommonLevel_Brown,FairySystem.VeryCommonLevel),
                    (1.1f,Color.White,FairySystem.CommonLevel),
                    (1.2f,Color.LawnGreen,FairySystem.UncommonLevel),
                    (1.3f,Color.DodgerBlue,FairySystem.RareLevel),
                    (1.4f,Color.Yellow,FairySystem.SpecialLevel),
                    (1.5f,Color.HotPink,FairySystem.UniqueLevel),
                    (float.MaxValue,Color.Orange,FairySystem.EternalLevel),
                }
            );

        public static FairyIVAppraise FairyLifeMaxAppraise = new FairyIVAppraise
            (
                new (float, Color, LocalizedText)[]
                {
                    (0.9f,Color.Gray,FairySystem.WeakLevel),
                    (1,FairySystem.VeryCommonLevel_Brown,FairySystem.VeryCommonLevel),
                    (1.25f,Color.White,FairySystem.CommonLevel),
                    (1.5f,Color.LawnGreen,FairySystem.UncommonLevel),
                    (1.75f,Color.DodgerBlue,FairySystem.RareLevel),
                    (2f,Color.Yellow,FairySystem.SpecialLevel),
                    (2.25f,Color.HotPink,FairySystem.UniqueLevel),
                    (float.MaxValue,Color.Orange,FairySystem.EternalLevel),
                }
            );

        public readonly (Color, LocalizedText) GetAppraiseResult(float magnification)
        {
            foreach (var level in AppraiseLevels)
            {
                if (magnification < level.Item1)
                    return (level.Item2, level.Item3);
            }

            return (AppraiseLevels[^1].Item2, AppraiseLevels[^1].Item3);
        }

        public readonly (Color, LocalizedText) GetAppraiseResult(float baseValue, float scaledValue)
        {
            if (baseValue == 0)
                return (Color.White, FairySystem.NotHaveLevel);
            return GetAppraiseResult(scaledValue / baseValue);
        }
    }
}
