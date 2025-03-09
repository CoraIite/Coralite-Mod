using Coralite.Core;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Biomes
{
    public class CrystallineSkyIsland : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Temp_CrystallineSkyIsland");

        //public override string BestiaryIcon => AssetDirectory.Biomes + "MagicCrystalCaveIcon";

        public override ModWaterStyle WaterStyle => ModContent.GetInstance<CrystallineSkyIslandWaterStyle>();

        public override bool IsBiomeActive(Player player)
        {
            bool b1 = ModContent.GetInstance<CoraliteTileCount>().CrystallineSkyIslandTileCount >= 400;
            bool b2 = player.Center.Y / 16 < Main.worldSurface * 0.8f;

            return b1 && b2;
        }
    }

    public class CrystallineSkyIslandEffect : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        //public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("Coralite/MagicCrystalCaveBackground");

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Temp_CrystallineSkyIsland");

        public override ModWaterStyle WaterStyle => ModContent.GetInstance<CrystallineSkyIslandWaterStyle>();

        public override bool IsSceneEffectActive(Player player)
        {
            bool b1 = ModContent.GetInstance<CoraliteTileCount>().CrystallineSkyIslandTileCount >= 400;
            bool b2 = player.Center.Y / 16 < Main.worldSurface * 0.8f;

            return b1 && b2;
        }
    }

    public class CrystallineSkyIslandWaterfallStyle : ModWaterfallStyle
    {
        public override string Texture => AssetDirectory.Biomes + Name;

        // Makes the waterfall provide light
        public override void AddLight(int i, int j) =>
            Lighting.AddLight(new Vector2(i, j).ToWorldCoordinates(), new Vector3(0.2f, 0.3f, 0.3f));
    }

    public class CrystallineSkyIslandWaterStyle : ModWaterStyle
    {
        public override string Texture => AssetDirectory.Biomes + Name;

        //private Asset<Texture2D> rainTexture;
        //public override void Load()
        //{
        //    rainTexture = Mod.Assets.Request<Texture2D>("Content/Biomes/ExampleRain");
        //}

        public override int ChooseWaterfallStyle()
        {
            return ModContent.GetInstance<CrystallineSkyIslandWaterfallStyle>().Slot;
        }

        public override int GetSplashDust()
        {
            return DustID.Water_Jungle;// ModContent.DustType<ExampleSolution>();
        }

        public override int GetDropletGore()
        {
            return ModContent.GoreType<CrystallineSkyIslandDroplet>();
        }

        public override void LightColorMultiplier(ref float r, ref float g, ref float b)
        {
            r = 0.9f;
            g = 1f;
            b = 1f;
        }

        public override Color BiomeHairColor()
        {
            return Color.White;
        }

        public override byte GetRainVariant()
        {
            return (byte)Main.rand.Next(3);
        }

        //public override Asset<Texture2D> GetRainTexture() => rainTexture;
    }

    public class CrystallineSkyIslandDroplet : ModGore
    {
        public override string Texture => AssetDirectory.Biomes + Name;

        public override void SetStaticDefaults()
        {
            ChildSafety.SafeGore[Type] = true;
            GoreID.Sets.LiquidDroplet[Type] = true;

            // Rather than copy in all the droplet specific gore logic, this gore will pretend to be another gore to inherit that logic.
            UpdateType = GoreID.WaterDrip;
        }
    }

}
