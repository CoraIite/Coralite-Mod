using System;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Projectiles.Projectiles_Shoot
{
    /// <summary>
    /// ai0用于控制光圈及扭曲的旋转角度
    /// ai1用于控制alpha
    /// localAI0用于控制光圈scale
    /// localAI1用于控制光雾scale
    /// </summary>
    public class HyacinthExplosion : ModProjectile, IDrawNonPremultiplied, IDrawWarp
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "Halo";

        public float highlightAlpha;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 200;
            Projectile.timeLeft = 20;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 4;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ai[0] = Main.rand.NextFloat(6.282f);
            Projectile.localAI[0] += 0.1f;
            SoundEngine.PlaySound(CoraliteSoundID.BigBOOM_Item62, Projectile.Center);
        }

        public override void AI()
        {
            Projectile.ai[0] += 0.1f;
            if (Projectile.timeLeft > 15)
            {
                Projectile.localAI[0] += 0.15f;
                Projectile.ai[1] += 0.2f;
                highlightAlpha += 0.2f;
            }
            else
            {
                Projectile.localAI[0] -= 0.03f;
                Projectile.ai[1] -= 0.066f;
                if (highlightAlpha > 0)
                {
                    highlightAlpha -= 0.2f;
                    if (highlightAlpha < 0.01f)
                        highlightAlpha = 0f;
                }
            }

            Projectile.localAI[1] += 0.07f;
            Projectile.ai[1] = Math.Clamp(Projectile.ai[1], 0f, 1f);

            Lighting.AddLight(Projectile.Center, new Vector3(1, 1, 1));

            float rot = Main.rand.NextFloat(6.282f);

            for (int i = 0; i < 2; i++)
            {
                Vector2 dir = rot.ToRotationVector2();
                Vector2 vel = dir.RotatedBy(1.57f) * Main.rand.NextFloat(2.3f, 3.5f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + dir * Main.rand.Next(10, 110), DustID.Granite, vel, Scale: Main.rand.NextFloat(1.4f, 1.6f));
                dust.noGravity = true;

                rot = Main.rand.NextFloat(6.282f);
            }

            rot = Main.rand.NextFloat(6.282f);
            for (int i = 0; i < 3; i++)
            {
                Vector2 dir2 = rot.ToRotationVector2();
                Vector2 vel2 = dir2.RotatedBy(1.57f) * Main.rand.NextFloat(1.5f, 2.5f);
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center + dir2 * Main.rand.Next(10, 110), DustID.DesertTorch, vel2, Scale: Main.rand.NextFloat(1f, 1.2f));
                dust2.noGravity = true;

                rot += 2f;
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Vector2 center = Projectile.Center - Main.screenPosition;

            Texture2D haloTex = TextureAssets.Projectile[Type].Value;
            Vector2 haloOrigin = haloTex.Size() / 2;

            Color black = new Color(0, 0, 0, (int)(Projectile.ai[1] * 255));
            Color red = new Color(255, 20, 20, (int)(Projectile.ai[1] * 165));
            Color white = new Color(255, 255, 255, (int)(highlightAlpha * 255));

            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(haloTex, center, null, black, Projectile.ai[0] + i * 2f, haloOrigin, Projectile.localAI[0], SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(haloTex, center, null, red * 0.5f, Projectile.ai[0] - 3f, haloOrigin, Projectile.localAI[0], SpriteEffects.None, 0f);
            spriteBatch.Draw(haloTex, center, null, red, Projectile.ai[0] + 2f, haloOrigin, Projectile.localAI[0], SpriteEffects.None, 0f);
            spriteBatch.Draw(haloTex, center, null, white, Projectile.ai[0] + 2.4f, haloOrigin, Projectile.localAI[0], SpriteEffects.None, 0f);

            Texture2D fogTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightFog").Value;
            Vector2 fogOrigin = fogTex.Size() / 2;

            spriteBatch.Draw(fogTex, center, null, black, Projectile.ai[0], fogOrigin, Projectile.localAI[1], SpriteEffects.None, 0f);
            spriteBatch.Draw(fogTex, center, null, red * 0.8f, Projectile.ai[0] - 3f, fogOrigin, Projectile.localAI[1] + 0.1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(fogTex, center, null, red, Projectile.ai[0] + 2.4f, fogOrigin, Projectile.localAI[1], SpriteEffects.None, 0f);

        }

        public void DrawWarp()
        {
            Texture2D warpTex = TextureAssets.Projectile[Type].Value;
            Color warpColor = new Color(45, 45, 45) * Projectile.ai[1];
            for (int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(warpTex, Projectile.Center - Main.screenPosition, null, warpColor, Projectile.ai[0] + i * 2f, warpTex.Size() / 2, Projectile.localAI[0], SpriteEffects.None, 0f);
            }
        }
    }
}