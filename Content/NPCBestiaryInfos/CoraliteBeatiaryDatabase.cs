//using Coralite.Core;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria.GameContent.Bestiary;

//namespace Coralite.Content.NPCBestiaryInfos
//{
//    public class CoraliteBeatiaryDatabase : ModSystem, ILocalizedModType
//    {
//        public string LocalizationCategory => "Systems";

//        public static class Biomes
//        {
//            /// <summary>
//            /// 蕴魔空岛
//            /// </summary>
//            public static CoraliteSpawnConditionBestiaryInfo CrystallineSkyIsland;
//        }

//        public override void Load()
//        {
//            this.GetLocalization(nameof(Biomes.CrystallineSkyIsland));
//            Biomes.CrystallineSkyIsland
//                = new(this.GetLocalizationKey(nameof(Biomes.CrystallineSkyIsland))
//                , AssetDirectory.Biomes + "CrystallineSkyIslandIcon", AssetDirectory.Backgrounds + "CrystallineSkyIslandMap");
//        }
//    }
//}
