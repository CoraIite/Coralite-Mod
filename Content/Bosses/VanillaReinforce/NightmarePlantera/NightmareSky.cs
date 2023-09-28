using Coralite.Core;
using Coralite.Core.Configs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class NightmareSky : CustomSky
    {
        public int Timeleft = 100; //弄一个计时器，让天空能自己消失
        private int particleTimer;
        public Color color = new Color(204, 170, 242);

        public static FlowerParticle[] flowers;
        public static Rectangle ScreenRetangle;

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (minDepth < 9 && maxDepth > 9)//绘制在背景景物后面，防止遮挡
            {
                Texture2D sky = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "NightmareSky").Value;

                Rectangle screen = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
                if (VisualEffectSystem.UseNightmareSky)
                {
                    Effect e = Filters.Scene["GlowingMarblingBlack2"].GetShader().Shader;
                    e.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 10);
                    e.Parameters["viewRange"].SetValue(0.7f + MathF.Sin(Main.GlobalTimeWrappedHourly / 3) * 0.2f);
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

                Color c = color;
                c.A = (byte)(c.A * Timeleft / 100f);
                foreach (var f in flowers)
                    if (f.active && f.Depth < maxDepth && f.Depth >= minDepth)
                        f.DrawSelf(spriteBatch, c);
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
                    if (Main.rand.NextBool(3))
                    {
                        for (int i = 0; i < flowers.Length; i++)
                        {
                            FlowerParticle flower = flowers[i];
                            if (flower.active)
                                continue;

                            flower.active = true;
                            flower.Position = Main.screenPosition + Main.rand.NextVector2FromRectangle(new Rectangle(0, 0, Main.screenWidth, Main.screenHeight));
                            flower.frameX = Main.rand.Next(4);
                            flower.Depth = Main.rand.NextFloat() * 10f;

                            flower.frameY = 1;
                            if (flower.Depth > 5f)
                                flower.frameY += 1;
                            flower.Scale = Main.rand.NextFloat(0.5f, 1f) * (1f + flower.Depth * 0.75f);
                            if (flower.Depth < 1.5f)
                            {
                                flower.frameX = 4;
                            }
                            flower.Rotation = Main.rand.NextFloat(-MathHelper.TwoPi, MathHelper.TwoPi);
                            flower.timeleft = 0;
                            flower.alpha = 0;
                            break;
                        }
                    }

                    particleTimer = 0;
                }

                float speed = 0.01f + Main.LocalPlayer.velocity.Length() / 400;
                for (int i = 0; i < flowers.Length; i++)
                {
                    FlowerParticle flower = flowers[i];
                    if (flower.active)
                    {
                        flower.Update();
                        flower.Rotation += Math.Sign(flower.Rotation) * speed;
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
        public Vector2 Position;
        public bool active;
        public float Rotation;
        public float Scale;
        public float Depth;
        public float alpha;
        public int frameX;
        public int frameY;

        public int timeleft;

        public void Update()
        {
            do
            {
                if (timeleft < 60)
                {
                    alpha += (1 / 60f) * Depth / 30f;
                    break;
                }

                if (timeleft < 440)
                {
                    Position += new Vector2(0, -0.5f);

                    break;
                }

                if (alpha > 0)
                {
                    alpha -= (1 / 60f) * Depth / 30f;
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

            if (!Helpers.Helper.OnScreen(Position - Main.screenPosition, new Vector2(112, 112) * Scale))
            {
                active = false;
                alpha = 0;
                timeleft = 0;
            }

            timeleft++;
        }

        public void DrawSelf(SpriteBatch spriteBatch, Color color)
        {
            Texture2D tex = NightmarePlantera.flowerParticleTex.Value;
            Rectangle frameBox = tex.Frame(5, 3, frameX, frameY);
            Vector2 origin = frameBox.Size() / 2;
            Vector2 pos = Position - Main.screenPosition;

            color.A = (byte)(color.A * alpha);
            spriteBatch.Draw(tex, pos, frameBox, color, Rotation, origin, Scale, 0, 0);
            color.A = (byte)(color.A * 0.5f);
            spriteBatch.Draw(tex, pos, frameBox, color, Rotation, origin, Scale, 0, 0);
        }
    }
}
