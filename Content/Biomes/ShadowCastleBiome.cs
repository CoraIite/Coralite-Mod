using Coralite.Core;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Biomes
{
    public class ShadowCastleBiome : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/temp_ShadowCastle");

        public override bool IsBiomeActive(Player player)
        {
            bool b1 = CoraliteSets.WallShadowCastle[Framing.GetTileSafely(player.position.ToTileCoordinates()).WallType];

            bool b2 = player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight;

            return b1 && b2 && b2;
        }

    }
}
