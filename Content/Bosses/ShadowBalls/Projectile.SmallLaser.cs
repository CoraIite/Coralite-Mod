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
    public class SmallLaser : ModProjectile
    {
        public override string Texture => AssetDirectory.ShadowCastleEvents + "Trail";

        public List<Vector2> laserTrailPoints = new List<Vector2>();

        protected ref float OwnerIndex => ref Projectile.ai[0];
        protected ref float ShootTime => ref Projectile.ai[1];
        protected ref float LaserWidth => ref Projectile.localAI[0];

        protected float timer;

        public static Asset<Texture2D> gradientTex;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                gradientTex = ModContent.Request<Texture2D>(AssetDirectory.ShadowBalls + "LaserGradient");
            }
        }

        public override void Unload()
        {
            if (!Main.dedServ)
            {
                gradientTex = null;
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

            for (int i = 0; i < 300; i++)
            {
                Vector2 currentPos = originPos + dir * i * 8;
                if (!CoraliteWorld.shadowBallsFightArea.Contains(currentPos.ToPoint()))
                    break;

                laserTrailPoints.Add(currentPos);
            }

            if (timer < 8)
            {
                LaserWidth += 60 / 8f;
            }
            else if (timer < (ShootTime - 10))
            {
                LaserWidth = Helper.Lerp(LaserWidth, 25, 0.4f);
            }
            else
            {
                LaserWidth -=25/10f;
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
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

                Effect effect = Filters.Scene["ShadowLaser"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(Projectile.GetTexture());
                effect.Parameters["gradientTexture"].SetValue(gradientTex.Value);

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }

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
    }

    /// <summary>
    /// 激光预判线，通过ai0传入持有者,ai1传入蓄力时间
    /// </summary>
    public class SmallLaserPredictionLine : SmallLaser
    {
        float alpha;
        ref float ChannelTime => ref Projectile.ai[1];

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
                    break;

                laserTrailPoints.Add(currentPos);
            }
        }

        public override bool PreDraw(ref Color lightColor)
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
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

                Effect effect = Filters.Scene["ShadowLaser"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 5);
                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(Projectile.GetTexture());
                effect.Parameters["gradientTexture"].SetValue(gradientTex.Value);

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }

            return false;
        }

    }
}
