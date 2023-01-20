using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Projectiles.StarsProjectiles
{
    public class StarBookProj2 : ModProjectile
    {
        public override string Texture => AssetDirectory.StarsProjectiles + "StarRunic";

        public readonly int textureType;
        public ref float timer => ref Projectile.localAI[0];
        public ref float direction => ref Projectile.localAI[1];
        public Player Owner => Main.player[Projectile.owner];

        public StarBookProj2()
        {
            textureType = Main.rand.Next(6);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星之书-星符文");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.scale = 1.2f;

            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 400;
            Projectile.friendly = false;
            Projectile.netImportant = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;

            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
        }

        #region AI

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Main.rand.Next(8) * 0.785f;
        }

        public override void AI()
        {
            switch (Projectile.ai[0])
            {
                default:
                case 0:
                    //蓄力途中的AI
                    OnChannel();
                    break;

                case 1:
                    //释放后的状态
                    AutoTracking();
                    break;

            }

            timer++;
        }

        public void OnChannel()
        {
            if (timer<2)
                direction = (Projectile.Center - Owner.Center).ToRotation();

            float lenth = (Owner.Center - Projectile.Center).Length();
            if (lenth > 5)
            {
                lenth -= 3f;
                Projectile.Center = Owner.Center + direction.ToRotationVector2() * lenth;
                
            }
            else
            {
                int projType = ProjectileType<StarBookProj1>();
                for (int i = 0; i < 1000; i++)
                {
                    if (Main.projectile[i].type == projType && Main.projectile[i].owner==Projectile.owner)
                    {
                        Main.projectile[i].ai[0] += 1;
                        Projectile.Kill();
                        break;
                    }
                }
            }

        }

        public void AutoTracking()
        {
            if (!ProjectilesHelper.AutomaticTracking(Projectile, 1f, 14,2000f))
            {
                Projectile.velocity *= 0.98f;
            }
        }


        #endregion

        #region 绘制

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(0, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Texture2D mainTex = Request<Texture2D>(Texture).Value;
            Rectangle source = new Rectangle(16 * textureType, 0, 16, 16);     //<---简单粗暴地填数字了，前提是贴图不能有改动
            Vector2 origin = new Vector2(8, 8);

            float sinProgress = (float)Math.Sin(timer * 0.1f);      //<---别问我这是什么神秘数字，问就是乱写的
            int r = (int)(238.5f + sinProgress * 16.5f);
            int g = (int)(230.5f + sinProgress * 23.5f);
            int b = (int)(156 + sinProgress * 35);
            for (int i = 0; i < 3; i++)     //这里是绘制类似于影子拖尾的东西，简单讲就是随机位置画几个透明度低的自己
            {
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + origin * Projectile.scale - Main.screenPosition, source,
                                                    new Color(r, g, b, 120 - i * 30), Projectile.oldRot[i], origin, Projectile.scale + i * 0.1f, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D mainTex = Request<Texture2D>(Texture).Value;
            Rectangle source = new Rectangle(16 * textureType, 0, 16, 16);
            Vector2 origin = new Vector2(8, 8);

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, source,
                                                    Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        }

        #endregion
    }
}
