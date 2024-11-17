﻿using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.Generation;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld : ModSystem, ILocalizedModType
    {
        public string LocalizationCategory => "WorldGeneration";

        public const string DigDigDigSaveKey = "digdigdig";

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

            if (ShiniesIndex != -1)
            {
                tasks.Insert(ShiniesIndex - 1, new PassLegacy("Coralite Basalt Small Biome", GenBasaltSmallBiome));
                ShiniesIndex++;
                tasks.Insert(ShiniesIndex - 1, new PassLegacy("Coralite Magic Crystal Cave", GenMagicCrystalCave));
                ShiniesIndex++;
                tasks.Insert(ShiniesIndex + 1, new PassLegacy("CoreKeeper Clear Gemstone Maze", GenClearGemstoneMaze));
                tasks.Insert(ShiniesIndex + 2, new PassLegacy("CoreKeeper Chipped Blade Temple", GenChippedBladeTemple));
            }

            int EvilBiome = tasks.FindIndex(genpass => genpass.Name.Equals("Corruption"));
            if (EvilBiome != -1)
            {
                tasks.Insert(EvilBiome + 1, new PassLegacy("Coralite Evil Chest", GenEvilChest));
            }

            int Jungle = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle"));

            if (Jungle != -1)
            {
                tasks.Insert(Jungle + 1, new PassLegacy("Coralite Crystalline Sky Island", GenCrystallineSkyIsland));
            }

            int Dungeon = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
            bool shadowCastle = ShadowCastle;
            if (shadowCastle)
            {
                tasks.RemoveAt(Dungeon);
            }

            int Lakes = tasks.FindIndex(genpass => genpass.Name.Equals("Lakes"));
            if (IceBiomeIndex != -1)
            {
                tasks.Insert(Lakes + 1, new PassLegacy("Coralite Ice Dragon Nest", GenIceDragonNest));
            }

            //int MudsWallsInJungle = tasks.FindIndex(genpass => genpass.Name.Equals("Muds Walls In Jungle"));
            //if (MudsWallsInJungle!=-1)
            //{
            //}

            int PlaceFallenLog = tasks.FindIndex(genpass => genpass.Name.Equals("Place Fallen Log"));
            if (PlaceFallenLog != -1)
            {
                tasks.Insert(PlaceFallenLog + 1, new PassLegacy("Coralite Wind Stone Tablet", GenWindStoneTablet));
            }

            int FinalCleanup = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
            if (FinalCleanup != -1)
            {
                tasks.Insert(FinalCleanup + 1, new PassLegacy("Coralite Replase Vanilla Chest", ReplaceVanillaChest));
                if (shadowCastle)
                    tasks.Insert(FinalCleanup - 1, new PassLegacy("Coralite Shadow Castle", GenShadowCastle));
            }

            if (CoralCatWorld)
            {
                int SettleLiquids = tasks.FindIndex(genpass => genpass.Name.Equals("Settle Liquids Again"));
                int FinalCleanup2 = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));

                if (SettleLiquids != -1)
                    tasks.Insert(SettleLiquids - 1, new PassLegacy("Coralite CoralCat World", CoralCatWorldGen));
                if (SettleLiquids != -1)
                    tasks.Insert(FinalCleanup2 - 1, new PassLegacy("Coralite CoralCat World Spawn", CoralCatWorldSpawn));
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
    }
}
