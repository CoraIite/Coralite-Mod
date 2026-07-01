using Coralite.Content.WorldGeneration;
using Coralite.Content.WorldGeneration.WorldValues;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.NPCs.Crystalline
{
    public class SentinelSpawner : ModSystem
    {
        //public const int delayStart = 60 * 60 * 8;
        public const int respawnDelay = 60 * 60 * 6;
        private const int recheckStart = 60 * 60;

        public static double delay;
        public static double recheck;

        public override void PostUpdateTime()
        {
            if (VaultUtils.isClient)
                return;

            delay -= Main.desiredWorldEventsUpdateRate;
            if (delay < 0)
                delay = 0;

            recheck -= Main.desiredWorldEventsUpdateRate;
            if (recheck < 0)
                recheck = 0;

            if (delay == 0 && recheck == 0)
            {
                recheck = recheckStart;
                //if (NPC.AnyDanger())
                //    recheck *= 6;
                //else
                //{
                TrySpawning(CoraliteWorld.CrystallineSentinelAreaCenter.X, CoraliteWorld.CrystallineSentinelAreaCenter.Y);
                //}
            }
        }

        public static void SentinelKilled()
        {
            delay = respawnDelay;
        }

        public static void SpawnNow()
        {
            delay = 0;
            recheck = 0;
        }

        public static void TrySpawning(int x, int y)
        {
            if (CheckSentinel(x, y) && !WorldGen.PlayerLOS(x - 6, y) && !WorldGen.PlayerLOS(x + 6, y))
                NPC.NewNPC(new EntitySource_WorldEvent(), (x * 16) + 8, ((y - 4) * 16) - 8, ModContent.NPCType<FakeSentinel>());
        }

        private static bool CheckSentinel(int x, int y)
        {
            if (delay != 0 || !ModContent.GetInstance<CrystallineSkyIsland_PermissionFlag>().Value)
                return false;

            if (y < 7 )
                return false;

            if (NPC.AnyNPCs(ModContent.NPCType<FakeSentinel>())||NPC.AnyNPCs(ModContent.NPCType<CrystallineSentinel>()))
                return false;

            Vector2 center = new((x * 16) + 8, (y * 16) - 8);
            if (!CheckFloor(center))
                return false;

            return true;
        }

        public static bool CheckFloor(Vector2 Center)
        {
            Point point = Center.ToTileCoordinates();
            for (int i = -3; i <= 3; i++)
                for (int j = -5; j < 12; j++)
                {
                    int x = point.X + i;
                    int y = point.Y + j;
                    if ((WorldGen.SolidTile(x, y) || TileID.Sets.Platforms[Framing.GetTileSafely(x, y).TileType]) &&
                        (!Collision.SolidTiles(x - 1, x + 1, y - 3, y - 1)))
                    {
                        return true;
                    }
                }

            return false;
        }

    }

}
