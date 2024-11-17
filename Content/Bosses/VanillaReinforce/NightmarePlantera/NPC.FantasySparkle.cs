using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 使用ai0控制状态<br></br>
    /// 为0时尝试缓慢逃离玩家，当玩家接触后触发梦魇花的特殊动作，之后状态变为-1，将会环绕在玩家身边<br></br>
    /// 为1时自身不动，当玩家靠近后同上<br></br>
    /// 为2时会逃离梦魇花，玩家靠近时同上<br></br>
    /// -2状态为聚合所有的自身并生成美梦神<br></br>
    /// 3状态为帮助，会先固定玩家位置，之后将玩家投射出以躲避噩梦之咬<br></br>
    /// 使用ai1和ai2传入目标点
    /// </summary>
    public class FantasySparkle : ModNPC/*, IDrawNonPremultiplied*/
    {
        public override string Texture => AssetDirectory.NightmarePlantera + Name;

        public ref float State => ref NPC.ai[0];
        public Vector2 TargetPos
        {
            get => new(NPC.ai[1], NPC.ai[2]);
            set
            {
                NPC.ai[1] = value.X;
                NPC.ai[2] = value.Y;
            }
        }

        private static NPC NightmareOwner => Main.npc[NightmarePlantera.NPBossIndex];
        public Player Target => Main.player[NPC.target];

        public Vector2 mainSparkleScale;
        public float circleSparkleScale;
        public int timer;

        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.friendly = true;
            NPC.knockBackResist = 0.5f;
            NPC.lifeMax = 230;
            NPC.width = NPC.height = 85;

            mainSparkleScale = new Vector2(2f, 4f);
            circleSparkleScale = 1f;

            NPC.HitSound = CoraliteSoundID.MountSummon_Item25;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.ModProjectile is BaseNightmareProj)
            {
                modifiers.SourceDamage += 5f;
                modifiers.SetCrit();
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 14; i++)
            {
                Dust dust = Dust.NewDustPerfect(NPC.Center, DustID.PlatinumCoin, (i * MathHelper.TwoPi / 14).ToRotationVector2() * Main.rand.NextFloat(10f, 11f), Scale: Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;
                dust = Dust.NewDustPerfect(NPC.Center, DustID.GoldCoin, (i * MathHelper.TwoPi / 14).ToRotationVector2() * Main.rand.NextFloat(9f, 10f), Scale: Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;
            }

            float rot = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < 7; i++)
            {
                PRTLoader.NewParticle(NPC.Center, (rot + (i * MathHelper.TwoPi / 7)).ToRotationVector2() * 5, CoraliteContent.ParticleType<HorizontalStar>(), NightmarePlantera.phantomColors[i], 0.3f);
            }

            SoundEngine.PlaySound(CoraliteSoundID.ManaCrystal_Item29, NPC.Center);
        }

        public override void AI()
        {
            if (!NightmarePlantera.NightmarePlanteraAlive(out NPC np))
            {
                NPC.Kill();
                return;
            }

            switch ((int)State)
            {
                case -2://聚合并生成美梦神
                    {
                        NPC.Center = Vector2.Lerp(NPC.Center, TargetPos, 0.04f);
                        NPC.velocity *= 0;
                        if (Vector2.Distance(NPC.Center, TargetPos) < 48)
                        {
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                NPC n = Main.npc[i];
                                if (n.active && n.type == ModContent.NPCType<FantasyGod>())
                                {
                                    n.ai[3] += 1 / 7f;
                                    if (n.ai[3] > 1)
                                        n.ai[3] = 1;

                                    break;
                                }
                            }

                            NPC.Kill();
                        }
                    }
                    break;
                case -1://在玩家头顶盘旋
                    {
                        int index = 0;
                        int total = 0;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC npc = Main.npc[i];
                            if (npc.active && npc.target == NPC.target && npc.ModNPC is FantasySparkle && npc.ai[0] == -1)
                            {
                                if (NPC.whoAmI > i)
                                    index++;

                                total++;
                            }
                        }

                        float angle = Main.GlobalTimeWrappedHourly + (index * (MathHelper.TwoPi / total));
                        NPC.Center = Vector2.Lerp(NPC.Center, Target.Center + (angle.ToRotationVector2() * 96), 0.25f);

                        NPC.dontTakeDamage = true;

                        if (Target.dead || !Target.active)
                        {
                            NPC.Kill();
                        }
                    }
                    break;
                default:
                case 0://缓慢靠近玩家
                    {
                        NPC.TargetClosest();

                        timer++;
                        if (timer > 45)
                        {
                            Rectangle rectangle = new((int)NPC.Center.X, (int)NPC.Center.Y, 2, 2);
                            CombatText.NewText(rectangle, Color.LightGoldenrodYellow, "帮帮我！");
                            timer = 0;
                        }

                        if (Vector2.Distance(Target.Center, NPC.Center) > 200)
                        {
                            float length = NPC.velocity.Length();

                            if (length < 8)
                            {
                                length += 0.2f;
                            }

                            Vector2 v = (NPC.Center - Target.Center).SafeNormalize(Vector2.One) * length / 4;
                            NPC.velocity.X -= v.X;
                            NPC.velocity.Y -= v.Y;
                            if (NPC.velocity.Length() > 8)
                            {
                                NPC.velocity = NPC.velocity.SafeNormalize(Vector2.One) * 8;
                            }
                        }

                        if ((NPC.Center - Target.Center).Length() < 48)
                        {
                            State = -1;
                            (np.ModNPC as NightmarePlantera).Exchange2DreamingStates();
                        }
                    }
                    break;
                case 1://仅检测玩家靠近，自身不动
                    {
                        if ((NPC.Center - Target.Center).Length() < 48)
                        {
                            State = -1;
                            (np.ModNPC as NightmarePlantera).Exchange2DreamingStates();
                        }
                    }
                    break;
                case 2://逃离梦魇花
                    {
                        NPC.TargetClosest();

                        if (Vector2.Distance(np.Center, NPC.Center) > 200)
                        {
                            float length = NPC.velocity.Length();

                            if (length < 10)
                            {
                                length += 0.2f;
                            }

                            Vector2 v = (NPC.Center - np.Center).SafeNormalize(Vector2.One) * length / 4;
                            NPC.velocity.X += v.X;
                            NPC.velocity.Y -= v.Y;
                            if (NPC.velocity.Length() > 8)
                            {
                                NPC.velocity = NPC.velocity.SafeNormalize(Vector2.One) * 8;
                            }
                        }

                        if ((NPC.Center - Target.Center).Length() < 48)
                        {
                            State = -1;
                            (np.ModNPC as NightmarePlantera).Exchange2DreamingStates();
                        }
                    }
                    break;
                case 3:
                    {
                        if (timer == 2)
                        {
                            Target.AddImmuneTime(ImmunityCooldownID.Bosses, 275);
                            Target.AddImmuneTime(ImmunityCooldownID.General, 275);
                        }
                        if (timer == 10)
                        {
                            Rectangle rectangle = new((int)NPC.Center.X, (int)NPC.Center.Y, 2, 2);
                            CombatText.NewText(rectangle, Color.LightGoldenrodYellow, "别害怕！！");
                        }

                        if (timer == 60)
                        {
                            Rectangle rectangle = new((int)NPC.Center.X, (int)NPC.Center.Y, 2, 2);
                            CombatText.NewText(rectangle, Color.LightGoldenrodYellow, "我会帮你！");
                        }

                        if (timer < 100)
                        {
                            Target.Center = NPC.Center;
                            Target.velocity *= 0f;

                            if (Target.TryGetModPlayer(out NightmarePlayerCamera player))
                                player.FanfasyImmuneTime = 125;
                        }
                        else if (timer == 100)
                        {
                            Target.velocity += (Target.Center - np.Center).SafeNormalize(Vector2.Zero) * 14f;
                        }
                        else if (timer < 120)
                        {
                            Target.velocity += (Target.Center - np.Center).SafeNormalize(Vector2.Zero) * 0.65f;
                        }
                        else if (timer > 1300)
                        {
                            NPC.Kill();
                        }

                        timer++;
                    }
                    break;
            }

            NPC.rotation = NPC.velocity.X * 0.05f;
            if (Main.rand.NextBool(3))
                for (int i = -1; i < 2; i += 2)
                {
                    int type = Main.rand.NextFromList(DustID.PlatinumCoin, DustID.GoldCoin);
                    Vector2 dir = new(i, 0);
                    Dust d = Dust.NewDustPerfect(NPC.Center, type, dir.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * Main.rand.NextFloat(1, 4), Scale: Main.rand.NextFloat(1, 1.5f));
                    //d.noGravity = true;
                }
        }

        public override bool PreKill()
        {
            if (NightmarePlantera.NightmarePlanteraAlive(out NPC np))
                (np.ModNPC as NightmarePlantera).KillFantasySparkle();

            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float factor = MathF.Sin(Main.GlobalTimeWrappedHourly);
            Vector2 pos = NPC.Center - screenPos;
            float rot = NPC.rotation;
            Color shineColor = new(252, 233, 194);
            //中心的闪光

            Texture2D lightTex = BaseNightmareSparkle.MainLight.Value;
            var origin = lightTex.Size() / 2;

            Color c = shineColor;
            c.A = 0;
            var scale = mainSparkleScale * 0.15f;
            spriteBatch.Draw(lightTex, pos, null, c, rot, origin, scale, 0, 0);
            spriteBatch.Draw(lightTex, pos, null, c * 0.5f, rot, origin, scale, 0, 0);

            //ProjectilesHelper.DrawPrettyStarSparkle(NPC.Opacity, 0, pos, Color.White, shineColor * 0.6f,
            //    0.5f, 0f, 0.5f, 0.5f, 1f, rot, mainSparkleScale, Vector2.One);

            Texture2D flowTex = BaseNightmareSparkle.Flow.Value;
            origin = flowTex.Size() / 2;

            Color shineC = shineColor * 0.75f;
            shineC.A = 0;

            var scale2 = scale.X * 0.55f;
            spriteBatch.Draw(flowTex, pos, null, shineC, NPC.rotation + 1.57f + Main.GlobalTimeWrappedHourly, origin, scale2, 0, 0);
            spriteBatch.Draw(flowTex, pos, null, c * 0.3f, NPC.rotation - Main.GlobalTimeWrappedHourly, origin, scale2, 0, 0);

            Vector2 secondScale = scale * 0.4f;
            for (int i = -1; i < 2; i += 2)
            {
                Vector2 offsetPos = pos + (rot.ToRotationVector2() * i * 22);
                float rot3 = rot + (i * 0.3f);
                spriteBatch.Draw(lightTex, offsetPos, null, c, rot3, origin, secondScale, 0, 0);
                //spriteBatch.Draw(lightTex, offsetPos, null, c, rot3, origin, secondScale, 0, 0);

                //ProjectilesHelper.DrawPrettyStarSparkle(NPC.Opacity, 0, pos + new Vector2(i * 16, 0), Color.White, shineColor * 0.6f,
                //    0.5f, 0f, 0.5f, 0.5f, 1f, rot, mainSparkleScale * 0.4f, Vector2.One);
            }

            //周围一圈小星星
            for (int i = 0; i < 7; i++)
            {
                float rot2 = (Main.GlobalTimeWrappedHourly * 2) + (i * MathHelper.TwoPi / 7);
                Vector2 dir = rot2.ToRotationVector2();
                dir = pos + (dir * (36 + (factor * 4)));
                rot2 += 1.57f;
                Color phantomC = NightmarePlantera.phantomColors[i];
                phantomC.A = 0;
                spriteBatch.Draw(lightTex, dir, null, phantomC, rot2, origin, circleSparkleScale * 0.2f, 0, 0);

                //Vector2 dir = (Main.GlobalTimeWrappedHourly * 2 + i * MathHelper.TwoPi / 7).ToRotationVector2();
                //ProjectilesHelper.DrawPrettyStarSparkle(NPC.Opacity, 0, pos + dir * (36 + factor * 4), Color.White, NightmarePlantera.phantomColors[i],
                //    0.5f + factor * 0.1f, 0f, 0.5f, 0.5f, 1f, rot, new Vector2(circleSparkleScale, circleSparkleScale), Vector2.One);
            }

            //绘制额外旋转的星星，和上面叠起来变成一个
            //Helpers.ProjectilesHelper.DrawPrettyStarSparkle(NPC.Opacity, 0, pos, Color.White * 0.3f, shineColor * 0.5f,
            //    0.5f - factor * 0.1f, 0f, 0.5f, 0.5f, 1f, NPC.rotation + MathHelper.PiOver4, new Vector2(mainSparkleScale.Y * 0.75f), Vector2.One);

            //绘制一层小的更加亮的来让星星中心变亮点
            //Helpers.ProjectilesHelper.DrawPrettyStarSparkle(NPC.Opacity, 0, pos, Color.White * 0.7f, Color.White * 0.4f,
            //    0.5f + factor * 0.1f, 0f, 0.5f, 0.5f, 1f, rot, mainSparkleScale * 0.5f, Vector2.One * 2);
            return false;
        }

        //public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        //{
        //    //Texture2D mainTex = TextureAssets.Npc[NPC.type].Value;
        //    //spriteBatch.Draw(mainTex, NPC.Center - Main.screenPosition, null, Color.White, 0, mainTex.Size() / 2, (1 + 0.1f * MathF.Sin(Main.GlobalTimeWrappedHourly)) * mainSparkleScale.Y / 4, 0, 0);
        //}
    }
}
