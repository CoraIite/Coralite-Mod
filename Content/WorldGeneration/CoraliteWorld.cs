using Coralite.Content.Walls.Magike;
using Coralite.Content.WorldGeneration.Generators;
using Microsoft.Xna.Framework.Graphics;
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
        /// 用于清除物块的字典
        /// </summary>
        internal static Dictionary<Color, int> clearDic = new()
        {
            [Color.White] = -2,
            [Color.Black] = -1
        };

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

            //添加蕴魔空岛
            AddGenPass(tasks, VanillaGenPassName.FinalCleanup, "Coralite Crystalline Sky Island", GenCrystallineSkyIsland);

            //添加冰龙巢穴
            AddGenPass(tasks, VanillaGenPassName.Lakes, "Coralite Ice Dragon Nest", GenIceDragonNest);

            //放置风石碑牌
            AddGenPass(tasks, VanillaGenPassName.PlaceFallenLog, "Coralite Wind Stone Tablet", GenWindStoneTablet);

            //在箱子中塞入更多战利品
            AddGenPass(tasks, VanillaGenPassName.FinalCleanup, "Coralite Replase Vanilla Chest", ReplaceVanillaChest);

            //珊瑚笔记
            AddGenPass(tasks, VanillaGenPassName.FinalCleanup, "Coralite Note Room", GenCoraliteNoteRoom, 0);

            //符文之歌相关地形
            AddGenPass(tasks, VanillaGenPassName.FinalCleanup, "CoreKeeper Clear Gemstone Maze", GenClearGemstoneMaze, 0);
            AddGenPass(tasks, VanillaGenPassName.FinalCleanup, "CoreKeeper Chipped Blade Temple", GenChippedBladeTemple, 0);



            //if (ShiniesIndex != -1)
            //{
            //    tasks.Insert(ShiniesIndex - 1, new PassLegacy("Coralite Basalt Small Biome", GenBasaltSmallBiome));
            //    ShiniesIndex++;
            //    tasks.Insert(ShiniesIndex - 1, new PassLegacy("Coralite Magic Crystal Cave", GenMagicCrystalCave));
            //}

            //int EvilBiome = tasks.FindIndex(genpass => genpass.Name.Equals("Corruption"));
            //if (EvilBiome != -1)
            //{
            //    tasks.Insert(EvilBiome + 1, new PassLegacy("Coralite Evil Chest", GenEvilChest));
            //}

            //int Jungle = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle"));

            //if (Jungle != -1)
            //{
            //    tasks.Insert(Jungle + 1, new PassLegacy("Coralite Crystalline Sky Island", GenCrystallineSkyIsland));
            //}

            int Dungeon = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
            bool shadowCastle = ShadowCastle;
            if (shadowCastle)
            {
                tasks.RemoveAt(Dungeon);
            }

            //int Lakes = tasks.FindIndex(genpass => genpass.Name.Equals("Lakes"));
            //if (IceBiomeIndex != -1)
            //{
            //    tasks.Insert(Lakes + 1, new PassLegacy("Coralite Ice Dragon Nest", GenIceDragonNest));
            //}

            //int MudsWallsInJungle = tasks.FindIndex(genpass => genpass.Name.Equals("Muds Walls In Jungle"));
            //if (MudsWallsInJungle!=-1)
            //{
            //}

            //int PlaceFallenLog = tasks.FindIndex(genpass => genpass.Name.Equals("Place Fallen Log"));
            //if (PlaceFallenLog != -1)
            //{
            //    tasks.Insert(PlaceFallenLog + 1, new PassLegacy("Coralite Wind Stone Tablet", GenWindStoneTablet));
            //}

            int FinalCleanup = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
            if (FinalCleanup != -1)
            {
                //tasks.Insert(FinalCleanup + 1, new PassLegacy("Coralite Replase Vanilla Chest", ReplaceVanillaChest));
                if (shadowCastle)
                    tasks.Insert(FinalCleanup - 1, new PassLegacy("Coralite Shadow Castle", GenShadowCastle));

                //FinalCleanup = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
                //tasks.Insert(FinalCleanup, new PassLegacy("Coralite Note Room", GenCoraliteNoteRoom));
                //FinalCleanup++;
                //tasks.Insert(FinalCleanup, new PassLegacy("CoreKeeper Clear Gemstone Maze", GenClearGemstoneMaze));
                //FinalCleanup++;
                //tasks.Insert(FinalCleanup, new PassLegacy("CoreKeeper Chipped Blade Temple", GenChippedBladeTemple));
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

            if (DigDigDigWorld)
                ModifyDigdigdigWorldGen(tasks, ref totalWeight);
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

        public static void GenByTexture(Texture2D clearTex, Texture2D mainTex, Texture2D wallClearTex, Texture2D wallTex,
            Dictionary<Color, int> clearDic, Dictionary<Color, int> roomDic, Dictionary<Color, int> wallClearDic, Dictionary<Color, int> wallDic,
            int genOrigin_x, int genOrigin_y)
        {
            bool genned = false;
            bool placed = false;

            WorldGenHelper.ClearLiuid(genOrigin_x, genOrigin_y, clearTex.Width, clearTex.Height);

            Texture2TileGenerator clearGenerator = null;
            Texture2TileGenerator roomGenerator = null;
            Texture2WallGenerator wallClearGenerator = null;
            Texture2WallGenerator wallGenerator = null;

            while (!genned)
            {
                if (placed)
                    continue;

                Main.QueueMainThreadAction(() =>
                {
                    //清理范围
                    clearGenerator = TextureGeneratorDatas.GetTex2TileGenerator(clearTex, clearDic);

                    //生成主体地形
                    roomGenerator = TextureGeneratorDatas.GetTex2TileGenerator(mainTex, roomDic);

                    //清理范围
                    if (wallClearTex != null)
                        wallClearGenerator = TextureGeneratorDatas.GetTex2WallGenerator(wallClearTex, wallClearDic);

                    //生成墙壁
                    if (wallTex != null)
                        wallGenerator = TextureGeneratorDatas.GetTex2WallGenerator(wallTex, wallDic);

                    genned = true;
                });
                placed = true;
            }

            clearGenerator?.Generate(genOrigin_x, genOrigin_y, true);
            roomGenerator?.Generate(genOrigin_x, genOrigin_y, true);
            wallClearGenerator?.Generate(genOrigin_x, genOrigin_y, true);
            wallGenerator?.Generate(genOrigin_x, genOrigin_y, true);
        }

        public static void GenLiquidByTexture(Texture2D liquidTex, Dictionary<Color, int> liquidDic, Point topLeft)
        {
            bool genned = false;
            bool placed = false;

            Texture2Liquid liquidGenerator = null;

            while (!genned)
            {
                if (placed)
                    continue;

                Main.QueueMainThreadAction(() =>
                {
                    //生成液体
                    if (liquidTex != null)
                        liquidGenerator = TextureGeneratorDatas.GetTex2LiquidGenerator(liquidTex, liquidDic);

                    genned = true;
                });
                placed = true;
            }

            liquidGenerator?.Generate(topLeft.X, topLeft.Y, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="genPassList"></param>
        /// <param name="findVanillaIndex"></param>
        /// <param name="genPassName"></param>
        /// <param name="method"></param>
        /// <param name="addToIndex">1 表示插在后面，0 表示插在前面</param>
        public void AddGenPass(List<GenPass> genPassList, string findVanillaIndex, string genPassName, WorldGenLegacyMethod method, int addToIndex = 1)
        {
            int index = genPassList.FindIndex(genpass => genpass.Name.Equals(findVanillaIndex));
            if (index != -1)
                genPassList.Insert(index + addToIndex, new PassLegacy(genPassName, method));
        }
    }
}
