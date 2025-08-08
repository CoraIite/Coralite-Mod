using Coralite.Content.DamageClasses;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class BaseGloveProj(float exRot = MathHelper.PiOver4) : BaseHeldProj
    {
        public override bool CanFire => Timer < 2;

        /// <summary> 距离中心的长度 </summary>
        public float DistanceToCenter;

        /// <summary> 基础角度，设置为朝向鼠标 </summary>
        public float BaseAngle;
        /// <summary> 基础角度的偏移量 </summary>
        public float BaseAngleOffset;


        /// <summary> 扩展角度，从负的该值挥舞到正的该值 </summary>
        public float OffsetAngle = 0.2f;
        /// <summary> 决定是正向挥舞还是反向挥舞 </summary>
        public int Direction = 1;
        /// <summary> 挥舞时间，减小则挥地更快 </summary>
        public int MaxTime = 30;
        /// <summary> 长度控制 </summary>
        public (float, float) DistanceController = (-20, 40);

        /// <summary>
        /// 是否在捕捉
        /// </summary>
        public ref float Catch => ref Projectile.ai[0];
        public ref float LengthBonus => ref Projectile.ai[1];
        public ref float AlphaModify => ref Projectile.ai[2];

        public ref float Timer => ref Projectile.localAI[0];

        public float alpha=1;

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            Projectile.DamageType = FairyDamage.Instance;

            SetOtherDefaults();
        }

        /// <summary>
        /// 在此设置各种基础值<br></br>
        /// 需要设置弹幕长宽<br></br>
        /// <see cref="OffsetAngle"/> 挥舞的扩展角度，默认0.2f<br></br>
        /// <see cref="Direction"/> 挥舞方向，默认1<br></br>
        /// <see cref="MaxTime"/> 挥舞时间，默认30<br></br>
        /// <see cref="DistanceController"/> 挥舞距离，默认(-20, 40)<br></br>
        /// </summary>
        public virtual void SetOtherDefaults()
        {
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            return Collision.CanHit(Owner.MountedCenter, 1, 1, targetHitbox.Center(), 1, 1)
                && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center
                , Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.height, Projectile.width / 2,ref a);
        }

        public override void Initialize()
        {
            BaseAngle = ToMouseA;
            MaxTime = (int)(Owner.GetAttackSpeed(FairyDamage.Instance) * MaxTime);
            if (MaxTime < 2)
                MaxTime = 2;

            if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                foreach (var acc in fcp.FairyAccessories)
                    acc.ModifyGloveInit(this);

            if (BaseAngleOffset == 0)
                BaseAngleOffset = Main.rand.NextFloat(-0.3f, 0.3f);//TODO: 需要同步

            DistanceToCenter = DistanceController.Item1;
            Projectile.spriteDirection = Owner.direction;
        }

        public override void AI()
        {
            //使用弹幕角度作为角度
            if (Timer<=MaxTime)
            {
                float factor = Timer / MaxTime;

                Projectile.rotation = GetRotation(factor);
                Projectile.Center = GetCenter() + Projectile.rotation.ToRotationVector2() * DistanceToCenter;

                UpdateDistanceToCenter(factor);
                SpawnDustOnSwing();
            }
            else
            {
                Projectile.Center = GetCenter() + Projectile.rotation.ToRotationVector2() * DistanceToCenter;
                DistanceToCenter *=   0.8f;
                alpha -= 0.1f;
                if (alpha < 0)
                    Projectile.Kill();
                return;
            }

            Timer++;

        }

        public virtual Vector2 GetCenter()
        {
            return Owner.MountedCenter;
        }

        public virtual float GetRotation(float factor)
        {
            //基础角度
            float startRot = BaseAngle + BaseAngleOffset;
            //随时间增加的旋转角度
            startRot += Direction * Projectile.spriteDirection * Helper.Lerp(-OffsetAngle, OffsetAngle, Helper.BezierEase(factor));

            return startRot;
        }

        public virtual void UpdateDistanceToCenter(float factor)
        {
            factor = Helper.HeavyEase(factor);
            DistanceToCenter = Helper.Lerp(DistanceController.Item1, DistanceController.Item2, factor);
        }

        public virtual void SpawnDustOnSwing()
        {

        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();

            Vector2 pos = Projectile.Center + Projectile.height / 2 * Projectile.rotation.ToRotationVector2() - Main.screenPosition;

            int direction = Direction * Projectile.spriteDirection;
            float rot = Projectile.rotation + (direction > 0 ? exRot : MathHelper.Pi - exRot);
            SpriteEffects effect = direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            lightColor *= alpha;

            DrawSelf(lightColor, tex, pos, rot, effect);

            return false;
        }

        public virtual void DrawSelf(Color lightColor, Texture2D tex, Vector2 pos, float rot, SpriteEffects effect)
        {
            tex.QuickCenteredDraw(Main.spriteBatch, pos, lightColor, rot, Projectile.scale, effect);
        }
    }
}
