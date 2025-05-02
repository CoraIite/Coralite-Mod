using Coralite.Content.ModPlayers;
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

        //public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/CrystallineSkyIslandDay");

        public override void Load()
        {
            //BackgroundTextureLoader.AddBackgroundTexture(Mod, AssetDirectory.Backgrounds + nameof(CrystallineSkyIslandBackground) + "Far");
            //BackgroundTextureLoader.AddBackgroundTexture(Mod, AssetDirectory.Backgrounds + nameof(CrystallineSkyIslandBackground) + "Mid");
            BackgroundTextureLoader.AddBackgroundTexture(Mod, AssetDirectory.Backgrounds + nameof(CrystallineSkyIslandBackground) + "0");
        }

        public override string MapBackground => AssetDirectory.Backgrounds + "CrystallineSkyIslandMap";

        public override string BestiaryIcon => AssetDirectory.Biomes + "CrystallineSkyIslandIcon";

        public override string BackgroundPath => AssetDirectory.Backgrounds + "CrystallineSkyIslandMap";

        public override ModWaterStyle WaterStyle => ModContent.GetInstance<CrystallineSkyIslandWaterStyle>();

        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<CrystallineSkyIslandBackground>();

        public override bool IsBiomeActive(Player player)
        {
            bool b1 = ModContent.GetInstance<CoraliteTileCount>().InCrystallineSkyIsland;
            bool b2 = player.Center.Y / 16 < Main.worldSurface * 0.8f;

            return b1 && b2;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (!isActive)
                return;

            for (int i = 0; i < Main.maxClouds; i++)
            {
                Main.cloud[i].Alpha *= 0.8f;
            }

            if (!CoraliteWorld.HasPermission &&
                !Main.projectile.Any(p => p.active && p.owner == Main.myPlayer && p.type == ModContent.ProjectileType<CrystallineSkyIslandCloudScreen>()))
            {
                Projectile.NewProjectile(new EntitySource_WorldEvent(), Main.LocalPlayer.Center, Vector2.Zero
                    , ModContent.ProjectileType<CrystallineSkyIslandCloudScreen>(), 0, 0, Main.myPlayer);
            }
        }
    }

    public class CrystallineSkyIslandEffect : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        public override string MapBackground => AssetDirectory.Backgrounds + "CrystallineSkyIslandMap";

        public override ModWaterStyle WaterStyle => ModContent.GetInstance<CrystallineSkyIslandWaterStyle>();

        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<CrystallineSkyIslandBackground>();

        public override bool IsSceneEffectActive(Player player)
        {
            return player.TryGetModPlayer(out CoralitePlayer cp) && cp.CrystallineSkyIslandEffect > 0;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (!isActive)
                return;

            for (int i = 0; i < Main.maxClouds; i++)
                Main.cloud[i].Alpha *= 0.8f;
        }
    }

    public class CrystallineSkyIslandSystem : ModSystem
    {
        public static float BiomeTimer;
        public const float BiomeTimerMax = 20000;

        public override void PostUpdateTime()
        {
            BiomeTimer += 0.5f;
            //BiomeTimer += 30f;
            if (BiomeTimer > BiomeTimerMax)
                BiomeTimer = 0;
        }

        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            if (Main.LocalPlayer.InModBiome<CrystallineSkyIsland>()
                || ModContent.GetInstance<CrystallineSkyIslandEffect>().IsSceneEffectActive(Main.LocalPlayer))
            {
                tileColor = (new Color(220, 240, 255) * (tileColor.R / 255f)) with { A = 255 };
                backgroundColor = (new Color(230, 230, 255) * (backgroundColor.R / 255f)) with { A = 255 };
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

    [AutoLoadTexture(Path = AssetDirectory.Backgrounds)]
    public class CrystallineSkyIslandBackground : ModSurfaceBackgroundStyle
    {
        public static ATex CrystallineSkyIslandBackground0 { get; private set; }
        public static ATex CrystallineSkyIslandBackground1 { get; private set; }
        public static ATex CrystallineSkyIslandBackground2 { get; private set; }
        public static ATex CrystallineSkyIslandBackground3 { get; private set; }
        public static ATex CrystallineSkyIslandBackground4 { get; private set; }
        public static ATex CrystallineSkyIslandBackground5 { get; private set; }
        public static ATex CrystallineSkyIslandBackground6 { get; private set; }
        public static ATex CrystallineSkyIslandBackground7 { get; private set; }

        public static ATex CrystallineSkyIslandBackgroundStar { get; private set; }

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
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, transformationMatrix);

            float timeFactor = CrystallineSkyIslandSystem.BiomeTimer / CrystallineSkyIslandSystem.BiomeTimerMax;

            float timeFactorTwoPI = timeFactor * MathHelper.TwoPi;

            float yOffset = Main.screenPosition.Y;
            yOffset = -Math.Clamp((yOffset - 2000) / 15, -100, 0);

            float alpha = Main.bgAlphaFrontLayer[Slot] * 0.9f;
            float sinTime = MathF.Sin(timeFactorTwoPI);
            float cosTime = MathF.Cos(timeFactorTwoPI);

            //最底下的蓝色背景层
            DrawBackgroundBack(spriteBatch, CrystallineSkyIslandBackground0.Value
                , alpha * 0.2f, timeFactor * CrystallineSkyIslandBackground0.Width(), 60, yOffset + 80);

            DrawBackground(spriteBatch, CrystallineSkyIslandBackground1.Value
                , alpha * 0.5f, sinTime * 100, 50, yOffset + 140);

            DrawBackgroundStar(spriteBatch, CrystallineSkyIslandBackgroundStar.Value
                , alpha, timeFactor * CrystallineSkyIslandBackgroundStar.Width(), 55, yOffset - 200);

            yOffset = -Math.Clamp((Main.screenPosition.Y - 2200) / 10, -200, 1200);

            //小石子
            DrawBackground(spriteBatch, CrystallineSkyIslandBackground2.Value
                , alpha, timeFactor * CrystallineSkyIslandBackground2.Width(), 45, yOffset);

            //各种云层
            DrawBackground(spriteBatch, CrystallineSkyIslandBackground3.Value
                , alpha, (1 - timeFactor) * CrystallineSkyIslandBackground3.Width(), 40, yOffset);

            yOffset = -Math.Clamp((Main.screenPosition.Y - 2600) / 6, -400, 1000);

            DrawBackground(spriteBatch, CrystallineSkyIslandBackground4.Value
                , alpha, -350 + sinTime * 400, 16, yOffset);
            DrawBackground(spriteBatch, CrystallineSkyIslandBackground5.Value
                , alpha, 100 - cosTime * 100, 18, yOffset);

            yOffset = -Math.Clamp((Main.screenPosition.Y - 2600) / 4, -600, 800);

            DrawBackground(spriteBatch, CrystallineSkyIslandBackground6.Value
                , alpha, timeFactor * CrystallineSkyIslandBackground6.Width() * 2, 12, yOffset);
            DrawBackground(spriteBatch, CrystallineSkyIslandBackground7.Value
                , alpha, sinTime * 150, 8, yOffset);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, transformationMatrix);

            return false;
        }

        /// <summary>
        /// 绘制一个背景层
        /// </summary>
        /// <param name="spriteBatch">嘻嘻</param>
        /// <param name="bgTex">嘻嘻</param>
        /// <param name="alpha">绘制的透明度</param>
        /// <param name="time">全局时间插值，-1 ~ 1，用于左右移动</param>
        /// <param name="moveRange">左右移动的最大距离</param>
        /// <param name="xFarAway">跟随玩家移动，数字越大移动效果越不明显</param>
        /// <param name="yoffset"> y坐标的偏移量，用于上升或下降时的偏移 </param>
        public void DrawBackground(SpriteBatch spriteBatch, Texture2D bgTex, float alpha, float xMove, float xFarAway, float yoffset)
        {
            Color drawColor = Main.ColorOfTheSkies * alpha;
            float xOffset = Main.screenPosition.X / xFarAway + xMove;
            int xOffsetInt = (int)xOffset;
            float scale = (Main.screenWidth + 2) / (float)bgTex.Width;

            xOffset = 1f - (xOffset - xOffsetInt);
            xOffset *= scale;

            Vector2 pos = new Vector2(Main.screenWidth / 2f + xOffset, Main.screenHeight / 2f + yoffset);

            Rectangle frameBox =
                    new Rectangle(xOffsetInt, 0, bgTex.Width, bgTex.Height);

            spriteBatch.Draw(bgTex,
                pos, frameBox, drawColor, 0f,
                frameBox.Size() / 2, scale, 0, 0f);
        }

        public void DrawBackgroundStar(SpriteBatch spriteBatch, Texture2D bgTex, float alpha, float xMove, float xFarAway, float yoffset)
        {
            Color drawColor = Color.Lerp(Color.White, Main.ColorOfTheSkies, 0.5f) * alpha;
            float xOffset = Main.screenPosition.X / xFarAway + xMove;
            int xOffsetInt = (int)xOffset;

            xOffset = 1f - (xOffset - xOffsetInt);
            Vector2 pos = new Vector2(Main.screenWidth / 2f + xOffset, Main.screenHeight / 2f + yoffset);

            Rectangle frameBox =
                    new Rectangle(xOffsetInt, 0, Main.screenWidth + 2, bgTex.Height);

            spriteBatch.Draw(bgTex,
                pos, frameBox, drawColor, 0f,
                frameBox.Size() / 2, 1.001f, 0, 0f);
        }

        /// <summary>
        /// 绘制一个背景层
        /// </summary>
        /// <param name="spriteBatch">嘻嘻</param>
        /// <param name="bgTex">嘻嘻</param>
        /// <param name="alpha">绘制的透明度</param>
        /// <param name="time">全局时间插值，-1 ~ 1，用于左右移动</param>
        /// <param name="moveRange">左右移动的最大距离</param>
        /// <param name="xFarAway">跟随玩家移动，数字越大移动效果越不明显</param>
        /// <param name="yoffset"> y坐标的偏移量，用于上升或下降时的偏移 </param>
        public void DrawBackgroundBack(SpriteBatch spriteBatch, Texture2D bgTex, float alpha, float xMove, float xFarAway, float yoffset)
        {
            Color drawColor = Main.ColorOfTheSkies * alpha;
            float xOffset = Main.screenPosition.X / xFarAway + xMove;
            int xOffsetInt = (int)xOffset;

            xOffset = 1f - (xOffset - xOffsetInt);
            Vector2 pos = new Vector2(Main.screenWidth / 2f + xOffset * 2, Main.screenHeight / 2f + yoffset);

            Rectangle frameBox =
                new Rectangle(xOffsetInt, 0, Main.screenWidth, bgTex.Height);

            spriteBatch.Draw(bgTex,
                pos, frameBox, drawColor, 0f,
                frameBox.Size() / 2, 2f, 0, 0f);

            //frameBox =
            //    new Rectangle(0, bgTex.Height - 2, Main.screenWidth, 2);

            //spriteBatch.Draw(bgTex,
            //    pos + new Vector2(0, bgTex.Height / 2), frameBox, drawColor, 0f,
            //    new Vector2(frameBox.Width / 2, 0), new Vector2(1.001f, (Main.screenHeight - bgTex.Height) / 2), 0, 0f);
        }

        //public void DrawFarBackground(SpriteBatch spriteBatch, float alpha, float time)
        //{
        //    Texture2D farTex = FarTex.Value;
        //    Color drawColor = Main.ColorOfTheSkies * alpha * 0.35f;

        //    time = MathF.Sin(time * MathHelper.TwoPi);

        //    float yoffset = 300;
        //    yoffset -= Math.Clamp(Main.screenPosition.Y / 15, 0, 600);

        //    Vector2 pos = new Vector2(Main.screenWidth / 2 + time * 300, Main.screenHeight / 2 + yoffset);
        //    Rectangle frameBox =
        //        new Rectangle((int)(Main.screenPosition.X / 50), 0, Main.screenWidth + 800, farTex.Height);

        //    spriteBatch.Draw(farTex,
        //        pos, frameBox, drawColor, 0f,
        //        frameBox.Size() / 2, 1f, 0, 0f);
        //}

        //public void DrawMiddleBackground(SpriteBatch spriteBatch, float alpha,float time)
        //{
        //    Texture2D farTex = MidTex.Value;
        //    Color drawColor = Main.ColorOfTheSkies * alpha;

        //    time = MathF.Cos(time * MathHelper.TwoPi);

        //    float yoffset = -100;
        //    yoffset -= Math.Clamp(Main.screenPosition.Y / 10, 0, 600);

        //    Vector2 pos = new Vector2(Main.screenWidth / 2 + time * 200, Main.screenHeight / 2 +yoffset);
        //    Rectangle frameBox =
        //        new Rectangle((int)(Main.screenPosition.X / 30) , 0, Main.screenWidth + 600, farTex.Height);

        //    spriteBatch.Draw(farTex,
        //        pos, frameBox, drawColor, 0f,
        //        frameBox.Size() / 2, 1.1f, 0, 0f);
        //}

        //public void DrawCloseBackground(SpriteBatch spriteBatch, float alpha,float time)
        //{
        //    Texture2D farTex = CloseTex.Value;
        //    Color drawColor = Main.ColorOfTheSkies * alpha;

        //    time = MathF.Cos(time * MathHelper.TwoPi + MathHelper.PiOver4);

        //    float yoffset = 800;
        //    yoffset -= Math.Clamp(Main.screenPosition.Y / 6, 0, 500);

        //    Vector2 pos = new Vector2(Main.screenWidth / 2 + time * 100, Main.screenHeight / 2 + yoffset);
        //    Rectangle frameBox =
        //        new Rectangle((int)(Main.screenPosition.X / 10), 0, Main.screenWidth + 600, farTex.Height);

        //    spriteBatch.Draw(farTex,
        //        pos, frameBox, drawColor, 0f,
        //        frameBox.Size() / 2, 1f, 0, 0f);
        //}
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
