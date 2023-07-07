using Coralite.Content.Tiles.Magike;
using Coralite.Core;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Biomes
{
    public class MagicCrystalCave : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("Coralite/MagicCrystalCaveBackground");

        public override bool IsBiomeActive(Player player)
        {
            bool b1 = ModContent.GetInstance<MagicCrystalCaveTileCount>().BasaltTileCount >= 120;

            bool b2 = Math.Abs(player.position.ToTileCoordinates().X - Main.maxTilesX / 2) < Main.maxTilesX / 4;

            bool b3 = player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight;

            return b1 && b2 && b3;
        }
    }

    public class MaficCrystalCaveEffect : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("Coralite/MagicCrystalCaveBackground");

        public override bool IsSceneEffectActive(Player player)
        {
            bool b1 = ModContent.GetInstance<MagicCrystalCaveTileCount>().BasaltTileCount >= 120;

            bool b2 = Math.Abs(player.position.ToTileCoordinates().X - Main.maxTilesX / 2) < Main.maxTilesX / 4;

            bool b3 = player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight;

            return b1 && b2 && b3;
        }
    }

    public class MagicCrystalCaveBackground : ModUndergroundBackgroundStyle
    {
        public override void FillTextureArray(int[] textureSlots)
        {
            textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot(AssetDirectory.Backgrounds + "MagicCrystalCaveBackground0");
            textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot(AssetDirectory.Backgrounds + "MagicCrystalCaveBackground1");
            textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot(AssetDirectory.Backgrounds + "MagicCrystalCaveBackground2");
            textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot(AssetDirectory.Backgrounds + "MagicCrystalCaveBackground3");
            textureSlots[4] = BackgroundTextureLoader.GetBackgroundSlot(AssetDirectory.Backgrounds + "MagicCrystalCaveBackground2");
        }
    }

    public class MagicCrystalCaveTileCount : ModSystem
    {
        public int BasaltTileCount;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            BasaltTileCount = tileCounts[ModContent.TileType<BasaltTile>()];
        }
    }

}
