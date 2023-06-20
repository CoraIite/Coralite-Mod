using Coralite.Content.Particles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public partial class BabyIceDragon
    {
        public void SmashDown()
        {
            switch (movePhase)
            {
                case 0:
                    {
                        bool lowerThanTarget = NPC.Center.Y > (Target.Center.Y - 430);
                        float xLength = Math.Abs(NPC.Center.X - Target.Center.X);
                        if (lowerThanTarget || xLength > 250)
                        {
                            SetDirection();
                            if (lowerThanTarget)
                                FlyUp();
                            else
                            {
                                NPC.velocity.Y *= 0.9f;
                                ChangeFrameNormally();
                            }
                            if (xLength > 150)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 14f, 0.3f, 0.25f, 0.96f);
                            else
                                NPC.velocity.X *= 0.96f;

                            if (Timer > 300)
                            {
                                ResetStates();
                                return;
                            }

                            break;
                        }

                        movePhase = 1;
                        Timer = 0;
                        NPC.frame.Y = 0;
                        NPC.velocity.X *= 0.4f;
                        NPC.velocity.Y *= 0;
                        NPC.noGravity = true;
                        NPC.noTileCollide = true;
                        SetDirection();
                        Particle.NewParticle(NPC.Center, Vector2.Zero, CoraliteContent.ParticleType<Flash_WithOutLine>(), Coralite.Instance.IcicleCyan, 0.8f);
                        SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, NPC.Center);
                        NPC.netUpdate = true;
                    }
                    break;
                case 1:     //下落
                    {
                        NPC.rotation = NPC.rotation.AngleTowards(NPC.velocity.ToRotation() + (NPC.direction > 0 ? 0 : 3.14f), 0.14f);
                        NPC.velocity.Y += 0.9f;

                        if (Timer % 2 == 0)
                        {
                            Dust dust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(32, 32), DustID.FrostStaff, -NPC.velocity * 0.3f, Scale: Main.rand.NextFloat(1.8f, 2f));
                            dust.noGravity = true;
                        }

                        //向玩家加速
                        if (Timer < 40)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 5f, 0.4f, 0.3f, 0.96f);
                        else
                            NPC.velocity.X *= 0.96f;

                        if (Timer < 60)
                        {
                            if (NPC.Center.Y > (Target.Top.Y - 32))
                            {
                                Timer = 61;
                                NPC.noTileCollide = false;
                            }
                            break;
                        }

                        if (Timer == 60)
                            NPC.noTileCollide = false;

                        if (Timer > 60)   //检测下方物块
                        {
                            Vector2 position = NPC.BottomLeft;
                            position /= 16;
                            for (int i = 0; i < 4; i++)
                                for (int j = 0; j < 2; j++)
                                    if (WorldGen.ActiveAndWalkableTile((int)position.X + i, (int)position.Y + j))    //砸地，生成冰刺弹幕
                                    {
                                        SpawnIceThorns();
                                        NPC.velocity *= 0;
                                        HaveARest(30);
                                        return;
                                    }
                        }

                        if (Timer > 200)
                        {
                            NPC.velocity *= 0;
                            HaveARest(30);
                        }
                    }
                    break;
                default:
                    ResetStates();
                    return;
            }

            if (NPC.velocity.Y > 24)
                NPC.velocity.Y = 24;

            Timer++;
        }
    }
}
