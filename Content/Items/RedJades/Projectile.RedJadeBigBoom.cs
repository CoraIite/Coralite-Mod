using Coralite.Content.Dusts;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeBigBoom : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 200;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 10;

            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            Helper.PlayPitched("RedJade/RedJadeBoom", 0.8f, -1f, Projectile.Center);
            Color red = new Color(221, 50, 50);
            Color grey = new Color(91, 93, 102);
            for (int i = 0; i < 18; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<HalfCircleDust>(), Main.rand.NextVector2CircularEdge(11, 11), 0, red, Main.rand.NextFloat(1.8f, 2.3f));
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<HalfCircleDust>(), Main.rand.NextVector2CircularEdge(7, 7), 0, grey, Main.rand.NextFloat(1.5f, 1.9f));
            }
        }

        public override bool PreAI() => false;
        public override bool PreDraw(ref Color lightColor) => false;

        public override bool? CanHitNPC(NPC target)
        {
            return Collision.CanHitLine(Projectile.Center,1,1,target.Center,1,1)&&Vector2.Distance(Projectile.Center,target.Center)<200;
        }
    }
}
