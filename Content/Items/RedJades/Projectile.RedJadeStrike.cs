﻿using Coralite.Core;
using Coralite.Core.Configs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeStrike : ModProjectile
    {
        public override string Texture => AssetDirectory.RedJadeProjectiles + Name;

        private bool span;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 14;

            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 55;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public void Initialize()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if (Projectile.ai[0] == 0)//用于同步输入的ai0，这个ai0是用于控制弹幕是否能爆炸的
                Projectile.scale = 1.5f;

            Projectile.netUpdate = true;
        }

        public override void AI()
        {
            if (!span)
            {
                Initialize();
                span = true;
            }
            if (Projectile.velocity.Y < 14)
                Projectile.velocity.Y += 0.04f;

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.netMode != NetmodeID.Server)
                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5), DustID.GemRuby, -Projectile.velocity * 0.4f, 0, default, 0.7f);
                    dust.noGravity = true;
                }
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] == 0 && Projectile.IsOwnedByLocalPlayer())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RedJadeBoom>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner);
                return;
            }

            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 6; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GemRuby, Main.rand.NextVector2Circular(3, 3), 0, default, Main.rand.NextFloat(1f, 1.3f));
                    dust.noGravity = true;
                }

        }
    }
}
