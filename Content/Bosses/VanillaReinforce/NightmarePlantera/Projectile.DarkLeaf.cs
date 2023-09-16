using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// ai0 为1时为强化大小的红色<br></br>
    /// </summary>
    public class DarkLeaf : ModProjectile
    {
        public override string Texture => AssetDirectory.NightmarePlantera + Name;

        private bool init = true;
        private Color glowColor;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;

            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            if (init)   //根据ai设置不同的弹幕大小
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                if (Projectile.ai[0] == 1)
                {
                    glowColor = NightmarePlantera.nightmareRed;
                    Vector2 center = Projectile.Center;
                    Projectile.scale = 1.2f;
                    Projectile.width = (int)(Projectile.width * Projectile.scale);
                    Projectile.height = (int)(Projectile.height * Projectile.scale);
                    Projectile.Center = center;
                }
                else
                {
                    glowColor = NightmarePlantera.lightPurple;
                }
                init = false;
            }

            if (Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16), DustID.VilePowder,
                      Projectile.velocity * 0.4f, 240, glowColor, Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            Dust d;
            for (int i = 0; i < 8; i++)
            {
                d = Dust.NewDustPerfect(Projectile.Center, DustID.SpookyWood, Helper.NextVec2Dir() * Main.rand.NextFloat(0.5f, 2), Scale: Main.rand.NextFloat(1f, 2f));
                d.noGravity = true;
                Dust.NewDustPerfect(Projectile.Center, DustID.VilePowder, Helper.NextVec2Dir() * Main.rand.NextFloat(1f, 3), Scale: Main.rand.NextFloat(1f, 2f));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;
            var pos = Projectile.Center - Main.screenPosition;

            Color color = glowColor;
            var frameBox = mainTex.Frame(1, 2, 0, 1);

            //绘制发光

            Main.spriteBatch.Draw(mainTex, pos, frameBox, color, Projectile.rotation, frameBox.Size() / 2, Projectile.scale, 0, 0);
            Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2);

            for (int i = 1; i < 6; i += 2)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, frameBox,
                    color * (0.6f - i * 0.04f), Projectile.oldRot[i], frameBox.Size() / 2, (Projectile.scale - i * 0.05f), 0, 0);

            //绘制自己
            frameBox = mainTex.Frame(1, 2, 0, 0);

            Main.spriteBatch.Draw(mainTex, pos, frameBox, glowColor, Projectile.rotation, frameBox.Size() / 2, Projectile.scale, 0, 0);

            return false;
        }
    }
}
