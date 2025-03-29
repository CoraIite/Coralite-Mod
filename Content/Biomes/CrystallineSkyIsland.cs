using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Core.Attributes;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Biomes
{
    public class CrystallineSkyIsland : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Temp_CrystallineSkyIsland");

        public override void Load()
        {
            //BackgroundTextureLoader.AddBackgroundTexture(Mod, AssetDirectory.Backgrounds + nameof(CrystallineSkyIslandBackground) + "Far");
            //BackgroundTextureLoader.AddBackgroundTexture(Mod, AssetDirectory.Backgrounds + nameof(CrystallineSkyIslandBackground) + "Mid");
            BackgroundTextureLoader.AddBackgroundTexture(Mod, AssetDirectory.Backgrounds + nameof(CrystallineSkyIslandBackground) + "Close");
        }

        //public override string BestiaryIcon => AssetDirectory.Biomes + "MagicCrystalCaveIcon";

        public override ModWaterStyle WaterStyle => ModContent.GetInstance<CrystallineSkyIslandWaterStyle>();

        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<CrystallineSkyIslandBackground>();

        public override Color? BackgroundColor => Color.LightCyan;

        public override bool IsBiomeActive(Player player)
        {
            bool b1 = ModContent.GetInstance<CoraliteTileCount>().CrystallineSkyIslandTileCount >= 400;
            bool b2 = player.Center.Y / 16 < Main.worldSurface * 0.8f;

            return b1 && b2;
        }
    }

    public class CrystallineSkyIslandEffect : ModSceneEffect
    {
        public static float BiomeTimer;
        public const float BiomeTimerMax=20000;

        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        //public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("Coralite/MagicCrystalCaveBackground");

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Temp_CrystallineSkyIsland");

        public override ModWaterStyle WaterStyle => ModContent.GetInstance<CrystallineSkyIslandWaterStyle>();

        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<CrystallineSkyIslandBackground>();

        public override bool IsSceneEffectActive(Player player)
        {
            bool b1 = ModContent.GetInstance<CoraliteTileCount>().CrystallineSkyIslandTileCount >= 400;
            bool b2 = player.Center.Y / 16 < Main.worldSurface * 0.8f;

            return b1 && b2;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (!isActive)
                return;

            BiomeTimer += 0.5f;
            if (BiomeTimer > BiomeTimerMax)
                BiomeTimer = 0;

            for (int i = 0; i < Main.maxClouds; i++)
            {
                Main.cloud[i].active = false;
            }

            if (!CoraliteWorld.HasPermission &&
                !Main.projectile.Any(p => p.active && p.owner == Main.myPlayer && p.type == ModContent.ProjectileType<CrystallineSkyIslandCloudScreen>()))
            {
                Projectile.NewProjectile(new EntitySource_WorldEvent(), Main.LocalPlayer.Center, Vector2.Zero
                    , ModContent.ProjectileType<CrystallineSkyIslandCloudScreen>(), 0, 0, Main.myPlayer);
            }
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

    [AutoLoadTexture(Path =AssetDirectory.Backgrounds)]
    public class CrystallineSkyIslandBackground : ModSurfaceBackgroundStyle
    {
        [AutoLoadTexture(Name = "CrystallineSkyIslandBackgroundFar")]
        public static ATex FarTex { get;private set; }
        [AutoLoadTexture(Name = "CrystallineSkyIslandBackgroundMid")]
        public static ATex MidTex { get;private set; }
        [AutoLoadTexture(Name = "CrystallineSkyIslandBackgroundClose")]
        public static ATex CloseTex { get;private set; }

        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            for (int i = 0; i < fades.Length; i++)
            {
                if (i == Slot)
                {
                    fades[i] += transitionSpeed;
                    if (fades[i] > 1f)
                    {
                        fades[i] = 1f;
                    }
                }
                else
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                    {
                        fades[i] = 0f;
                    }
                }
            }
        }



        //public override int ChooseFarTexture()
        //{
        //    return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Backgrounds/CrystallineSkyIslandBackgroundFar");
        //}

        //public override int ChooseMiddleTexture()
        //{
        //    return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Backgrounds/CrystallineSkyIslandBackgroundMid");
        //}

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        {
            return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Backgrounds/CrystallineSkyIslandBackgroundClose");
        }

        public override bool PreDrawCloseBackground(SpriteBatch spriteBatch)
        {
            Matrix transformationMatrix = Main.BackgroundViewMatrix.TransformationMatrix;
            transformationMatrix.Translation -= Main.BackgroundViewMatrix.ZoomMatrix.Translation * new Vector3(1f, Main.BackgroundViewMatrix.Effects.HasFlag(SpriteEffects.FlipVertically) ? (-1f) : 1f, 1f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, transformationMatrix);

            float timeFactor = CrystallineSkyIslandEffect.BiomeTimer / CrystallineSkyIslandEffect.BiomeTimerMax;

            //绘制3层背景
            float alpha = Main.bgAlphaFrontLayer[Slot];
            DrawFarBackground(spriteBatch, alpha,timeFactor);
            DrawMiddleBackground(spriteBatch, alpha, timeFactor);
            DrawCloseBackground(spriteBatch, alpha, timeFactor);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, transformationMatrix);

            return false;
        }

        public void DrawFarBackground(SpriteBatch spriteBatch, float alpha, float time)
        {
            Texture2D farTex = FarTex.Value;
            Color drawColor = Main.ColorOfTheSkies * alpha * 0.35f;

            time = MathF.Sin(time * MathHelper.TwoPi);

            float yoffset = 300;
            yoffset -= Math.Clamp(Main.screenPosition.Y / 15, 0, 600);

            Vector2 pos = new Vector2(Main.screenWidth / 2 + time * 300, Main.screenHeight / 2 + yoffset);
            Rectangle frameBox =
                new Rectangle((int)(Main.screenPosition.X / 50), 0, Main.screenWidth + 800, farTex.Height);

            spriteBatch.Draw(farTex,
                pos, frameBox, drawColor, 0f,
                frameBox.Size() / 2, 1f, 0, 0f);
        }

        public void DrawMiddleBackground(SpriteBatch spriteBatch, float alpha,float time)
        {
            Texture2D farTex = MidTex.Value;
            Color drawColor = Main.ColorOfTheSkies * alpha;

            time = MathF.Cos(time * MathHelper.TwoPi);

            float yoffset = -100;
            yoffset -= Math.Clamp(Main.screenPosition.Y / 10, 0, 600);

            Vector2 pos = new Vector2(Main.screenWidth / 2 + time * 200, Main.screenHeight / 2 +yoffset);
            Rectangle frameBox =
                new Rectangle((int)(Main.screenPosition.X / 30) , 0, Main.screenWidth + 600, farTex.Height);

            spriteBatch.Draw(farTex,
                pos, frameBox, drawColor, 0f,
                frameBox.Size() / 2, 1.1f, 0, 0f);
        }

        public void DrawCloseBackground(SpriteBatch spriteBatch, float alpha,float time)
        {
            Texture2D farTex = CloseTex.Value;
            Color drawColor = Main.ColorOfTheSkies * alpha;

            time = MathF.Cos(time * MathHelper.TwoPi + MathHelper.PiOver4);

            float yoffset = 800;
            yoffset -= Math.Clamp(Main.screenPosition.Y / 6, 0, 500);

            Vector2 pos = new Vector2(Main.screenWidth / 2 + time * 100, Main.screenHeight / 2 + yoffset);
            Rectangle frameBox =
                new Rectangle((int)(Main.screenPosition.X / 10), 0, Main.screenWidth + 600, farTex.Height);

            spriteBatch.Draw(farTex,
                pos, frameBox, drawColor, 0f,
                frameBox.Size() / 2, 1f, 0, 0f);
        }
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
