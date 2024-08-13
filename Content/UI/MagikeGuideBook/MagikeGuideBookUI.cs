using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.UI.MagikeGuideBook
{
    public class MagikeGuideBookUI : BetterUIState
    {
        /// <summary> 是否可见 </summary>
        public override bool Visible { get => visible; set => visible = value; }

        public static MagikeGuideBookPanel BookPanel = new();
        //public static SwitchArrow leftArrow = new SwitchArrow(SpriteEffects.FlipHorizontally);
        //public static SwitchArrow rightArrow = new SwitchArrow(SpriteEffects.None);

        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        /// <summary> 每个目录页能够容纳多少个跳转按钮 </summary>
        public const int HowManyButtonPerCatalogPage = 14;
        public bool visible;
        public bool openingBook;
        public bool closeingBook;
        public bool drawExtra;
        public static Vector2 basePos = new(Main.screenWidth / 2, Main.screenHeight / 2);
        public Vector2 bookSize;
        public float bookWidth;
        public int Timer;
        public readonly ParticleGroup particles = new();

        public MagicCircle magicCircle;

        public override void OnInitialize()
        {
            basePos = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
            BookPanel.InitSize();
            BookPanel.SetPosition(basePos);
            BookPanel.InitPageGroups();
            BookPanel.OnScrollWheel += PlaySound;
            Append(BookPanel);
        }

        private void PlaySound(UIScrollWheelEvent evt, UIElement listeningElement)
        {
            Helper.PlayPitched("Misc/Pages", 0.4f, 0f, Main.LocalPlayer.Center);
        }

        public override void Recalculate()
        {
            basePos = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
            BookPanel.SetPosition(basePos);
            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.playerInventory)
                closeingBook = true;

            UpdateVisualEffect();
            if (UpdateOpeningAnmi())
                UpdateClosingAnmi();

            base.Update(gameTime);
        }

        public void UpdateVisualEffect()
        {
            if (!drawExtra)
                return;

            //更新特效
            particles.UpdateParticles();

            if (magicCircle != null)
            {
                magicCircle.Update();
                if (!magicCircle.active)
                    magicCircle = null;
            }

            if (!openingBook && !particles.Any())
                drawExtra = false;
        }

        public bool UpdateOpeningAnmi()
        {
            if (!openingBook)
                return true;

            bookWidth += 25;
            BookPanel.Width.Set(bookWidth, 0f);

            if (bookWidth >= bookSize.X || Timer > 1000)        //超出时跳出
            {
                openingBook = false;
                BookPanel.OverflowHidden = false;
                BookPanel.Width.Set(bookSize.X, 0f);

                base.Recalculate();
                return true;
            }

            Timer++;
            base.Recalculate();

            //CalculatedStyle dimensions = BookPanel.GetDimensions();

            //particles.Add(new HorizontalStar()
            //{
            //    center = dimensions.Position() + new Vector2(bookWidth, Main.rand.Next(0, (int)bookSize.Y)),
            //    scale = Main.rand.NextFloat(0.1f, 0.7f),
            //    velocity = new Vector2(Main.rand.NextFloat(0f, 4), 0),
            //    color = Main.rand.Next(3) switch
            //    {
            //        0 => Color.Purple,
            //        1 => Color.RosyBrown,
            //        _ => Color.Brown
            //    }
            //});

            return false;
        }

        public void UpdateClosingAnmi()
        {
            if (!closeingBook)//关闭书本时的效果
                return;

            BookPanel.alpha -= 0.05f;
            BookPanel.Top.Pixels += 80;
            if (BookPanel.alpha < 0.01f)
            {
                BookPanel.alpha = 0;
                closeingBook = false;
                visible = false;
                drawExtra = false;
            }

            base.Recalculate();
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (drawExtra)      //分成两半绘制magicCircle
            {
                magicCircle?.DrawFrontCircle(spriteBatch);
            }

            base.Draw(spriteBatch);

            if (drawExtra)
            {
                RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
                spriteBatch.End();

                Main.graphics.GraphicsDevice.BlendState = BlendState.NonPremultiplied;

                //绘制特效
                foreach (var particle in particles)
                {
                    if (particle is IDrawParticlePrimitive idpp)
                        idpp.DrawPrimitives();
                }

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

                particles.DrawParticles(spriteBatch);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);

                magicCircle?.DrawBackCircle(spriteBatch);
            }
        }


        public void OpenBook()
        {
            Helper.PlayPitched("Stars/StarsSpawn", 0.4f, 0f, Main.LocalPlayer.Center);
            BookPanel.alpha = 1;
            BookPanel.OverflowHidden = true;
            bookSize = BookPanel.PanelTex.Size() * BookPanel.scale;
            bookWidth = 0;
            BookPanel.Width.Set(0, 0);
            Timer = 0;

            //初始化特效
            particles.Clear();
            Vector2 position = BookPanel.GetDimensions().Position();
            magicCircle = new MagicCircle(bookSize.X + 50, bookSize.Y / 2 + 40)
            {
                center = position + new Vector2(25, bookSize.Y / 2),
                color = Color.White,
                velocity = new Vector2(25, 0),
            };

            //particles.Add(FlashLine.Spawn(position + new Vector2(80, bookSize.Y / 2),
            //    new Vector2(20, 0), bookSize.X - 240, (bookSize.Y / 2) - 100));

            openingBook = true;
            closeingBook = false;
            visible = true;
            drawExtra = true;
        }
    }


    public class MagicCircle
    {
        public readonly float maxLength;
        public float length;
        public float rotation2;
        public float radius;
        public float width = 25;       //宽度

        //先画在2维的圆
        public Vector2[] vector2D;
        //转化为3D的坐标
        public readonly Vector3[] vector3s = new Vector3[180];
        //给顶点的2D坐标
        public readonly Vector2[] vector2s = new Vector2[180];
        public Vector3 normal = Vector3.Zero;

        public Vector2 center;
        public Vector2 velocity;
        public float fadeIn;
        public float rotation;
        public bool active = true;
        public Color color;
        public Trail trail;

        public Asset<Texture2D> Texture2D { get; private set; }

        public string Texture => AssetDirectory.MagikeGuideBook + "MagicCircleGlow";

        public MagicCircle(float maxLength, float radius) : base()
        {
            Texture2D = !string.IsNullOrEmpty(Texture) ? Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad) : TextureAssets.Dust;

            this.maxLength = maxLength;
            this.radius = radius;
            if (vector2D == null)
            {
                vector2D = new Vector2[180];
                for (int i = 0; i < 180; i++)
                {
                    //先画个圆
                    vector2D[i] = (new Vector2(0, 1).RotatedBy(i * (MathHelper.TwoPi / 180)));
                }
            }

            rotation = 1.57f - 0.2f;
            rotation2 = Main.rand.NextFloat();
        }

        public void Update()
        {
            rotation2 += 0.02f;
            for (int i = 0; i < 180; i++)
            {
                //再画在3维空间 然后转起来
                vector3s[i] = Vector3.Transform(new Vector3(vector2D[i].X, vector2D[i].Y, 0), Matrix.CreateRotationY(rotation));
                normal = Vector3.Transform(new Vector3(0, 0, 1), Matrix.CreateRotationY(rotation));
            }

            for (int i = 0; i < 180; i++)
            {
                //重新投影到二维
                float k1 = -1000 / (vector3s[i].Z - 1000);
                vector2s[i] = k1 * new Vector2(vector3s[i].X, vector3s[i].Y);
            }

            if (length < maxLength)
            {
                center += velocity;
                rotation += 0.2f * 2 / (maxLength / velocity.X);
                length += velocity.X;   //应该是abs的，但我懒得写了
            }
            else
            {
                color *= 0.8f;
                radius *= 0.98f;
                width *= 0.98f;
                center += new Vector2(5, 0);
                if (color.R < 10)
                    active = false;
            }

            fadeIn++;
            if (fadeIn > 300)
                active = false;
        }


        //由于特殊需求不得不把绘制掰成两半
        //不然大概会出现透视BUG

        public void DrawBackCircle(SpriteBatch spriteBatch)
        {
            DrawCircle(spriteBatch, 0, 90);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public void DrawFrontCircle(SpriteBatch spriteBatch)
        {
            DrawCircle(spriteBatch, 90, vector2s.Length);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        }

        public void DrawCircle(SpriteBatch spriteBatch, int start, int end)
        {
            Texture2D Texture = Texture2D.Value;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap/*注意了奥*/, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);

            List<CustomVertexInfo> bars = new();
            //对法向量进行一个投影
            float k1 = -1000 / (normal.Z - 1000);
            var normalDir = k1 * new Vector2(normal.X, normal.Y);

            for (int i = start; i < end; ++i)
            {
                //一些数据
                float factor = (float)i * 4 / end + rotation2;
                var w = 1;//暂时无用
                float a = vector3s[i].Z + 1.3f;

                bars.Add(new CustomVertexInfo(center + vector2s[i] * radius + normalDir * width, color * a, new Vector3(factor, 1, w)));
                bars.Add(new CustomVertexInfo(center + vector2s[i] * radius + normalDir * -width, color * a, new Vector3(factor, 0, w)));
                if (i == vector2s.Length - 1)
                {
                    bars.Add(new CustomVertexInfo(center + vector2s[0] * radius + normalDir * width, color * a, new Vector3(factor, 1, w)));
                    bars.Add(new CustomVertexInfo(center + vector2s[0] * radius + normalDir * -width, color * a, new Vector3(factor, 0, w)));
                }
            }

            List<CustomVertexInfo> Vx = new();
            if (bars.Count > 2)
            {
                for (int i = 0; i < bars.Count - 2; i += 2)
                {
                    Vx.Add(bars[i]);
                    Vx.Add(bars[i + 2]);
                    Vx.Add(bars[i + 1]);

                    Vx.Add(bars[i + 1]);
                    Vx.Add(bars[i + 2]);
                    Vx.Add(bars[i + 3]);
                }
            }

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vx.ToArray(), 0, Vx.Count / 3);
        }
    }

    //public class FlashLine : TrailParticle
    //{
    //    public override string Texture => AssetDirectory.OtherProjectiles + "FlashLine";

    //    public const int maxPointCount = 10;

    //    private float length;
    //    private float alpha;
    //    private float maxLength;

    //    public override void OnSpawn()
    //    {
    //        oldCenter = new Vector2[maxPointCount];
    //    }

    //    public override void Update()
    //    {
    //        trail ??= new Trail(Main.instance.GraphicsDevice, maxPointCount, new NoTip(), factor => Rotation,
    //            factor => new Color(235, 130, 255, (byte)(alpha * 255)) * (MathF.Cos(factor.Y * 6.282f) * 0.5f + 0.5f));

    //        if (length < maxLength)
    //        {
    //            length += Velocity.X;   //应该是abs的，但我懒得写了
    //            if (length > maxLength)
    //                length = maxLength;
    //            //控制顶点
    //            int j = -maxPointCount / 2;
    //            for (int i = 0; i < maxPointCount; i++)
    //            {
    //                oldCenter[i] = Center + new Vector2(j * 25, 0);
    //                j++;
    //            }
    //        }
    //        else
    //        {
    //            alpha -= 0.1f;
    //            if (alpha < 0.01f)
    //                active = false;
    //        }

    //        trail.Positions = oldCenter;
    //    }

    //    public override void Draw(SpriteBatch spriteBatch) { }

    //    public static FlashLine Spawn(Vector2 center, Vector2 velocity, float maxLength, float width)
    //    {
    //        FlashLine p = ;
    //        p.type = CoraliteContent.ParticleType<FlashLine>();
    //        p.Center = center;
    //        p.Velocity = velocity;
    //        p.Rotation = width;
    //        p.active = true;
    //        p.shouldKilledOutScreen = false;
    //        p.Scale = 1;
    //        ParticleLoader.SetupParticle(p);
    //        p.datas = new object[3]
    //        {
    //            0f,
    //            1f,
    //            maxLength
    //        };

    //        return p;
    //    }

    //    public override void DrawPrimitives()
    //    {
    //        Effect effect = Filters.Scene["Flow"].GetShader().Shader;

    //        //Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
    //        Matrix view = Main.GameViewMatrix.ZoomMatrix;
    //        Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

    //        Main.instance.GraphicsDevice.Textures[0] = Texture2D.Value;

    //        effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
    //        effect.Parameters["transformMatrix"].SetValue(/*world **/ view * projection);
    //        effect.Parameters["uTextImage"].SetValue(Texture2D.Value);

    //        trail?.Render(effect);

    //        //画两遍
    //        effect.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly * 0.2f);
    //        trail?.Render(effect);
    //    }
    //}
}

