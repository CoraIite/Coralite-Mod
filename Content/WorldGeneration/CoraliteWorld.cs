using System.Collections.Generic;
using Terraria.GameContent.Generation;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
            int DesertIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
            int IceBiomeIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Generate Ice Biome"));

            if (ShiniesIndex != -1)
            {
                tasks.Insert(ShiniesIndex - 1, new PassLegacy("Coralite Magic Crystal Cave", GenMagicCrystalCave));
            }

            int EvilBiome = tasks.FindIndex(genpass => genpass.Name.Equals("Corruption"));
            if (EvilBiome != -1)
            {
                tasks.Insert(EvilBiome + 1, new PassLegacy("Coralite Evil Chest", GenEvilChest));
            }

            int Dungeon = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
            if (ShadowCastle)
            {
                tasks.RemoveAt(Dungeon);
                tasks.Insert(Dungeon, new PassLegacy("Coralite Shadow Castle", GenShadowCastle));
            }

            int Lakes = tasks.FindIndex(genpass => genpass.Name.Equals("Lakes"));
            if (IceBiomeIndex != -1)
            {
                tasks.Insert(Lakes + 1, new PassLegacy("Coralite Ice Dragon Nest", GenIceDragonNest));
            }

            int FinalCleanup = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
            if (FinalCleanup != -1)
            {
                tasks.Insert(FinalCleanup + 1, new PassLegacy("Coralite Replase Vanilla Chest", ReplaceVanillaChest));
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Add("IceNestCenterX", NestCenter.X);
            tag.Add("IceNestCenterY", NestCenter.Y);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            NestCenter.X = tag.Get<int>("IceNestCenterX");
            NestCenter.Y = tag.Get<int>("IceNestCenterY");
        }
    }
}
