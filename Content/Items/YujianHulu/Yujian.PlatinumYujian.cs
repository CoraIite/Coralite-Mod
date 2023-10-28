using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.Trails;
using Coralite.Core.Systems.YujianSystem;
using Coralite.Core.Systems.YujianSystem.YujianAIs;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.YujianHulu
{
    public class PlatinumYujian : BaseYujian
    {
        public PlatinumYujian() : base(ItemRarityID.White, Item.sellPrice(0, 0, 20, 0), 11, 1.5f) { }

        public override int ProjType => ModContent.ProjectileType<PlatinumYujianProj>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PlatinumBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class PlatinumYujianProj : BaseYujianProj
    {
        public PlatinumYujianProj() : base(
            new YujianAI[]
            {
                new YujianAI_BetterSpurt(80,20,35,180,0.95f),
                new YujianAI_PreciseSlash(startTime: 100,
                        slashWidth: 80,
                        slashTime: 70,
                        startAngle: -2f,
                        totalAngle: 3f,
                        turnSpeed: 2.2f,
                        roughlyVelocity: 0.9f,
                        halfShortAxis: 1f,
                        halfLongAxis: 1.5f,
                        Coralite.Instance.HeavySmootherInstance),
            },
            null,
            new YujianAI_PlatinumSlash(startTime: 100,
                    slashWidth: 80,
                    slashTime: 70,
                    startAngle: -2f,
                    totalAngle: 3f,
                    turnSpeed: 2.2f,
                    roughlyVelocity: 0.9f,
                    halfShortAxis: 1f,
                    halfLongAxis: 1.5f,
                    Coralite.Instance.HeavySmootherInstance),
            PowerfulAttackCost: 150,
            attackLength: 400,
            width: 30, height: 58,
             new Color(63, 59, 57), new Color(151, 149, 163),
             trailCacheLength: 18
            )
        { }
    }

    public class YujianAI_PlatinumSlash : YujianAI_BaseSlash
    {
        protected readonly float canSlashLength;

        private bool canSlash = false;

        private Trail trail;

        public YujianAI_PlatinumSlash(int startTime, int slashWidth, int slashTime, float startAngle, float totalAngle, float turnSpeed, float roughlyVelocity, float halfShortAxis, float halfLongAxis, ISmoother smoother) : base(startTime, slashWidth, slashTime, startAngle, totalAngle, turnSpeed, roughlyVelocity, halfShortAxis, halfLongAxis, smoother)
        {
            canSlashLength = SlashWidth * halfLongAxis / startElliptical;
        }

        protected override void Attack(BaseYujianProj yujianProj)
        {
            Projectile Projectile = yujianProj.Projectile;

            //先尝试接近NPC，距离小于一定值后开始斩击
            if (canSlash)
            {
                //斩击AI
                int time = StartTime - (int)yujianProj.Timer;

                if (time < SlashTime)
                {
                    Slash(Projectile, time);
                    int index = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Platinum);
                    Main.dust[index].noGravity = true;
                    if (time == SlashTime / 7 && Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, targetRotation.ToRotationVector2() * 10,
                            ModContent.ProjectileType<PlatinumSplash>(), (int)(Projectile.damage * 1.5f), Projectile.knockBack, Projectile.owner, 1, 30);
                    }
                    return;
                }

                if (time == SlashTime)
                {
                    canDamage = false;
                    AfterSlash(Projectile);
                }

                return;
            }

            TryGetClosed2Target(yujianProj, out float distance, out float targetAngle);

            if (distance < canSlashLength && distance > canSlashLength * 0.8f)
            {
                canSlash = true;
                canDamage = true;
                StartSlash(Projectile, targetAngle);
                yujianProj.InitTrailCaches();
                trail?.SetVertical(StartAngle < 0);      //开始角度为正时设为false
                SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
            }
        }

        public override void UpdateVelocityWhenTracking(Projectile Projectile, float distance, Vector2 targetDirection)
        {
            if (distance > canSlashLength)
                Projectile.velocity = (Projectile.velocity * (20f + roughlyVelocity) + targetDirection * turnSpeed) / 21f;
            else if (distance < canSlashLength * 0.8f)
                Projectile.velocity = (Projectile.velocity * (20f + roughlyVelocity) - targetDirection * turnSpeed) / 21f;
            else if (Projectile.velocity == Vector2.Zero)
                Projectile.velocity = (Projectile.velocity * (20f + roughlyVelocity) + targetDirection * turnSpeed) / 21f;
        }

        protected override void OnStartAttack(BaseYujianProj yujianProj)
        {
            //StartAngle = -StartAngle;
            yujianProj.Projectile.velocity += (yujianProj.Projectile.rotation - 1.57f).ToRotationVector2() * 0.02f;
            canDamage = false;
            canSlash = false;
        }

        protected override bool UpdateTime(BaseYujianProj yujianProj)
        {
            trail ??= new Trail(Main.instance.GraphicsDevice, yujianProj.Projectile.oldPos.Length, new NoTip(), factor => yujianProj.Projectile.height / 2,
            factor =>
            {
                return Color.Lerp(yujianProj.color1, yujianProj.color2, factor.X) * 0.8f;
            }, flipVertical: StartAngle < 0);

            trail.Positions = yujianProj.Projectile.oldPos;
            return canSlash;
        }

        public override void DrawPrimitives(BaseYujianProj yujianProj)
        {
            int time = StartTime - (int)yujianProj.Timer;
            if (!canSlash || time > SlashTime || time < yujianProj.trailCacheLength || smoother.Smoother(time, SlashTime) > 0.99f)
                return;

            Effect effect = Filters.Scene["SimpleTrail"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>(yujianProj.SlashTexture).Value);

            trail?.Render(effect);
        }
    }


    public class PlatinumSplash : ModProjectile
    {
        public override string Texture => AssetDirectory.YujianHulu + Name;

        public ref float OwnerDirection => ref Projectile.ai[0];
        public ref float MaxTime => ref Projectile.ai[1];

        public ref float Alpha => ref Projectile.localAI[1];
        public ref float Timer => ref Projectile.localAI[0];
        public bool fadeIn = true;
        public bool canDamage = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 70;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 30;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            if (fadeIn)
            {
                if (Alpha == 0f)
                {
                    Projectile.timeLeft = (int)MaxTime;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }

                Alpha += 0.15f;
                if (Alpha > 1)
                {
                    Alpha = 1;
                    fadeIn = false;
                }
            }

            Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(44, 44), DustID.Platinum, -Projectile.velocity * 0.6f);
            dust.noGravity = true;

            if (canDamage)
            {
                Vector2 targetDir = Projectile.rotation.ToRotationVector2();
                for (int i = 0; i < 4; i++)
                {
                    if (Framing.GetTileSafely(Projectile.Center + targetDir * i * 16).HasSolidTile())
                    {
                        Projectile.timeLeft = 10;
                        canDamage = false;
                        Projectile.netUpdate = true;
                        break;
                    }
                }
            }

            if (Projectile.timeLeft < 10)
            {
                Projectile.velocity *= 0.8f;
                if (Alpha > 0f)
                {
                    if (!fadeIn)
                        Alpha -= 0.1f;
                    Projectile.timeLeft += 1;
                }
            }

            Timer += 1;
            Lighting.AddLight(Projectile.Center, new Vector3(0.6f, 0.6f, 0.6f));
        }

        public override bool? CanDamage() => canDamage;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Collision.CanHitLine(Projectile.Center, 1, 1, targetHitbox.Center.ToVector2(), 1, 1))
                return base.Colliding(projHitbox, targetHitbox);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.timeLeft > 15)
                Projectile.timeLeft -= 10;

            Projectile.damage = (int)(Projectile.damage * 0.65f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> mainTex = TextureAssets.Projectile[Type];
            Vector2 center = Projectile.Center - Main.screenPosition;
            SpriteEffects effects = OwnerDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            float factor = Timer / MaxTime;
            float num3 = Utils.Remap(factor, 0f, 0.6f, 0f, 1f) * Utils.Remap(factor, 0.6f, 1f, 1f, 0f);

            Main.spriteBatch.Draw(mainTex.Value, center, null, Color.White * 0.8f * Alpha, Projectile.rotation, mainTex.Size() / 2, Projectile.scale, effects, 0f);

            float rotation = Projectile.rotation - OwnerDirection * 0.4f;
            for (int i = -1; i < 2; i++)
            {
                float scale = 2 - Math.Abs(i);
                Vector2 drawPos = center + (rotation + i * 0.6f + Utils.Remap(factor, 0f, 2f, 0f, (float)Math.PI / 2f) * OwnerDirection).ToRotationVector2() * (mainTex.Width() * 0.5f - 4f) * Projectile.scale;
                ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPos, new Color(255, 255, 255, 0) * num3 * 0.5f, Color.White, factor, 0f, 0.5f, 0.5f, 1f, (float)Math.PI / 4f, new Vector2(scale, scale), Vector2.One);
            }

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(canDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            canDamage = reader.ReadBoolean();
        }
    }

}
