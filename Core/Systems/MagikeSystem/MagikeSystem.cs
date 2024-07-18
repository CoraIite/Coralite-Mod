﻿using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem : ModSystem
    {
        public static MagikeSystem Instance { get; private set; }

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

            RegisterRemodel();
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

            remodelRecipes?.Clear();
            remodelRecipes = null;

            polymerizeRecipes?.Clear();
            polymerizeRecipes = null;
        }

        //额...每增加一个变量就要在这边多写一段，说实话显得很蠢，以后有机会需要修改掉
        public override void SaveWorldData(TagCompound tag)
        {
            List<string> Knowledge = new List<string>();
            if (learnedMagikeBase)
                Knowledge.Add("learnedMagikeBase");
            if (learnedMagikeAdvanced)
                Knowledge.Add("learnedMagikeAdvanced");

            SaveData_2_1(Knowledge);

            tag.Add("Knowledge", Knowledge);

        }

        public override void LoadWorldData(TagCompound tag)
        {
            IList<string> list = tag.GetList<string>("Knowledge");
            learnedMagikeBase = list.Contains("learnedMagikeBase");
            learnedMagikeAdvanced = list.Contains("learnedMagikeAdvanced");

            LoadData_2_1(list);
        }
    }
}
