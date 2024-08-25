using Coralite.Content.Items.Magike.PolarizedFilters;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem : ModSystem
    {
        public static MagikeSystem Instance { get; private set; }

        public static bool DrawSpecialTileText;
        public static string SpecialTileText;

        public MagikeSystem()
        {
            Instance = this;
        }
         
        /// <summary> 是否学习过 书页：魔能基础 </summary>
        public static bool learnedMagikeBase;
        /// <summary> 是否学习过卷轴：强化魔能提炼 </summary>
        public static bool learnedMagikeAdvanced;

        public override void PostAddRecipes()
        {
            if (Main.dedServ)
                return;

            RegisterPolymerize();
        }

        public override void Load()
        {
            LoadLocalization();
            LoadAssets();
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            UnloadLocalization();
            UnloadAssets();

            magikeCraftRecipes?.Clear();
            magikeCraftRecipes = null;
            MagikeCraftRecipes = null;
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (DrawSpecialTileText)
            {
                DrawSpecialTileText = false;
                UICommon.TooltipMouseText(SpecialTileText);
            }
        }

        public static Color GetColor(MagikeApparatusLevel level)
        {
            return level switch
            {
                MagikeApparatusLevel.MagicCrystal => Coralite.MagicCrystalPink,
                MagikeApparatusLevel.Glistent => Coralite.GlistentGreen,
                MagikeApparatusLevel.Crimson => Coralite.CrimsonRed,
                MagikeApparatusLevel.Corruption => Coralite.CorruptionPurple,
                MagikeApparatusLevel.Icicle => Coralite.IcicleCyan,
                MagikeApparatusLevel.Shadow => Coralite.ShadowPurple,
                MagikeApparatusLevel.CrystallineMagike => Coralite.CrystallineMagikePurple,
                MagikeApparatusLevel.Hallow => Coralite.HallowYellow,
                MagikeApparatusLevel.Soul => Coralite.SoulCyan,
                MagikeApparatusLevel.Feather => Coralite.FeatherLime,
                MagikeApparatusLevel.HolyLight => Main.DiscoColor,
                MagikeApparatusLevel.SplendorMagicore => Coralite.SplendorMagicoreLightBlue,
                _ => Color.Gray,
            };
        }

        public static int GetPolarizedFilterItemType(MagikeApparatusLevel level)
        {
            return level switch
            {
                MagikeApparatusLevel.MagicCrystal => ModContent.ItemType<MagicCrystalPolarizedFilter>(),
                MagikeApparatusLevel.Glistent => ModContent.ItemType<GlistentPolarizedFilter>(),
                MagikeApparatusLevel.Crimson => ModContent.ItemType<CrimsonPolarizedFilter>(),
                MagikeApparatusLevel.Corruption => ModContent.ItemType<CorruptionPolarizedFilter>(),
                MagikeApparatusLevel.Icicle => ModContent.ItemType<IciclePolarizedFilter>(),
                MagikeApparatusLevel.Shadow => ModContent.ItemType<ShadowPolarizedFilter>(),
                MagikeApparatusLevel.CrystallineMagike => ModContent.ItemType<CrystallineMagikePolarizedFilter>(),
                MagikeApparatusLevel.Hallow => ModContent.ItemType<HallowPolarizedFilter>(),
                MagikeApparatusLevel.Soul => ModContent.ItemType<SoulPolarizedFilter>(),
                MagikeApparatusLevel.Feather => ModContent.ItemType<FeatherPolarizedFilter>(),
                MagikeApparatusLevel.HolyLight => ModContent.ItemType<HolyLightPolarizedFilter>(),
                MagikeApparatusLevel.SplendorMagicore => ModContent.ItemType<SplendorMagicorePolarizedFilter>(),
                MagikeApparatusLevel.Seashore => ModContent.ItemType<SeashorePolarizedFilter>(),
                MagikeApparatusLevel.Eiderdown => ModContent.ItemType<EiderdownPolarizedFilter>(),
                MagikeApparatusLevel.Emperor => ModContent.ItemType<EmperorPolarizedFilter>(),
                MagikeApparatusLevel.Pelagic => ModContent.ItemType<PelagicPolarizedFilter>(),
                MagikeApparatusLevel.Flight => ModContent.ItemType<FlightPolarizedFilter>(),
                MagikeApparatusLevel.Hellstone => ModContent.ItemType<HellstonePolarizedFilter>(),
                MagikeApparatusLevel.Quicksand => ModContent.ItemType<QuicksandPolarizedFilter>(),
                MagikeApparatusLevel.Forbidden => ModContent.ItemType<ForbiddenPolarizedFilter>(),
                MagikeApparatusLevel.EternalFlame => ModContent.ItemType<EternalFlamePolarizedFilter>(),
                _ => 0,
            };
        }

        //额...每增加一个变量就要在这边多写一段，说实话显得很蠢，以后有机会需要修改掉
        public override void SaveWorldData(TagCompound tag)
        {
            List<string> Knowledge = new();
            if (learnedMagikeBase)
                Knowledge.Add("learnedMagikeBase");
            if (learnedMagikeAdvanced)
                Knowledge.Add("learnedMagikeAdvanced");

            tag.Add("ConnectLineType", (int)CurrentConnectLineType);

            SaveData_2_1(Knowledge);

            tag.Add("Knowledge", Knowledge);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            IList<string> list = tag.GetList<string>("Knowledge");
            learnedMagikeBase = list.Contains("learnedMagikeBase");
            learnedMagikeAdvanced = list.Contains("learnedMagikeAdvanced");

            CurrentConnectLineType = (ConnectLineType)tag.GetInt("ConnectLineType");

            LoadData_2_1(list);
        }
    }
}
