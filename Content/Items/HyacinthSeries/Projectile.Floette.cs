using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.HyacinthSeries
{
    [VaultLoaden(AssetDirectory.HyacinthSeriesItems)]
    public class FloetteHeldProj : BaseGunHeldProj
    {
        public FloetteHeldProj() : base(0.3f, 14, -8, AssetDirectory.HyacinthSeriesItems) { }

        public static ATex FloetteFire { get; private set; }

        public override void ModifyAI(float factor)
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.6f, 0.1f));
            if (Projectile.timeLeft != MaxTime && Projectile.timeLeft % 2 == 0)
            {
                Projectile.frame++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);

            if (Projectile.frame > 3)
                return false;

            Texture2D effect = FloetteFire.Value;
            Rectangle frameBox = effect.Frame(1, 4, 0, Projectile.frame);

            float rot = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
            float n = rot - DirSign * MathHelper.PiOver2;

            Main.spriteBatch.Draw(effect, Projectile.Center + rot.ToRotationVector2() * 16 + n.ToRotationVector2() * 4 - Main.screenPosition, frameBox, Color.Lerp(lightColor, Color.White, 0.5f)
                , rot, new Vector2(0, frameBox.Height / 2), Projectile.scale, 0, 0f);
            return false;
        }
    }

    public class PosionedSeedPlantera : SeedPlantera
    {
        public override string Texture => "Terraria/Images/Projectile_276";

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.PoisonSeedPlantera);
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(20, Main.rand.Next(60 * 2, 60 * 4));
        }
    }
}
