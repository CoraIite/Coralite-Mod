using Coralite.Core;
using Coralite.Core.Systems.YujianSystem;
using Coralite.Core.Systems.YujianSystem.YujianAIs;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.YujianHulu
{
    public class DemoniteYujian : BaseYujian
    {
        public DemoniteYujian() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 20, 0), 12, 1f) { }

        public override int ProjType => ModContent.ProjectileType<DemoniteYujianProj>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DemoniteBar, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class DemoniteYujianProj : BaseYujianProj
    {
        public DemoniteYujianProj() : base(
        new YujianAI[]
        {
             new YujianAI_DemoniteDoubleSlash(),
             new YujianAI_PreciseSlash(startTime: 65,
                 slashWidth: 60,
                 slashTime: 60,
                 startAngle: -2.4f,
                 totalAngle: 4f,
                 turnSpeed: 1.5f,
                 roughlyVelocity: 0.8f,
                 halfShortAxis: 1f,
                 halfLongAxis: 1.5f,
                 Coralite.Instance.HeavySmootherInstance),
             new YujianAI_BetterSpurt(75,16,30,180,0.93f),
        },
        new YujianAI_DemoniteSpurt(50, 12, 25, 180, 0.92f),
        PowerfulAttackCost: 150,
        attackLength: 430,
        width: 30, height: 60,
          new Color(28, 10, 94), new Color(158, 111, 235),
        trailCacheLength: 16,
        yujianAIsRandom: new int[]
        {
            5,
            2,
            3
        }
        )
        { }
    }

    public class YujianAI_DemoniteSpurt : YujianAI
    {
        /// <summary> 大于此时间时为准备阶段 </summary>
        public readonly int firstPhaseTime;
        /// <summary> 大于此时间时为突刺阶段，小于此时间为休息阶段 </summary>
        public readonly int SecondPhaseTime;
        public readonly int distanceToKeep;
        public readonly float slowdownFactor;

        public Vector2 targetCenter;
        public float targetDir;

        /// <summary>
        /// 只需要输入对应的3个时间，内部将自动计算timer
        /// </summary>
        /// <param name="readyTime">准备时间</param>
        /// <param name="spurtTime">突刺时间</param>
        /// <param name="restTime">休息时间</param>
        public YujianAI_DemoniteSpurt(int readyTime, int spurtTime, int restTime, int distanceToKeep, float slowdownFactor)
        {
            StartTime = readyTime + spurtTime + readyTime;
            firstPhaseTime = spurtTime + restTime;
            SecondPhaseTime = restTime;
            this.distanceToKeep = distanceToKeep;
            this.slowdownFactor = slowdownFactor;
        }

        protected override void Attack(BaseYujianProj yujianProj)
        {
            Projectile Projectile = yujianProj.Projectile;

            if (yujianProj.Timer > firstPhaseTime)  //准备阶段
            {
                Vector2 targetCenter = yujianProj.GetTargetCenter(IsAimingMouse);
                Vector2 targetVector = targetCenter - Projectile.Center;
                Vector2 targetDirection = targetVector.SafeNormalize(Vector2.Zero);

                Projectile.rotation += 0.15f;
                float length = targetVector.Length();

                if (length > distanceToKeep + 20)
                    Projectile.velocity = (Projectile.velocity * 20f + targetDirection * 2) / 21f;
                else if (length < distanceToKeep - 20)
                    Projectile.velocity = (Projectile.velocity * 20f + targetDirection * -2) / 21f;
                else
                    Projectile.velocity *= slowdownFactor;

                Projectile.velocity += targetDirection.RotatedBy(1.57f) * 0.15f;

                return;
            }

            if (yujianProj.Timer == firstPhaseTime)     //转为突刺阶段
            {
                targetCenter = yujianProj.GetTargetCenter(IsAimingMouse);
                int spurtTime = firstPhaseTime - SecondPhaseTime;
                float speed = (Vector2.Distance(targetCenter, Projectile.Center) + distanceToKeep * 0.3f) / spurtTime;
                Vector2 targetDir = (targetCenter - Projectile.Center).SafeNormalize(Vector2.One);

                Projectile.velocity = targetDir * speed;
                Projectile.rotation = targetDir.ToRotation() + 1.57f;
                Projectile.tileCollide = false;

                //只是生成一些粒子
                for (int i = 0; i < 8; i++)
                {
                    Vector2 vel = -targetDir.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)) * Main.rand.NextFloat(1f, 2f);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32), DustID.Granite, vel);
                    dust.noGravity = true;
                }

                for (int i = 0; i < 4; i++)
                {
                    Vector2 vel = -targetDir.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)) * Main.rand.NextFloat(1f, 2f);
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32), DustID.Shadowflame, vel);
                    dust2.noGravity = true;
                }

                yujianProj.InitTrailCaches();

                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * (Projectile.extraUpdates + 1), ModContent.ProjectileType<DemoniteSpurt>(),
                        Projectile.damage * 2, Projectile.knockBack, Projectile.owner, spurtTime / (Projectile.extraUpdates + 1), 48);

                SoundEngine.PlaySound(CoraliteSoundID.Slash_Item71, Projectile.Center);
            }

            if (yujianProj.Timer > SecondPhaseTime)     //突刺阶段
            {
                yujianProj.UpdateCaches();

                Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.One) * 4f;
                for (int i = 0; i < 2; i++)
                {
                    int index = Dust.NewDust(Projectile.position, 32, 32, DustID.Demonite, vel.X, vel.Y, Scale: Main.rand.NextFloat(0.8f, 1.2f));
                    Main.dust[index].noGravity = true;
                }

                if (yujianProj.Timer % 3 == 0 && Main.myPlayer == Projectile.owner)
                {
                    Vector2 vel2 = Projectile.velocity.SafeNormalize(Vector2.One).RotatedBy(1.57f + Main.rand.NextFloat(-0.6f, 0.6f)) * Main.rand.Next(60, 110);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, vel2, ModContent.ProjectileType<DemoniteStrike>(),
                        Projectile.damage, 0f, Projectile.owner, Main.rand.Next(24, 32));
                }
                return;
            }

            if (yujianProj.Timer == SecondPhaseTime)
            {
                Projectile.tileCollide = yujianProj.TileCollide;
            }

            //后摇休息阶段
            Projectile.velocity *= slowdownFactor;
        }

        protected override void OnStartAttack(BaseYujianProj yujianProj)
        {
            yujianProj.InitTrailCaches();
            canDamage = false;
        }

        public override void DrawAdditive(SpriteBatch spriteBatch, BaseYujianProj yujianProj)
        {
            Projectile Projectile = yujianProj.Projectile;

            //绘制影子拖尾
            Texture2D mainTex = Projectile.GetTexture();
            Rectangle source = mainTex.Frame();
            Vector2 origin = new Vector2(mainTex.Width / 2, mainTex.Height / 2);
            float scale = 1.2f + yujianProj.trailCacheLength * 0.015f;

            for (int i = yujianProj.trailCacheLength - 1; i > 0; i -= 3)
            {
                Color shadowColor = Color.Lerp(yujianProj.color1, yujianProj.color2, (float)i / yujianProj.trailCacheLength);
                int a = 20 + i * 4;
                if (a > 255)
                    a = 255;
                shadowColor.A = (byte)a;
                spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, source, shadowColor, Projectile.oldRot[i], origin, scale - i * 0.015f, SpriteEffects.None, 0f);
            }
        }
    }

    public class DemoniteSpurt : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float maxTime => ref Projectile.ai[0];
        public ref float Width => ref Projectile.ai[1];
        public ref float Alpha => ref Projectile.localAI[1];
        public Vector2 center;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;

            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            center = Projectile.Center;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localNPCHitCooldown = (int)maxTime + 12;
                Projectile.timeLeft = (int)maxTime + 12;
                Projectile.localAI[0] = 1;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            }

            if (Projectile.timeLeft < 6)
                Alpha -= 1 / 6f;
            else if (Projectile.timeLeft < 12)
                Alpha += 1 / 6f;
        }

        public override bool ShouldUpdatePosition()
        {
            return Projectile.timeLeft > 6;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.timeLeft < 12)
                return false;

            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), center, Projectile.Center, Width, ref a);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 12)
                return false;

            Texture2D mainTex = TextureAssets.Extra[98].Value;
            Vector2 scale = new Vector2(Width / mainTex.Width, (Projectile.Center - center).Length() / mainTex.Height);
            Vector2 Center = center - Projectile.velocity - Main.screenPosition;
            Vector2 origin = new Vector2(mainTex.Width / 2, 0);
            float rotation = Projectile.rotation + 3.141f;

            Main.spriteBatch.Draw(mainTex, Center, null, Color.Purple * Alpha, rotation, origin, scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(mainTex, Center, null, Color.MediumPurple * Alpha, rotation, origin, scale * 0.9f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(mainTex, Center, null, Color.Black * 0.8f * Alpha, rotation, origin, scale * 0.6f, SpriteEffects.None, 0);

            return false;
        }

    }

    /// <summary>
    /// 使用速度来存储长度，ai0存储宽度
    /// </summary>
    public class DemoniteStrike : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float Alpha => ref Projectile.localAI[1];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 12;

            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.timeLeft = 12;
                Projectile.localAI[0] = Projectile.velocity.Length() * 2;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            }

            if (Projectile.timeLeft > 6)
                Alpha += 1 / 6f;
            else
                Alpha -= 1 / 6f;

        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - Projectile.velocity, Projectile.Center + Projectile.velocity, Projectile.ai[0], ref a);
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Extra[98].Value;
            Vector2 scale = new Vector2(Projectile.ai[0] / mainTex.Width, Projectile.localAI[0] / mainTex.Height);
            Vector2 Center = Projectile.Center - Main.screenPosition;
            Vector2 origin = mainTex.Size() / 2;
            float rotation = Projectile.rotation + 3.141f;

            Main.spriteBatch.Draw(mainTex, Center, null, Color.Purple * Alpha, rotation, origin, scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(mainTex, Center, null, Color.MediumPurple * Alpha, rotation, origin, scale * 0.9f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(mainTex, Center, null, Color.Black * 0.8f * Alpha, rotation, origin, scale * 0.6f, SpriteEffects.None, 0);

            return false;
        }

    }

    public class YujianAI_DemoniteDoubleSlash : YujianAI_DoubleSlash
    {
        public YujianAI_DemoniteDoubleSlash() : base(90, 65, 40, -2f, 4f, 1.5f, 0.8f, 1f, 1f, Coralite.Instance.NoSmootherInstance) { }

        public override void Reset()
        {
            StartTime = 90;

            SlashTime = 70;
            StartAngle = 2f;

            halfShortAxis = 1f;
            halfLongAxis = 1f;
            smoother = Coralite.Instance.HeavySmootherInstance;
        }

        public override void Init()
        {
            StartTime = 85;

            SlashTime = 35;
            StartAngle = -2f;

            halfShortAxis = 1f;
            halfLongAxis = 1.5f;
            smoother = Coralite.Instance.NoSmootherInstance;
        }


    }

}
