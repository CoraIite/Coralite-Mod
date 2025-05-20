using Coralite.Content.Particles;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.SmoothFunctions;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class PremissionProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public Vector2 backLightScale;
        public float sparkleScale;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Vector2 Scale1 = new Vector2( 0.6f, 0.25f);
            Vector2 Scale2 = new Vector2(2.5f,0 );

            Lighting.AddLight(Projectile.Center, Coralite.CrystallinePurple.ToVector3());

            switch (State)
            {
                default:
                case 0://展开
                    {
                        if (Timer < 30)
                        {
                            float factor = Timer / 30;
                            factor = Helper.HeavyEase(factor);
                            backLightScale = Vector2.Lerp(Vector2.Zero, Scale1, factor);
                            sparkleScale = factor*2;
                        }
                        else
                        {
                            backLightScale = Scale1;
                            sparkleScale = 2;
                            State++;

                            PRTLoader.NewParticle<CrystallineFairy>(Projectile.Center, new Vector2(0, -15)
                                , Coralite.CrystallinePurple, 1);

                            Timer = 0;
                            return;
                        }

                        Timer++;
                    }
                    break;
                case 1://消失
                    {
                        if (Timer > 60)
                        {
                            float factor = (Timer - 60) / 40;

                            factor = Helper.HeavyEase(1 - factor);
                            backLightScale = Vector2.Lerp(Scale2, Scale1, factor);
                            sparkleScale = factor*2;

                            if (Timer > 60 + 40)
                            {
                                Projectile.Kill();

                                CoraliteWorld.HasPermission = true;
                                for (int i = 0; i < 30; i++)
                                {
                                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Clentaminator_Purple
                                         , (i * MathHelper.TwoPi / 30).ToRotationVector2() * Main.rand.NextFloat(2, 3)
                                         , Alpha: 100, Scale: Main.rand.NextFloat(1, 2));
                                    d.noGravity = true;

                                     d = Dust.NewDustPerfect(Projectile.Center, DustID.Clentaminator_Purple
                                         , (i * MathHelper.TwoPi / 30).ToRotationVector2() * Main.rand.NextFloat(6, 7)
                                         , Alpha: 100, Scale: Main.rand.NextFloat(1, 2));
                                    d.noGravity = true;
                                }

                                Vector2 pos = Projectile.Center - new Vector2(0, 16 );
                                CombatText.NewText(new Rectangle((int)pos.X, (int)pos.Y, 1, 1), Coralite.CrystallinePurple
                                    , MagikeSystem.CrystallineSkyIslandUnlock.Value, true);

                                var p = PRTLoader.NewParticle<RoaringWave>(Projectile.Center, Vector2.Zero
                                     , Coralite.CrystallinePurple, 0.2f);

                                p.ScaleMul = 1.2f;
                                p.FadePercent = 0.85f;

                                Helper.PlayPitched(CoraliteSoundID.ShieldDestroyed_NPCDeath58, Projectile.Center);
                                Helper.PlayPitched("UI/GetSkill", 0.4f, 0, Projectile.Center);
                            }
                        }

                        Timer++;
                    }
                    break;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D backTex = CoraliteAssets.Trail.BoosterASP.Value;
            Vector2 backOrigin = new Vector2(backTex.Width, backTex.Height / 2);
            Vector2 pos = Projectile.Center - Main.screenPosition;

            Color c = Coralite.CrystallinePurple;
            Main.spriteBatch.Draw(backTex, pos + new Vector2(0, 60), null
                , (c * 0.3f) with { A = 0 }, MathHelper.PiOver2, backOrigin, backLightScale, 0, 0);
            Main.spriteBatch.Draw(backTex, pos + new Vector2(0, 60), null
                , (c * 0.3f) with { A = 0 }, MathHelper.PiOver2, backOrigin, new Vector2(backLightScale.X,backLightScale.Y*0.5f), 0, 0);

            float time = Main.GlobalTimeWrappedHourly * 8;
            float timer = ((int)Main.timeForVisualEffects / 240f) + (time * 0.04f);

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
                time = 2f - time;
            time = (time * 0.5f) + 0.5f;

            Vector2 drawPos = pos + new Vector2(0, -16 );
            Color c2 = Coralite.CrystallinePurple;
            c2.A = 50;
            Helper.DrawPrettyStarSparkle(1, 0, drawPos, c2, Coralite.CrystallinePurple ,
                time, 0, 0.3f, 0.7f, 1, timer * MathHelper.TwoPi, (timer * MathHelper.TwoPi).ToRotationVector2() * sparkleScale, Vector2.One/2);
            Helper.DrawPrettyStarSparkle(1, 0, drawPos, c2, Coralite.CrystallinePurple,
                0.4f + (time * 0.2f), 0, 0.3f, 0.7f, 1, -timer * MathHelper.Pi, new Vector2(2, 2) * sparkleScale, Vector2.One);

            Helper.DrawPrettyStarSparkle(1, 0, drawPos, c2 * 0.7f, Coralite.CrystallinePurple,
                0.4f + (time * 0.2f), 0, 0.5f, 0.5f, 1, 0, Vector2.One * sparkleScale, Vector2.One *0.75f);

            return false;
        }
    }

    public class CrystallineFairy : TrailParticle
    {
        public override string Texture => AssetDirectory.Particles + "LozengeParticle2";

        private float scale;
        private SecondOrderDynamics_Vec2 PosMove;
        private Vector2 TargetPos;
        private Vector2 RecordPos;

        public override bool ShouldUpdatePosition() => false;

        public override void SetProperty()
        {
            Color = Coralite.CrystallinePurple;
            trail = new Trail(Main.instance.GraphicsDevice, 12, new EmptyMeshGenerator()
                , factor => 2 * Scale, factor => Color.Lerp(Color.Transparent, Coralite.CrystallinePurple, factor.X));
            InitializePositionCache(12);

            PRTDrawMode = PRTDrawModeEnum.NonPremultiplied;
        }

        public override void AI()
        {
            if (PosMove==null)
            {
                PosMove = new SecondOrderDynamics_Vec2(0.95f, 0.5f, 0, Position);
                TargetPos = Position;
            }

            Lighting.AddLight(Position, Coralite.CrystallinePurple.ToVector3());

            if (Opacity % 2 == 0)
            {
                Dust dust = Dust.NewDustPerfect(Position + Main.rand.NextVector2CircularEdge(8, 8), DustID.PurpleCrystalShard, -Velocity * 0.2f);
                dust.noGravity = true;
            }

            if (scale < 1)
            {
                scale += 0.1f;
                if (scale > 1)
                    scale = 1;
            }

            Opacity++;
            if (Opacity > 360)
                active = false;

            const int FlyTime1 = 15;
            const int RotateTime = 60;

            if (Opacity < FlyTime1)//向上飞
            {
                TargetPos += Velocity;
            }
            else if (Opacity == FlyTime1)//绕个圈
            {
                RecordPos = TargetPos;
            }
            else if (Opacity < FlyTime1 + RotateTime)
            {
                float factor = (Opacity - FlyTime1) / RotateTime;
                TargetPos = RecordPos + (factor * MathHelper.TwoPi * 1.52f - MathHelper.PiOver2).ToRotationVector2() * 240;
            }
            else
            {
                TargetPos += Velocity;
                Velocity *= 1.05f;
            }

            Position = PosMove.Update(1 / 60f, TargetPos);

            UpdatePositionCache(12);
            trail.TrailPositions = oldPositions;
        }

        public override void DrawPrimitive()
        {
            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            //effect.Texture = Texture2D.Value;
            EffectLoader.ColorOnlyEffect.World = world;
            EffectLoader.ColorOnlyEffect.View = view;
            EffectLoader.ColorOnlyEffect.Projection = projection;

            trail?.DrawTrail(EffectLoader.ColorOnlyEffect);
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = TexValue;
            var frameBox = mainTex.Frame(2, 1, 0, 0);
            Vector2 origin = frameBox.Size() / 2;

            spriteBatch.Draw(mainTex, Position - Main.screenPosition, frameBox, Color * 0.75f
                , 0, origin, scale, SpriteEffects.None, 0f);

            frameBox = mainTex.Frame(2, 1, 1, 0);
            spriteBatch.Draw(mainTex, Position - Main.screenPosition, frameBox, Color.White
                , 0, origin, scale, SpriteEffects.None, 0f);

            Texture2D ballTex = CoraliteAssets.Halo.EnergyA.Value;

            spriteBatch.Draw(ballTex, Position - Main.screenPosition, null, Color
                , (float)Main.timeForVisualEffects * 0.1f, ballTex.Size() / 2, 0.05f * scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(ballTex, Position - Main.screenPosition, null, Color.White with { A=150}
                , (float)Main.timeForVisualEffects * 0.1f, ballTex.Size() / 2, 0.05f * scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
