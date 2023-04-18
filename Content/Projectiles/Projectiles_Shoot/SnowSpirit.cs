using System;
using Coralite.Core;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Content.Projectiles.Projectiles_Shoot
{
    public class SnowSpirit:ModProjectile,IDrawPrimitive
    {
        BasicEffect effect;
        private Trail trail;

        public SnowSpirit()
        {
            Main.QueueMainThreadAction(() =>
            {
                effect = new BasicEffect(Main.instance.GraphicsDevice);
                effect.VertexColorEnabled = true;
            });
        }

        public override string Texture => AssetDirectory.OtherProjectiles+"Blank";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.timeLeft = 180;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.oldPos = new Vector2[24];
            for (int i = 0; i < 24; i++)
                Projectile.oldPos[i] = Projectile.Center;
        }


        public override void AI()
        {
            //抄自叶绿弹，看不太懂它在写一些什么玩意
            float velLength = Projectile.velocity.Length();
            float num185 = Projectile.localAI[0];
            if (num185 == 0f)
            {
                Projectile.localAI[0] = velLength;
                num185 = velLength;
            }

            float num186 = Projectile.position.X;
            float num187 = Projectile.position.Y;
            float chasingLength = 900f;
            bool flag5 = false;
            int targetIndex = 0;
            if (Projectile.ai[1] == 0f)
            {
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(Projectile) && (Projectile.ai[1] == 0f || Projectile.ai[1] == (i + 1)))
                    {
                        float targetX = Main.npc[i].Center.X;
                        float targetY = Main.npc[i].Center.Y;
                        float num193 = Math.Abs(Projectile.Center.X - targetX) + Math.Abs(Projectile.Center.Y - targetY);
                        if (num193 < chasingLength)
                        {
                            chasingLength = num193;
                            num186 = targetX;
                            num187 = targetY;
                            flag5 = true;
                            targetIndex = i;
                        }
                    }
                }

                if (flag5)
                    Projectile.ai[1] = targetIndex + 1;

                flag5 = false;
            }

            if (Projectile.ai[1] > 0f)
            {
                int targetIndex2 = (int)(Projectile.ai[1] - 1f);
                if (Main.npc[targetIndex2].active && Main.npc[targetIndex2].CanBeChasedBy(this, ignoreDontTakeDamage: true) && !Main.npc[targetIndex2].dontTakeDamage)
                {
                    float num195 = Main.npc[targetIndex2].Center.X;
                    float num196 = Main.npc[targetIndex2].Center.Y;
                    if (Math.Abs(Projectile.Center.X - num195) + Math.Abs(Projectile.Center.Y - num196) < 1000f)
                    {
                        flag5 = true;
                        num186 = Main.npc[targetIndex2].Center.X;
                        num187 = Main.npc[targetIndex2].Center.Y;
                    }
                }
                else
                    Projectile.ai[1] = 0f;
            }

            if (flag5)
            {
                float num197 = num185;
                Vector2 center = Projectile.Center;
                float num198 = num186 - center.X;
                float num199 = num187 - center.Y;
                float dis2Target = MathF.Sqrt(num198 * num198 + num199 * num199);
                dis2Target = num197 / dis2Target;
                num198 *= dis2Target;
                num199 *= dis2Target;
                int chase = 24;

                Projectile. velocity.X = (Projectile.velocity.X * (chase - 1) + num198) / chase;
                Projectile.velocity.Y = (Projectile.velocity.Y * (chase - 1) + num199) / chase;
            }


            trail ??= new Trail(Main.instance.GraphicsDevice, 24, new NoTip(), factor => Helper.Lerp(0, 2, factor), factor =>
            {
                if (factor.X > 0.7f)
                    return Color.Lerp(new Color(152, 192, 70, 60), Color.White, (factor.X - 0.7f) / 0.3f);

                return Color.Lerp(new Color(0, 0, 0, 0), new Color(152, 192, 70, 60), factor.X / 0.7f);
            });

            for (int i = 0; i < 23; i++)
                Projectile.oldPos[i] = Projectile.oldPos[i + 1];

            Projectile.oldPos[23] = Projectile.Center + Projectile.velocity;
            trail.Positions = Projectile.oldPos;
        }

        public void DrawPrimitives()
        {
            if (effect == null)
                return;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            trail?.Render(effect);
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}