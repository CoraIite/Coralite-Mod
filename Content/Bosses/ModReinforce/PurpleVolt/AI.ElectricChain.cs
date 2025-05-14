using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    public partial class ZacurrentDragon
    {
        public bool ElectricChain(int ChainTime)
        {
            switch (SonState)
            {
                default:
                case 0://先短暂远离准备冲刺
                    {
                        if (Timer == 0)
                        {
                            NPC.frame.Y = 1;
                            Timer = 1;
                        }

                        SetSpriteDirectionFoTarget();
                        SetRotationNormally(0.2f);

                        if (NPC.velocity.Length() < 8)
                            NPC.velocity += (NPC.Center - Target.Center).SafeNormalize(Vector2.Zero) * 0.65f;

                        //向后扇一下翅膀
                        if (++NPC.frameCounter > 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y++;
                            if (NPC.frame.Y > 7)
                            {
                                SonState = 1;
                                Timer = 0;
                                Recorder2 = (NPC.Center - Target.Center).ToRotation()+MathHelper.PiOver4;
                                ResetAllOldCaches();
                                canDrawShadows = true;
                                shadowScale = 1.2f;

                                Helper.PlayPitched("Electric/ElectricShoot", 0.4f, 0, NPC.Center);
                                SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, NPC.Center);
                                IsDashing = true;

                                Vector2 dir = (Target.Center+ Recorder2.ToRotationVector2()*500-NPC.Center).SafeNormalize(Vector2.Zero);
                                NPC.velocity = dir * 50;
                                NPC.rotation = NPC.velocity.ToRotation();
                                NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);
                                SetBackgroundLight(0.4f, 25, 8);

                                WindCircle.Spawn(NPC.Center, -dir * 2, dir.ToRotation(), ZacurrentPurple
                                    , 0.6f, 3f, new Vector2(1.25f, 1f));

                                int damage = Helper.GetProjDamage(200, 250, 300);
                                Recorder = NPC.NewProjectileInAI<ElectricChain>(NPC.Center, Vector2.Zero, damage, 0, NPC.target, -1, ChainTime);
                            }
                        }
                    }
                    return false;
                case 1://转圈飞行
                    {
                        UpdateAllOldCaches();

                        const int RollingTime = 60;
                        float factor = Timer / RollingTime;
                        float currentRot = Recorder2 + (factor * MathHelper.TwoPi * 1.5f);

                        float length = 550 + factor * 200;
                        Vector2 center = Target.Center + (currentRot.ToRotationVector2() * 600);
                        Vector2 dir = center - NPC.Center;

                        float velRot = NPC.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = NPC.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 300f, 0, 1) * 100;

                        NPC.velocity = velRot.AngleTowards(targetRot, 0.35f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.85f);
                        NPC.rotation = NPC.velocity.ToRotation();

                        Timer++;
                        if (Timer % 3 == 0)
                        {
                            int damage = Helper.GetProjDamage(200, 250, 300);
                            Recorder = NPC.NewProjectileInAI<ElectricChain>(NPC.Center, Vector2.Zero, damage
                                , 0, NPC.target, Recorder, ChainTime + Timer / 3 * 30);
                        }

                        if (Timer > RollingTime)
                        {
                            SonState = 2;
                            Timer = 0;

                            IsDashing = false;
                            canDrawShadows = false;
                            NPC.velocity *= 0.2f;
                        }
                    }
                    return false;
                case 2://短暂后摇
                    {
                        FlyingFrame();
                        SetSpriteDirectionFoTarget();
                        TurnToNoRot();

                        float distance = NPC.Center.Distance(Target.Center);
                        if (distance < 400)
                        {
                            if (NPC.velocity.Length() < 8)
                                NPC.velocity += (NPC.Center - Target.Center).SafeNormalize(Vector2.Zero) * 0.65f;
                        }
                        else if (distance > 900)
                        {
                            if (NPC.velocity.Length() < 12)
                                NPC.velocity += (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.65f;
                        }
                        else
                            NPC.velocity *= 0.95f;

                        Timer++;
                        if (Timer > 20)
                            return true;
                    }
                    return false;

            }
        }
    }
}
