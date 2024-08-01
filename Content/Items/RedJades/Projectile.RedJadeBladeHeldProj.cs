using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeBladeHeldProj : BaseSwingProj
    {
        public override string Texture => AssetDirectory.RedJadeItems + "RedJadeBlade";

        public ref float Combo => ref Projectile.ai[0];

        public override void SetDefs()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 22;
            Projectile.width = 34;
            Projectile.height = 68;
            Projectile.extraUpdates = 1;

            distanceToOwner = 6;
            minTime = 0;
            onHitFreeze = 4;
        }

        protected override void Initializer()
        {
            if (Main.myPlayer == Projectile.owner)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
            switch (Combo)
            {
                default:
                case 0:
                case 1:
                    maxTime = Owner.itemTimeMax * 2;
                    startAngle = 2.2f;
                    totalAngle = 4.6f;
                    Smoother = Coralite.Instance.NoSmootherInstance;

                    break;
                case 2:
                    maxTime = Owner.itemTimeMax * 2;
                    startAngle = -1.2f;
                    totalAngle = -4.2f;
                    Smoother = Coralite.Instance.NoSmootherInstance;

                    break;
                case 3: //强化挥舞
                    minTime = 14;
                    maxTime = 18 + Owner.itemTimeMax * 2;
                    startAngle = 2.2f;
                    totalAngle = 4.8f;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, Projectile.Center);
                    break;
            }

            base.Initializer();
        }

        protected override float GetStartAngle() => Owner.direction > 0 ? 0f : MathHelper.Pi;

        protected override void BeforeSlash()
        {
            _Rotation += -Owner.direction * 0.03f;
            Slasher();
        }

        protected override void SpawnDustOnSlash()
        {
            if (Main.myPlayer == Projectile.owner && Timer == maxTime / 2 && Combo == 3)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center + (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero) * 64, Vector2.Zero,
                    ModContent.ProjectileType<RedJadeBigBoom>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
            }
        }

        protected override void AfterSlash()
        {
            Slasher();
            if (Timer > maxTime + 6)
                Projectile.Kill();
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            base.DrawSelf(mainTex, origin, lightColor, extraRot);
            if (Timer < minTime)
            {
                float factor = Timer / minTime;
                Helper.DrawPrettyStarSparkle(1, SpriteEffects.None, Projectile.Center - Main.screenPosition + RotateVec2 * 20, new Color(255, 255, 255, 0) * 0.8f,
                    Coralite.RedJadeRed, factor, 0, 0.4f, 0.6f, 1f, -Owner.direction * factor * 1f, new Vector2(2, 1f), Vector2.One);
            }
        }
    }
}
