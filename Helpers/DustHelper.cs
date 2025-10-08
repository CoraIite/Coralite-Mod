using Coralite.Content.Particles;
using Coralite.Core;
using InnoVault.PRT;
using System;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Helpers
{
    public static partial class Helper
    {
        public static void RedJadeExplosion(Vector2 center, bool canMakeSound = true)
        {
            if (VaultUtils.isServer)
                return;

            if (canMakeSound)
                PlayPitched("RedJade/RedJadeBoom", 0.4f, 0f, center);

            Color red = new(221, 50, 50);
            int type = CoraliteContent.ParticleType<LightBall>();

            for (int i = 0; i < 2; i++)
            {
                PRTLoader.NewParticle(center, NextVec2Dir() * Main.rand.NextFloat(16, 18), type, red, Main.rand.NextFloat(0.1f, 0.15f));
            }
            for (int i = 0; i < 5; i++)
            {
                PRTLoader.NewParticle(center, NextVec2Dir() * Main.rand.NextFloat(10, 14), type, red, Main.rand.NextFloat(0.1f, 0.15f));
                PRTLoader.NewParticle(center, NextVec2Dir() * Main.rand.NextFloat(10, 14), type, Color.White, Main.rand.NextFloat(0.05f, 0.1f));
                Dust dust = Dust.NewDustPerfect(center, DustID.GemRuby, NextVec2Dir() * Main.rand.NextFloat(4, 8), Scale: Main.rand.NextFloat(1.6f, 1.8f));
                dust.noGravity = true;
            }

            Content.Items.RedJades.RedExplosionParticle.Spawn(center, 0.4f, Coralite.RedJadeRed);
            Content.Items.RedJades.RedGlowParticle.Spawn(center, 0.35f, Coralite.RedJadeRed, 0.2f);
            Content.Items.RedJades.RedGlowParticle.Spawn(center, 0.35f, Coralite.RedJadeRed, 0.2f);

        }

        public static void RedJadeBigBoom(Vector2 center, bool canMakeSound = true)
        {
            if (VaultUtils.isServer)
                return;

            if (canMakeSound)
                PlayPitched("RedJade/RedJadeBoom", 0.8f, -1f, center);

            Color red = new(221, 50, 50);
            int type = CoraliteContent.ParticleType<LightBall>();

            for (int i = 0; i < 4; i++)
            {
                PRTLoader.NewParticle(center, NextVec2Dir() * Main.rand.NextFloat(32, 34), type, red, Main.rand.NextFloat(0.15f, 0.2f));
            }
            for (int i = 0; i < 10; i++)
            {
                PRTLoader.NewParticle(center, NextVec2Dir() * Main.rand.NextFloat(18, 26), type, red, Main.rand.NextFloat(0.1f, 0.15f));
                PRTLoader.NewParticle(center, NextVec2Dir() * Main.rand.NextFloat(18, 26), type, Color.White, Main.rand.NextFloat(0.05f, 0.1f));
                Dust dust = Dust.NewDustPerfect(center, DustID.GemRuby, NextVec2Dir() * Main.rand.NextFloat(6, 10), Scale: Main.rand.NextFloat(2f, 2.4f));
                dust.noGravity = true;
            }

            Content.Items.RedJades.RedExplosionParticle.Spawn(center, 0.9f, Coralite.RedJadeRed);
            Content.Items.RedJades.RedGlowParticle.Spawn(center, 0.8f, Coralite.RedJadeRed, 0.4f);
            Content.Items.RedJades.RedGlowParticle.Spawn(center, 0.8f, Coralite.RedJadeRed, 0.4f);

            var modifier = new PunchCameraModifier(center, NextVec2Dir(), 6, 4f, 14, 1000f);
            Main.instance.CameraModifiers.Add(modifier);
        }

        /// <summary>
        /// 生成轨迹粒子，可以自由控制位置以及速度
        /// </summary>
        /// <param name="center"></param>
        /// <param name="type"></param>
        /// <param name="velocity"></param>
        /// <param name="Alpha"></param>
        /// <param name="newColor"></param>
        /// <param name="Scale"></param>
        /// <param name="noGravity"></param>
        public static void SpawnTrailDust(Vector2 center, int type, Func<Dust, Vector2> velocity, int Alpha = 0, Color newColor = default, float Scale = 1f, bool noGravity = true)
        {
            if (VaultUtils.isServer)
                return;

            Dust dust = Dust.NewDustPerfect(center, type, Alpha: Alpha, newColor: newColor, Scale: Scale);
            dust.noGravity = noGravity;
            dust.velocity = velocity.Invoke(dust);
        }

        /// <summary>
        /// 在弹幕身上生成粒子，默认为与弹幕相反的速度，如果需要调整速度请将<paramref name="velocityMult"/>设置为负数
        /// </summary>
        /// <param name="Projectile">弹幕自身</param>
        /// <param name="type">弹幕种类</param>
        /// <param name="velocityMult">速度系数</param>
        /// <param name="Alpha"></param>
        /// <param name="newColor"></param>
        /// <param name="Scale"></param>
        /// <param name="noGravity">粒子重力</param>
        public static void SpawnTrailDust(this Projectile Projectile, int type, float velocityMult, int Alpha = 0, Color newColor = default, float Scale = 1f, bool noGravity = true)
        {
            if (VaultUtils.isServer)
                return;

            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, type, Alpha: Alpha, newColor: newColor, Scale: Scale);
            dust.noGravity = noGravity;
            dust.velocity = -Projectile.velocity * velocityMult;
        }

        public static void SpawnTrailDust(this Projectile Projectile, int type, float velocityMult, float extraRot, int Alpha = 0, Color newColor = default, float Scale = 1f, bool noGravity = true)
        {
            if (VaultUtils.isServer)
                return;

            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, type, Alpha: Alpha, newColor: newColor, Scale: Scale);
            dust.noGravity = noGravity;
            dust.velocity = -Projectile.velocity.RotatedBy(extraRot) * velocityMult;
        }

        public static void SpawnTrailDust(this Projectile Projectile, float width, int type, float velocityMult, int Alpha = 0, Color newColor = default, float Scale = 1f, bool noGravity = true)
        {
            if (VaultUtils.isServer)
                return;

            Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(width, width), type, Vector2.Zero, Alpha: Alpha, newColor: newColor, Scale: Scale);
            dust.noGravity = noGravity;
            dust.velocity = -Projectile.velocity * velocityMult;
        }

        public static void SpawnTrailDust(Vector2 pos, Vector2 velocity, float width, int type, float velocityMult, int Alpha = 0, Color newColor = default, float Scale = 1f, bool noGravity = true)
        {
            if (VaultUtils.isServer)
                return;

            Dust dust = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(width, width), type, Vector2.Zero, Alpha: Alpha, newColor: newColor, Scale: Scale);
            dust.noGravity = noGravity;
            dust.velocity = -velocity * velocityMult;
        }


        /// <summary>
        /// 生成随机的粒子流，随机方向
        /// </summary>
        /// <param name="center"></param>
        /// <param name="jetCount">粒子流数量</param>
        /// <param name="howManyPerJet">每个粒子流的粒子数</param>
        /// <param name="speed"></param>
        /// <param name="type"></param>
        /// <param name="Alpha"></param>
        /// <param name="newColor"></param>
        /// <param name="Scale"></param>
        /// <param name="noGravity"></param>
        public static void SpawnRandomDustJet(Vector2 center, int jetCount, int howManyPerJet, Func<int, float> speed, int type, int Alpha = 0, Color newColor = default, float Scale = 1f, bool noGravity = true)
        {
            if (VaultUtils.isServer)
                return;

            for (int i = 0; i < jetCount; i++)
            {
                Vector2 dir = NextVec2Dir();
                for (int j = 0; j < howManyPerJet; j++)
                {
                    Dust dust = Dust.NewDustPerfect(center, type, dir.RotatedBy(Main.rand.NextFloat(-0.05f, 0.05f)) * speed.Invoke(j),
                        Alpha, newColor, Scale: Scale);
                    dust.noGravity = noGravity;
                }
            }
        }

        /// <summary>
        /// 生成粒子流，可控方向
        /// </summary>
        /// <param name="center"></param>
        /// <param name="direction">方向委托</param>
        /// <param name="jetCount">粒子流数量</param>
        /// <param name="howManyPerJet">每个粒子流的粒子数</param>
        /// <param name="speed"></param>
        /// <param name="type"></param>
        /// <param name="Alpha"></param>
        /// <param name="newColor"></param>
        /// <param name="Scale"></param>
        /// <param name="noGravity"></param>
        public static void SpawnDirDustJet(Vector2 center, Func<Vector2> direction, int jetCount, int howManyPerJet, Func<int, float> speed, int type, int Alpha = 0, Color newColor = default, float Scale = 1f, bool noGravity = true, float extraRandRot = 0.05f)
        {
            if (VaultUtils.isServer)
                return;

            for (int i = 0; i < jetCount; i++)
            {
                Vector2 dir = direction.Invoke();
                for (int j = 0; j < howManyPerJet; j++)
                {
                    Dust dust = Dust.NewDustPerfect(center, type, dir.RotatedBy(Main.rand.NextFloat(-extraRandRot, extraRandRot)) * speed.Invoke(j),
                        Alpha, newColor, Scale: Scale);
                    dust.noGravity = noGravity;
                }
            }
        }
    }
}
