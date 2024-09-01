using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    /// <summary>
    /// ai0传入主人，ai1传入角度，如果为0的话就会随机选取一个
    /// </summary>
    public class CrossLightingBall : LightningBall
    {
        public ref float OwnerIndex => ref Projectile.ai[0];
        public ref float Angle => ref Projectile.ai[1];

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (circles == null)
            {
                circles = new ThunderTrail[5];
                trails = new ThunderTrail[3];
                Asset<Texture2D> thunderTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "ThunderTrailB");

                for (int i = 0; i < circles.Length; i++)
                {
                    if (i < 2)
                        circles[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin, ThunderColorFunc_Yellow, GetAlpha);
                    else
                        circles[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin, ThunderColorFunc2_Orange, GetAlpha);

                    circles[i].SetRange((0, 10));
                    circles[i].SetExpandWidth(6);
                }

                for (int i = 0; i < trails.Length; i++)
                {
                    trails[i] = new ThunderTrail(thunderTex, ThunderWidthFunc2, ThunderColorFunc_Yellow, GetAlphaFade);
                    trails[i].SetRange((0, 6));
                    trails[i].SetExpandWidth(4);
                }

                float cacheLength = Projectile.velocity.Length() / 2;
                foreach (var trail in trails)
                {
                    if (cacheLength < 3)
                        trail.CanDraw = false;
                    else
                    {
                        Vector2[] vec = new Vector2[(int)cacheLength];
                        Vector2 basePos = Projectile.Center + (Helper.NextVec2Dir() * 5);
                        Vector2 dir = -Projectile.velocity;
                        vec[0] = basePos;

                        for (int i = 1; i < (int)cacheLength; i++)
                        {
                            vec[i] = basePos + (dir * i);
                        }

                        trail.BasePositions = vec;
                        trail.RandomThunder();
                    }
                }

                foreach (var circle in circles)
                {
                    circle.CanDraw = true;
                    int trailPointCount = Main.rand.Next(15, 30);
                    Vector2[] vec = new Vector2[trailPointCount];

                    float baseRot = Main.rand.NextFloat(6.282f);
                    for (int i = 0; i < trailPointCount; i++)
                    {
                        vec[i] = Projectile.Center + ((baseRot + (i * MathHelper.TwoPi / 40)).ToRotationVector2()
                            * ((Projectile.width / 2) + Main.rand.NextFloat(-8, 8)));
                    }

                    circle.BasePositions = vec;
                    circle.RandomThunder();
                }
            }

            foreach (var circle in circles)
                circle.UpdateTrail(Projectile.velocity);
            foreach (var trail in trails)
                trail.UpdateTrail(Projectile.velocity);

            if (Timer < 30)
                if (Timer % 5 == 0)
                {
                    float cacheLength = Projectile.velocity.Length() * 0.55f;

                    foreach (var trail in trails)
                    {
                        if (cacheLength < 3)
                            trail.CanDraw = false;
                        else
                        {
                            trail.CanDraw = Main.rand.NextBool();
                            if (trail.CanDraw)
                            {
                                Vector2[] vec = new Vector2[(int)cacheLength];
                                Vector2 basePos = Projectile.Center + (Helper.NextVec2Dir() * 10);
                                Vector2 dir = -Projectile.velocity * 0.55f;
                                vec[0] = basePos;

                                for (int i = 1; i < (int)cacheLength; i++)
                                {
                                    vec[i] = basePos + (dir * i);
                                }

                                trail.BasePositions = vec;
                                trail.RandomThunder();
                            }
                        }
                    }
                }

            if (Timer % 4 == 0)
            {
                ElectricParticle_Follow.Spawn(Projectile.Center, Main.rand.NextVector2Circular(30, 30),
                    () => Projectile.Center, Main.rand.NextFloat(0.5f, 0.75f));

                foreach (var circle in circles)
                {
                    circle.CanDraw = Main.rand.NextBool();
                    if (circle.CanDraw)
                    {
                        int width = Main.rand.Next(Projectile.width / 5, Projectile.width / 2);
                        float angle = MathHelper.TwoPi / (20 + (15 * Helper.Lerp(width / (float)(Projectile.width / 2), 0, 1)));
                        int trailPointCount = Main.rand.Next(5, 20);
                        Vector2[] vec = new Vector2[trailPointCount];

                        float baseRot = Main.rand.NextFloat(6.282f);
                        for (int i = 0; i < trailPointCount; i++)
                        {
                            vec[i] = Projectile.Center + ((baseRot + (i * angle)).ToRotationVector2()
                                * width);
                        }

                        circle.BasePositions = vec;
                        circle.RandomThunder();
                    }
                }
            }

            if (ThunderAlpha < 1)
            {
                ThunderWidth = 16;
                ThunderAlpha += 1 / 30f;
                if (ThunderAlpha > 1)
                    ThunderAlpha = 1;
            }

            Projectile.SpawnTrailDust(30f, DustID.PortalBoltTrail, Main.rand.NextFloat(0.1f, 0.4f),
                newColor: new Color(255, 202, 101), Scale: Main.rand.NextFloat(1f, 1.3f));

            if (Timer == 60)
            {
                if (Angle == 0)
                    Angle = Main.rand.NextFloat();
                Projectile.velocity *= 0;
                foreach (var trail in trails)
                {
                    trail.CanDraw = false;
                }
            }
            if (Timer > 60)
            {
                SpawnThunderDusts();

                if (Timer > 150)
                {
                    SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, Projectile.Center);
                    SpawnThunders();
                    Projectile.Kill();
                }
            }
            Timer++;
        }

        public virtual void SpawnThunderDusts()
        {
            float factor = (Timer - 60) / 90f;

            float length = Helper.Lerp(20, 400, factor);
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = (Angle + (i * MathHelper.PiOver2)).ToRotationVector2();
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(6, 6) + (dir * Main.rand.NextFloat(20, length)), DustID.PortalBoltTrail
                    , dir.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(2f, 6f), newColor: new Color(255, 202, 101),
                    Scale: Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = true;
            }
        }

        public virtual void SpawnThunders()
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = (Angle + (i * MathHelper.PiOver2)).ToRotationVector2();
                Vector2 pos = Projectile.Center + (dir * 40);
                Projectile.NewProjectileFromThis<LightingBreath>(pos + (dir * 550), pos, Projectile.damage, 0, 20,
                    (int)OwnerIndex, 70);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Timer < 60)
            {
                Timer = 60;
                if (Angle == 0)
                    Angle = Main.rand.NextFloat();
                Projectile.velocity *= 0;
                foreach (var trail in trails)
                {
                    trail.CanDraw = false;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// ai0传入主人，ai1传入角度，如果为0的话就会随机选取一个
    /// </summary>
    public class StrongerCrossLightingBall : CrossLightingBall
    {
        public override Color ThunderColorFunc_Yellow(float factor)
        {
            return Color.Lerp(ThunderveinDragon.ThunderveinPurpleAlpha, ThunderveinDragon.ThunderveinYellowAlpha, MathF.Sin(factor * MathHelper.Pi)) * ThunderAlpha;
        }

        public override Color ThunderColorFunc2_Orange(float factor)
        {
            return Color.Lerp(ThunderveinDragon.ThunderveinPurpleAlpha, ThunderveinDragon.ThunderveinOrangeAlpha, MathF.Sin(factor * MathHelper.Pi)) * ThunderAlpha;
        }

        public override void SpawnThunderDusts()
        {
            float factor = (Timer - 60) / 90f;

            float length = Helper.Lerp(20, 500, factor);
            for (int i = 0; i < 2; i++)
            {
                Vector2 dir = (Angle + (i * MathHelper.Pi)).ToRotationVector2();
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(6, 6) + (dir * Main.rand.NextFloat(20, length)), DustID.PortalBoltTrail
                    , dir.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(2f, 6f), newColor: new Color(255, 202, 101),
                    Scale: Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = true;
            }
        }

        public override void SpawnThunders()
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 dir = (Angle + (i * MathHelper.Pi)).ToRotationVector2();
                Vector2 pos = Projectile.Center + (dir * 40);
                Projectile.NewProjectileFromThis<StrongerLightingBreath>(pos + (dir * 650), pos, Projectile.damage, 0, 20,
                    (int)OwnerIndex, 70);
            }
        }
    }

    /// <summary>
    /// ai0传入主人，ai1传入角度，如果为0的话就会随机选取一个
    /// </summary>
    public class CrossLightingBallChasable : CrossLightingBall
    {
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (circles == null)
            {
                circles = new ThunderTrail[5];
                trails = new ThunderTrail[3];
                Asset<Texture2D> thunderTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "ThunderTrailB");

                for (int i = 0; i < circles.Length; i++)
                {
                    if (i < 2)
                        circles[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin, ThunderColorFunc_Yellow, GetAlpha);
                    else
                        circles[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin, ThunderColorFunc2_Orange, GetAlpha);

                    circles[i].SetRange((0, 10));
                    circles[i].SetExpandWidth(6);
                }

                for (int i = 0; i < trails.Length; i++)
                {
                    trails[i] = new ThunderTrail(thunderTex, ThunderWidthFunc2, ThunderColorFunc_Yellow, GetAlphaFade);
                    trails[i].SetRange((0, 6));
                    trails[i].SetExpandWidth(4);
                }

                float cacheLength = Projectile.velocity.Length() / 2;
                foreach (var trail in trails)
                {
                    if (cacheLength < 3)
                        trail.CanDraw = false;
                    else
                    {
                        Vector2[] vec = new Vector2[(int)cacheLength];
                        Vector2 basePos = Projectile.Center + (Helper.NextVec2Dir() * 5);
                        Vector2 dir = -Projectile.velocity;
                        vec[0] = basePos;

                        for (int i = 1; i < (int)cacheLength; i++)
                        {
                            vec[i] = basePos + (dir * i);
                        }

                        trail.BasePositions = vec;
                        trail.RandomThunder();
                    }
                }

                foreach (var circle in circles)
                {
                    circle.CanDraw = true;
                    int trailPointCount = Main.rand.Next(15, 30);
                    Vector2[] vec = new Vector2[trailPointCount];

                    float baseRot = Main.rand.NextFloat(6.282f);
                    for (int i = 0; i < trailPointCount; i++)
                    {
                        vec[i] = Projectile.Center + ((baseRot + (i * MathHelper.TwoPi / 40)).ToRotationVector2()
                            * ((Projectile.width / 2) + Main.rand.NextFloat(-8, 8)));
                    }

                    circle.BasePositions = vec;
                    circle.RandomThunder();
                }
            }

            foreach (var circle in circles)
                circle.UpdateTrail(Projectile.velocity);
            foreach (var trail in trails)
                trail.UpdateTrail(Projectile.velocity);

            if (Timer < 300)
            {
                Projectile.velocity += (player.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                if (Projectile.velocity.Length() > 20)
                    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 20;

                if (Vector2.Distance(player.Center, Projectile.Center) < 180)
                    Timer = 300;
                if (Timer % 5 == 0)
                {
                    float cacheLength = Projectile.velocity.Length() * 0.55f;

                    foreach (var trail in trails)
                    {
                        if (cacheLength < 3)
                            trail.CanDraw = false;
                        else
                        {
                            trail.CanDraw = Main.rand.NextBool();
                            if (trail.CanDraw)
                            {
                                Vector2[] vec = new Vector2[(int)cacheLength];
                                Vector2 basePos = Projectile.Center + (Helper.NextVec2Dir() * 10);
                                Vector2 dir = -Projectile.velocity * 0.55f;
                                vec[0] = basePos;

                                for (int i = 1; i < (int)cacheLength; i++)
                                {
                                    vec[i] = basePos + (dir * i);
                                }

                                trail.BasePositions = vec;
                                trail.RandomThunder();
                            }
                        }
                    }
                }
            }

            if (Timer % 4 == 0)
            {
                ElectricParticle_Follow.Spawn(Projectile.Center, Main.rand.NextVector2Circular(30, 30),
                    () => Projectile.Center, Main.rand.NextFloat(0.5f, 0.75f));

                foreach (var circle in circles)
                {
                    circle.CanDraw = Main.rand.NextBool();
                    if (circle.CanDraw)
                    {
                        int width = Main.rand.Next(Projectile.width / 5, Projectile.width / 2);
                        float angle = MathHelper.TwoPi / (20 + (15 * Helper.Lerp(width / (float)(Projectile.width / 2), 0, 1)));
                        int trailPointCount = Main.rand.Next(5, 20);
                        Vector2[] vec = new Vector2[trailPointCount];

                        float baseRot = Main.rand.NextFloat(6.282f);
                        for (int i = 0; i < trailPointCount; i++)
                        {
                            vec[i] = Projectile.Center + ((baseRot + (i * angle)).ToRotationVector2()
                                * width);
                        }

                        circle.BasePositions = vec;
                        circle.RandomThunder();
                    }
                }
            }

            if (ThunderAlpha < 1)
            {
                ThunderWidth = 16;
                ThunderAlpha += 1 / 30f;
                if (ThunderAlpha > 1)
                    ThunderAlpha = 1;
            }

            Projectile.SpawnTrailDust(30f, DustID.PortalBoltTrail, Main.rand.NextFloat(0.1f, 0.4f),
                newColor: new Color(255, 202, 101), Scale: Main.rand.NextFloat(1f, 1.3f));

            if (Timer == 300)
            {
                if (Angle == 0)
                    Angle = Main.rand.NextFloat();
                Projectile.velocity *= 0;
                foreach (var trail in trails)
                {
                    trail.CanDraw = false;
                }
            }
            if (Timer > 300)
            {
                SpawnThunderDusts();
                if (Timer > 300 + 90)
                {
                    SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, Projectile.Center);
                    SpawnThunders();
                    Projectile.Kill();
                }
            }
            Timer++;
        }

        public override void SpawnThunderDusts()
        {
            float factor = (Timer - 300) / 90f;

            float length = Helper.Lerp(20, 400, factor);
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = (Angle + (i * MathHelper.PiOver2)).ToRotationVector2();
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(6, 6) + (dir * Main.rand.NextFloat(20, length)), DustID.PortalBoltTrail
                    , dir.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(2f, 6f), newColor: new Color(255, 202, 101),
                    Scale: Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Timer < 300)
            {
                Timer = 300;
                if (Angle == 0)
                    Angle = Main.rand.NextFloat();
                Projectile.velocity *= 0;
                foreach (var trail in trails)
                {
                    trail.CanDraw = false;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// ai0传入主人，ai1传入角度，如果为0的话就会随机选取一个
    /// </summary>
    public class StrongerCrossLightingBallChasable : CrossLightingBallChasable
    {
        public override Color ThunderColorFunc_Yellow(float factor)
        {
            return Color.Lerp(ThunderveinDragon.ThunderveinPurpleAlpha, ThunderveinDragon.ThunderveinYellowAlpha, MathF.Sin(factor * MathHelper.Pi)) * ThunderAlpha;
        }

        public override Color ThunderColorFunc2_Orange(float factor)
        {
            return Color.Lerp(ThunderveinDragon.ThunderveinPurpleAlpha, ThunderveinDragon.ThunderveinOrangeAlpha, MathF.Sin(factor * MathHelper.Pi)) * ThunderAlpha;
        }

        public override void SpawnThunders()
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = (Angle + (i * MathHelper.PiOver2)).ToRotationVector2();
                Vector2 pos = Projectile.Center + (dir * 40);
                Projectile.NewProjectileFromThis<StrongerLightingBreath>(pos + (dir * 650), pos, Projectile.damage, 0, 20,
                    (int)OwnerIndex, 70);
            }
        }
    }
}
