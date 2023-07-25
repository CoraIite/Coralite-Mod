using Coralite.Content.Items.Icicle;
using Coralite.Content.Particles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public partial class BabyIceDragon
    {
        public void IceCloud()
        {
            switch (movePhase)
            {
                case 0:
                    {
                        if (Vector2.Distance(NPC.Center, Target.Center) > 440)
                        {
                            SetDirection();
                            NPC.directionY = (Target.Center.Y - 200) > NPC.Center.Y ? 1 : -1;
                            float yLength = Math.Abs(Target.Center.Y - 200 - NPC.Center.Y);
                            if (yLength > 50)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 8f, 0.25f, 0.3f, 0.96f);
                            else
                                NPC.velocity.Y *= 0.96f;

                            if (Math.Abs(Target.Center.X - NPC.Center.X) > 160)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 16f, 0.25f, 0.3f, 0.96f);
                            else
                                NPC.velocity.X *= 0.96f;
                            NormallyFlyingFrame();
                            if (Timer > 400)
                                ResetStates();

                            break;
                        }

                        movePhase = 1;
                        Timer = 0;
                        NPC.netUpdate = true;
                    }
                    break;
                case 1:
                    {
                        NPC.velocity *= 0.97f;
                        NPC.rotation = NPC.rotation.AngleTowards(0, 0.04f);
                        if ((int)Timer == 30)
                        {
                            NPC.frame.X = 0;
                            NPC.frame.Y = 2;
                            NPC.velocity *= 0;
                        }

                        if (Timer < 50)
                            break;

                        if ((int)Timer == 50)
                        {
                            NPC.frame.X = 1;
                            NPC.frame.Y = 0;
                            SoundEngine.PlaySound(CoraliteSoundID.Roar, NPC.Center);
                            GetMouseCenter(out _, out Vector2 mouseCenter);
                            Particle.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.1f);
                            PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, new Vector2(0.8f, 0.8f), 5f, 20f, 40, 1000f, "BabyIceDragon");
                            Main.instance.CameraModifiers.Add(modifier);
                        }

                        if ((int)Timer == 60 && Main.netMode != NetmodeID.MultiplayerClient)        //生成冰云NPC
                        {
                            Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), Target.Center + new Vector2(Main.rand.Next(-80, 80), -300),
                                Vector2.Zero, ModContent.ProjectileType<IceyCloud>(), 1, 1, NPC.target);
                            GetMouseCenter(out _, out Vector2 mouseCenter);

                            for (int j = 0; j < 2; j++)
                                IceStarLight.Spawn(mouseCenter,
                                    (proj.Center - NPC.Center).SafeNormalize(Vector2.One).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * 10,
                                    1f, () => proj.Center, 16);
                        }

                        if (Timer < 90)
                        {
                            GetMouseCenter(out _, out Vector2 mouseCenter);
                            if ((int)Timer % 10 == 0)
                                Particle.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringWave>(), Color.White, 0.1f);
                            if ((int)Timer % 20 == 0)
                                Particle.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.1f);

                            break;
                        }

                        NormallyFlyingFrame();

                        if (Timer > 120)
                            ResetStates();
                    }
                    break;
                default:
                    ResetStates();
                    break;
            }

            Timer++;

        }
    }
}
