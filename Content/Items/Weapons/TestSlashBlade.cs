//using Coralite.Core;
//using Coralite.Helpers;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Terraria;
//using Terraria.GameContent;
//using Terraria.Graphics.Effects;
//using Terraria.ID;
//using Terraria.ModLoader;
//using static Terraria.ModLoader.ModContent;

//namespace Coralite.Content.Items.Weapons
//{
//    public class TestSlashBlade : ModItem
//    {
//        public override string Texture => AssetDirectory.ShadowItems + "ShadowSword";

//        public override void SetStaticDefaults()
//        {
//            DisplayName.SetDefault("测试用拔刀剑");
//        }

//        public override void SetDefaults()
//        {
//            Item.damage = 50;
//            Item.knockBack = 1.2f;
//            Item.useTime = Item.useAnimation = 20;
//            Item.noMelee = true;
//            Item.useStyle = ItemUseStyleID.Shoot;
//            Item.width = 72;
//            Item.height = 88;
//            //Item.UseSound = SoundID.Item1;
//            Item.rare = ItemRarityID.Red;
//            Item.DamageType = DamageClass.Melee;
//            Item.useTurn = false;
//            Item.autoReuse = true;
//            Item.shoot = ProjectileType<TestSlash>();
//            Item.shootSpeed = 10f;
//            Item.noUseGraphic = true;
//            Item.value = 805932;
//            Item.channel = true;
//        }
//    }

//    public class TestSlash : SlashBladeProj
//    {
//        public override string Texture => AssetDirectory.OtherProjectiles + "Yamato";

//        protected override void BeforeSlash()
//        {

//        }

//        public override void SetDefs()
//        {
//            CircleTrail = false;
//            startAngle = 1f;
//            maxTime = 22;
//            xScale = 0.6f;
//            halfShortAxis = 0.3f;
//            projLenth = 206;
//        }

//        protected override void AfterSlash()
//        {
//            Projectile.friendly = false;
//            Projectile.Center = Owner.Center + RotateVec2 * (projScale / 2 + distanceToHand);
//            if (Timer > 26)
//                Projectile.Kill();

//        }

//        protected override void SpawnDustOnSlash()
//        {

//        }
//    }

//    public abstract class SlashBladeProj : ModProjectile, IDrawWarp
//    {
//        protected Texture2D WarpTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "Warp0").Value;//扭曲贴图
//        public Player Owner => Main.player[Projectile.owner];

//        #region 变量声明
//        protected float[] oldRotate;
//        protected float[] oldScale;

//        /// <summary>
//        /// 是否为第零帧
//        /// </summary>
//        private bool onStart = false;
//        /// <summary>
//        /// 是否锁定角度为玩家面朝的角度
//        /// </summary>
//        protected bool hasStaticAngle = false;
//        /// <summary>
//        /// 是否应用圆形轨迹
//        /// </summary>
//        protected bool CircleTrail = true;

//        /// <summary>
//        /// 开始挥舞前的时间
//        /// </summary>
//        protected int minTime = 0;
//        /// <summary>
//        /// 挥舞所用时间
//        /// </summary>
//        protected int maxTime = 60;
//        //关于角度控制
//        /// <summary>
//        /// 起始角度，起始角度为正时则从人物头顶向下挥舞
//        /// </summary>
//        protected float startAngle = 2.5f;
//        /// <summary>
//        /// 终止角度，挥舞结束时的角度
//        /// </summary>
//        protected float endAngle = 2.5f;
//        /// <summary>
//        /// 目标角度，一般为人物向鼠标方向
//        /// </summary>
//        private float targetAngle;
//        /// <summary>
//        /// 挥舞时每一帧旋转的角度
//        /// </summary>
//        private float perAngle;
//        /// <summary>
//        /// 实际角度，通过一系列计算得到的每一帧的弹幕角度
//        /// </summary>
//        public ref float RotateAngle => ref Projectile.ai[1];

//        /// <summary>
//        /// 计时器
//        /// </summary>
//        public ref float Timer => ref Projectile.ai[0];
//        /// <summary>
//        /// 拖尾数组长度
//        /// </summary>
//        protected int TrailLengh = 15;

//        /// <summary>
//        /// 弹幕下端距离玩家手部的距离
//        /// </summary>
//        protected float distanceToHand = 15;
//        /// <summary>
//        /// 弹幕大小，应用圆形轨迹时为最大缩放值
//        /// 应用椭圆形轨迹时根据最小缩放值和最大缩放值计算得值
//        /// </summary>
//        protected float projScale = 100f;
//        /// <summary>
//        /// 弹幕宽度的缩放
//        /// </summary>
//        protected float xScale = 1f;
//        /// <summary>
//        /// 最大缩放值,同时也是椭圆轨迹的长轴半长
//        /// </summary>
//        protected float CircleScale = 1f;
//        /// <summary>
//        /// 椭圆的长半轴长,a
//        /// </summary>
//        protected float halfLongAxis = 1f;
//        /// <summary>
//        /// 椭圆的短半轴长,b
//        /// </summary>
//        protected float halfShortAxis = 0.5f;
//        /// <summary>
//        /// 椭圆焦距半长平方,c的2次方
//        /// </summary>
//        private float halfFocalLength2;
//        /// <summary>
//        /// 弹幕大小
//        /// </summary>
//        protected float projLenth = 100;

//        protected Vector2 RotateVec2;
//        private Vector2 Top;
//        private Vector2 Bottom;
//        #endregion

//        public override void SetStaticDefaults()
//        {
//            DisplayName.SetDefault("测试剑");
//        }

//        public override void SetDefaults()
//        {
//            Projectile.width = 16;
//            Projectile.height = 16;
//            Projectile.tileCollide = false;
//            Projectile.friendly = true;
//            Projectile.penetrate = -1;
//            Projectile.timeLeft = 2000;//设置为2000以防止以外的错误
//            SetDefs();
//            oldRotate = new float[TrailLengh];
//            oldScale = new float[TrailLengh];
//            Top = new Vector2(0, 10);//没什么用的初始化，但懒得删了
//            Bottom = new Vector2(10, 0);
//        }
//        /// <summary>
//        /// 需要设定的值（等号后面的是默认值）
//        /// <para>hasStaticAngle=false，CircleTrail=true</para>
//        /// <para>minTime = 0，maxTime=60</para>
//        /// <para>startAngle=2.5f，endAngle = 2.5f</para>
//        /// <para>TrailLengh = 15，distanceToHand = 15</para>
//        /// <para>xScale=1f，CircleScale=1f（仅应用圆形轨迹时需要更改）</para>
//        /// <para>halfLongAxis=1f，halfShortAxis=0.5f（仅应用椭圆轨迹时需要更改，为1是弹幕为本身大小）</para>
//        /// <para>projLenth=100（应该设置为弹幕贴图的高度）</para>
//        /// 除此之外的变量不要乱改！
//        /// </summary>
//        public abstract void SetDefs();

//        #region AI
//        public sealed override void AI()
//        {
//            AIBefore();
//            if (Timer == 0)
//            {
//                Projectile.velocity *= 0f;//归0速度，本弹幕速度永远为0
//                //设定目标角度
//                if (!hasStaticAngle)//如果为固定角度的挥舞（设定为挥舞向玩家面朝的方向）
//                    targetAngle = (Main.MouseWorld - Owner.Center).ToRotation();
//                else
//                    targetAngle = Owner.direction > 0 ? 0f : 3.14f;
//                RotateAngle = targetAngle - Owner.direction * startAngle;//设定起始角度
//                RotateVec2 = RotateAngle.ToRotationVector2();//设定起始角度的方向向量
//                if (CircleTrail)//判断是否为圆形轨迹，如果不是则进行椭圆的计算
//                    projScale = CircleScale * projLenth;
//                else
//                {//此处运用椭圆的极坐标方程，通过输入的角度得到长度
//                    halfFocalLength2 = halfLongAxis * halfLongAxis - halfShortAxis * halfShortAxis;//c方=a方-b方
//                    float i = (float)Math.Sqrt(halfLongAxis * halfLongAxis - halfFocalLength2 * (Math.Cos(2 * (RotateAngle - targetAngle)) + 1) / 2);//椭圆极坐标方程的分母
//                    projScale = projLenth * halfLongAxis * halfShortAxis / i;
//                }
//                perAngle = Owner.direction * Math.Sign(startAngle) * (Math.Abs(endAngle) + Math.Abs(startAngle)) / maxTime;//计算每一帧应当旋转的角度
//                Projectile.Center = Owner.Center + RotateVec2 * (projScale / 2 + distanceToHand);//设置弹幕中心，虽说并没有使用原版的碰撞检测方法，但是为了绘制的时候方便所以还是计算了一下
//                Projectile.rotation = RotateAngle - 1.57f;//设置旋转
//                onStart = true;//这个其实只用于初始化拖尾数组
//                Projectile.netUpdate = true;
//            }
//            else
//            {
//                if (Timer < minTime)//弹幕生成到开始挥舞之前
//                {
//                    BeforeSlash();
//                }
//                else if (Timer < maxTime)//挥舞过程中
//                {
//                    RotateAngle = targetAngle - Owner.direction * startAngle + (Timer - minTime) * perAngle;//和前面基本一样，不多赘述
//                    RotateVec2 = RotateAngle.ToRotationVector2();
//                    if (!CircleTrail)
//                    {
//                        float i = (float)Math.Sqrt(halfLongAxis * halfLongAxis - halfFocalLength2 * (Math.Cos(2 * (RotateAngle - targetAngle)) + 1) / 2);//椭圆极坐标方程的分母
//                        projScale = projLenth * halfLongAxis * halfShortAxis / i;
//                    }
//                    Projectile.Center = Owner.Center + RotateVec2 * (projScale / 2 + distanceToHand);// 似乎用不上这个了（补充：并不，这里不计算一下的话绘制的时候还得计算）
//                    Projectile.rotation = RotateAngle - 1.57f;
//                    SpawnDustOnSlash();
//                }
//            }

//            if (Timer > maxTime)
//                AfterSlash();
//            AIAfter();
//        }

//        /// <summary>
//        /// 主体AI执行之前执行
//        /// 主要用于控制玩家
//        /// </summary>
//        protected virtual void AIBefore()
//        {
//            Owner.heldProj = Projectile.whoAmI;//让弹幕图层在在玩家手中

//            Owner.itemTime = Owner.itemAnimation = 2;
//            Owner.itemRotation = RotateAngle;
//            if (Timer < minTime)
//            {
//                Owner.direction = Main.MouseWorld.X < Owner.Center.X ? -1 : 1;
//            }
//        }
//        /// <summary>
//        /// 主体AI执行之后执行
//        /// 用于计时
//        /// 用于计算Top和Bottom的值
//        /// 同时要管理拖尾数组
//        /// </summary>
//        protected virtual void AIAfter()
//        {
//            Timer++;
//            Bottom = Owner.Center + RotateVec2 * distanceToHand;//弹幕的底端和顶端计算，用于检测碰撞以及绘制
//            Top = Owner.Center + RotateVec2 * (projScale + distanceToHand);
//            ManageCaches();
//            if (Main.myPlayer != Owner.whoAmI)
//                checkHits();
//        }
//        /// <summary>
//        /// 在挥舞之前执行，设置想要的前摇
//        /// 不需要的话留空，并把minTime设为0
//        /// </summary>
//        protected abstract void BeforeSlash();
//        /// <summary>
//        /// 在挥舞之后执行的，可以用于将Timer设置为0来重新执行挥舞动作（虽说拖尾应该会鬼畜吧）
//        /// 或者可以用于生成其他弹幕
//        /// 或是直接Kill弹幕
//        /// </summary>
//        protected virtual void AfterSlash()
//        {
//            Projectile.Kill();
//        }

//        protected abstract void SpawnDustOnSlash();

//        protected void ManageCaches()
//        {
//            if (!onStart)
//            {
//                if (maxTime < 30)
//                {
//                    for (int i = TrailLengh - 1; i > 0; i--)
//                    {
//                        oldRotate[i] = oldRotate[i - 1];
//                        oldScale[i] = oldScale[i - 1];
//                    }
//                    oldRotate[0] = RotateAngle - perAngle / 2;
//                    float r = (float)Math.Sqrt(halfLongAxis * halfLongAxis - halfFocalLength2 * (Math.Cos(2 * (oldRotate[0] - targetAngle)) + 1) / 2);//椭圆极坐标方程的分母
//                    oldScale[0] = projLenth * halfLongAxis * halfShortAxis / r;
//                }
//                for (int i = TrailLengh - 1; i > 0; i--)
//                {
//                    oldRotate[i] = oldRotate[i - 1];
//                    oldScale[i] = oldScale[i - 1];
//                }
//                oldRotate[0] = RotateAngle;
//                oldScale[0] = projScale;
//            }
//            else
//            {
//                for (int j = TrailLengh - 1; j >= 0; j--)
//                {
//                    oldRotate[j] = 100f;
//                    oldScale[j] = projScale;
//                }
//                onStart = false;
//            }
//        }

//        #endregion

//        #region 碰撞检测

//        public void checkHits()
//        {
//            // done manually for clients that aren't the Projectile owner since onhit methods are clientside
//            foreach (NPC NPC in Main.npc.Where(n => n.active &&
//                 !n.dontTakeDamage &&
//                 !n.townNPC &&
//                 n.immune[Owner.whoAmI] <= 0 &&
//                 Colliding(new Rectangle(), n.Hitbox) == true))
//            {
//                OnHitNPC(NPC, 0, 0, false);
//            }
//        }

//        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
//        {
//            target.velocity += Vector2.UnitX.RotatedBy((target.Center - Owner.Center).ToRotation()) * 10 * target.knockBackResist;

//            target.immune[Projectile.owner] = 10; //等效于普通无敌帧，但显示表示用于兼容多人模式

//            Helper.CheckLinearCollision(Bottom, Top, target.Hitbox, out Vector2 hitPoint); //here to get the point of impact, ideally we dont have to do this twice but for some reasno colliding hook dosent have an actual NPC ref, soo...
//        }

//        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
//        {
//            if (Timer > 1 && Helper.CheckLinearCollision(Bottom, Top, targetHitbox, out Vector2 hitPoint))
//                return true;

//            return false;
//        }
//        #endregion

//        #region 绘制

//        public override bool PreDraw(ref Color lightColor) => false;

//        public override void PostDraw(Color lightColor)
//        {
//            SpriteBatch sb = Main.spriteBatch;
//            DrawSelf(sb, lightColor);
//        }
//        //绘制扭曲
//        public void DrawWarp()
//        {
//            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();
//            float counts = 0f;
//            for (int i = 0; i < oldRotate.Length; i++)
//            {
//                if (oldRotate[i] != 100f)
//                {
//                    counts += 1f;
//                }
//            }
//            for (int j = 0; j < oldRotate.Length; j++)
//            {
//                if (!(oldRotate[j] == 100f))
//                {
//                    float factor = 1f - j / counts;
//                    float w = 1f;
//                    float r = oldRotate[j] % 6.18f;
//                    float dir = (r >= 3.14f ? r - 3.14f : r + 3.14f) / 6.28318548f;
//                    Vector2 direction = oldRotate[j].ToRotationVector2();
//                    bars.Add(new CustomVertexInfo(Owner.Center + direction * distanceToHand, new Color(dir, w, 0f, 1f), new Vector3(factor, 1f, w)));
//                    bars.Add(new CustomVertexInfo(Owner.Center + direction * (oldScale[j] + distanceToHand), new Color(dir, w, 0f, 1f), new Vector3(factor, 0f, w)));
//                }
//            }
//            Main.spriteBatch.End();
//            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

//            Matrix projection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, 0f, 1f);
//            Matrix model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0f)) * Main.GameViewMatrix.ZoomMatrix;

//            Effect effect = Filters.Scene["KEx"].GetShader().Shader;

//            effect.Parameters["uTransform"].SetValue(model * projection);
//            Main.graphics.GraphicsDevice.Textures[0] = WarpTexture;
//            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
//            effect.CurrentTechnique.Passes[0].Apply();
//            if (bars.Count >= 3)
//            {
//                Main.graphics.GraphicsDevice.DrawUserPrimitives<CustomVertexInfo>(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
//            }
//            Main.spriteBatch.End();
//            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
//        }

//        //绘制自己
//        public virtual void DrawSelf(SpriteBatch spriteBatch, Color lightColor)
//        {
//            Main.spriteBatch.End();
//            Main.spriteBatch.Begin(0, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
//            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;

//            Vector2 origin;
//            origin = new(tex.Width / 2, tex.Height / 2);
//            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition,
//                default,
//                Projectile.GetAlpha(lightColor),
//                Projectile.rotation, origin,
//                new Vector2(xScale, projScale / projLenth) * Projectile.scale,
//                (Owner.direction == -1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
//            Main.spriteBatch.End();
//            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
//        }
//        // 绘制拖尾
//        //public virtual void DrawTrail(Color color)
//        //{
//        //    List<VertexInfo> bars = new List<VertexInfo>();
//        //    float counts = 0f;
//        //    for (int i = 0; i < this.TrailVec.Length; i++)
//        //    {
//        //        if (this.TrailVec[i] != Vector2.Zero)
//        //        {
//        //            counts += 1f;
//        //        }
//        //    }
//        //    for (int j = 0; j < TrailVec.Length; j++)
//        //    {
//        //        if (!(TrailVec[j] == Vector2.Zero))
//        //        {
//        //            float factor = 1f - j / counts;
//        //            float w = TrailAlpha(factor) * 1.1f;
//        //            if (!longHandle)
//        //            {
//        //                bars.Add(new VertexInfo(Projectile.Center + TrailVec[j] * 0.15f * Projectile.scale, new Vector3(factor, 1f, 0f)));
//        //                bars.Add(new VertexInfo(Projectile.Center + TrailVec[j] * Projectile.scale, new Vector3(factor, 0f, w)));
//        //            }
//        //            else
//        //            {
//        //                bars.Add(new VertexInfo(Projectile.Center + TrailVec[j] * 0.3f * Projectile.scale, new Vector3(factor, 1f, 0f)));
//        //                bars.Add(new VertexInfo(Projectile.Center + TrailVec[j] * Projectile.scale, new Vector3(factor, 0f, w)));
//        //            }
//        //        }
//        //    }
//        //    Main.spriteBatch.End();
//        //    Main.spriteBatch.Begin(SpriteSortMode.Immediate, TrailBlendState(), SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
//        //    Matrix projection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, 0f, 1f);
//        //    Matrix model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0f)) * Main.GameViewMatrix.ZoomMatrix;

//        //    Effect effect = Filters.Scene["KnifeLight"].GetShader().Shader;

//        //    effect.Parameters["uTransform"].SetValue(model * projection);
//        //    effect.Parameters["tex0"].SetValue(Request<Texture2D>(AssetDirectory.Items+"Knief1").Value);
//        //    effect.Parameters["tex1"].SetValue(Request<Texture2D>(AssetDirectory.Items+"img_color").Value);
//        //    effect.CurrentTechnique.Passes["Trail0"].Apply();
//        //    if (bars.Count >= 3)
//        //    {
//        //        Main.graphics.GraphicsDevice.DrawUserPrimitives<VertexInfo>(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
//        //    }
//        //    Main.spriteBatch.End();
//        //    Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
//        //}

//        public float TrailAlpha(float factor)
//        {
//            if (factor <= 0.5f)
//            {
//                return MathHelper.Lerp(0f, 0.5f, factor * 2f);
//            }
//            return MathHelper.Lerp(0.5f, 0.7f, factor);
//        }
//        public BlendState TrailBlendState() => BlendState.Additive;

//        #endregion
//    }
//}
