using System.Collections.Generic;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        /// <summary>
        /// 挖挖挖的世界！
        /// </summary>
        public static bool DigDigDigWorld { get; set; }
        public static int DigDigDigWorldDungeonSide { get; set; }

        private static int CrystalCaveRadius { get => 58; }
        private static int GraniteMarbleRadius { get => 125; }
        private static int MushroomRadius { get => 230; }

        public void ModifyDigdigdigWorldGen(List<GenPass> tasks, ref double totalWeight)
        {
            tasks.Clear();

            //设置一些基础值
            tasks.Add(new PassLegacy("Coralite Dig Reset", DigReset));

            //放置一层石头背景板，并且随机生成背景墙以及泥沙块
            tasks.Add(new PassLegacy("Coralite Dig Stone Background", GenShoneBack));

            //生成丛林的基本样子
            tasks.Add(new PassLegacy("Coralite Dig Jungle", GenDigJungle));

            //生成雪地的基本样子
            tasks.Add(new PassLegacy("Coralite Dig Ice", GenDigIce));

            //生成沙漠的基本样子
            tasks.Add(new PassLegacy("Coralite Dig Desert", GenDigDesert));

            //生成地狱的基本样子
            tasks.Add(new PassLegacy("Coralite Dig Hell", GenDigHell));

            //生成天空的基本样子
            tasks.Add(new PassLegacy("Coralite Dig Sky", GenDigSky));


            //生成出生点的魔力水晶环
            tasks.Add(new PassLegacy("Coralite Dig Crystal Cave", GenDigCrystalCave));

            //生成大理石和花岗岩的环
            tasks.Add(new PassLegacy("Coralite Dig Granite Marble Circle", GenGMHalfCircle));

            //生成蘑菇草的环
            tasks.Add(new PassLegacy("Coralite Dig Mushroom Circle", GenMushroomCircle));

            //生成邪恶地狱
            tasks.Add(new PassLegacy("Coralite Dig Evil", GenDigEvil));

            //生成天空微光湖
            tasks.Add(new PassLegacy("Coralite Dig Shimmer", GenDigShimmer));

            //生成丛林神庙
            tasks.Add(new PassLegacy("Coralite Dig Jungle Temple", GenDigJungleTemple));

            //生成冰龙巢
            tasks.Add(new PassLegacy("Coralite Dig Ice Dragon Nest", GenDigIceDragonNest));

            //生成地牢
            tasks.Add(new PassLegacy("Coralite Dig Dungeon", GenGenDigDungeon));


            //生成NPC
            tasks.Add(new PassLegacy("Coralite Dig NPC", SpawnDigNPC));

            //最后清理
            tasks.Add(new PassLegacy("Coralite Dig Clear", GenDigClear));
        }
    }
}
