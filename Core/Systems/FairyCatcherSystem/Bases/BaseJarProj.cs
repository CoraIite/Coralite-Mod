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
    /// 罐子类武器的基类
    /// </summary>
    public abstract class BaseJarProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.FairyCatcherJar + Name;

        /// <summary>
        /// 最大蓄力时间
        /// </summary>
        public int MaxChannelTime = 40;

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
        public virtual Color ShineColor { get => Color.White; }
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
        public bool FullCharge { get; set; }

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
            Projectile.Center = Owner.Center + new Vector2(-Owner.direction * 4, 0) + Projectile.rotation.ToRotationVector2() * (8 + Projectile.width / 2);

            OnHeld();
        }

        /// <summary>
        /// 手握时调用，用于生成粒子之类的
        /// </summary>
        public virtual void OnHeld()
        {

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
                {
                    acc.OnJarShoot(this);
                    acc.ModifyFlyTime(ref MaxFlyTime);
                }

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

            FlyingRotation();
        }

        /// <summary>
        /// 飞行过程中的旋转
        /// </summary>
        public virtual void FlyingRotation()
        {
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
                eff = Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            PreDrawSpecial(eff, ref lightColor);
            Texture2D tex = Projectile.GetTexture();

            DrawJar(Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), lightColor, eff, tex);

            PostDrawSpecial(eff, lightColor);

            if (State == AIStates.Held)
            {
                if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp) && fcp.DrawJarAimLine)
                    DrawShootLine();

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
                    float factor = Math.Abs(MathF.Sin(MathHelper.PiOver2 + (Timer - MaxChannelTime) * 0.1f));
                    c *= factor;
                }

                tex.QuickCenteredDraw(Main.spriteBatch, Projectile.Center - Main.screenPosition, c, Projectile.rotation
                    , scale, eff);
            }

            return false;
        }

        public virtual void DrawJar(Vector2 pos, Color lightColor, SpriteEffects eff, Texture2D tex)
        {
            tex.QuickCenteredDraw(Main.spriteBatch, pos
                , lightColor, Projectile.rotation
                , Projectile.scale, eff);
        }

        /// <summary>
        /// 绘制特殊内容，比如仙灵
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="lightColor"></param>
        public virtual void PreDrawSpecial(SpriteEffects effect, ref Color lightColor)
        {

        }

        public virtual void PostDrawSpecial(SpriteEffects effect, Color lightColor)
        {

        }

        /// <summary>
        /// 绘制瞄准线
        /// </summary>
        public virtual void DrawShootLine()
        {
            Texture2D lineTex = FairySystem.JarAimLine.Value;

            //获取初速度
            (float speed, int maxFlyTime) = GetSpeed();
            Vector2 pos = Projectile.Center + new Vector2(0, Owner.gfxOffY);
            Vector2 velocity = UnitToMouseV * speed;
            List<ColoredVertex> bars = new();

            Color c = Color.Lerp(Color.Red, Color.Lime, Timer / MaxChannelTime) * 0.6f;
            float recordU = -Main.GlobalTimeWrappedHourly;

            //前半部分的直线飞行
            Vector2 normal = UnitToMouseV.RotatedBy(MathHelper.PiOver2);
            for (int i = 0; i < maxFlyTime; i++)
            {
                Vector2 Top = pos + normal * 1;
                Vector2 Bottom = pos - normal * 1;

                float factor = (float)i / maxFlyTime * 0.5f;

                float alpha = 1;
                if (factor < 0.4f)
                    alpha = factor / 0.4f;

                bars.Add(new(Top - Main.screenPosition, c * alpha, new Vector3(recordU, 0, 1)));
                bars.Add(new(Bottom - Main.screenPosition, c * alpha, new Vector3(recordU, 1, 1)));

                recordU += speed / lineTex.Width;
                pos += velocity;
                if (Helper.PointInTile(pos))
                    goto over;
            }

            //后半部分坠机
            int FallTime = (int)(12 * 70 / speed);
            for (int i = 0; i < FallTime; i++)
            {
                Vector2 normal2 = velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);

                Vector2 Top = pos + normal2 * 1;
                Vector2 Bottom = pos - normal2 * 1;

                float factor = 0.5f + (float)i / FallTime * 0.5f;

                float alpha = 1;
                if (factor > 0.7f)
                    alpha = 1 - (factor - 0.7f) / 0.3f;

                bars.Add(new(Top - Main.screenPosition, c * alpha, new Vector3(recordU, 0, 1)));
                bars.Add(new(Bottom - Main.screenPosition, c * alpha, new Vector3(recordU, 1, 1)));

                velocity.Y += FallAcc;
                velocity.Y = Math.Clamp(velocity.Y, -MaxYFallSpeed, MaxYFallSpeed);
                velocity.X *= XSlowDown;

                recordU += velocity.Length() / lineTex.Width;

                pos += velocity;
                if (Helper.PointInTile(pos))
                    goto over;
            }


        over:

            var state = Main.graphics.GraphicsDevice.SamplerStates[0];
            Main.graphics.GraphicsDevice.Textures[0] = lineTex;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            Main.graphics.GraphicsDevice.SamplerStates[0] = state;

            FairySystem.JarAimTarget.Value.QuickCenteredDraw(Main.spriteBatch
                , Projectile.Center - Main.screenPosition + UnitToMouseV * (Projectile.width / 2 + 16), rotation: ToMouseA);


            (float, int) GetSpeed()
            {
                float factor = Math.Clamp(Helper.X2Ease(Timer / MaxChannelTime), 0, 1);

                float speed = Helper.Lerp(Owner.HeldItem.shootSpeed / 2, Owner.HeldItem.shootSpeed, factor);
                int maxFlyTime = (int)Helper.Lerp(MaxFlyTime / 2, MaxFlyTime, factor);
                if (maxFlyTime < 1)
                    maxFlyTime = 1;

                if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                    foreach (var acc in fcp.FairyAccessories)
                        acc.ModifyFlyTime(ref maxFlyTime);

                return (speed, maxFlyTime);
            }
        }

        /// <summary>
        /// 一个帮助方法，绘制仙灵
        /// </summary>
        /// <param name="drawPos"></param>
        public void DrawFairyItem(Vector2 drawPos, Color c, SpriteEffects effect)
        {
            if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp)
                && fcp.CurrentFairyIndex != -1
                && fcp.TryGetFairyBottle(out BaseFairyBottle bottle))
            {
                int itemType = bottle.FightFairies[fcp.CurrentFairyIndex].type;
                Texture2D mainTex = TextureAssets.Item[itemType].Value;
                Rectangle rectangle2;

                if (Main.itemAnimations[itemType] != null)
                    rectangle2 = Main.itemAnimations[itemType].GetFrame(mainTex, -1);
                else
                    rectangle2 = mainTex.Frame();

                float itemScale = 1f;

                Main.spriteBatch.Draw(mainTex, drawPos, rectangle2
                    , c, Projectile.rotation, rectangle2.Size() / 2, itemScale, effect, 0f);
            }
        }

        #endregion
    }
}
