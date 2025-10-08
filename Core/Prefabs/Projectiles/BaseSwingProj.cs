using Coralite.Content.Items.Icicle;
using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Enums;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core.Prefabs.Projectiles
{
    public abstract class BaseSwingProj(float spriteRotation = 0.785f, short trailCount = 15) : BaseHeldProj
    {
        #region Data
        /// <summary>
        /// 旧的旋转角度记录
        /// </summary>
        protected float[] oldRotate;
        /// <summary>
        /// 旧的与玩家的距离记录
        /// </summary>
        protected float[] oldDistanceToOwner;
        /// <summary>
        /// 旧的长度记录
        /// </summary>
        public float[] oldLength;
        /// <summary>
        /// 刀光贴图路径
        /// </summary>
        protected string TrailTexture = AssetDirectory.Trails + "Slash";
        /// <summary>
        /// 是否处于初始状态，默认 <see langword="true"/>
        /// </summary>
        protected bool onStart = true;
        /// <summary>
        /// 是否已初始化，默认 <see langword="true"/>
        /// </summary>
        protected bool init = true;
        /// <summary>
        /// 是否发送基础网络数据，默认 <see langword="true"/>
        /// </summary>
        protected bool netSendBasicValues = false;
        /// <summary>
        /// 是否可绘制自身贴图，默认 <see langword="true"/>
        /// </summary>
        protected bool canDrawSelf = true;
        /// <summary>
        /// 是否启用影子拖尾，默认 <see langword="false"/>
        /// </summary>
        protected bool useShadowTrail = false;
        /// <summary>
        /// 是否启用刀光拖尾，默认 <see langword="false"/>
        /// </summary>
        protected bool useSlashTrail = false;
        /// <summary>
        /// 是否在初始时启用转向，默认 <see langword="true"/>
        /// </summary>
        protected bool useTurnOnStart = true;
        /// <summary>
        /// 受击计时器，默认 0
        /// </summary>
        protected byte onHitTimer = 0;
        /// <summary>
        /// 受击冻结时间，默认 5 帧
        /// </summary>
        protected byte onHitFreeze = 5;
        /// <summary>
        /// 影子拖尾数量，默认 5
        /// </summary>
        protected int shadowCount = 5;
        /// <summary>
        /// 挥舞前的最小时间，默认 0 帧
        /// </summary>
        protected int minTime = 0;
        /// <summary>
        /// 挥舞的最大时间，默认 60 帧
        /// </summary>
        protected int maxTime = 60;
        /// <summary>
        /// 拖尾顶部延伸距离，默认 10
        /// </summary>
        protected int trailTopWidth = 10;
        /// <summary>
        /// 拖尾底部延伸距离，默认 10
        /// </summary>
        protected int trailBottomWidth = 10;
        /// <summary>
        /// 初始角度（正值表示从头顶向下挥舞），默认 2.5f
        /// </summary>
        protected float startAngle = 2.5f;
        /// <summary>
        /// 终止角度，默认 2.5f
        /// </summary>
        protected float totalAngle = 2.5f;
        /// <summary>
        /// 实际角度，每帧计算得到
        /// </summary>
        public float _Rotation;
        /// <summary>
        /// 计时器
        /// </summary>
        public float Timer;
        /// <summary>
        /// 贴图旋转角度（水平向右为 0）
        /// </summary>
        protected float spriteRotation = spriteRotation;
        /// <summary>
        /// 拖尾数组长度，默认 15
        /// </summary>
        protected short trailCount = trailCount;
        /// <summary>
        /// 弹幕底部与玩家中心的距离，默认 15
        /// </summary>
        public float distanceToOwner = 15;
        /// <summary>
        /// 平滑处理器接口
        /// </summary>
        protected ISmoother Smoother;
        /// <summary>
        /// 旋转向量引用（指向弹幕速度）
        /// </summary>
        public ref Vector2 RotateVec2 => ref Projectile.velocity;
        /// <summary>
        /// 顶部位置
        /// </summary>
        public Vector2 Top;
        /// <summary>
        /// 底部位置
        /// </summary>
        public Vector2 Bottom;
        public override bool CanFire => true;
        #endregion
        /// <summary>
        /// 设定挥舞属性
        /// </summary>
        public abstract void SetSwingProperty();

        public sealed override void SetDefaults()
        {
            Projectile.scale = 1;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Smoother = new BezierEaseSmoother();
            SetSwingProperty();

            Projectile.netUpdate = true;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override bool ShouldUpdatePosition() => false;
        public override bool? CanCutTiles() => false;

        /// <summary>
        /// 是否能破坏特殊物块，默认判断伤害>0可破坏
        /// </summary>
        /// <returns></returns>
        public virtual bool CanCutTiles_Sp() => Projectile.damage > 0;

        #region AI

        public sealed override void AI()
        {
            AIBefore();

            if (/*VaultUtils.isClient &&*/ onHitTimer != 0 && VisualEffectSystem.HitEffect_HitFreeze
                && onHitTimer < onHitFreeze)//轻微的卡肉效果
            {
                Projectile.Center = OwnerCenter() + (RotateVec2 * ((Projectile.scale * Projectile.height / 2) + distanceToOwner));
                Top = Projectile.Center + (RotateVec2 * ((Projectile.scale * Projectile.height / 2) + trailTopWidth));
                Bottom = Projectile.Center - (RotateVec2 * (Projectile.scale * Projectile.height / 2));//弹幕的底端和顶端计算，用于检测碰撞以及绘制
                onHitTimer++;
                return;
            }

            if (onStart)
            {
                if (init && Projectile.IsOwnedByLocalPlayer())
                {
                    InitBasicValues();
                    InitializeSwing();
                }

                if (!Projectile.IsOwnedByLocalPlayer())//客户端和其他端等待本地端同步完数据后再运行
                    return;

                return;
            }

            if ((int)Timer <= minTime)//弹幕生成到开始挥舞之前
            {
                if (useTurnOnStart)
                    Owner.direction = InMousePos.X < Owner.Center.X ? -1 : 1;
                BeforeSlash();
            }
            else if ((int)Timer <= maxTime)//挥舞过程中
            {
                OnSlash();
                SpawnDustOnSlash();
                if (Projectile.IsOwnedByLocalPlayer() && CanCutTiles_Sp())
                {
                    bool[] tileCutIgnorance = Owner.GetTileCutIgnorance(allowRegrowth: false, false);
                    DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
                    DelegateMethods.tileCutIgnore = tileCutIgnorance;
                    Utils.PlotTileLine(Top, Bottom, Projectile.width, new Utils.TileActionAttempt(DelegateMethods.CutTiles));
                }
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
            Top = Projectile.Center + (RotateVec2 * ((Projectile.scale * Projectile.height / 2) + trailTopWidth));
            Bottom = Projectile.Center - (RotateVec2 * (Projectile.scale * Projectile.height / 2));//弹幕的底端和顶端计算，用于检测碰撞以及绘制
            Owner.itemRotation = _Rotation + (Owner.direction > 0 ? 0 : MathHelper.Pi);

            if (!VaultUtils.isServer && (useShadowTrail || useSlashTrail))
                UpdateCaches();
        }

        /// <summary>
        /// 用于各项初始化操作
        /// </summary>
        protected virtual void InitializeSwing()
        {
            Projectile.velocity *= 0f;
            if (Projectile.IsOwnedByLocalPlayer())
            {
                _Rotation = startAngle = GetStartAngle() - (DirSign * startAngle);//设定起始角度
                totalAngle *= DirSign;
                Projectile.netUpdate = true;
                onStart = false;
                netSendBasicValues = true;
                init = false;
            }

            Slasher();
            Smoother.ReCalculate(maxTime - minTime);

            if (!VaultUtils.isServer && (useShadowTrail || useSlashTrail))
            {
                oldRotate ??= new float[trailCount];
                oldDistanceToOwner ??= new float[trailCount];
                oldLength ??= new float[trailCount];
                InitializeCaches();
            }
        }

        protected virtual void InitBasicValues()
        {

        }

        /// <summary>
        /// 获取起始角度，一般是鼠标的角度，也可以固定成指定的角度
        /// </summary>
        /// <returns></returns>
        protected virtual float GetStartAngle()
        {
            return (InMousePos - Owner.Center).ToRotation();
        }

        /// <summary>
        /// 用于计时或按需要计时
        /// </summary>
        protected virtual void TimeUpdater()
        {
            Timer++;
        }
        
        #region 关于挥舞

        /// <summary>
        /// <para>在挥舞之前执行，设置想要的前摇</para>
        /// <para>不需要的话留空，并把minTime设为0</para>
        /// </summary>
        protected virtual void BeforeSlash() { }

        protected virtual void OnSlash()
        {
            _Rotation = startAngle + (totalAngle * Smoother.Smoother((int)Timer - minTime, maxTime - minTime));
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
        protected virtual void Slasher()
        {
            RotateVec2 = _Rotation.ToRotationVector2();
            Projectile.Center = OwnerCenter() + (RotateVec2 * ((Projectile.scale * Projectile.height / 2) + distanceToOwner));
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
            if (VaultUtils.isServer || oldRotate == null || oldDistanceToOwner == null || oldLength == null)
            {
                return;
            }
            for (int j = trailCount - 1; j >= 0; j--)
            {
                oldRotate[j] = 100f;
                oldDistanceToOwner[j] = distanceToOwner;
                oldLength[j] = Projectile.height * Projectile.scale;
            }
        }

        protected virtual void UpdateCaches()
        {
            if (VaultUtils.isServer || oldRotate == null || oldDistanceToOwner == null || oldLength == null)
            {
                return;
            }

            for (int i = trailCount - 1; i > 0; i--)
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
            {
                onHitTimer = 1;
                if (Projectile.IsOwnedByLocalPlayer())
                    Projectile.netUpdate = true;
            }

            OnHitEvent(target, hit, damageDone);
        }

        /// <summary>
        /// 可用于在命中时生成粒子或执行特定操作，例如给予玩家无敌帧
        /// </summary>
        protected virtual void OnHitEvent(NPC target, NPC.HitInfo hit, int damageDone) { }

        public override bool? CanHitNPC(NPC target)
        {
            if (Timer < minTime || Timer > maxTime)
                return false;

            if (target.noTileCollide || target.friendly || Projectile.hostile)
                return null;

            if (Collision.CanHit(Owner, target))
                return null;

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if ((int)Timer < minTime /*|| !Collision.CanHit(OwnerCenter(), 0, 0, targetHitbox.Center.ToVector2(), 0,0)*/)
                return false;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Bottom, Top, Projectile.width / 2, ref Projectile.localAI[1]);
        }

        #endregion

        #region 同步

        public override BitsByte SendBitsByte(BitsByte flags)
        {
            flags = base.SendBitsByte(flags);

            flags[2] = onStart;
            flags[3] = netSendBasicValues;

            return flags;
        }

        public override void ReceiveBitsByte(BitsByte flags)
        {
            base.ReceiveBitsByte(flags);

            onStart = flags[2];
            netSendBasicValues = flags[3];
        }

        public override void NetHeldSend(BinaryWriter writer)
        {
            if (netSendBasicValues)
            {
                writer.Write(Timer);
                writer.Write(_Rotation);
                writer.Write(startAngle);
                writer.Write(totalAngle);
                writer.Write(distanceToOwner);

                netSendBasicValues = false;
            }

            writer.Write(onHitTimer);
        }

        public override void NetHeldReceive(BinaryReader reader)
        {
            if (netSendBasicValues)
            {
                if (init)
                    InitBasicValues();

                Timer = reader.ReadSingle();
                _Rotation = reader.ReadSingle();
                startAngle = reader.ReadSingle();
                totalAngle = reader.ReadSingle();
                distanceToOwner = reader.ReadSingle();

                if (init)
                {
                    InitializeSwing();
                    init = false;
                }

                if (VaultUtils.isServer)
                {
                    Projectile.netUpdate = true;
                    netSendBasicValues = true;
                }
                else
                    netSendBasicValues = false;
            }

            onHitTimer = reader.ReadByte();
        }

        #endregion

        #region 绘制

        public override bool PreDraw(ref Color lightColor)
        {
            if (onStart)
                return false;

            if (useSlashTrail && VisualEffectSystem.DrawKniefLight && Timer > minTime)
                DrawSlashTrail();

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            if (onStart)
                return;

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(0, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Texture2D mainTex = Projectile.GetTexture();
            Vector2 origin = new(mainTex.Width / 2, mainTex.Height / 2);

            float extraRot = GetExRot();

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
            List<ColoredVertex> bars = new();

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
                Vector2 Top = Center + (oldRotate[i].ToRotationVector2() * (Projectile.height + trailTopWidth + oldDistanceToOwner[i]));
                Vector2 Bottom = Center + (oldRotate[i].ToRotationVector2() * (Projectile.height - ControlTrailBottomWidth(factor) + oldDistanceToOwner[i]));

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
                        Main.spriteBatch.Draw(mainTex, Owner.Center + (oldRotate[i].ToRotationVector2() * oldDistanceToOwner[i]) - Main.screenPosition, mainTex.Frame(),
                                                            lightColor * (0.1f + (i * 0.01f)), oldRotate[i] + extraRot, origin, Projectile.scale * (1f - (i * 0.1f)), CheckEffect(), 0);
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

        protected virtual float GetExRot()
        {
            int dir = Math.Sign(totalAngle);
            float extraRot = DirSign < 0 ? MathHelper.Pi : 0;
            extraRot += DirSign == dir ? 0 : MathHelper.Pi;
            extraRot += spriteRotation * dir;

            return extraRot;
        }

        protected virtual SpriteEffects CheckEffect()
        {
            if (DirSign < 0)
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
            if (VaultUtils.isServer)
            {
                return;
            }

            if (oldRotate == null)
                return;

            for (int i = 0; i < oldRotate.Length; i++)
                if (oldRotate[i] != 100f)
                    count += 1f;
        }

        #endregion

        #region OtherHelperMethod

        public void WarpDrawer(float trailBottomExtraMult, float alpha = 1f, float warpStrength = 0.25f)
        {
            if (Timer < minTime || oldRotate == null)
                return;

            List<ColoredVertex> bars = new();
            GetCurrentTrailCount(out float count);

            float w = 1f;
            for (int i = 0; i < count; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - (i / count);
                Vector2 Center = GetCenter(i);
                float r = oldRotate[i] % 6.18f;
                float dir = (r >= 3.14f ? r - 3.14f : r + 3.14f) / MathHelper.TwoPi;
                Vector2 Top = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]));
                Vector2 Bottom = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] - (ControlTrailBottomWidth(factor) * trailBottomExtraMult) + oldDistanceToOwner[i]));

                bars.Add(new ColoredVertex(Top, new Color(dir, warpStrength, 0f, alpha), new Vector3(factor, 0f, w)));
                bars.Add(new ColoredVertex(Bottom, new Color(dir, warpStrength, 0f, alpha), new Vector3(factor, 1f, w)));
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, 0f, 1f);
            Matrix model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0f)) * Main.GameViewMatrix.TransformationMatrix;

            Effect effect = ShaderLoader.GetShader("KEx2");

            effect.Parameters["uTransform"].SetValue(model * projection);
            Main.graphics.GraphicsDevice.Textures[0] = CoraliteAssets.Trail.WarpTex.Value;
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
