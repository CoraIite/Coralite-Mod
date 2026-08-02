using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>
    /// ai
    /// </summary>
    [VaultLoaden(AssetDirectory.ShadowBalls)]
    public class ShadowProj : ModProjectile, IDrawPrimitive
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float State => ref Projectile.ai[0];
        public ref float ChaseIndex => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        public ref float Recorder => ref Projectile.localAI[0];
        public ref float TrailWidth => ref Projectile.localAI[1];

        public Trail trail;
        public static ATex ShadowProjGradient { get; private set; }

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.OnlyPosition, 20);
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 16;
            Projectile.hostile = true;
        }

        public override void AI()
        {
            if (!VaultUtils.isServer && trail == null)
            {
                trail ??= new Trail(Main.graphics.GraphicsDevice, 20, new EmptyMeshGenerator(), factor => TrailWidth, factor => Color.White);
            }

            switch (State)
            {
                default://状态1：追踪大球，
                case 0:
                    break;
                case 1://状态2：追踪小球，追到后让小球扩展
                    ChaseSmallBall();
                    break;
            }

            if (!VaultUtils.isServer)
            {
                trail.TrailPositions = Projectile.oldPos;
            }
        }

        public void ChaseSmallBall()
        {
            if (!ChaseIndex.GetNPCOwner<SmallShadowBall>(out NPC smallBall, Projectile.Kill))
                return;

            if (Timer <= 20)
            {
                TrailWidth = Helper.X2Ease(Timer / 20f) * 20;
            }

            if (Timer < 30)//弹幕随心飞，妈妈永相随
            {
                if (Projectile.velocity.Length() > 7)
                {
                    Projectile.velocity *= 0.94f;
                }
                Timer++;
            }
            else if (Timer == 30)//一直一直追
            {
                Projectile.ChaseGradually(smallBall.Center, 16, 30, 29);

                if (Vector2.DistanceSquared(Projectile.Center, smallBall.Center) < 20 * 20)//终于等到你~还好我没放弃~
                {
                    Projectile.Center = smallBall.Center;
                    Projectile.velocity = Vector2.Zero;
                    Projectile.netUpdate = true;

                    if (!VaultUtils.isClient)
                        (smallBall.ModNPC as SmallShadowBall).AcceptShadow();

                    Timer++;
                }
            }
            else if (Timer < 30 + 20)
            {
                TrailWidth = Helper.SqrtEase(1 - (Timer - 30) / 20f) * 20;
            }
            else if (Timer > 30 + 20)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawPrimitives()
        {
            if (trail == null)
                return;

            Effect effect = ShaderLoader.GetShader("StarsTrail");

            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.LightShot.Value);
            effect.Parameters["gradientTexture"].SetValue(ShadowProjGradient.Value);
            effect.Parameters["worldSize"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 5);
            effect.Parameters["uExchange"].SetValue(0.87f + (0.05f * MathF.Sin(Main.GlobalTimeWrappedHourly)));

            trail.DrawTrail(effect);
        }
    }
}
