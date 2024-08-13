using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 使用ai0传入颜色
    /// </summary>
    public class GhostHand : BaseNightmareProj
    {
        public override string Texture => AssetDirectory.Blank;

        private Player Owner => Main.player[Projectile.owner];
        public ref float ColorState => ref Projectile.ai[0];
        private Color drawColor;

        private bool Init = true;
        private SpriteEffects filp;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 32;
            Projectile.timeLeft = 300;
        }

        public override void OnSpawn(IEntitySource source)
        {
            filp = Main.rand.Next() switch
            {
                0 => SpriteEffects.None,
                _ => SpriteEffects.FlipVertically
            };
        }

        public override void AI()
        {
            if (Init)
            {
                if (ColorState == -1)
                    drawColor = NightmarePlantera.lightPurple;
                else if (ColorState == -2)
                    drawColor = new Color(255, 20, 20, 130);
                else
                    drawColor = Main.hslToRgb(new Vector3(Math.Clamp(ColorState, 0, 1f), 1f, 0.8f));

                Init = false;
            }

            if (Projectile.timeLeft > 240)
            {
                Projectile.velocity *= 0.98f;
                Projectile.rotation += Projectile.velocity.Length() / 20;
                return;
            }

            if (Projectile.timeLeft == 240)
            {
                Projectile.velocity = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 20;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            float dir = ((Projectile.timeLeft % 50) > 25 ? -1 : 1) * 0.02f;

            Projectile.velocity = Projectile.velocity.RotatedBy(dir);
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.rand.NextBool(4))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(48, 48), DustID.VilePowder,
                      Projectile.velocity * 0.4f, 240, drawColor, Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(ProjectileID.InsanityShadowHostile);
            Texture2D mainTex = TextureAssets.Projectile[ProjectileID.InsanityShadowHostile].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Vector2 origin = mainTex.Size() / 2;

            Color c = drawColor * 0.5f;

            Vector2 toCenter = new(Projectile.width / 2, Projectile.height / 2);

            for (int i = 0; i < 6; i++)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                Color.Black * (0.5f - i * 0.08f), Projectile.oldRot[i], origin, 0.9f - i * 0.08f, filp, 0);

            for (int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(mainTex, pos + (Main.GlobalTimeWrappedHourly / 5 + i * MathHelper.TwoPi / 3).ToRotationVector2() * 6
                    , null, c, Projectile.rotation, origin, 0.75f, filp, 0);
            }

            Main.spriteBatch.Draw(mainTex, pos, null, Color.Black, Projectile.rotation, origin, 0.75f, filp, 0);

            return false;
        }
    }
}
