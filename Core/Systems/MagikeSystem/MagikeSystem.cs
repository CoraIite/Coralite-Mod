using Coralite.Content.Items.Magike;
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

            RegisterMagikeCraft();
            LoadPolarizeFilter();
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
            UnLoadPolarizeFilter() ;

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
