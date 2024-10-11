using Coralite.Content.WorldGeneration;
using Terraria;

namespace Coralite.Content.CustomHooks
{
    public class DigDigDigBiomes:HookGroup
    {
        public override void Load()
        {
            On_Player.UpdateBiomes += On_Player_UpdateBiomes;
        }

        public override void Unload()
        {
            On_Player.UpdateBiomes -= On_Player_UpdateBiomes;
        }

        private void On_Player_UpdateBiomes(On_Player.orig_UpdateBiomes orig, Player self)
        {
            orig.Invoke(self);

            if (CoraliteWorld.DigDigDigWorld)
            {
                self.ZoneBeach = self.Center.Y / 16 < Main.maxTilesY * 0.2f;
                if (CoraliteWorld.DigDigDigWorldDungeonSide > 0)
                {
                    self.ZoneOverworldHeight = self.Center.X / 16 > Main.maxTilesX - Main.maxTilesX / 14;
                    self.ZoneSkyHeight = self.Center.X / 16 > Main.maxTilesX - Main.maxTilesX / 28;
                }
                else
                {
                    self.ZoneOverworldHeight = self.Center.X / 16 < Main.maxTilesX / 14;
                    self.ZoneSkyHeight = self.Center.X / 16 < Main.maxTilesX / 28;
                }
            }
        }
    }
}
