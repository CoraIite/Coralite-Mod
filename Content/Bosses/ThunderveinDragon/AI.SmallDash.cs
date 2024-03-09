using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public partial class ThunderveinDragon
    {
        public void SmallDashP1()
        {
            const int smallDashTime = 15;

            switch (SonState)
            {
                default:
                    ResetStates();
                    break;
                case 0://短距离位移
                    {
                        if (Timer == 0)
                        {
                            DashFrame();
                            ResetAllOldCaches();
                            canDrawShadows = true;
                            isDashing = true;
                            shadowScale = 1.15f;
                            shadowAlpha = 1;

                            currentSurrounding = true;

                            //生成弹幕并随机速度方向
                            NPC.TargetClosest();
                            int damage = Helper.GetProjDamage(20, 30, 40);
                            NPC.NewProjectileDirectInAI<LightningDash>(NPC.Center, Vector2.Zero, damage, 0
                                , NPC.target, smallDashTime, NPC.whoAmI, 55);

                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, NPC.Center);
                            float targetrot = (Target.Center - NPC.Center).ToRotation();

                            targetrot += Main.rand.NextFromList(-1, 1) * Main.rand.NextFloat(0.9f, 1f);
                            NPC.velocity = targetrot.ToRotationVector2() * 35;
                            NPC.rotation = NPC.velocity.ToRotation();
                            NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);
                        }

                        UpdateAllOldCaches();

                        Timer++;
                        if (Timer > smallDashTime - 2)
                        {
                            if (Vector2.Distance(Target.Center, NPC.Center) > 700)
                            {
                                SonState = 1;
                            }
                            else
                            {
                                SonState = 2;
                                NPC.frame.X = 0;
                                NPC.frame.Y = 4;
                                NPC.frameCounter = 0;
                                NPC.rotation = NPC.direction > 0 ? 0 : 3.141f;
                                isDashing = false;
                                currentSurrounding = false;
                            }
                            Timer = 0;
                            NPC.velocity *= 0;
                            NPC.TargetClosest();
                        }
                    }
                    break;
                case 1://短距离位移
                    {
                        if (Timer == 0)
                        {
                            //生成弹幕并随机速度方向
                            NPC.TargetClosest();
                            int damage = Helper.GetProjDamage(20, 30, 40);
                            NPC.NewProjectileDirectInAI<LightningDash>(NPC.Center, Vector2.Zero, damage, 0
                                , NPC.target, smallDashTime, NPC.whoAmI, 55);

                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, NPC.Center);
                            float targetrot = (Target.Center - NPC.Center).ToRotation();

                            NPC.velocity = targetrot.ToRotationVector2() * 35;
                            NPC.rotation = NPC.velocity.ToRotation();
                            NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);
                        }

                        UpdateAllOldCaches();

                        Timer++;
                        if (Timer > smallDashTime - 2)
                        {
                            SonState++;
                            Timer = 0;
                            NPC.velocity *= 0;
                            NPC.frame.X = 0;
                            NPC.frame.Y = 4;
                            NPC.frameCounter = 0;
                            NPC.rotation = NPC.direction > 0 ? 0 : 3.141f;
                            NPC.TargetClosest();
                            isDashing = false;
                            currentSurrounding = false;
                        }
                    }
                    break;
                case 2:
                    {
                        FlyingFrame();
                        TurnToNoRot(0.2f);

                        Timer++;
                        if (Timer > 7)
                            ResetStates();
                    }
                    break;
            }
        }

        public void SmallDashP2()
        {
            const int smallDashTime = 17;

            switch (SonState)
            {
                default:
                    ResetStates();
                    break;
                case 0://短距离位移
                    {
                        if (Timer == 0)
                        {
                            DashFrame();
                            ResetAllOldCaches();
                            canDrawShadows = true;
                            isDashing = true;
                            shadowScale = 1.15f;
                            shadowAlpha = 1;

                            currentSurrounding = true;

                            //生成弹幕并随机速度方向
                            NPC.TargetClosest();
                            int damage = Helper.GetProjDamage(20, 30, 40);
                            NPC.NewProjectileDirectInAI<StrongLightningDash>(NPC.Center, Vector2.Zero, damage, 0
                                , NPC.target, smallDashTime, NPC.whoAmI, 55);

                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, NPC.Center);
                            float targetrot = (Target.Center - NPC.Center).ToRotation();

                            targetrot += Main.rand.NextFromList(-1, 1) * Main.rand.NextFloat(0.9f, 1f);
                            NPC.velocity = targetrot.ToRotationVector2() * 35;
                            NPC.rotation = NPC.velocity.ToRotation();
                            NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);
                        }

                        UpdateAllOldCaches();

                        Timer++;
                        if (Timer > smallDashTime - 2)
                        {
                            if (Vector2.Distance(Target.Center, NPC.Center) > 700)
                            {
                                SonState = 1;
                            }
                            else
                            {
                                SonState = 3;
                                NPC.frame.X = 0;
                                NPC.frame.Y = 4;
                                NPC.frameCounter = 0;
                                NPC.rotation = NPC.direction > 0 ? 0 : 3.141f;
                                isDashing = false;
                                currentSurrounding = false;
                            }
                            Timer = 0;
                            NPC.velocity *= 0;
                            NPC.TargetClosest();
                        }
                    }
                    break;
                case 1://短距离位移
                case 2:
                    {
                        if (Timer == 0)
                        {
                            //生成弹幕并随机速度方向
                            NPC.TargetClosest();
                            int damage = Helper.GetProjDamage(20, 30, 40);
                            NPC.NewProjectileDirectInAI<StrongLightningDash>(NPC.Center, Vector2.Zero, damage, 0
                                , NPC.target, smallDashTime, NPC.whoAmI, 55);

                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, NPC.Center);
                            float targetrot = (Target.Center - NPC.Center).ToRotation();

                            NPC.velocity = targetrot.ToRotationVector2() * 35;
                            NPC.rotation = NPC.velocity.ToRotation();
                            NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);
                        }

                        UpdateAllOldCaches();

                        Timer++;
                        if (Timer > smallDashTime - 2)
                        {
                            if (Vector2.Distance(Target.Center, NPC.Center) > 700)
                            {
                                SonState++;
                            }
                            else
                            {
                                SonState = 3;
                                NPC.frame.X = 0;
                                NPC.frame.Y = 4;
                                NPC.frameCounter = 0;
                                NPC.rotation = NPC.direction > 0 ? 0 : 3.141f;
                                isDashing = false;
                                currentSurrounding = false;
                            }

                            Timer = 0;
                            NPC.velocity *= 0;
                            NPC.TargetClosest();
                        }
                    }
                    break;
                case 3:
                    {
                        FlyingFrame();
                        TurnToNoRot(0.2f);

                        Timer++;
                        if (Timer > 7)
                            ResetStates();
                    }
                    break;
            }
        }
    }
}
