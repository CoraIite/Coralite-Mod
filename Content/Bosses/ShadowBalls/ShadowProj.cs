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
    /// ai0控制状态，0追踪大球，1追踪小球<br></br>
    /// ai1填入追踪的NPC索引
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

        //public override void SetStaticDefaults()
        //{
        //    //Projectile.QuickTrailSets(Helper.TrailingMode.OnlyPosition, 20);
        //}

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
                trail ??= new Trail(Main.graphics.GraphicsDevice, 28, new EmptyMeshGenerator(), factor => TrailWidth, factor => Color.White);

                Projectile.InitOldPosCache(28);
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
                Projectile.UpdateOldPosCache();
                trail.TrailPositions = Projectile.oldPos;
            }
        }

        public void ChaseSmallBall()
        {
            if (!ChaseIndex.GetNPCOwner<SmallShadowBall>(out NPC smallBall, Projectile.Kill))
                return;

            if (Timer <= 20)
            {
                TrailWidth = Helper.X2Ease(Timer / 20f) * 30;
            }

            if (Timer < 60)//弹幕随心飞，妈妈永相随
            {
                if (Projectile.velocity.Length() > 3)
                {
                    Projectile.velocity *= 0.9f;
                }

                Projectile.velocity = Projectile.velocity.RotatedBy(0.06f);

                Timer++;
            }
            else if (Timer == 60)//一直一直追
            {
                float distance = Vector2.DistanceSquared(Projectile.Center, smallBall.Center);

                if (distance < 100 * 100)
                    Projectile.ChaseGradually(smallBall.Center, 30, 3, 4);
                else
                    Projectile.ChaseGradually(smallBall.Center, 20, 15, 16);

                if (distance < 21 * 21)//终于等到你~还好我没放弃~
                {
                    //Projectile.Center = smallBall.Center;
                    Projectile.velocity *=0.2f;
                    Projectile.netUpdate = true;

                    if (!VaultUtils.isClient)
                        (smallBall.ModNPC as SmallShadowBall).AcceptShadow();

                    Timer++;
                }
            }
            else if (Timer < 60 + 20)
            {
                //Projectile.ChaseGradually(smallBall.Center, 5, 20, 21);

                Projectile.velocity *= 0.7f;

                TrailWidth = Helper.SqrtEase(1 - (Timer - 60) / 20f) * 40;
                Timer++;
            }
            else if (Timer >= 60 + 20)
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
            effect.Parameters["uExchange"].SetValue(0.3f);

            trail.DrawTrail(effect);
        }
    }
}
