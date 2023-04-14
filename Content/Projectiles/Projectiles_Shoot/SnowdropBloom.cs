using System;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Coralite.Content.Projectiles.Projectiles_Shoot
{
    public class SnowdropBloom : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles_Shoot + Name;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 70;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.netImportant = true;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            int timer = 70 - Projectile.timeLeft;
            float factor = timer * 0.157f;   //魔法数字
            Projectile.rotation = MathF.Sin(factor)*0.4f;

            if (timer == 6 || timer == 26 || timer == 46)
            {
                if (Main.myPlayer == Projectile.owner && Helpers.ProjectilesHelper.FindClosestEnemy(Projectile.Center, 600, npc => npc.active && !npc.friendly && npc.CanBeChasedBy()) is not null)
                {
                    Vector2 center = Projectile.Top + (Projectile.rotation + 1.57f).ToRotationVector2() * 40;
                    Vector2 dir = (center - Projectile.Top).SafeNormalize(Vector2.Zero);

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), center, dir * 7, ModContent.ProjectileType<SnowSpirit>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner);
                    for (int i = 0; i < 2; i++)
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SnowdropPetal>(), dir.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(1f, 2f));
                }
            }

            Projectile.velocity *= 0.99f;

            Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 0.6f, 0.3f));
            if (timer < 10)
                Projectile.ai[0] += 0.07f;
            else if (timer > 60)
                Projectile.ai[0] -= 0.07f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;
            Vector2 center = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(28, 0);

            Main.spriteBatch.Draw(mainTex, center, mainTex.Frame(1, 2, 0, 0), lightColor * Projectile.ai[0], Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(mainTex, center, mainTex.Frame(1, 2, 0, 1), Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}