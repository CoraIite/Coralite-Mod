using Coralite.Content.Dusts;
using Coralite.Core;
using Terraria.Audio;
using Terraria.ID;
using Terraria;

namespace Coralite.Content.Bosses.Originate.Pioneer
{
    //public class Pioneer_Pro2 : ModProjectile
    //{
    //    public override string Texture => AssetDirectory.Pioneer;

    //    public override void SetStaticDefaults()
    //    {
    //        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    //        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
    //    }
    //    public override void SetDefaults()
    //    {
    //        Projectile.width = 16;
    //        Projectile.height = 16;
    //        Projectile.scale = 1f;
    //        Projectile.timeLeft = 600;
    //        Projectile.aiStyle = -1;
    //        Projectile.penetrate = -1;
    //        Projectile.extraUpdates = 1;
    //        Projectile.tileCollide = false;
    //        Projectile.hostile = true;
    //    }
    //    public override bool PreDraw(ref Color lightColor)
    //    {
    //        Color color = new(35, 160, 200);
    //        color *= 0.5f;
    //        color.A = 0;
    //        Projectile.ProShadow(color, null, 1f, false);
    //        return false;
    //    }
    //    public override void AI()
    //    {
    //        Color color = new(35, 160, 200);
    //        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
    //        if (Projectile.soundDelay == 0)
    //        {
    //            SoundEngine.PlaySound(SoundID.Item33, Projectile.Center);
    //            for (int i = 0; i <= 10; i++)
    //            {
    //                Vector2 Vel = Projectile.velocity.RotatedByRandom(0.6f);
    //                int dust = Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<GlowDust_Prismatic>(), 0, 0, 100, color, 1f);
    //                Main.dust[dust].position = Projectile.Center;
    //                Main.dust[dust].velocity += Vel;
    //                Main.dust[dust].velocity *= Main.rand.NextFloat(1.5f);
    //                Main.dust[dust].noGravity = true;
    //            }
    //            Projectile.soundDelay = -1;
    //        }
    //        if (Main.rand.NextBool(3))
    //        {
    //            int dust = Dust.NewDust(Projectile.position, 8, 8, ModContent.DustType<GlowDust_Prismatic>(), 0, 0, 100, color, 1f);
    //            Main.dust[dust].velocity *= 0.1f;
    //            Main.dust[dust].velocity += Projectile.velocity * 0.5f;
    //            Main.dust[dust].noGravity = true;
    //        }
    //        base.AI();
    //    }
    //}
}
