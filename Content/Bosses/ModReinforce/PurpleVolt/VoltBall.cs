using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    /// <summary>
    /// 使用ai0传入持有者
    /// </summary>
    [VaultLoaden(AssetDirectory.Particles)]
    public class PurpleVoltBall : ModNPC
    {
        public override string Texture => AssetDirectory.ZacurrentDragon + "LightingBall";

        public ref float OwnerIndex => ref NPC.ai[0];
        public ref float State => ref NPC.ai[1];

        /// <summary>
        /// 状态计时器。<b>必须落在 <c>ai[]</c> 上</b>：它驱动整台状态机（300 帧后瞬移到本体并放雷链），
        /// 而原版 NPC 包只同步 <c>ai[0..3]</c>，放在 <c>localAI</c> 里两端各按自己的钟走，中途加入的客户端更是从零起跑（C3）。
        /// <c>ai[2]</c> / <c>ai[3]</c> 本来就是空的，直接占 <c>ai[2]</c>。
        /// </summary>
        public ref float Timer => ref NPC.ai[2];

        /// <summary>
        /// 淡入度。纯表现量，但阶段 0 的出口读它，而且阶段 1 之后不再推进——
        /// 中途加入的客户端若从 0 起算就会看到一颗<b>全透明却照样撞人</b>的球，所以随 <see cref="SendExtraAI"/> 过线。
        /// </summary>
        public ref float Alpha => ref NPC.localAI[1];
        public ref float ThunderWidth => ref NPC.localAI[3];
        public ref float ThunderAlpha => ref NPC.localAI[2];

        public float Fade;

        public ThunderTrail trail;

        /// <summary>
        /// 联机接线：本体 <see cref="ZacurrentDragon"/> 已退出原版平滑，紫电球作为部件同策略（C6 / C7 / C10）。
        /// </summary>
        private readonly CoraliteMinionNetSync net = new CoraliteMinionNetSync();

        public static ATex HorizontalStar { get; private set; }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            NPC.width = 55;
            NPC.height = 55;
            NPC.damage = 30;
            NPC.defense = 0;
            NPC.lifeMax = 750;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = 750;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (State == 1)
                return true;

            return false;
        }


        public float GetAlpha(float factor)
        {
            if (factor > Fade)
                return 0;

            return (Fade - factor) / Fade;
        }

        public virtual Color ThunderColorFunc(float factor)
        {
            return ZacurrentDragon.ZacurrentPurple;
        }

        public virtual float ThunderWidthFunc_Sin(float factor)
        {
            return MathF.Sin(factor * MathHelper.Pi) * ThunderWidth;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (ProjectileID.Sets.CultistIsResistantTo[projectile.type])
                modifiers.SourceDamage -= 0.35f;
        }

        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner<ZacurrentDragon>(out NPC owner, NPC.Kill))
                return;

            // C6 / C7：清原版平滑，并按本地预测消化上一包的纠偏量。必须在任何读位置的代码之前。
            net.BeginClientFrame(NPC);

            Lighting.AddLight(NPC.Center, ZacurrentDragon.ZacurrentPurple.ToVector3());
            //生成后以极快的速度前进
            switch (State)
            {
                default:
                case 0://刚生成，等待透明度变高后开始寻敌
                    {
                        NPC.rotation = (owner.Center - NPC.Center).ToRotation();
                        Alpha += 0.03f;

                        if (Alpha > 1)
                        {
                            Alpha = 1;
                            State = 1;
                            net.MarkDecision(NPC);//决策点：换态
                        }
                    }
                    break;
                case 1:
                    {
                        float speed = NPC.velocity.Length();
                        float factor = Timer / (60 * 5);

                        if (speed < 4f - factor * 3)
                            speed += 0.05f;
                        else
                            speed -= 0.05f;

                        Vector2 targetDir = (owner.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                        NPC.velocity = targetDir.RotatedBy(2f * MathF.Sin(factor * MathHelper.TwoPi) * (1 - factor)) * speed;

                        NPC.rotation = NPC.velocity.ToRotation();

                        if (Timer % 4 == 0)
                        {
                            ElectricParticle_PurpleFollow.Spawn(NPC.Center, Main.rand.NextVector2Circular(30, 30),
                                () => NPC.Center, Main.rand.NextFloat(0.5f, 0.75f));
                        }

                        if (Main.rand.NextBool())
                        {
                            Dust dust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(30, 30), DustID.PortalBoltTrail
                            , Vector2.Zero, newColor: ZacurrentDragon.ZacurrentDustPurple, Scale: Main.rand.NextFloat(1f, 1.3f));
                            dust.noGravity = true;
                            dust.velocity = -NPC.velocity * Main.rand.NextFloat(1, 2);
                        }

                        Timer++;
                        if (Timer > 60 * 5)
                        {
                            State = 2;
                            Timer = 0;

                            NPC.dontTakeDamage = true;
                            Helper.PlayPitched(CoraliteSoundID.QuietElectric_DD2_LightningAuraZap, NPC.Center);

                            ATex trailTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "ThunderTrailB2");
                            trail = new ThunderTrail(trailTex, ThunderWidthFunc_Sin, ThunderColorFunc, GetAlpha)
                            {
                                CanDraw = true,
                                //UseNonOrAdd = true,
                                PartitionPointCount = 3,
                            };
                            trail.SetRange((0, 14));
                            trail.SetExpandWidth(7);

                            float perLength = 60;
                            float distance = Vector2.Distance(NPC.Center, owner.Center);
                            Vector2 p = owner.Center;
                            Vector2 dir = (NPC.Center - owner.Center).SafeNormalize(Vector2.Zero);

                            List<Vector2> points = new();

                            int count = (int)(distance / perLength) + 1;
                            for (int i = 0; i < count; i++)
                            {
                                float distance2 = p.Distance(NPC.Center);
                                if (distance2 < perLength)
                                    perLength = distance2;
                                points.Add(p);
                                p += dir * perLength;
                            }

                            trail.BasePositions = [.. points];
                            trail.RandomThunder();

                            NPC.Center = owner.Center;
                            net.MarkDecision(NPC);//决策点：换态 + 瞬移，这一包必须立刻出门
                        }
                    }
                    break;
                case 2://后摇，闪电逐渐消失
                    {
                        Timer++;

                        float factor = Timer / 15;
                        Fade = 1 - factor;
                        ThunderWidth = (1 - factor) * 45;

                        // 寿终正寝两端同跑：Timer 现在过线，两端在同一帧到点；
                        // 客户端本地 Kill 才能就地放出 HitEffect 的死亡粒子，改成只在权威端会让队友看不到（7.9）。
                        if (Timer > 15)
                            NPC.Kill();
                    }
                    break;
            }

            net.EndClientFrame(NPC);//客户端记下预测位置，供下一次收包对账
            net.Heartbeat(NPC);//慢频兜底
        }

        /// <summary>
        /// 与位置 / 速度 / <c>ai[]</c> 同包原子过线。<see cref="Timer"/> 与 <see cref="State"/> 已经在 <c>ai[]</c> 里，
        /// 这里只补一个 <see cref="Alpha"/>；真正必须挂在这个钩子上的是收包时刻的纠偏（C7），它没有别的入口。
        /// </summary>
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Alpha);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Alpha = reader.ReadSingle();

            // 先把字段读完再纠偏：此时 position / velocity / ai[] 已是服务端值，流也已对齐。
            // 帧差留 0——球的巡航速度只有 4 px/f 上下，沿速度投影一帧的收益不到 5 px，
            // 而唯一的大位移是瞬移（不是速度驱动的），投影对它没有意义。
            net.OnSnapshot(NPC);
        }

        public override bool PreKill() => false;

        public override void HitEffect(NPC.HitInfo hit)
        {
            //增加兹雷龙的紫伏值
            if (NPC.life <= 0)
            {
                if (State > 1 && OwnerIndex.GetNPCOwner<ZacurrentDragon>(out NPC owner))
                    (owner.ModNPC as ZacurrentDragon).GetPurpleVolt(false);

                PRTLoader.NewParticle(NPC.Center, Vector2.Zero, CoraliteContent.ParticleType<LightningParticlePurple>(), Scale: 2.5f);

                float baseRot = Main.rand.NextFloat(6.282f);
                for (int i = 0; i < 5; i++)
                {
                    PRTLoader.NewParticle(NPC.Center + ((baseRot + (i * MathHelper.TwoPi / 5)).ToRotationVector2() * Main.rand.NextFloat(20, 30))
                        , Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Purple>());
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (State < 2)//绘制本体的球
            {
                Texture2D mainTex = CoraliteAssets.Halo.Circle.Value;
                Texture2D tex2 = NPC.GetTexture();

                Color c = ZacurrentDragon.ZacurrentPurpleAlpha;
                c *= Alpha;

                Vector2 position = NPC.Center - screenPos;

                PurpleElectricBall.BallBack.Value.QuickCenteredDraw(Main.spriteBatch, position
                    , Color.Black * 0.6f * Alpha, 0, 0.3f);

                Main.spriteBatch.Draw(mainTex, position, null, c, 0,
                    mainTex.Size() / 2, 0.2f, 0, 0);
                Main.spriteBatch.Draw(mainTex, position, null, c, 0,
                    mainTex.Size() / 2, 0.3f, 0, 0);

                Texture2D exTex = HorizontalStar.Value;

                Vector2 origin = exTex.Size() / 2;
                Main.spriteBatch.Draw(exTex, position, null, c, 0,
                    origin, 1f, 0, 0);

                c = drawColor;
                c.A = 0;
                c *= Alpha;
                Main.spriteBatch.Draw(exTex, position, null, c, 0,
                    origin, 0.75f, 0, 0);
                Main.spriteBatch.Draw(tex2, position, null, Color.White * Alpha, NPC.rotation,
                    tex2.Size() / 2, 1f, 0, 0);
            }

            if (State > 1)
                trail?.DrawThunder(Main.instance.GraphicsDevice);

            return false;
        }
    }

    /// <summary>
    /// 使用ai0传入持有者
    /// </summary>
    public class RedVoltBall : BaseZacurrentProj
    {
        public override string Texture => AssetDirectory.ZacurrentDragon + Name;

        public ref float OwnerIndex => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public float Timer;
        public float Alpha;

        public float Fade;

        public ThunderTrail trail;

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;

            Projectile.tileCollide = false;
            Projectile.hostile = true;
        }

        public override bool? CanDamage()
        {
            if (State == 1)
                return null;

            return false;
        }

        public override float GetAlpha(float factor)
        {
            if (factor > Fade)
                return 0;

            return (Fade - factor) / Fade;
        }

        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner<ZacurrentDragon>(out NPC owner, Projectile.Kill))
                return;

            Lighting.AddLight(Projectile.Center, ZacurrentDragon.ZacurrentRed.ToVector3());
            switch (State)
            {
                default:
                case 0://刚生成，等待透明度变高后开始寻敌
                    {
                        Projectile.rotation = (owner.Center - Projectile.Center).ToRotation();

                        Alpha += 0.03f;

                        if (Alpha > 1)
                        {
                            Alpha = 1;
                            State = 1;
                        }
                    }
                    break;
                case 1:
                    {
                        if (Projectile.velocity.Length() < 1f)
                            Projectile.velocity += (owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 0.05f;


                        Projectile.rotation = Projectile.velocity.ToRotation();

                        if (Timer % 4 == 0)
                        {
                            ElectricParticle_RedFollow.Spawn(Projectile.Center, Main.rand.NextVector2Circular(30, 30),
                                () => Projectile.Center, Main.rand.NextFloat(0.5f, 0.75f));
                        }

                        Timer++;
                        if (Timer > 60 * 5)
                        {
                            State = 2;
                            Timer = 0;

                            ATex trailTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "ThunderTrailB2");
                            trail = new ThunderTrail(trailTex, ThunderWidthFunc_Sin, ThunderColorFunc_Red, GetAlpha)
                            {
                                CanDraw = true,
                                //UseNonOrAdd = true,
                                PartitionPointCount = 3,
                            };
                            trail.SetRange((0, 14));
                            trail.SetExpandWidth(7);

                            float perLength = 45;
                            float distance = Vector2.Distance(Projectile.Center, owner.Center);
                            Vector2 p = owner.Center;
                            Vector2 dir = (Projectile.Center - owner.Center).SafeNormalize(Vector2.Zero);

                            List<Vector2> points = new();

                            int count = (int)(distance / perLength) + 1;
                            for (int i = 0; i < count; i++)
                            {
                                float distance2 = p.Distance(Projectile.Center);
                                if (distance2 < perLength)
                                    perLength = distance2;
                                points.Add(p);
                                p += dir * perLength;
                            }

                            trail.BasePositions = [.. points];
                            trail.RandomThunder();

                            Projectile.Center = owner.Center;
                        }
                    }
                    break;
                case 2://后摇，闪电逐渐消失
                    {
                        Timer++;
                        float factor = Timer / 15;

                        Fade = 1 - factor;
                        ThunderWidth = (1 - factor) * 45;

                        if (Timer > 15)
                            Projectile.Kill();
                    }
                    break;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (State > 1 && OwnerIndex.GetNPCOwner<ZacurrentDragon>(out NPC owner))
                (owner.ModNPC as ZacurrentDragon).GetPurpleVolt(true);

            PRTLoader.NewParticle(Projectile.Center, Vector2.Zero, CoraliteContent.ParticleType<LightningParticlePurple>(), Scale: 2.5f);

            float baseRot = Main.rand.NextFloat(6.282f);
            for (int i = 0; i < 5; i++)
            {
                PRTLoader.NewParticle(Projectile.Center + ((baseRot + (i * MathHelper.TwoPi / 5)).ToRotationVector2() * Main.rand.NextFloat(20, 30))
                    , Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Purple>());
            }
        }

        public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
        {
            if (State < 2)//绘制本体的球
            {
                Texture2D mainTex = CoraliteAssets.Halo.Circle.Value;
                Texture2D tex2 = Projectile.GetTextureValue();

                Color c = ZacurrentDragon.ZacurrentRed;
                c.A = 0;
                c *= Alpha;

                Vector2 position = Projectile.Center - Main.screenPosition;

                PurpleElectricBall.BallBack.Value.QuickCenteredDraw(Main.spriteBatch, position
                    , Color.Black * 0.6f * Alpha, 0, Projectile.scale * 0.2f);

                Main.spriteBatch.Draw(mainTex, position, null, c, 0,
                    mainTex.Size() / 2, 0.25f, 0, 0);

                Texture2D exTex = PurpleVoltBall.HorizontalStar.Value;

                Vector2 origin = exTex.Size() / 2;
                Main.spriteBatch.Draw(exTex, position, null, c, 0,
                    origin, 1f, 0, 0);

                c = lightColor;
                c.A = 0;
                c *= Alpha;
                Main.spriteBatch.Draw(exTex, position, null, c, 0,
                    origin, 0.75f, 0, 0);
                Main.spriteBatch.Draw(tex2, position, null, Color.White * Alpha, Projectile.rotation,
                    tex2.Size() / 2, 1f, 0, 0);
            }

            if (State > 1)
                trail?.DrawThunder(Main.instance.GraphicsDevice);

            return false;
        }
    }
}
