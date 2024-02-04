using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>
    /// 使用ai0传入持有者,ai1传入射击时间
    /// </summary>
    public class SmallLaser : ModProjectile,IShadowBallPrimitive
    {
        public override string Texture => AssetDirectory.ShadowCastleEvents + "Trail";

        public List<Vector2> laserTrailPoints = new List<Vector2>();

        protected ref float OwnerIndex => ref Projectile.ai[0];
        protected ref float ShootTime => ref Projectile.ai[1];
        protected ref float LaserWidth => ref Projectile.localAI[0];

        protected float timer;

        public static Asset<Texture2D> gradientTex;
        public static Asset<Texture2D> extraTex;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                gradientTex = ModContent.Request<Texture2D>(AssetDirectory.ShadowBalls + "LaserGradient");
                extraTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "ExtraLaser");
            }
        }

        public override void Unload()
        {
            if (!Main.dedServ)
            {
                gradientTex = null;
                extraTex = null;
            }
        }

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 200;
            Projectile.timeLeft = 300;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (laserTrailPoints.Count<1)
                return false;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, laserTrailPoints[^1], 10, ref Projectile.localAI[2]);
        }

        public override bool? CanCutTiles() => false;

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = (int)ShootTime;
        }

        public override void AI()
        {
            if (!GetOwner(out NPC owner))
                return;

            //始终朝向面朝方向
            laserTrailPoints.Clear();

            Projectile.rotation = owner.rotation;
            Vector2 dir = owner.rotation.ToRotationVector2();
            Vector2 originPos = owner.Center + dir * owner.width / 2;
            laserTrailPoints.Add(originPos);

            for (int i = 0; i < 200; i++)
            {
                Vector2 currentPos = originPos + dir * i * 12;
                if (!CoraliteWorld.shadowBallsFightArea.Contains(currentPos.ToPoint()))
                {
                    for (int j = 0; j < 12; j++)
                    {
                        if (!CoraliteWorld.shadowBallsFightArea.Contains(currentPos.ToPoint()))
                            currentPos -= dir;
                        else break;
                    }
                    laserTrailPoints.Add(currentPos);
                    break;
                }

                laserTrailPoints.Add(currentPos);
            }

            if (timer < 8)
            {
                LaserWidth += 50 / 8f;
            }
            else if (timer < (ShootTime - 10))
            {
                LaserWidth = Helper.Lerp(LaserWidth, 16, 0.5f);
            }
            else
            {
                LaserWidth -=16/10f;
            }

            timer++;
        }

        public bool GetOwner(out NPC owner)
        {
            if (!Main.npc.IndexInRange((int)OwnerIndex))
            {
                Projectile.Kill();
                owner = null;
                return false;
            }

            NPC npc = Main.npc[(int)OwnerIndex];
            if (!npc.active || npc.type != ModContent.NPCType<SmallShadowBall>())
            {
                Projectile.Kill();
                owner = null;
                return false;
            }

            owner = npc;
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "Light").Value;
            var pos = laserTrailPoints[^1] - Main.screenPosition;
            var origin = mainTex.Size() / 2;
            Color c = new Color(189, 109, 255, 0);
            c.A = 0;

            Vector2 scale = new Vector2(LaserWidth / 90, LaserWidth / 130);

            Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, scale, 0, 0); 
            Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, scale*0.75f, 0, 0); 
            Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, scale*0.5f, 0, 0); 

            return false;
        }

        /// <summary>
        /// 从0-1的激光宽度
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public float GetWidh(float factor)
        {
            if (factor < 0.5f)
                return MathF.Sin(MathHelper.PiOver2 * factor / 0.5f) * LaserWidth;
            return LaserWidth;
        }

        public virtual void DrawPrimitive(SpriteBatch spriteBatch)
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            List<VertexPositionColorTexture> bars = new List<VertexPositionColorTexture>();
            float count = laserTrailPoints.Count;
            Vector2 dir = (Projectile.rotation + 1.57f).ToRotationVector2();
            for (int i = 0; i < count; i++)
            {
                float factor = 1f - i / count;
                Vector2 Center = laserTrailPoints[i];
                Vector2 width = GetWidh(1f - factor) * dir;
                Vector2 Top = Center + width;
                Vector2 Bottom = Center - width;

                bars.Add(new(Top.Vec3(), Color.White, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), Color.White, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Effect effect = Filters.Scene["ShadowLaser"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 6);
                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(Projectile.GetTexture());
                effect.Parameters["gradientTexture"].SetValue(gradientTex.Value);
                effect.Parameters["extTexture"].SetValue(extraTex.Value);

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
            }
        }
    }

    /// <summary>
    /// 激光预判线，通过ai0传入持有者,ai1传入蓄力时间
    /// </summary>
    public class SmallLaserPredictionLine : SmallLaser
    {
        public override string Texture => AssetDirectory.OtherProjectiles+ "LaserBody";

        float alpha;
        ref float ChannelTime => ref Projectile.ai[1];

        public static Asset<Texture2D> gradientTex2;
        public static Asset<Texture2D> extraTex2;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                gradientTex2 = ModContent.Request<Texture2D>(AssetDirectory.ShadowBalls + "PredictionGradient");
                extraTex2 = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LaserTrail");
            }
        }

        public override void Unload()
        {
            if (!Main.dedServ)
            {
                gradientTex2 = null;
                extraTex2 = null;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool? CanDamage() => false;

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = (int)ChannelTime;
        }

        public override void AI()
        {
            if (!GetOwner(out NPC owner))
                return;

            if (timer < ChannelTime / 2)
            {
                LaserWidth = 16;
                if (alpha < 0.8f)
                {
                    alpha += 0.8f / (ChannelTime / 2);
                }
            }
            else
            {
                LaserWidth -= 16 / (ChannelTime / 2);
            }

            timer++;

            //始终朝向面朝方向
            laserTrailPoints.Clear();

            Projectile.rotation = owner.rotation;
            Vector2 dir = owner.rotation.ToRotationVector2();
            Vector2 originPos = owner.Center + dir * owner.width / 2;
            laserTrailPoints.Add(originPos);

            for (int i = 0; i < 300; i++)
            {
                Vector2 currentPos = originPos + dir * i * 8;
                if (!CoraliteWorld.shadowBallsFightArea.Contains(currentPos.ToPoint()))
                {
                    currentPos.X = MathHelper.Clamp(currentPos.X,
                        CoraliteWorld.shadowBallsFightArea.X,
                        CoraliteWorld.shadowBallsFightArea.X + CoraliteWorld.shadowBallsFightArea.Width);
                    currentPos.Y = MathHelper.Clamp(currentPos.Y,
                        CoraliteWorld.shadowBallsFightArea.Y,
                        CoraliteWorld.shadowBallsFightArea.Y + CoraliteWorld.shadowBallsFightArea.Height);

                    laserTrailPoints.Add(currentPos);
                    break;
                }

                laserTrailPoints.Add(currentPos);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void DrawPrimitive(SpriteBatch spriteBatch)
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            List<VertexPositionColorTexture> bars = new List<VertexPositionColorTexture>();
            float count = laserTrailPoints.Count;
            Vector2 dir = (Projectile.rotation + 1.57f).ToRotationVector2();
            for (int i = 0; i < count; i++)
            {
                float factor = 1f - i / count;
                Vector2 Center = laserTrailPoints[i];
                Vector2 width = GetWidh(1f - factor) * dir;
                Vector2 Top = Center + width;
                Vector2 Bottom = Center - width;

                var color = Color.Lerp(Color.White * alpha, Color.Transparent, 1 - factor);
                bars.Add(new(Top.Vec3(), color, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), color, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Effect effect = Filters.Scene["ShadowLaser"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 5);
                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(Projectile.GetTexture());
                effect.Parameters["gradientTexture"].SetValue(gradientTex2.Value);
                effect.Parameters["extTexture"].SetValue(extraTex2.Value);

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
            }
        }
    }
}
