using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    /// <summary>
    /// 使用ai0传入持有者
    /// </summary>
    [AutoLoadTexture(Path = AssetDirectory.Particles)]
    public class PurpleVoltBall : ModNPC
    {
        public override string Texture => AssetDirectory.ZacurrentDragon + "LightingBall";

        public ref float OwnerIndex => ref NPC.ai[0];
        public ref float State => ref NPC.ai[1];
        public ref float Timer => ref NPC.localAI[0];
        public ref float Alpha => ref NPC.localAI[1];
        public ref float ThunderWidth => ref NPC.localAI[3];
        public ref float ThunderAlpha => ref NPC.localAI[2];

        public float Fade;

        public ThunderTrail trail;

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
                            NPC.Kill();
                    }
                    break;
            }
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

        public override bool PreDraw(ref Color lightColor)
        {
            if (State < 2)//绘制本体的球
            {
                Texture2D mainTex = CoraliteAssets.Halo.Circle.Value;
                Texture2D tex2 = Projectile.GetTexture();

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
