using Coralite.Content.DamageClasses;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    /// <summary>
    /// 抓手类弹幕，弹幕本体贴图为抓手贴图，把手贴图需要额外传入
    /// </summary>
    public abstract class BaseTongsProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.FairyCatcherTong + Name;

        /// <summary>
        /// 把手位置的偏移量
        /// </summary>
        public virtual Vector2 HandelOffset { get => Vector2.Zero; }

        /// <summary>
        /// 把手的长度，决定弹幕常态位于何处
        /// </summary>
        public abstract Vector2 TongPosOffset { get; }

        /// <summary>
        /// 抓手最大飞行距离
        /// </summary>
        public abstract int MaxFlyLength { get; }
        /// <summary>
        /// 物品类型，手持非此物品时会沙雕弹幕
        /// </summary>
        public abstract int ItemType { get; }

        /// <summary>
        /// 抓手的距离，经过增幅后的值
        /// </summary>
        public int FlyLength;
        private int catchPower;

        public ref float Catch => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public AIStates State
        {
            get => (AIStates)Projectile.ai[2];
            set => Projectile.ai[2] = (int)value;
        }

        public ref float HandleRot => ref Projectile.localAI[0];
        public ref float RecordSpeed => ref Projectile.localAI[1];

        /// <summary>
        /// 是否把手柄绘制在最顶部
        /// </summary>
        public virtual bool DrawHnadleOnTop { get; } = false;

        public enum AIStates
        {
            /// <summary>
            /// 在手中的样子
            /// </summary>
            Idle,
            /// <summary>
            /// 投掷出的状态
            /// </summary>
            Flying,
            /// <summary>
            /// 收回的状态
            /// </summary>
            Backing
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.hide = true;
            Projectile.penetrate = -1;
            Projectile.width = Projectile.height = 16;
        }

        public override bool ShouldUpdatePosition()
        {
            return State != AIStates.Idle;
        }

        public override bool? CanDamage()
        {
            if (State == AIStates.Idle || Catch == 1)
                return false;

            return null;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return projHitbox.Intersects(targetHitbox)
                || Utils.CenteredRectangle(GetHandleCenter(), new Vector2(Projectile.width, Projectile.height)).Intersects(targetHitbox);
        }

        #region AI

        public override void AI()
        {
            if (Owner.dead || Owner.HeldItem.type != ItemType)
            {
                Projectile.Kill();
                return;
            }

            SetHeld();
            Projectile.timeLeft = 2;

            switch (State)
            {
                default:
                case AIStates.Idle:
                    HandleRot = HandleRot.AngleLerp((InMousePos - Owner.MountedCenter).ToRotation(), 0.4f);

                    //跟随鼠标转动
                    //if (Math.Abs(InMousePos.X - Owner.Center.X) > 8)
                    Owner.direction = (InMousePos.X - Owner.Center.X) > 0 ? 1 : -1;

                    Projectile.Center = GetHandleTipPos();
                    Projectile.rotation = HandleRot;
                    break;
                case AIStates.Flying:
                    Vector2 targetPos = GetHandleCenter();
                    if (Vector2.Distance(Projectile.Center, targetPos) < 32)
                        HandleRot = HandleRot.AngleLerp((Projectile.Center - Owner.MountedCenter).ToRotation(), 0.5f);
                    else
                        HandleRot = HandleRot.AngleLerp((Projectile.Center - targetPos).ToRotation(), 0.5f);
                    Owner.itemTime = Owner.itemAnimation = Owner.itemTimeMax;

                    //朝向弹幕位置
                    if (Math.Abs(Projectile.Center.X - Owner.Center.X) > 14)
                        Owner.direction = (Projectile.Center.X - Owner.Center.X) > 0 ? 1 : -1;

                    Flying();

                    if (Catch == 1)//捕捉
                    {
                        if (Helper.CheckCollideWithFairyCircleSingle(Owner, Projectile.getRect(), catchPower))
                            TurnToBack();
                    }

                    break;
                case AIStates.Backing:
                    HandleRot = (Projectile.Center - GetHandleCenter()).ToRotation();
                    Owner.itemTime = Owner.itemAnimation = Owner.itemTimeMax;

                    if (Math.Abs(Projectile.Center.X - Owner.Center.X) > 14)
                        Owner.direction = (Projectile.Center.X - Owner.Center.X) > 0 ? 1 : -1;

                    Backing();

                    break;
            }

            Owner.itemRotation = HandleRot + (Owner.direction > 0 ? 0 : MathHelper.Pi);
            Owner.SetCompositeArmFront(true
                , Player.CompositeArmStretchAmount.Full, Owner.itemRotation - Owner.direction * MathHelper.PiOver2);
        }

        /// <summary>
        /// 开始攻击
        /// </summary>
        /// <param name="catch">0表示抛出仙灵，1表示捕捉</param>
        public virtual void StartAttack(int @catch, float speed, int damage, float knockack)
        {
            Catch = @catch;
            State = AIStates.Flying;
            Timer = 0;
            Projectile.damage = damage;
            Projectile.knockBack = knockack;

            Owner.direction = (InMousePos.X - Owner.Center.X) > 0 ? 1 : -1;
            RecordSpeed = speed;
            Projectile.velocity = UnitToMouseV * speed;
            Projectile.StartAttack();
            Projectile.tileCollide = true;

            FlyLength = MaxFlyLength;

            if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
            {
                catchPower = fcp.GetCatchPowerByHeldItem();
                foreach (var acc in fcp.FairyAccessories)
                    acc.OnTongStartAttack(this);
            }

            OnStartAttack();
        }

        /// <summary>
        /// 开始攻击时触发，大多是生成粒子或音效
        /// </summary>
        public virtual void OnStartAttack()
        {
            Helper.PlayPitched(CoraliteSoundID.Blowgun_Item64, Projectile.Center);
        }

        /// <summary>
        /// 抓手飞行时调用
        /// </summary>
        public virtual void Flying()
        {
            Timer++;

            Projectile.rotation = Projectile.velocity.ToRotation();

            //飞行一段时间后返回
            if (Timer > 180 || Vector2.Distance(Projectile.Center, GetHandleTipPos()) > FlyLength)
            {
                if (Catch == 0)
                    ShootFairy();
                TurnToBack();
            }
        }

        /// <summary>
        /// 飞回玩家
        /// </summary>
        public virtual void Backing()
        {
            Timer++;
            Vector2 targetPos = GetHandleTipPos();

            float speed = Projectile.velocity.Length();

            if (speed < RecordSpeed)
                speed = RecordSpeed;

            if (Timer > 60)//超过一段时间后加速回归
                speed *= 1.05f;

            Projectile.velocity = (targetPos - Projectile.Center).SafeNormalize(Vector2.Zero) * speed;
            Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.velocity.ToRotation() + MathHelper.Pi, 0.1f);

            if (Vector2.Distance(Projectile.Center, targetPos) < speed * 1.5f)
                TurnToIdle();
        }

        public void TurnToBack()
        {
            State = AIStates.Backing;
            Timer = 0;
            Projectile.velocity = (GetHandleTipPos() - Projectile.Center).SafeNormalize(Vector2.Zero) * RecordSpeed;
            Projectile.tileCollide = false;
        }

        public void TurnToIdle()
        {
            State = AIStates.Idle;
            Timer = 0;
            Projectile.velocity = Vector2.Zero;
            Projectile.tileCollide = false;
        }

        /// <summary>
        /// 把手顶端的位置，用于获取抓手位置
        /// </summary>
        /// <returns></returns>
        public virtual Vector2 GetHandleTipPos()
        {
            Vector2 dir = HandleRot.ToRotationVector2();
            return Owner.MountedCenter +
                dir * (HandelOffset.X + TongPosOffset.X)
                + ((HandleRot + 1.57f).ToRotationVector2() * (HandelOffset.Y + TongPosOffset.Y) * Owner.direction)
                + new Vector2(0, Owner.gfxOffY);
        }

        protected virtual void ShootFairy()
        {
            OnShootFairy();

            if (!Projectile.IsOwnedByLocalPlayer())
                return;

            Vector2 center = Projectile.Center;

            if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp)
                && fcp.TryGetFairyBottle(out BaseFairyBottle bottle))
            {
                int staminaR = 0;
                if (Owner.HeldItem.ModItem is BaseTongsItem t)
                    staminaR = t.StaminaReduce;

                foreach (var item in bottle.GetShootableFairy(Owner))
                {
                    if (item.ShootFairy(Owner, Projectile.GetSource_FromAI(), center
                        , GetFairyVeloity(), Projectile.knockBack, 60, staminaR))
                        return;
                }
            }
        }

        public virtual Vector2 GetFairyVeloity()
        {
            return Projectile.velocity.SafeNormalize(Vector2.Zero) * 1.25f;
        }

        public virtual void OnShootFairy() { }

        #endregion

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (State == AIStates.Flying)
            {
                OnHitNPCFlying(target, hit, damageDone);
                TurnToBack();
                ShootFairy();
            }
        }

        /// <summary>
        /// 飞行中命中NPC触发效果
        /// </summary>
        /// <param name="target"></param>
        /// <param name="hit"></param>
        /// <param name="damageDone"></param>
        public virtual void OnHitNPCFlying(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity;
            ModifyTileHit();

            if (State == AIStates.Flying)
            {
                TurnToBack();
                if (Catch == 0)
                    ShootFairy();
            }

            return false;
        }

        /// <summary>
        /// 自定义碰撞物块后的效果，默认发出粒子和音效
        /// </summary>
        public virtual void ModifyTileHit()
        {
            Collision.HitTiles(Projectile.Center, Projectile.velocity, Projectile.width, Projectile.height);
            Helper.PlayPitched(CoraliteSoundID.Hit_Item10, Projectile.Center);
        }

        #region 绘制部分

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 handleCenter = GetHandleCenter();

            PreDrawTong(handleCenter, lightColor);

            if (!DrawHnadleOnTop)
                DrawHandle(GetHandleTex(), handleCenter, lightColor);

            //绘制连线
            if (State == AIStates.Flying || State == AIStates.Backing)
                DrawLine(GetLineTex(), GetHandleTipPos() + LineDrawStartPosOffset() - Main.screenPosition, Projectile.Center - Main.screenPosition, lightColor);

            DrawTong();

            if (DrawHnadleOnTop)
                DrawHandle(GetHandleTex(), handleCenter, lightColor);

            if (State == AIStates.Flying && Catch == 0)
                DrawFairyItem();

            return false;
        }

        public virtual Vector2 GetHandleCenter()
        {
            Vector2 dir = HandleRot.ToRotationVector2();
            return Owner.MountedCenter +
                dir * HandelOffset.X
                + ((HandleRot + 1.57f).ToRotationVector2() * HandelOffset.Y * Owner.direction)
                + new Vector2(0, Owner.gfxOffY);
        }

        /// <summary>
        /// 线段起点的绘制偏移量，主要用于更好的视觉效果
        /// </summary>
        public virtual Vector2 LineDrawStartPosOffset()
            => Vector2.Zero;

        public abstract Texture2D GetHandleTex();
        public virtual Texture2D GetLineTex()
            => FairySystem.DefaultLine.Value;

        /// <summary>
        /// 绘制线条
        /// </summary>
        /// <param name="c"></param>
        public virtual void DrawLine(Texture2D lineTex, Vector2 startPos, Vector2 endPos, Color color)
        {
            List<ColoredVertex> bars = new();

            float halfLineWidth = lineTex.Height / 2;

            Vector2 recordPos = startPos;
            float recordUV = 0;

            int lineLength = (int)(startPos - endPos).Length();   //链条长度
            int pointCount = lineLength / 16 + 3;
            Vector2 controlPos = endPos - Projectile.rotation.ToRotationVector2() * Vector2.Distance(startPos, endPos) / 4;

            //贝塞尔曲线
            for (int i = 0; i < pointCount; i++)
            {
                float factor = (float)i / pointCount;

                Vector2 P1 = Vector2.Lerp(startPos, controlPos, factor);
                Vector2 P2 = Vector2.Lerp(controlPos, endPos, factor);

                Vector2 Center = Vector2.Lerp(P1, P2, factor);
                var Color = GetStringColor(Center + Main.screenPosition);

                Vector2 normal = (P2 - P1).SafeNormalize(Vector2.One).RotatedBy(MathHelper.PiOver2);
                Vector2 Top = Center + normal * halfLineWidth;
                Vector2 Bottom = Center - normal * halfLineWidth;

                recordUV += (Center - recordPos).Length() / lineTex.Width;

                bars.Add(new(Top, Color, new Vector3(recordUV, 0, 1)));
                bars.Add(new(Bottom, Color, new Vector3(recordUV, 1, 1)));

                recordPos = Center;
            }

            var state = Main.graphics.GraphicsDevice.SamplerStates[0];
            Main.graphics.GraphicsDevice.Textures[0] = lineTex;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            Main.graphics.GraphicsDevice.SamplerStates[0] = state;

            //var laserTarget = new Rectangle((int)startPos.X, (int)startPos.Y, lineLength, lineTex.Height);  //目标矩形
            //var laserSource = new Rectangle(0, 0, lineLength, lineTex.Height);   //把自身拉伸到目标矩形
            //var origin2 = new Vector2(0, lineTex.Height / 2);

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            //Main.spriteBatch.Draw(lineTex, laserTarget, laserSource, color
            //    , (endPos - startPos).ToRotation(), origin2, 0, 0);

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
        }

        public virtual Color GetStringColor(Vector2 pos)
        {
            return Lighting.GetColor((int)pos.X / 16, (int)(pos.Y / 16f), Color.White);
        }

        /// <summary>
        /// 绘制把手
        /// </summary>
        /// <param name="handleTex"></param>
        public virtual void DrawHandle(Texture2D handleTex, Vector2 pos, Color lightColor)
        {
            handleTex.QuickCenteredDraw(Main.spriteBatch, pos - Main.screenPosition
                , lightColor, HandleRot, effect: Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically);
        }

        /// <summary>
        /// 绘制抓手
        /// </summary>
        public virtual void DrawTong()
        {
            Texture2D tex = Projectile.GetTexture();
            tex.QuickCenteredDraw(Main.spriteBatch, Projectile.Center - Main.screenPosition
                , Lighting.GetColor(Projectile.Center.ToTileCoordinates()), Projectile.rotation, effect: Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically);
        }

        /// <summary>
        /// 在绘制抓手前绘制
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="lightColor"></param>
        public virtual void PreDrawTong(Vector2 pos, Color lightColor)
        {

        }

        public virtual void DrawFairyItem()
        {
            if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp)
                && fcp.CurrentFairyIndex != -1
                && fcp.TryGetFairyBottle(out BaseFairyBottle bottle))
            {
                int itemType = bottle.FightFairies[fcp.CurrentFairyIndex].type;
                Texture2D mainTex = TextureAssets.Item[itemType].Value;
                Rectangle rectangle2;
                Color c = Lighting.GetColor(Projectile.Center.ToTileCoordinates());

                if (Main.itemAnimations[itemType] != null)
                    rectangle2 = Main.itemAnimations[itemType].GetFrame(mainTex, -1);
                else
                    rectangle2 = mainTex.Frame();

                float itemScale = 1f;

                Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, rectangle2
                    , c, 0f, rectangle2.Size() / 2, itemScale, Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }
        }

        #endregion
    }
}
