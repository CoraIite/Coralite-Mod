using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    /// <summary>
    /// ai0传入主人，ai1传入角度，如果为0的话就会随机选取一个
    /// </summary>
    public class CrossLightingBall : LightingBall
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
                Asset<Texture2D> thunderTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightingBody");

                for (int i = 0; i < circles.Length; i++)
                {
                    if (i < 2)
                        circles[i] = new ThunderTrail(thunderTex, ThunderWidthFunc, ThunderColorFunc);
                    else
                        circles[i] = new ThunderTrail(thunderTex, ThunderWidthFunc, ThunderColorFunc2);

                    circles[i].SetRange((0, 10));
                    circles[i].SetExpandWidth(6);
                }

                for (int i = 0; i < trails.Length; i++)
                {
                    trails[i] = new ThunderTrail(thunderTex, ThunderWidthFunc2, ThunderColorFunc_Fade);
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
                        Vector2 basePos = Projectile.Center + Helper.NextVec2Dir() * 5;
                        Vector2 dir = -Projectile.velocity;
                        vec[0] = basePos;

                        for (int i = 1; i < (int)cacheLength; i++)
                        {
                            vec[i] = basePos + dir * i;
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
                        vec[i] = Projectile.Center + (baseRot + i * MathHelper.TwoPi / 40).ToRotationVector2()
                            * (Projectile.width / 2 + Main.rand.NextFloat(-8, 8));
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
                                Vector2 basePos = Projectile.Center + Helper.NextVec2Dir() * 10;
                                Vector2 dir = -Projectile.velocity * 0.55f;
                                vec[0] = basePos;

                                for (int i = 1; i < (int)cacheLength; i++)
                                {
                                    vec[i] = basePos + dir * i;
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
                        float angle = MathHelper.TwoPi / (20 + 15 * Helper.Lerp(width / (float)(Projectile.width / 2), 0, 1));
                        int trailPointCount = Main.rand.Next(5, 20);
                        Vector2[] vec = new Vector2[trailPointCount];

                        float baseRot = Main.rand.NextFloat(6.282f);
                        for (int i = 0; i < trailPointCount; i++)
                        {
                            vec[i] = Projectile.Center + (baseRot + i * angle).ToRotationVector2()
                                * width;
                        }

                        circle.BasePositions = vec;
                        circle.RandomThunder();
                    }
                }
            }

            Timer++;
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
                float factor = (Timer - 60) / 90f;

                float length = Helper.Lerp(20, 400, factor);
                for (int i = 0; i < 4; i++)
                {
                    Vector2 dir = (Angle + i * MathHelper.PiOver2).ToRotationVector2();
                    Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(6, 6) + dir * Main.rand.NextFloat(20, length), DustID.PortalBoltTrail
                        , dir.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(2f, 6f), newColor: new Color(255, 202, 101),
                        Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                }

                if (Timer > 150)
                {
                    SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, Projectile.Center);
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 dir = (Angle + i * MathHelper.PiOver2).ToRotationVector2();
                        Vector2 pos = Projectile.Center + dir * 40;
                        Projectile.NewProjectileFromThis<LightingBreath>(pos + dir * 550, pos, Projectile.damage, 0, 20,
                            (int)OwnerIndex, 70);
                    }
                    Projectile.Kill();
                }
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
}
