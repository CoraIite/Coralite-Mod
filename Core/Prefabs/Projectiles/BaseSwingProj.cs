using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core.Prefabs.Projectiles
{
    public abstract class BaseSwingProj : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public override bool ShouldUpdatePosition() => false;

        protected float[] oldRotate;
        protected float[] oldDistanceToOwner;

        protected string TrailTexture = AssetDirectory.OtherProjectiles + "SlashTrail";

        protected Color color1 = Color.White;
        protected Color color2 = Color.White;

        protected bool onStart = true;
        protected bool isSymmetrical = true;
        protected bool canDrawSelf = true;
        protected bool useShadowTrail = false;
        protected bool useSlashTrail = false;
        protected byte onHit = 0;
        protected byte onHitFreeze = 5;

        protected int shadowCount = 5;
        protected int minTime = 0;
        protected int maxTime = 60;
        protected int trailTopWidth = 10;
        protected int trailBottomWidth = 10;
        protected float startAngle = 2.5f;
        protected float totalAngle = 2.5f;
        public ref float _Rotation => ref Projectile.ai[1];// 实际角度，通过一系列计算得到的每一帧的弹幕角度
        protected readonly float spriteRotation;//2.445?之前写的是这个数，但我忘了当时是怎么得到这个数的，重新算了一下发现不对劲
        public ref float timer => ref Projectile.localAI[0];

        protected readonly short trailLength;
        public float distanceToOwner = 15;

        protected ISmoother Smoother;

        public ref Vector2 RotateVec2 => ref Projectile.velocity;
        public Vector2 Top;
        private Vector2 Bottom;

        public BaseSwingProj(float spriteRotation = 2.356f, short trailLength = 15)
        {
            this.spriteRotation = spriteRotation;
            this.trailLength = trailLength;
        }

        public sealed override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Smoother = new BezierEaseSmoother();
            SetDefs();

            Projectile.netUpdate = true;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        /// <summary>
        /// 需要设定的值（等号后面的是默认值）
        /// <para>开始挥舞前的时间  minTime = 0，挥舞所用时间 maxTime = 60</para>
        /// <para>起始角度，为正时则从人物头顶向下挥舞 startAngle = 2.5f，终止角度 endAngle = 2.5f</para>
        /// <para>弹幕中心与玩家中心的距离 distanceToOwner = 15</para>
        /// <para>贴图旋转角度 SpriteRotation（竖直向下:  0f ;右上斜45度: +2.356f）</para>
        /// <para>卡肉时长 onHitFreeze = 5</para>
        /// <![CDATA[拖尾部分]]>
        /// <para>是否应用影子拖尾 useShadowTrail = false，是否应用刀光效果useSlashTrail = false，刀光贴图TrailTexture</para>
        /// <para>是否可绘制自己的贴图 canDrawSelf = true</para>
        /// <para>影子拖尾数量 ShadowCount，拖尾数组长度 TrailLength = 15，刀光渐变色 color1，color2</para>
        /// <para>刀光拖尾宽度 trailBottomLength = 10,trailTopWidth = 10</para>
        /// <para>是否为对称的，是的话就不需要多加Flip贴图 isSymmetrical = true</para>
        /// 除此之外的变量不要乱改！除非你知道你在做什么（笑）
        /// </summary>
        public abstract void SetDefs();

        #region AI

        public sealed override void AI()
        {
            AIBefore();

            if (onHit != 0 && onHit < onHitFreeze)//轻微的卡肉效果
            {
                Projectile.Center = Owner.Center + RotateVec2 * (Projectile.height / 2 + distanceToOwner);
                onHit++;
                return;
            }

            if (onStart)
            {
                Initializer();
                return;
            }

            if ((int)timer <= minTime)//弹幕生成到开始挥舞之前
            {
                Owner.direction = Main.MouseWorld.X < Owner.Center.X ? -1 : 1;
                BeforeSlash();
            }
            else if ((int)timer <= maxTime)//挥舞过程中
            {
                Slash();
                SpawnDustOnSlash();
            }
            else
                AfterSlash();

            AIAfter();
            TimeUpdater();
        }

        /// <summary>
        /// 主要用于控制玩家
        /// </summary>
        protected virtual void AIBefore()
        {
            Owner.heldProj = Projectile.whoAmI;//让弹幕图层在在玩家手中
            Owner.itemTime = Owner.itemAnimation = 2;//这个东西不为0的时候就无法使用其他物品
        }

        /// <summary>
        /// <para>计算Top和Bottom的值</para>
        /// <para>同时管理拖尾数组</para>
        /// </summary>
        protected virtual void AIAfter()
        {
            Top = Projectile.Center + RotateVec2 * (Projectile.height / 2 + trailTopWidth);
            Bottom = Projectile.Center - RotateVec2 * (Projectile.height / 2);//弹幕的底端和顶端计算，用于检测碰撞以及绘制
            Owner.itemRotation = Owner.direction > 0 ? _Rotation : _Rotation - 3.141f;

            if (useShadowTrail || useSlashTrail)
                UpdateCaches();
        }

        /// <summary>
        /// 用于各项初始化操作
        /// </summary>
        protected virtual void Initializer()
        {
            Projectile.velocity *= 0f;
            if (Owner.whoAmI == Main.myPlayer)
            {
                _Rotation = startAngle = GetStartAngle() - Owner.direction * startAngle;//设定起始角度
                totalAngle *= Owner.direction;
            }

            Slasher();
            Smoother.ReCalculate(maxTime - minTime);

            if (useShadowTrail || useSlashTrail)
            {
                oldRotate = new float[trailLength];
                oldDistanceToOwner = new float[trailLength];
                InitializeCaches();
            }

            onStart = false;
            Projectile.netUpdate = true;
        }

        /// <summary>
        /// 获取起始角度，一般是鼠标的角度，也可以固定成指定的角度
        /// </summary>
        /// <returns></returns>
        protected virtual float GetStartAngle()
        {
            return (Main.MouseWorld - Owner.Center).ToRotation();
        }

        /// <summary>
        /// 用于计时或按需要计时
        /// </summary>
        protected virtual void TimeUpdater()
        {
            timer++;
        }

        #region 关于挥舞

        /// <summary>
        /// <para>在挥舞之前执行，设置想要的前摇</para>
        /// <para>不需要的话留空，并把minTime设为0</para>
        /// </summary>
        protected virtual void BeforeSlash() { }

        protected virtual void Slash()
        {
            _Rotation = startAngle + totalAngle * Smoother.Smoother((int)timer - minTime, maxTime - minTime);
            Slasher();
            Projectile.netUpdate = true;
        }

        /// <summary>
        /// <para>在挥舞之后执行的，可以用于将Timer设置为0来重新执行挥舞动作</para>
        /// <para>或者可以用于生成其他弹幕</para>
        /// <para>最后一定要Kill弹幕</para>
        /// </summary>
        protected virtual void AfterSlash()
        {
            Projectile.Kill();
        }

        /// <summary>
        /// 由_Rotation计算方向向量，并改变弹幕中心和角度
        /// </summary>
        protected void Slasher()
        {
            RotateVec2 = _Rotation.ToRotationVector2();
            Projectile.Center = Owner.Center + RotateVec2 * (Projectile.height / 2 + distanceToOwner);
            Projectile.rotation = _Rotation - 1.57f;
        }

        #endregion

        protected virtual void UpdateCaches()
        {
            for (int i = trailLength - 1; i > 0; i--)
            {
                oldRotate[i] = oldRotate[i - 1];
                oldDistanceToOwner[i] = oldDistanceToOwner[i - 1];
            }

            oldRotate[0] = _Rotation;
            oldDistanceToOwner[0] = distanceToOwner;
        }

        protected virtual void InitializeCaches()
        {
            for (int j = trailLength - 1; j >= 0; j--)
            {
                oldRotate[j] = 100f;
                oldDistanceToOwner[0] = distanceToOwner;
            }
        }

        protected virtual void SpawnDustOnSlash() { }

        #endregion

        #region 碰撞部分

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (onHit == 0)
                onHit = 1;

            OnHitEvent(target);
        }

        /// <summary>
        /// 可用于在命中时生成粒子或执行特定操作，例如给予玩家无敌帧
        /// </summary>
        protected virtual void OnHitEvent(NPC target) { }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if ((int)timer < minTime||!Collision.CanHitLine(Owner.Center,1,1,targetHitbox.Center.ToVector2(),1,1))
                return false;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Top, Bottom, Projectile.width / 2, ref Projectile.localAI[1]);
        }

        #endregion

        #region 绘制

        public override bool PreDraw(ref Color lightColor)
        {
            if (useSlashTrail)
                DrawSlashTrail();
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(0, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Texture2D mainTex = Request<Texture2D>(CheckTexture()).Value;
            Vector2 origin = new Vector2(mainTex.Width / 2, mainTex.Height / 2);

            if (useShadowTrail)
                DrawShadowTrail(mainTex, origin, lightColor);

            if (canDrawSelf)
                Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, mainTex.Frame(),
                                                    lightColor, Projectile.rotation + spriteRotation, origin, Projectile.scale, CheckEffect(), 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        //public void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor)
        //{
        //    Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, mainTex.Frame(),
        //                        lightColor, Projectile.rotation + SpriteRotation, origin, Projectile.scale, CheckEffect(), 0f);
        //}

        protected void DrawSlashTrail()
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

            float length = 1;
            for (int i = 1; i < oldRotate.Length; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;
                length += 1;
            }

            for (int i = 1; i < oldRotate.Length; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = i / length;
                Vector2 Center = GetCenter(i);
                Vector2 Top = Center + oldRotate[i].ToRotationVector2() * (Projectile.height + trailTopWidth + oldDistanceToOwner[i]);
                Vector2 Bottom = Center + oldRotate[i].ToRotationVector2() * (Projectile.height - ControlTrailBottomWidth(factor) + oldDistanceToOwner[i]);

                var color = Color.Lerp(color1, color2, factor);
                var w = Helper.Lerp(0.5f, 0.05f, factor);
                bars.Add(new(Top - Main.screenPosition, color, new Vector3((float)Math.Sqrt(factor), 1, w)));
                bars.Add(new(Bottom - Main.screenPosition, color, new Vector3((float)Math.Sqrt(factor), 0, w)));
            }

            if (bars.Count > 2)
            {
                List<CustomVertexInfo> triangleList = new();
                for (int i = 0; i < bars.Count - 2; i += 2)
                {
                    triangleList.Add(bars[i]);
                    triangleList.Add(bars[i + 2]);
                    triangleList.Add(bars[i + 1]);

                    triangleList.Add(bars[i + 1]);
                    triangleList.Add(bars[i + 2]);
                    triangleList.Add(bars[i + 3]);
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

                Main.graphics.GraphicsDevice.Textures[0] = Request<Texture2D>(TrailTexture).Value;
                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin();
            }
        }

        protected void DrawShadowTrail(Texture2D mainTex, Vector2 origin, Color lightColor)
        {
            if ((int)timer > minTime)
            {
                for (int i = shadowCount; i > 0; i--)
                {
                    if (oldRotate[i] != 100f)
                        Main.spriteBatch.Draw(mainTex, Owner.Center + oldRotate[i].ToRotationVector2() * oldDistanceToOwner[i] - Main.screenPosition, mainTex.Frame(),
                                                            new Color(lightColor.R, lightColor.G, lightColor.B, 0.1f + i * 0.01f), oldRotate[i] - 1.57f + spriteRotation, origin, Projectile.scale * (1f - i * 0.1f), CheckEffect(), 0);
                }
            }
        }

        protected virtual float ControlTrailBottomWidth(float factor)
        {
            return trailBottomWidth * (1 - factor);
        }

        protected string CheckTexture()
        {
            if (totalAngle > 0)
                return Texture;

            if (isSymmetrical)
                return Texture;

            return Texture + "_Flip";
        }

        private SpriteEffects CheckEffect()
        {
            if (spriteRotation != 0f)
                return SpriteEffects.None;

            if (Owner.direction == -1)
                return SpriteEffects.None;

            return SpriteEffects.FlipHorizontally;
        }

        /// <summary>
        /// 用以获取中心点，默认为玩家中心。
        /// <para>设置为弹幕中心后可实现将弹幕扔出的挥舞。注意，要实现这种效果需要准备oldCenter数组，使用局部变量i来获取旧位置</para>
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        protected virtual Vector2 GetCenter(int i)
        {
            return Owner.Center;
        }


        #endregion
    }

}
