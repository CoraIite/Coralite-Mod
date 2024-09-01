using Coralite.Core;
using Coralite.Core.Configs;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class NightmareSky : CustomSky
    {
        public int Timeleft = 100; //弄一个计时器，让天空能自己消失
        private int particleTimer;
        public Color color = new(204, 170, 242);

        public static FlowerParticle[] flowers;
        public static Rectangle ScreenRetangle;

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (minDepth < 9 && maxDepth > 9)//绘制在背景景物后面，防止遮挡
            {
                Texture2D sky = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "NightmareSky").Value;

                Rectangle screen = new(0, 0, Main.screenWidth, Main.screenHeight);
                if (VisualEffectSystem.UseNightmareSky)
                {
                    Effect e = Filters.Scene["GlowingMarblingBlack2"].GetShader().Shader;
                    e.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 8);
                    e.Parameters["viewRange"].SetValue(0.7f + (MathF.Sin(Main.GlobalTimeWrappedHourly / 2) * 0.2f));
                    e.Parameters["uC1"].SetValue((color * (Timeleft / 100f)).ToVector3());

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, e);

                    spriteBatch.Draw(sky, screen, Color.White);

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                }
                else
                    spriteBatch.Draw(sky, screen, color * (Timeleft / 100f));
            }

            if (VisualEffectSystem.UseNightmareSky && flowers != null)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, default, default);

                float extraAlpha = Timeleft / 100f;
                foreach (var f in flowers)
                    if (f.active && f.Depth < maxDepth && f.Depth >= minDepth)
                        f.DrawSelf(spriteBatch, extraAlpha);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            }
        }

        public override bool IsActive()
        {
            return Timeleft > 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Main.gamePaused)//游戏暂停时不执行
            {
                if (Timeleft > 0)
                    Timeleft--;//只要激活时就会减少，这样就会在外部没赋值时自己消失了
                else if (SkyManager.Instance["NightmareSky"].IsActive())
                    SkyManager.Instance.Deactivate("NightmareSky");//消失

                if (flowers == null)
                    return;

                particleTimer++;
                if (particleTimer > 10)
                {
                    for (int i = 0; i < flowers.Length; i++)
                    {
                        FlowerParticle flower = flowers[i];
                        if (flower.active)
                            continue;

                        flower.active = true;
                        flower.onScreenPosition = Main.rand.NextVector2FromRectangle(new Rectangle(0, 0, Main.screenWidth, Main.screenHeight));
                        flower.frameX = Main.rand.Next(4);
                        flower.Depth = Main.rand.NextFloat() * 10f;

                        flower.frameY = 0;
                        if (flower.Depth > 3f)
                        {
                            flower.frameY += 1;
                            if (flower.Depth > 6.5f)
                                flower.frameY += 1;
                        }

                        flower.Scale = Main.rand.NextFloat(0.5f, 1f) * (1f + (flower.Depth * 0.75f));

                        flower.Rotation = Main.rand.NextFloat(-MathHelper.TwoPi, MathHelper.TwoPi);
                        flower.timeleft = 0;
                        flower.alpha = 0;

                        Vector3 c1 = Main.rgbToHsl(color);
                        c1.Z *= flower.Depth / 10f;
                        flower.color = Main.hslToRgb(c1);
                        if (Main.rand.NextBool(4))
                        {
                            Color c = NightmarePlantera.phantomColors[Main.rand.Next(7)];
                            c.A = 230;
                            flower.color = c;
                        }

                        break;
                    }

                    particleTimer = 0;
                }

                float speed = 0.007f + (Main.LocalPlayer.velocity.Length() / 450);
                for (int i = 0; i < flowers.Length; i++)
                {
                    FlowerParticle flower = flowers[i];
                    if (flower.active)
                    {
                        Vector2 velocity = Main.LocalPlayer.oldPosition + (new Vector2(Main.LocalPlayer.width, Main.LocalPlayer.height) / 2) - Main.LocalPlayer.Center;
                        flower.onScreenPosition += velocity / (flower.Depth * 0.75f);
                        flower.Update();
                        flower.Rotation += Math.Sign(flower.Rotation) * speed * (15 - flower.Depth) / 15;
                    }
                }
            }
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            flowers = new FlowerParticle[20];
            for (int i = 0; i < flowers.Length; i++)
                flowers[i] = new FlowerParticle();
        }

        //private int SortMethod(FlowerParticle flower1, FlowerParticle flower2) => flower2.Depth.CompareTo(flower1.Depth);

        public override void Deactivate(params object[] args)
        {
            flowers = null;
        }

        public override void Reset() { }
        public override float GetCloudAlpha() => 0f;

        public override Color OnTileColor(Color inColor)
        {
            return Color.Lerp(inColor, color, 0.2f * (Timeleft / 100f));
        }
    }

    public class FlowerParticle
    {
        public Vector2 onScreenPosition;
        public bool active;
        public float Rotation;
        public float Scale;
        public float Depth;
        public float alpha;
        public int frameX;
        public int frameY;

        public int timeleft;
        public Color color;

        public void Update()
        {
            do
            {
                if (timeleft < 60)
                {
                    alpha += 1 / 60f * Depth / 20f;
                    break;
                }

                if (timeleft < 440)
                {
                    onScreenPosition += new Vector2(0, -0.5f);

                    break;
                }

                if (alpha > 0)
                {
                    alpha -= 1 / 60f * Depth / 20f;
                    if (alpha < 0)
                        alpha = 0;
                }

                if (timeleft > 500)
                {
                    active = false;
                    alpha = 0;
                    timeleft = 0;
                }

            } while (false);

            if (!OnScreen(onScreenPosition, new Vector2(112, 112) * Scale))
            {
                active = false;
                alpha = 0;
                timeleft = 0;
            }

            timeleft++;
        }

        public static bool OnScreen(Vector2 pos, Vector2 size)
        {
            Rectangle rect = new((int)pos.X + 300, (int)pos.Y + 300, (int)size.X, (int)size.Y);
            return rect.Intersects(new Rectangle(0, 0, Main.screenWidth + 600, Main.screenHeight + 600));
        }

        public void DrawSelf(SpriteBatch spriteBatch, float extraAlpha)
        {
            Texture2D tex = NightmarePlantera.flowerParticleTex.Value;
            Rectangle frameBox = tex.Frame(5, 3, frameX, frameY);
            Vector2 origin = frameBox.Size() / 2;
            Vector2 pos = onScreenPosition;

            float a = extraAlpha * alpha;
            Color c = color;
            c.A = (byte)(c.A * a);
            spriteBatch.Draw(tex, pos, frameBox, c, Rotation, origin, Scale, 0, 0);
            c.A = (byte)(c.A * 0.25f);
            spriteBatch.Draw(tex, pos, frameBox, c, Rotation, origin, Scale, 0, 0);
        }
    }
}
