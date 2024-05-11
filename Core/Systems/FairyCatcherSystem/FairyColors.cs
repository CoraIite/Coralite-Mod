using System.Collections.Generic;
using FairyRarity = Coralite.Core.Systems.FairyCatcherSystem.FairyAttempt.Rarity;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public partial class FairySystem
    {
        public static Color VeryCommonLevel_Brown = new Color(143, 86, 59);

        public static Dictionary<FairyRarity, Color> RarityColors = new Dictionary<FairyRarity, Color>
        {
                { FairyRarity.C, new Color(255, 244, 279) },
                { FairyRarity.U, Color.LawnGreen },
                { FairyRarity.R, new Color(122, 233, 255) },
                { FairyRarity.RR, Color.DodgerBlue },
                { FairyRarity.SR, new Color(255, 13, 57) },
                { FairyRarity.UR, Color.Orange },
                { FairyRarity.RRR, new Color(138, 128, 255) },
                { FairyRarity.HR, new Color(255, 160, 209) },
                { FairyRarity.AR, Color.Orchid },
                { FairyRarity.MR, Color.Gold },
        };

        //----------------------------------------------
        //  以下注释保留，为了能大致知道对应的是个什么颜色
        //----------------------------------------------

        //public static Color RarityC_LiteYellow = new Color(255, 244, 279);
        //public static Color RarityU_LawnGreen = Color.LawnGreen;//new Color(77, 255, 163);
        //public static Color RarityR_SkyBlue = new Color(122, 233, 255);
        //public static Color RarityRR_Blue = Color.DodgerBlue;//new Color(0, 136, 255);
        //public static Color RaritySR_Red = new Color(255, 13, 57);
        //public static Color RarityUR_Orange = Color.Orange;

        //public static Color RarityRRR_BluePurple = new Color(138, 128, 255);
        //public static Color RarityHR_Pink = new Color(255, 160, 209);
        //public static Color RarityAR_Orchid = Color.Orchid;//new Color(191, 255, 97);
        //public static Color RarityMR_Gold = Color.Gold;
        public static Color RaritySP_Purple = new Color(215, 177, 255);
    }
}
