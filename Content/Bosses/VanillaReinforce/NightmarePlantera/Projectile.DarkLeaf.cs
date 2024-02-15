using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// ai0 为1时为强化大小的红色<br></br>
    /// </summary>
    public class DarkLeaf : ModProjectile
    {
        public override string Texture => AssetDirectory.NightmarePlantera + Name;

        public static Asset<Texture2D> HighlightTex;

        private bool init = true;
        private Color glowColor;

        public override void Load()
        {
            if (Main.dedServ)
                return;
            HighlightTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "DarkLeaf_Hightlight");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;
            HighlightTex = null;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 400;

            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            if (init)   //根据ai设置不同的弹幕大小
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
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

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 6)
                    Projectile.frame = 0;
            }

            if (Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16), DustID.VilePowder,
                      Projectile.velocity * 0.4f, 240, glowColor, Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
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
            Texture2D mainTex = Projectile.GetTexture();
            Texture2D highlightTex = HighlightTex.Value;
            var pos = Projectile.Center - Main.screenPosition;

            Color color = glowColor;
            var mainFrameBox = mainTex.Frame(1, 7, 0, Projectile.frame);
            var highlightFrameBox = highlightTex.Frame(1, 7, 0, Projectile.frame);
            Vector2 hightlightOrigin = highlightFrameBox.Size() / 2;

            //绘制发光
            Main.spriteBatch.Draw(highlightTex, pos, highlightFrameBox, color, Projectile.rotation, hightlightOrigin, Projectile.scale, 0, 0);
            Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2);

            for (int i = 1; i < 6; i++)
                Main.spriteBatch.Draw(highlightTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, highlightFrameBox,
                    color * (0.4f - i * 0.4f / 6), Projectile.oldRot[i], hightlightOrigin, (Projectile.scale - i * 0.05f), 0, 0);

            //绘制自己
            Main.spriteBatch.Draw(mainTex, pos, mainFrameBox, Color.Gray, Projectile.rotation, mainFrameBox.Size() / 2, Projectile.scale, 0, 0);

            return false;
        }
    }
}
