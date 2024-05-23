using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class FloetteHeldProj : BaseGunHeldProj
    {
        public FloetteHeldProj() : base(0.3f, 14, -8, AssetDirectory.HyacinthSeriesItems) { }

        public override void ModifyAI(float factor)
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.6f, 0.1f));
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
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(20, Main.rand.Next(60 * 2, 60 * 4));
        }
    }
}
