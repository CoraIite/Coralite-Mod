using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Coralite.Content.Projectiles.Projectiles_Shoot
{
    /// <summary>
    /// ai0用于控制弹幕大小
    /// </summary>
    public class StarsBreathExplosion:ModProjectile,IDrawAdditive
    {
        public override string Texture => AssetDirectory.Projectiles_Shoot + Name;

        public ref float frameX => ref Projectile.localAI[0];

        public bool cantDraw=true;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 112;
            Projectile.timeLeft = 200;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 30;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Main.rand.NextFloat(6.282f);
        }

        public override void AI()
        {
            if (Projectile.localAI[1] == 0)
            {
                Vector2 center = Projectile.Center;
                Projectile.scale = Projectile.ai[0];
                Projectile.width = Projectile.height = (int)(Projectile.scale * 112);

                Projectile.Center = center;
                Projectile.localAI[1] = 1;
            }
            else
            {
                frameX += 1f;
                if (frameX > 3)
                {
                    if (Projectile.frame < 3)
                    {
                        Color color = Projectile.frame switch
                        {
                            0 => new Color(126, 70, 219),
                            1 => new Color(219, 70, 178),
                            _ => Color.White
                        };

                        int width = (int)(Projectile.width * 0.25f);
                        Particle.NewParticle(Projectile.Center + Main.rand.NextVector2CircularEdge(width, width), Vector2.Zero,
                            CoraliteContent.ParticleType<HorizontalStar>(), color, Projectile.scale * 0.2f);
                    }

                    frameX = 0;
                    Projectile.frame += 1;
                    if (Projectile.frame > 3)
                        Projectile.Kill();
                }
            }
            Projectile.rotation += 0.01f;

            //控制帧图
            //Projectile.frameCounter++;
            //if (Projectile.frameCounter > 2)
            //{
            //    Projectile.frameCounter = 0;

            //}
        }


        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            if (cantDraw)
            {
                cantDraw = false;
                return;
            }
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;

            spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, mainTex.Frame(4, 4, (int)frameX, Projectile.frame), Color.White * 0.8f, Projectile.rotation, new Vector2(64, 64), Projectile.scale, SpriteEffects.None, 0f);
        }
    }
}