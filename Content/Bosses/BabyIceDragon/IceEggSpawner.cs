using Coralite.Content.WorldGeneration;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IceEggSpawner : ModSystem
    {
        //public const int delayStart = 60 * 60 * 8;
        public const int respawnDelay = 60 * 60 * 4;
        private const int timePerEgg = 60 * 60;
        private const int recheckStart = 60 * 10;

        public static double delay;
        public static double recheck;

        public override void PostUpdateTime()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
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
                    TrySpawning(CoraliteWorld.NestCenter.X, CoraliteWorld.NestCenter.Y);
                //}
            }
        }

        public static void BabyIceDragonSlain()
        {
            delay = timePerEgg;
        }

        public static void EggDestroyed()
        {
            delay = respawnDelay;
        }

        public static void TrySpawning(int x, int y)
        {
            if (CheckEgg(x, y) && !WorldGen.PlayerLOS(x - 6, y) && !WorldGen.PlayerLOS(x + 6, y))
                NPC.NewNPC(new EntitySource_WorldEvent(), (x * 16) + 8, ((y - 4) * 16) - 8, ModContent.NPCType<IceDragonEgg>());
        }

        private static bool CheckEgg(int x, int y)
        {
            if (delay != 0 || !NPC.downedBoss1)
                return false;

            if (y < 7 || WorldGen.SolidTile(Main.tile[x, y - 7]))
                return false;

            if (NPC.AnyNPCs(ModContent.NPCType<IceDragonEgg>()))
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
