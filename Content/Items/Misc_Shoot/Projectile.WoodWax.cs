using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Coralite.Core.Configs;

namespace Coralite.Content.Items.Misc_Shoot
{
    public class WoodWaxHeldProj : BaseGunHeldProj
    {
        public WoodWaxHeldProj() : base(0.6f, 10, -8, AssetDirectory.Misc_Shoot) { }

        public override void ModifyAI(float factor)
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.6f, 0.1f));
        }
    }

    public class WoodWaxBullet : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles_Shoot + Name;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 100;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            Projectile.rotation += 0.2f;

            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = 1;
                Projectile.timeLeft = (int)(Projectile.knockBack * 20);
                Projectile.rotation = Main.rand.NextFloat(6.282f);
                Projectile.netUpdate = true;
            }

            if (Projectile.timeLeft < 30)
            {
                Projectile.velocity *= 0.97f;
                Projectile.ai[1] = Projectile.timeLeft / 30f;
                Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 1.2f, 0.3f) * Projectile.ai[1]);
            }
            else if (Projectile.timeLeft % 3 == 0)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.JungleGrass, -Projectile.velocity * 0.2f, Scale: 1.2f);
                dust.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 3; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.JungleGrass, Projectile.velocity * 0.2f, Scale: 1.2f);
                    dust.noGravity = true;
                }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 90);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;
            Color drawColor = lightColor;
            if (Projectile.timeLeft < 30)
                drawColor = new Color(200, 200, 200, 200) * Projectile.ai[1];

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, new Vector2(4, 4), 1.4f, SpriteEffects.None, 0f);
            return false;
        }
    }
}
