using Coralite.Content.Items.Icicle;
using Coralite.Core.Configs;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Graphics.Effects;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core.Prefabs.Projectiles
{
    public abstract class BaseSwingProj : BaseHeldProj
    {
        protected float[] oldRotate;
        protected float[] oldDistanceToOwner;
        public float[] oldLength;

        protected string TrailTexture = AssetDirectory.OtherProjectiles + "NormalSlashTrail";

        protected bool onStart = true;
        protected bool canDrawSelf = true;
        protected bool useShadowTrail = false;
        protected bool useSlashTrail = false;
        protected bool useTurnOnStart = true;
        protected byte onHitTimer = 0;
        protected byte onHitFreeze = 5;

        protected int shadowCount = 5;
        protected int minTime = 0;
        protected int maxTime = 60;
        protected int trailTopWidth = 10;
        protected int trailBottomWidth = 10;
        protected float startAngle = 2.5f;
        protected float totalAngle = 2.5f;
        public float _Rotation;// 实际角度，通过一系列计算得到的每一帧的弹幕角度
        public float Timer;

        protected float spriteRotation;
        protected readonly short trailLength;
        public float distanceToOwner = 15;

        protected ISmoother Smoother;

        public ref Vector2 RotateVec2 => ref Projectile.velocity;
        public Vector2 Top;
        public Vector2 Bottom;

        public BaseSwingProj(float spriteRotation = 0.785f, short trailLength = 15)
        {
            this.spriteRotation = spriteRotation;
            this.trailLength = trailLength;
        }

        public sealed override void SetDefaults()
        {
            Projectile.scale = 1;
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
        /// 可以自由设定的值（等号后面的是默认值）
        /// <para></para>
        /// 开始挥舞前的时间  <see cref="minTime"/>  = 0<br></br>
        /// 挥舞所用时间 <see cref="maxTime"/> = 60<br></br>
        /// 起始角度，为正时则从人物头顶向下挥舞 <see cref="startAngle"/> = 2.5f<br></br>
        /// 终止角度 <see cref="totalAngle"/> = 2.5f<br></br>
        /// 弹幕底部与玩家中心的距离 <see cref="distanceToOwner"/> = 15<br></br>
        /// 贴图旋转角度 <see cref="spriteRotation"/>（水平向右为0）<br></br>
        /// 卡肉时长 onHitFreeze = 5 
        /// <para> 
        /// 拖尾相关部分
        /// </para>
        /// 是否应用影子拖尾  <see cref="useShadowTrail"/> = false，
        /// 是否应用刀光效果 <see cref="useSlashTrail"/> = false，
        /// 是否可绘制自己的贴图 <see cref="canDrawSelf"/> = true <br></br>
        /// 刀光贴图 <see cref="TrailTexture"/> 建议单独声明一个静态<see cref="Asset{T}"/>字段 <br></br>
        /// 影子拖尾数量 <see cref="shadowCount"/>
        /// 拖尾数组长度 <see cref="trailLength"/> = 15 <br></br>
        /// 刀光顶部延申的距离 <see cref="trailTopWidth"/> = 10,
        /// 刀光顶部延申的距离<see cref="trailBottomWidth"/> = 10 <br></br>
        /// </summary>
        public abstract void SetDefs();

        public override bool ShouldUpdatePosition() => false;

        #region AI

        public sealed override void AI()
        {
            AIBefore();

            if (onHitTimer != 0 && VisualEffectSystem.HitEffect_HitFreeze && onHitTimer < onHitFreeze)//轻微的卡肉效果
            {
                Projectile.Center = OwnerCenter() + RotateVec2 * (Projectile.scale * Projectile.height / 2 + distanceToOwner);
                Top = Projectile.Center + RotateVec2 * (Projectile.scale * Projectile.height / 2 + trailTopWidth);
                Bottom = Projectile.Center - RotateVec2 * (Projectile.scale * Projectile.height / 2);//弹幕的底端和顶端计算，用于检测碰撞以及绘制
                onHitTimer++;
                return;
            }

            if (onStart)
            {
                Initializer();
                return;
            }

            if ((int)Timer <= minTime)//弹幕生成到开始挥舞之前
            {
                if (useTurnOnStart)
                    Owner.direction = Main.MouseWorld.X < Owner.Center.X ? -1 : 1;
                BeforeSlash();
            }
            else if ((int)Timer <= maxTime)//挥舞过程中
            {
                OnSlash();
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
            Top = Projectile.Center + RotateVec2 * (Projectile.scale * Projectile.height / 2 + trailTopWidth);
            Bottom = Projectile.Center - RotateVec2 * (Projectile.scale * Projectile.height / 2);//弹幕的底端和顶端计算，用于检测碰撞以及绘制
            Owner.itemRotation = _Rotation + (Owner.direction > 0 ? 0 : MathHelper.Pi);

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
                _Rotation = startAngle = GetStartAngle() - OwnerDirection * startAngle;//设定起始角度
                totalAngle *= OwnerDirection;
            }

            Slasher();
            Smoother.ReCalculate(maxTime - minTime);

            if (useShadowTrail || useSlashTrail)
            {
                oldRotate = new float[trailLength];
                oldDistanceToOwner = new float[trailLength];
                oldLength = new float[trailLength];
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
            Timer++;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Timer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Timer = reader.ReadSingle();
        }

        #region 关于挥舞

        /// <summary>
        /// <para>在挥舞之前执行，设置想要的前摇</para>
        /// <para>不需要的话留空，并把minTime设为0</para>
        /// </summary>
        protected virtual void BeforeSlash() { }

        protected virtual void OnSlash()
        {
            _Rotation = startAngle + totalAngle * Smoother.Smoother((int)Timer - minTime, maxTime - minTime);
            Slasher();
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
            Projectile.Center = OwnerCenter() + RotateVec2 * (Projectile.scale * Projectile.height / 2 + distanceToOwner);
            Projectile.rotation = _Rotation;
        }

        /// <summary>
        /// 获取这个弹幕所有者的中心，如果是npc的话就要自己找一下npc了
        /// </summary>
        /// <returns></returns>
        protected virtual Vector2 OwnerCenter()
        {
            return Owner.Center;
        }

        #endregion

        protected virtual void InitializeCaches()
        {
            for (int j = trailLength - 1; j >= 0; j--)
            {
                oldRotate[j] = 100f;
                oldDistanceToOwner[j] = distanceToOwner;
                oldLength[j] = Projectile.height * Projectile.scale;
            }
        }

        protected virtual void UpdateCaches()
        {
            for (int i = trailLength - 1; i > 0; i--)
            {
                oldRotate[i] = oldRotate[i - 1];
                oldDistanceToOwner[i] = oldDistanceToOwner[i - 1];
                oldLength[i] = oldLength[i - 1];
            }

            oldRotate[0] = _Rotation;
            oldDistanceToOwner[0] = distanceToOwner;
            oldLength[0] = Projectile.height * Projectile.scale;
        }

        protected virtual void SpawnDustOnSlash() { }

        #endregion

        #region 碰撞部分

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (onHitTimer == 0)
                onHitTimer = 1;

            OnHitEvent(target, hit, damageDone);
        }

        /// <summary>
        /// 可用于在命中时生成粒子或执行特定操作，例如给予玩家无敌帧
        /// </summary>
        protected virtual void OnHitEvent(NPC target, NPC.HitInfo hit, int damageDone) { }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.noTileCollide || target.friendly || Projectile.hostile)
                return null;

            return Collision.CanHit(Owner, target);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if ((int)Timer < minTime /*|| !Collision.CanHit(OwnerCenter(), 0, 0, targetHitbox.Center.ToVector2(), 0,0)*/)
                return false;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Bottom, Top, Projectile.width / 2, ref Projectile.localAI[1]);
        }

        #endregion

        #region 绘制

        public override bool PreDraw(ref Color lightColor)
        {
            if (useSlashTrail && VisualEffectSystem.DrawKniefLight && Timer > minTime)
                DrawSlashTrail();

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(0, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Texture2D mainTex = Projectile.GetTexture();
            Vector2 origin = new Vector2(mainTex.Width / 2, mainTex.Height / 2);

            int dir = Math.Sign(totalAngle);
            float extraRot = OwnerDirection < 0 ? MathHelper.Pi : 0;
            extraRot += OwnerDirection == dir ? 0 : MathHelper.Pi;
            extraRot += spriteRotation * dir;

            if (useShadowTrail && Timer > minTime)
                DrawShadowTrail(mainTex, origin, lightColor, extraRot);

            if (canDrawSelf)
                DrawSelf(mainTex, origin, lightColor, extraRot);
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        protected virtual void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, mainTex.Frame(),
                                                lightColor, Projectile.rotation + extraRot, origin, Projectile.scale, CheckEffect(), 0f);
        }

        protected virtual void DrawSlashTrail()
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

                var color = GetTrailColor(factor);
                var w = Helper.Lerp(0.5f, 0.05f, factor);
                bars.Add(new(Top - Main.screenPosition, color, new Vector3((float)Math.Sqrt(factor), 1, w)));
                bars.Add(new(Bottom - Main.screenPosition, color, new Vector3((float)Math.Sqrt(factor), 0, w)));
            }

            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

                Main.graphics.GraphicsDevice.Textures[0] = Request<Texture2D>(TrailTexture).Value;
                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            }
        }

        protected virtual void DrawShadowTrail(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            if ((int)Timer > minTime)
            {
                for (int i = shadowCount; i > 0; i--)
                {
                    if (oldRotate[i] != 100f)
                        Main.spriteBatch.Draw(mainTex, Owner.Center + oldRotate[i].ToRotationVector2() * oldDistanceToOwner[i] - Main.screenPosition, mainTex.Frame(),
                                                            lightColor * (0.1f + i * 0.01f), oldRotate[i] + extraRot, origin, Projectile.scale * (1f - i * 0.1f), CheckEffect(), 0);
                }
            }
        }

        protected virtual float ControlTrailBottomWidth(float factor)
        {
            return trailBottomWidth * (1 - factor);
        }

        //protected string CheckTexture()
        //{
        //    if (totalAngle > 0)
        //        return Texture;

        //    if (isSymmetrical)
        //        return Texture;

        //    return Texture + "_Flip";
        //}

        protected SpriteEffects CheckEffect()
        {
            if (OwnerDirection < 0)
            {
                if (totalAngle > 0)
                    return SpriteEffects.None;
                return SpriteEffects.FlipHorizontally;
            }

            if (totalAngle > 0)
                return SpriteEffects.None;
            return SpriteEffects.FlipHorizontally;
        }

        /// <summary>
        /// 用以获取中心点，默认为玩家中心。
        /// <para>设置为弹幕中心后可实现将弹幕扔出的挥舞。注意，要实现这种效果需要准备oldCenter数组，使用局部变量i来获取旧位置</para>
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        protected virtual Vector2 GetCenter(int i) => OwnerCenter();

        protected virtual Color GetTrailColor(float factor) => Color.White;

        protected virtual void GetCurrentTrailCount(out float count)
        {
            count = 0f;
            if (oldRotate == null)
                return;

            for (int i = 0; i < oldRotate.Length; i++)
                if (oldRotate[i] != 100f)
                    count += 1f;
        }

        #endregion

        #region OtherHelperMethod

        public void WarpDrawer(float trailBottomExtraMult)
        {
            if (Timer < minTime || oldRotate == null)
                return;

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();
            GetCurrentTrailCount(out float count);

            float w = 1f;
            for (int i = 0; i < oldRotate.Length; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - i / count;
                Vector2 Center = GetCenter(i);
                float r = oldRotate[i] % 6.18f;
                float dir = (r >= 3.14f ? r - 3.14f : r + 3.14f) / MathHelper.TwoPi;
                Vector2 Top = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]);
                Vector2 Bottom = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] - ControlTrailBottomWidth(factor) * trailBottomExtraMult + oldDistanceToOwner[i]);

                bars.Add(new CustomVertexInfo(Top, new Color(dir, w, 0f, 1f), new Vector3(factor, 0f, w)));
                bars.Add(new CustomVertexInfo(Bottom, new Color(dir, w, 0f, 1f), new Vector3(factor, 1f, w)));
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, 0f, 1f);
            Matrix model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0f)) * Main.GameViewMatrix.TransformationMatrix;

            Effect effect = Filters.Scene["KEx"].GetShader().Shader;

            effect.Parameters["uTransform"].SetValue(model * projection);
            Main.graphics.GraphicsDevice.Textures[0] = FrostySwordSlash.WarpTexture.Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            effect.CurrentTechnique.Passes[0].Apply();
            if (bars.Count >= 3)
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        #endregion
    }
}
