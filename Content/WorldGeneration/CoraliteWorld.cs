using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld : ModSystem, ILocalizedModType
    {
        public string LocalizationCategory => "WorldGeneration";

        public const string DigDigDigSaveKey = "digdigdig";

        /// <summary>
        /// 用于生成液体的字典
        /// </summary>
        internal static Dictionary<Color, int> liquidDic = new()
        {
            [Color.Black] = -1,
            [Color.White] = LiquidID.Water,
            [new Color(255, 0, 0)] = LiquidID.Lava,
            [new Color(255, 255, 0)] = LiquidID.Honey,
            [new Color(255, 0, 255)] = LiquidID.Shimmer,
        };

        #region 加载文字

        public override void Load()
        {
            Type t = typeof(CoraliteWorld);

            var infos = t.GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            foreach (var info in from info in infos
                                 where info.PropertyType.Name == nameof(LocalizedText)
                                 select info)
            {
                info.SetValue(null, this.GetLocalization(info.Name));
            }
        }

        public override void Unload()
        {
            Type t = typeof(CoraliteWorld);

            var infos = t.GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            foreach (var info in from info in infos
                                 where info.PropertyType.Name == nameof(LocalizedText)
                                 select info)
            {
                info.SetValue(null, null);
            }
        }

        #endregion

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            if (chaosWorld)
            {
                GenChaosWorld(tasks);
                return;
            }

            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
            int DesertIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
            int IceBiomeIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Generate Ice Biome"));

            //添加魔力水晶洞
            AddGenPass(tasks, VanillaGenPassName.Shinies, "Coralite Basalt Small Biome", GenBasaltSmallBiome, 0);
            AddGenPass(tasks, VanillaGenPassName.Shinies, "Coralite Magic Crystal Cave", GenMagicCrystalCave, 0);

            //添加邪恶箱子地形
            AddGenPass(tasks, VanillaGenPassName.Corruption, "Coralite Evil Chest", GenEvilChest);

            //在箱子中塞入更多战利品
            AddGenPass(tasks, VanillaGenPassName.FinalCleanup, "Coralite Replase Vanilla Chest", ReplaceVanillaChest);

            //珊瑚笔记
            AddGenPass(tasks, VanillaGenPassName.FinalCleanup, "Coralite Note Room", GenCoraliteNoteRoom, 0);

            //符文之歌相关地形
            AddGenPass(tasks, VanillaGenPassName.FinalCleanup, "CoreKeeper Clear Gemstone Maze", GenClearGemstoneMaze, 0);
            AddGenPass(tasks, VanillaGenPassName.FinalCleanup, "CoreKeeper Chipped Blade Temple", GenChippedBladeTemple, 0);

            //添加蕴魔空岛
            AddGenPass(tasks, VanillaGenPassName.FinalCleanup, "Coralite Crystalline Sky Island", GenCrystallineSkyIsland);

            //添加冰龙巢穴
            AddGenPass(tasks, VanillaGenPassName.Lakes, "Coralite Ice Dragon Nest", GenIceDragonNest);

            //放置风石碑牌
            AddGenPass(tasks, VanillaGenPassName.PlaceFallenLog, "Coralite Wind Stone Tablet", GenWindStoneTablet);

            //int settleLiquids = tasks.FindIndex(genpass => genpass.Name.Equals("Settle Liquids"));
            //if (settleLiquids != -1)
            //{
            //    tasks.Insert(settleLiquids + 1, new PassLegacy("MyStructure", WorldGenTester.ExampleStructure));
            //}

            int Dungeon = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
            bool shadowCastle = ShadowCastle;
            if (shadowCastle)
            {
                tasks.RemoveAt(Dungeon);
            }

            int FinalCleanup = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
            if (FinalCleanup != -1)
            {
                if (shadowCastle)
                    tasks.Insert(FinalCleanup - 1, new PassLegacy("Coralite Shadow Castle", GenShadowCastle));
            }

            if (CoralCatWorld)
            {
                int SettleLiquids = tasks.FindIndex(genpass => genpass.Name.Equals("Settle Liquids Again"));

                if (SettleLiquids != -1)
                    tasks.Insert(SettleLiquids - 1, new PassLegacy("Coralite CoralCat World", CoralCatWorldGen));

                int FinalCleanup2 = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));

                if (FinalCleanup2 != -1)
                    tasks.Insert(FinalCleanup2, new PassLegacy("Coralite CoralCat World Spawn", CoralCatWorldSpawn));
            }

            //if (DigDigDigWorld)
            //    ModifyDigdigdigWorldGen(tasks, ref totalWeight);
        }

        public override void ModifyHardmodeTasks(List<GenPass> list)
        {
            if (DigDigDigWorld)
                DigDigDigHardMode(list);
        }

        public override void SaveWorldHeader(TagCompound tag)
        {
            if (DigDigDigWorld)
            {
                tag.Add(DigDigDigSaveKey, true);
            }
        }

        #region 存储

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Add("IceNestCenterX", NestCenter.X);
            tag.Add("IceNestCenterY", NestCenter.Y);

            tag.Add("shadowBallsFightAreaX", shadowBallsFightArea.X);
            tag.Add("shadowBallsFightAreaY", shadowBallsFightArea.Y);
            tag.Add("chaosWorld", chaosWorld);
            tag.Add("coralCat", CoralCatWorld);
            tag.Add(DigDigDigSaveKey, DigDigDigWorld);

            if (DigDigDigWorld)
            {
                tag.Add("digdigdigDungeonSide", DigDigDigWorldDungeonSide);
            }

            SaveSkyIsland(tag);
            SaveCrystalCave(tag);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            NestCenter.X = tag.Get<int>("IceNestCenterX");
            NestCenter.Y = tag.Get<int>("IceNestCenterY");

            shadowBallsFightArea = new Rectangle(
                tag.Get<int>("shadowBallsFightAreaX"),
                tag.Get<int>("shadowBallsFightAreaY"), 74 * 16, 59 * 16);

            chaosWorld = tag.Get<bool>("chaosWorld");
            CoralCatWorld = tag.Get<bool>("coralCat");
            DigDigDigWorld = tag.Get<bool>(DigDigDigSaveKey);

            if (DigDigDigWorld)
            {
                DigDigDigWorldDungeonSide = tag.Get<int>("DigDigDigWorldDungeonSide");
            }

            LoadSkyIsland(tag);
            LoadCrystalCave(tag);
        }

        #endregion

        public static T ValueByWorldSize<T>(T smallWorld, T middleWorld, T bigWorld)
        {
            return Main.maxTilesX switch
            {
                //小世界
                < 6000 => smallWorld,
                //中世界
                > 6000 and < 8000 => middleWorld,
                //大世界
                _ => bigWorld
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="genPassList"></param>
        /// <param name="findVanillaIndex"></param>
        /// <param name="genPassName"></param>
        /// <param name="method"></param>
        /// <param name="addToIndex">1 表示插在后面，0 表示插在前面</param>
        public static void AddGenPass(List<GenPass> genPassList, string findVanillaIndex, string genPassName, WorldGenLegacyMethod method, int addToIndex = 1)
        {
            int index = genPassList.FindIndex(genpass => genpass.Name.Equals(findVanillaIndex));
            if (index != -1)
                genPassList.Insert(index + addToIndex, new PassLegacy(genPassName, method));
        }
    }
}
