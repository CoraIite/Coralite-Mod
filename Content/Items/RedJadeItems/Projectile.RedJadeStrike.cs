using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.RedJadeItems
{
    public class RedJadeStrike : ModProjectile
    {
        public override string Texture => AssetDirectory.RedJadeProjectiles + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("赤玉刺");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 14;

            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if (Projectile.ai[0] == 0)//用于同步输入的ai0，这个ai0是用于控制弹幕是否能爆炸的
                Projectile.scale = 1.5f;

            Projectile.netUpdate = true;
        }

        public override void AI()
        {
            if (Projectile.velocity.Y < 14)
                Projectile.velocity.Y += 0.08f;

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.netMode != NetmodeID.Server)
                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5), DustID.GemRuby, -Projectile.velocity * 0.4f, 0, default, 0.7f);
                    dust.noGravity = true;
                }
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.ai[0] == 0 && Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RedJadeBoom>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner);
                return;
            }

            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int i = 0; i < 6; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GemRuby, Main.rand.NextVector2Circular(3, 3), 0, default, Main.rand.NextFloat(1f, 1.3f));
                dust.noGravity = true;
            }

        }
    }
}
