﻿using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 使用ai3传入大小
    /// </summary>
    public class FantasyGod : ModNPC, IDrawNonPremultiplied, IDrawWarp
    {
        public override string Texture => AssetDirectory.NightmarePlantera + Name;

        public FantasyTentacle[] leftTentacles;
        public FantasyTentacle[] rightTentacles;

        public ref float State => ref NPC.ai[0];
        public ref float Timer => ref NPC.ai[1];
        public ref float Angle => ref NPC.ai[2];
        public ref float LightScale => ref NPC.ai[3];

        public static Color shineColor = new(252, 233, 194);

        private bool canDrawWarp = false;
        private float warpScale = 0;

        public override void SetDefaults()
        {
            NPC.lifeMax = 1;
            NPC.dontTakeDamage = true;
            NPC.friendly = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override bool? CanBeHitByItem(Player player, Item item) => false;
        public override bool CanBeHitByNPC(NPC attacker) => false;
        public override bool? CanBeHitByProjectile(Projectile projectile) => false;

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;
        public override bool CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void OnSpawn(IEntitySource source)
        {
            Helper.PlayPitched(CoraliteSoundID.EmpressOfLight_Summoned_Item161, NPC.Center, pitch: 0.5f);
        }

        public override void AI()
        {
            if (!NightmarePlantera.NightmarePlanteraAlive(out NPC np))
            {
                NPC.Kill();
                return;
            }

            leftTentacles ??= new FantasyTentacle[3]
            {
                new(25,TentacleColor,TentacleWidth,NightmarePlantera.tentacleTex,NightmarePlantera.waterFlowTex),
                new(25,TentacleColor,TentacleWidth,NightmarePlantera.tentacleTex,NightmarePlantera.waterFlowTex),
                new(25,TentacleColor,TentacleWidth,NightmarePlantera.tentacleTex,NightmarePlantera.waterFlowTex)
            };

            rightTentacles ??= new FantasyTentacle[3]
            {
                new(25,TentacleColor,TentacleWidth,NightmarePlantera.tentacleTex,NightmarePlantera.waterFlowTex),
                new(25,TentacleColor,TentacleWidth,NightmarePlantera.tentacleTex,NightmarePlantera.waterFlowTex),
                new(25,TentacleColor,TentacleWidth,NightmarePlantera.tentacleTex,NightmarePlantera.waterFlowTex)
            };

            float sin = MathF.Sin(Main.GlobalTimeWrappedHourly);
            float length = LightScale * 400;
            float perLength = length / 25;
            float lightScale = LightScale * 0.75f;

            for (int i = 0; i < 3; i++)
            {
                float angle = -MathHelper.PiOver2 - 0.25f - (i * 0.35f);
                Vector2 targetPos = (angle + (sin * (i + 1) * 0.25f)).ToRotationVector2() * length;
                leftTentacles[i].SetValue(NPC.Center, NPC.Center + targetPos, angle);
                leftTentacles[i].UpdateTentacle(perLength * (1 - (i * 0.15f)), lightScale);

                angle = -MathHelper.PiOver2 + 0.25f + (i * 0.35f);
                targetPos = (angle - (sin * (i + 1) * 0.25f)).ToRotationVector2() * length;
                rightTentacles[i].SetValue(NPC.Center, NPC.Center + targetPos, angle);
                rightTentacles[i].UpdateTentacle(perLength * (1 - (i * 0.15f)), lightScale);
            }

            Lighting.AddLight(NPC.Center, new Vector3(4, 4, 4));

            switch ((int)State)
            {
                default:
                case 0://刚生成，移动到噩梦花身边 蓄力并射出一堆光球弹幕
                    {
                        np.rotation = np.rotation.AngleTowards((NPC.Center - np.Center).ToRotation(), 0.3f);

                        Vector2 dir = (np.Center - NPC.Center).SafeNormalize(Vector2.One);

                        float velLength = NPC.velocity.Length();
                        if (velLength < 20)
                        {
                            velLength += 0.25f;
                        }

                        float distance = Vector2.Distance(np.Center, NPC.Center);
                        bool far = distance > 600;
                        bool close = distance < 400;

                        if (far)
                            NPC.velocity = dir * velLength;
                        else if (close)
                            NPC.velocity = -dir * velLength;

                        if (distance <= 640 && distance >= 360 && LightScale > 0.8f)
                        {
                            NPC.velocity *= 0.5f;
                            State = 1;
                            Timer = 0;
                            canDrawWarp = true;
                            warpScale = 4f;
                            Angle = (NPC.Center - np.Center).ToRotation();
                            NPC.TargetClosest();

                            for (int i = 0; i < 4; i++)
                            {
                                SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, NPC.Center);
                                Vector2 center = np.Center + Main.rand.NextVector2CircularEdge(1000, 1000);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), center, (np.Center - center).SafeNormalize(Vector2.Zero),
                                   ModContent.ProjectileType<FantasySpike_Visual>(), 1, 0, NPC.target, 300, 1100);
                            }
                        }
                    }
                    break;
                case 1:
                    {
                        np.rotation = np.rotation.AngleTowards((NPC.Center - np.Center).ToRotation(), 0.3f);

                        Vector2 center = np.Center + ((Angle + (Timer / 460 * MathHelper.TwoPi)).ToRotationVector2() * 450);
                        Vector2 dir2 = center - NPC.Center;

                        float velRot = NPC.velocity.ToRotation();
                        float targetRot = dir2.ToRotation();

                        float speed = NPC.velocity.Length();
                        float aimSpeed = Math.Clamp(dir2.Length() / 400f, 0, 1) * 45;

                        NPC.velocity = velRot.AngleTowards(targetRot, 0.45f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.45f);

                        for (int i = 0; i < 4; i++)
                        {
                            Vector2 v = Helper.NextVec2Dir();
                            PRTLoader.NewParticle(NPC.Center + (v * Main.rand.Next(80, 100)), v * Main.rand.NextFloat(6, 24f),
                                CoraliteContent.ParticleType<BigFog>(), shineColor, Scale: Main.rand.NextFloat(0.5f, 1.25f));
                        }

                        //if (Timer == 2)
                        //{
                        //    SoundStyle st = CoraliteSoundID.EmpressOfLight_Summoned_Item161;
                        //    st.Pitch = 0.5f;
                        //    SoundEngine.PlaySound(st, NPC.Center);
                        //}

                        if (Timer < 30)
                        {
                            warpScale -= 4f / 30;
                        }

                        if (Timer == 30)
                        {
                            canDrawWarp = false;
                        }

                        if (Timer > 30)
                        {
                            if (Timer % 10 == 0)
                            {
                                SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, NPC.Center);
                                Vector2 dir = Helper.NextVec2Dir();
                                NPC.NewProjectileInAI<FantasyBall>(NPC.Center, dir * 20, 1200, 0);
                                var modifyer = new PunchCameraModifier(NPC.Center, dir, 8, 10, 10, 1000);
                                Main.instance.CameraModifiers.Add(modifyer);
                            }
                        }

                        if (Timer > 168)
                        {
                            NPC.velocity *= 0.5f;
                            State++;
                            Timer = 0;
                            float angle = (NPC.Center - np.Center).ToRotation() - (2 * 0.3f);
                            for (int i = 0; i < 4; i++)
                            {
                                NPC.NewProjectileInAI<FantasySpike>(NPC.Center, Vector2.Zero, 3000, 0, ai1: angle + (i * 0.3f), ai2: 60 + (i * 15));
                            }
                        }
                    }
                    break;
                case 2:
                    {
                        if (Timer == 50)
                        {
                            Helper.PlayPitched(CoraliteSoundID.EmpressOfLight_Summoned_Item161, NPC.Center, pitch: 0.5f);
                        }
                        else if (Timer > 65)
                        {
                            np.rotation += Main.rand.NextFloat(-0.1f, 0.25f);
                            if (Timer % 5 == 0)
                            {
                                Vector2 dir2 = Helper.NextVec2Dir();

                                var modifyer = new PunchCameraModifier(np.Center, dir2, 26, 12, 5, 1000);
                                Main.instance.CameraModifiers.Add(modifyer);
                            }

                            for (int i = 0; i < 2; i++)
                            {
                                int type = Main.rand.NextFromList(DustID.PlatinumCoin, DustID.GoldCoin);
                                Vector2 dir1 = Helper.NextVec2Dir();
                                Dust.NewDustPerfect(np.Center, type, dir1 * Main.rand.NextFloat(1, 20), Scale: Main.rand.NextFloat(1, 1.5f));
                            }

                            //if (Timer % 2 == 0)
                            //{
                            for (int i = 0; i < 4; i++)
                            {
                                Vector2 dir2 = Helper.NextVec2Dir();

                                Dust d = Dust.NewDustPerfect(np.Center, ModContent.DustType<NightmareStar>(),
                                    dir2 * Main.rand.NextFloat(1, 20), newColor: shineColor, Scale: Main.rand.NextFloat(1, 3f));
                                d.rotation = d.velocity.ToRotation() + MathHelper.PiOver2;
                                //}
                            }

                            //for (int i = 0; i < 2; i++)
                            //{
                            Vector2 v = Helper.NextVec2Dir();
                            PRTLoader.NewParticle(np.Center, v * Main.rand.NextFloat(6, 12f),
                                CoraliteContent.ParticleType<BigFog>(), shineColor, Scale: Main.rand.NextFloat(0.5f, 0.75f));
                            //}
                        }
                        NPC.velocity *= 0.9f;
                        if (Timer > 160)
                        {
                            State = 3;
                            Timer = 0;
                        }
                    }
                    break;
                case 3:
                    {
                        np.rotation += Main.rand.NextFloat(-0.1f, 0.25f);

                        LightScale -= 1 / 20f;

                        if (Timer > 20)
                        {
                            NPC.Kill();
                            NightmarePlantera.FantasyGod = -1;
                        }
                    }
                    break;
            }

            Timer++;
        }

        public static Color TentacleColor(float factor)
        {
            if (factor < 0.3f)
            {
                return Color.Lerp(Color.White, shineColor, factor / 0.3f);
            }

            return Color.Lerp(shineColor, new Color(0, 0, 0, 0), (factor - 0.3f) / 0.7f);
        }

        public static float TentacleWidth(float factor)
        {
            return Helper.Lerp(35, 0, factor);
        }



        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float factor = Math.Abs(MathF.Sin(Main.GlobalTimeWrappedHourly)) * 0.6f;
            Vector2 pos = NPC.Center - Main.screenPosition;

            if (leftTentacles != null && rightTentacles != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    leftTentacles[i].DrawTentacle(i => 6 * MathF.Sin(i * factor), 2);
                    rightTentacles[i].DrawTentacle(i => 6 * -MathF.Sin(i * factor), 2);
                }
            }

            Vector2 mainSparkleScale = new(8, 10);
            Helper.DrawPrettyStarSparkle(NPC.Opacity, 0, pos, Color.White, shineColor * 0.6f,
                LightScale, 0f, 1, 1, 1.5f, 0, mainSparkleScale, Vector2.One * 3); ;

            for (int i = -1; i < 2; i += 2)
            {
                Helper.DrawPrettyStarSparkle(NPC.Opacity, 0, pos + new Vector2(i * 38, 0), Color.White, shineColor * 0.6f,
                    LightScale, 0f, 1, 1, 1.5f, 0, mainSparkleScale * 0.6f, Vector2.One);
            }
            return false;
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = TextureAssets.Npc[NPC.type].Value;
            Vector2 pos = NPC.Center - Main.screenPosition;

            spriteBatch.Draw(mainTex, pos, null, Color.White, Main.GlobalTimeWrappedHourly / 2, mainTex.Size() / 2, LightScale * 0.75f, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < 7; i++)
            {
                Vector2 dir = ((Main.GlobalTimeWrappedHourly * 2) + (i * MathHelper.TwoPi / 7)).ToRotationVector2();
                Helper.DrawPrettyStarSparkle(NPC.Opacity, 0, pos + (dir * 48), Color.White, NightmarePlantera.phantomColors[i],
                    0.5f, 0f, 0.5f, 0.5f, 1f, 0, new Vector2(1, 3), Vector2.One);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public void DrawWarp()
        {
            if (canDrawWarp)
            {
                Texture2D mainTex = BlackHole.WarpTex.Value;

                Main.spriteBatch.Draw(mainTex, NPC.Center - Main.screenPosition, null, Color.White, Main.GlobalTimeWrappedHourly * 2, mainTex.Size() / 2, warpScale, 0, 0);
            }


        }
    }
}
