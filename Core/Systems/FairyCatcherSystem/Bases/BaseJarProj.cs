using Coralite.Content.DamageClasses;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    /// <summary>
    /// 罐子类武器的基类
    /// </summary>
    public abstract class BaseJarProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.FairyCatcherJar + Name;

        /// <summary>
        /// 最大蓄力时间
        /// </summary>
        public int MaxChannelTime = 60;

        /// <summary>
        /// 最大蓄力伤害加成
        /// </summary>
        public float MaxChannelDamageBonus = 2.5f;

        /// <summary>
        /// 投出后的飞行时间，在此时间段内不会下降
        /// </summary>
        public int MaxFlyTime = 10;

        /// <summary>
        /// 坠落速度，默认0.25f
        /// </summary>
        public float FallAcc = 0.25f;

        /// <summary>
        /// 坠落时X方向速度减少程度，默认0.97f
        /// </summary>
        public float XSlowDown = 0.97f;

        private int catchPower;
        public int recoilTime;

        /// <summary>
        /// 蓄力特效的闪光颜色
        /// </summary>
        public virtual Color ShineColor { get=>Color.White; }
        /// <summary>
        /// 最大掉落速度
        /// </summary>
        public virtual float MaxYFallSpeed { get => 12f; }

        public ref float Catch => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        //public ref float VisualEffectTimer => ref Projectile.localAI[0];

        /// <summary>
        /// 是否完全蓄力
        /// </summary>
        public bool FullCharge {  get; set; }

        public AIStates State
        {
            get => (AIStates)Projectile.ai[2];
            set => Projectile.ai[2] = (int)value;
        }

        public enum AIStates
        {
            /// <summary>
            /// 拿在手里的状态
            /// </summary>
            Held,
            /// <summary>
            /// 投掷出的状态
            /// </summary>
            Flying
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.hide = true;
        }

        public override bool ShouldUpdatePosition()
        {
            return State == AIStates.Flying;
        }

        public override bool? CanDamage()
        {
            if (State == AIStates.Flying)
                return null;

            return false;
        }

        public override void Initialize()
        {
            InitFields();
            if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                foreach (var acc in fcp.FairyAccessories)
                    acc.ModifyJarInit(this);
        }

        /// <summary>
        /// 在此初始化各类数值<br></br>
        /// <see cref="MaxChannelTime"/> 最大蓄力时间，默认60<br></br>
        /// <see cref="MaxChannelDamageBonus"/> 最大蓄力伤害加成，默认2.5<br></br>
        /// <see cref="MaxFlyTime"/> 投出后的飞行时间，默认10<br></br>
        /// <see cref="FallAcc"/> 坠落速度，默认0.25f<br></br>
        /// </summary>
        public virtual void InitFields() { }

        public override void AI()
        {
            switch (State)
            {
                default:
                case AIStates.Held:
                    SetHeld();
                    Owner.itemTime = Owner.itemAnimation = 2;
                    if (Math.Abs(InMousePos.X - Owner.Center.X) > 8)
                        Owner.direction = (InMousePos.X - Owner.Center.X) > 0 ? 1 : -1;
                    Held();

                    break;
                case AIStates.Flying:
                    Flying();
                    SpawnDustOnFlying(Timer < 1);
                    if (Catch == 1)
                    {
                        if (Helper.CheckCollideWithFairyCircleSingle(Owner, Projectile.getRect(), catchPower))
                            Projectile.Kill();
                    }
                    break;
            }
        }

        /// <summary>
        /// 拿在手里时调用
        /// </summary>
        public virtual void Held()
        {
            Timer++;

            //松手就丢出去
            if ((Catch == 0 && !DownLeft) || (Catch == 1 && !DownRight))
            {
                OnShoot();
                return;
            }

            if (Timer == MaxChannelTime)
            {
                FullCharge = true;
                OnChannelComplete();
            }
            else if (Timer > MaxChannelTime)
            {
                if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp) && fcp.AutoShootJar)
                {
                    OnShoot();
                    return;
                }
            }

            float factor = Math.Clamp(Helper.X2Ease(Timer / MaxChannelTime), 0, 1);

            float rot = Helper.Lerp(-0.85f, -2f, factor);
            Owner.itemRotation = Owner.direction * rot;
            Projectile.rotation = Owner.direction > 0 ? rot : (-rot + MathHelper.Pi);
            Projectile.Center = Owner.Center + new Vector2(-Owner.direction * 4, Owner.gfxOffY) + Projectile.rotation.ToRotationVector2() * (8 + Projectile.width / 2);
        }

        /// <summary>
        /// 在完成蓄力后调用，
        /// </summary>
        public virtual void OnChannelComplete()
        {
            Helper.PlayPitched(CoraliteSoundID.Ding_Item4, Projectile.Center);
        }

        /// <summary>
        /// 在射出时调用
        /// </summary>
        public virtual void OnShoot()
        {
            Projectile.hide = false;
            State = AIStates.Flying;
            Helper.PlayPitched(CoraliteSoundID.Swing_Item1, Projectile.Center);

            float factor = Math.Clamp(Helper.X2Ease(Timer / MaxChannelTime), 0, 1);

            float damageBonus = Helper.Lerp(1, MaxChannelDamageBonus, factor);
            float speed = Helper.Lerp(Owner.HeldItem.shootSpeed / 2, Owner.HeldItem.shootSpeed, factor);
            MaxFlyTime = (int)Helper.Lerp(MaxFlyTime / 2, MaxFlyTime, factor);
            if (MaxFlyTime < 1)
                MaxFlyTime = 1;

            Projectile.velocity = UnitToMouseV * speed;
            Projectile.damage = (int)(Projectile.damage * damageBonus);

            recoilTime = Owner.itemTimeMax;
            if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
            {
                foreach (var acc in fcp.FairyAccessories)
                    acc.OnJarShoot(this);

                catchPower = (int)(fcp.GetCatchPowerByHeldItem() * damageBonus);
            }

            Owner.itemTime = Owner.itemAnimation = recoilTime;
            Projectile.tileCollide = true;
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);

            Timer = MaxFlyTime;
        }

        public virtual void Flying()
        {
            Timer--;
            if (Timer < 1)
            {
                Projectile.velocity.Y += FallAcc;
                Projectile.velocity.Y = Math.Clamp(Projectile.velocity.Y, -MaxYFallSpeed, MaxYFallSpeed);
                Projectile.velocity.X *= XSlowDown;
            }

            if (-(Timer - MaxFlyTime) < recoilTime)
            {
                float factor = Helper.HeavyEase(-(Timer - MaxFlyTime) / recoilTime);

                float rot = Helper.Lerp(-2f, 0.2f, factor);
                Owner.itemRotation = Owner.direction * rot;
            }

            Projectile.rotation +=
                Math.Sign(Projectile.velocity.X) * Projectile.velocity.Length() / 50
                + Projectile.velocity.X / 30;
        }

        /// <summary>
        /// 生成粒子，判断<paramref name="outofTime"/>是否为结束飞行后
        /// </summary>
        /// <param name="outofTime"></param>
        public virtual void SpawnDustOnFlying(bool outofTime)
        {

        }

        public override void OnKill(int timeLeft)
        {
            if (Catch == 0)
            {
                ShootFairy();
            }
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
            return -Projectile.velocity.SafeNormalize(Vector2.Zero) * 1.25f;
        }

        public virtual void OnShootFairy() { }

        #region 绘制部分

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects eff = SpriteEffects.None;
            if (State == AIStates.Held)
            {
                eff = Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            }

            Texture2D tex = Projectile.GetTexture();

            tex.QuickCenteredDraw(Main.spriteBatch, Projectile.Center - Main.screenPosition, lightColor, Projectile.rotation
                , Projectile.scale, eff);

            if (State == AIStates.Held)
            {
                if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp) && fcp.DrawJarAimLine)
                {
                    DrawShootLine();
                }

                Color c = Color.White;
                c.A = 0;
                float scale = Projectile.scale;

                if (Timer < MaxChannelTime)
                {
                    float factor = Math.Clamp(Helper.X2Ease(Timer / MaxChannelTime), 0, 1);

                    scale *= factor;
                    c *= factor;
                }
                else
                {
                    float factor = Math.Abs(MathF.Sin(MathHelper.PiOver2 + (Timer-MaxChannelTime) * 0.1f));
                    c *= factor;
                }

                tex.QuickCenteredDraw(Main.spriteBatch, Projectile.Center - Main.screenPosition, c, Projectile.rotation
                    , scale, eff);
            }

            return false;
        }

        public void DrawShootLine()
        {

        }

        #endregion
    }
}
