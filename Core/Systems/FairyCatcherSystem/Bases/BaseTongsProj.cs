using Coralite.Content.DamageClasses;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq.Expressions;
using Terraria;
using Terraria.GameContent;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    /// <summary>
    /// 抓手类弹幕，弹幕本体贴图为抓手贴图，把手贴图需要额外传入
    /// </summary>
    public abstract class BaseTongsProj : BaseHeldProj
    {
        /// <summary>
        /// 把手位置的偏移量
        /// </summary>
        public virtual Vector2 HandelOffset { get => Vector2.Zero; }

        /// <summary>
        /// 把手的长度，决定弹幕常态位于何处
        /// </summary>
        public abstract int HandleLength { get; }

        /// <summary>
        /// 抓手最大飞行距离
        /// </summary>
        public abstract int MaxFlyLength { get; }

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

        #region AI

        public override void AI()
        {
            SetHeld();
            HandleRot = (Projectile.Center - Owner.MountedCenter).ToRotation();

            switch (State)
            {
                default:
                case AIStates.Idle:
                    //跟随鼠标转动
                    if (Math.Abs(InMousePos.X - Owner.Center.X) > 8)
                        Owner.direction = (InMousePos.X - Owner.Center.X) > 0 ? 1 : -1;

                    Projectile.Center = GetHandleTipPos();
                    Projectile.rotation = HandleRot;
                    break;
                case AIStates.Flying:
                    Owner.itemTime = Owner.itemAnimation = 2;
                    if (Math.Abs(Projectile.Center.X - Owner.Center.X) > 8)
                        Owner.direction = (Projectile.Center.X - Owner.Center.X) > 0 ? 1 : -1;

                    Flying();

                    if (Catch == 1)//捕捉
                    {
                        if (Helper.CheckCollideWithFairyCircleSingle(Owner, Projectile.getRect(), catchPower))
                            TurnToBack();
                    }


                    break;
                case AIStates.Backing:
                    Owner.itemTime = Owner.itemAnimation = 2;

                    break;
            }
        }

        /// <summary>
        /// 开始攻击
        /// </summary>
        /// <param name="catch">0表示抛出仙灵，1表示捕捉</param>
        public virtual void StartAttack(int @catch, float speed, int damage)
        {
            Catch = @catch;
            State = AIStates.Backing;
            Timer = 0;
            Projectile.damage = damage;

            Owner.direction = (InMousePos.X - Owner.Center.X) > 0 ? 1 : -1;

            Projectile.velocity = UnitToMouseV * speed;

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

        }

        /// <summary>
        /// 抓手飞行时调用
        /// </summary>
        public virtual void Flying()
        {
            Timer++;

            //飞行一段时间后返回
            if (Timer > 180 || Vector2.Distance(Projectile.Center, GetHandleTipPos()) > FlyLength)
            {
                TurnToBack();
                ShootFairy();
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
            if (Timer > 180)
                speed *= 1.02f;

            Projectile.velocity = (targetPos - Projectile.Center).SafeNormalize(Vector2.Zero);

            if (Vector2.Distance(Projectile.Center, targetPos) < speed * 1.5f)
                TurnToIdle();
        }

        public void TurnToBack()
        {
            State = AIStates.Backing;
            Timer = 0;
        }

        public void TurnToIdle()
        {
            State = AIStates.Backing;
            Timer = 0;
        }

        /// <summary>
        /// 把手顶端的位置，用于获取抓手位置
        /// </summary>
        /// <returns></returns>
        public virtual Vector2 GetHandleTipPos()
        {
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            return Owner.MountedCenter +
                dir * HandelOffset.X
                + ((Projectile.rotation + 1.57f).ToRotationVector2() * HandelOffset.Y)
                + dir * HandleLength;
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
                foreach (var item in bottle.GetShootableFairy(Owner))
                {
                    if (item.ShootFairy(Owner, Projectile.GetSource_FromAI(), center, GetFairyVeloity(), Projectile.knockBack, 60))
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
                TurnToBack();
                ShootFairy();
            }
        }

        #region 绘制部分

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 handleCenter = GetHandleCenter();

            //绘制连线
            DrawLine(GetLineTex(), GetHandleTipPos(), Projectile.Center, lightColor);

            DrawHandle(GetHandleTex(), handleCenter,lightColor);
            DrawTong();

            if (State == AIStates.Flying && Catch == 0)
            {
                DrawFairy();
            }

            return false;
        }

        public virtual Vector2 GetHandleCenter()
        {
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            return Owner.MountedCenter +
                dir * HandelOffset.X
                + ((Projectile.rotation + 1.57f).ToRotationVector2() * HandelOffset.Y)
                + dir * HandleLength;
        }

        public abstract Texture2D GetHandleTex();
        public virtual Texture2D GetLineTex()
            => FairySystem.DefaultLine.Value;

        /// <summary>
        /// 绘制线条
        /// </summary>
        /// <param name="c"></param>
        public virtual void DrawLine(Texture2D tex, Vector2 startPos, Vector2 endPos,Color color)
        {
            int width = (int)(startPos - endPos).Length();   //链条长度

            var laserTarget = new Rectangle((int)startPos.X, (int)startPos.Y, width, tex.Height);  //目标矩形
            var laserSource = new Rectangle(0, 0, width, tex.Height);   //把自身拉伸到目标矩形
            var origin2 = new Vector2(0, tex.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(tex, laserTarget, laserSource, color
                , Projectile.rotation, origin2, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
        }

        /// <summary>
        /// 绘制把手
        /// </summary>
        /// <param name="handleTex"></param>
        public virtual void DrawHandle(Texture2D handleTex, Vector2 pos, Color lightColor)
        {
            handleTex.QuickCenteredDraw(Main.spriteBatch, Projectile.Center - Main.screenPosition
                , lightColor);
        }

        /// <summary>
        /// 绘制抓手
        /// </summary>
        public virtual void DrawTong()
        {
            Texture2D tex = Projectile.GetTexture();
            tex.QuickCenteredDraw(Main.spriteBatch, Projectile.Center - Main.screenPosition
                , Lighting.GetColor(Projectile.Center.ToTileCoordinates()));
        }

        public virtual void DrawFairy()
        {

        }

        #endregion
    }
}
