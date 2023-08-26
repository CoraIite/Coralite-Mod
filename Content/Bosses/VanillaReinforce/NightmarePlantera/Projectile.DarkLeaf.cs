using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// ai0 为1时为强化大小的红色
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
                if (Projectile.ai[0]==1)
                {
                    glowColor = new Color(118,10,10);
                    Vector2 center = Projectile.Center;
                    Projectile.scale = 1.2f;
                    Projectile.width = (int)(Projectile.width * Projectile.scale);
                    Projectile.height = (int)(Projectile.height * Projectile.scale);
                    Projectile.Center = center;
                }
                else
                {
                    glowColor = new Color(144, 133, 155);
                }
                init = false;
            }
        }

        public override void Kill(int timeLeft)
        {
            Dust d;
            for (int i = 0; i < 8; i++)
            {
                d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<NightmareDust>(), Helper.NextVec2Dir() * Main.rand.NextFloat(0.5f, 2), Scale: Main.rand.NextFloat(1f, 2f));
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;
            var pos = Projectile.Center - Main.screenPosition;

            Color color = lightColor;
            var frameBox = mainTex.Frame(1, 2, 0, 0);

            //绘制自己
            Main.spriteBatch.Draw(mainTex, pos, frameBox, color, Projectile.rotation, frameBox.Size() / 2, Projectile.scale, 0, 0);

            color = glowColor;
            //color *= 0.75f;

            //绘制影子拖尾
            //Projectile.DrawShadowTrails(color, 0.3f, 0.03f, 1, 6, 2, Projectile.scale, frameBox);

            //绘制发光
            frameBox = mainTex.Frame(1, 2, 0, 1);

            Main.spriteBatch.Draw(mainTex, pos, frameBox, color, Projectile.rotation, frameBox.Size() / 2, Projectile.scale, 0, 0);
            Projectile.DrawShadowTrails(color, 0.6f, 0.04f, 1, 6, 2, Projectile.scale, frameBox);

            return false;
        }
    }
}
