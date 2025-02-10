using Coralite.Content.Buffs.Debuffs;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class SnowdropHeldProj : BaseGunHeldProj
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public SnowdropHeldProj() : base(0.8f, 24, -10, AssetDirectory.HyacinthSeriesItems) { }

        public override float Ease()
        {
            float x = 1.465f * Projectile.timeLeft / MaxTime;
            return x * MathF.Sin(x * x * x) / 1.186f;
        }

        public override void InitializeGun()
        {
            float rotation = TargetRot + (DirSign > 0 ? 0 : MathHelper.Pi);
            Vector2 dir = rotation.ToRotationVector2();
            Vector2 center = Projectile.Center + (dir * 32);
            for (int i = 0; i < 16; i++)
            {
                Dust dust = Dust.NewDustPerfect(center + Main.rand.NextVector2Circular(8, 8), DustID.Snow, dir.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)) * Main.rand.NextFloat(4f, 8f), Scale: Main.rand.NextFloat(0.8f, 1.2f));
                dust.noGravity = true;
            }
        }
    }

    /// <summary>
    /// ai0用于控制存活时间小于多少时产生粒子
    /// </summary>
    public class SnowBullet : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles_Shoot + Name;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 30;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 1;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void AI()
        {
            if (Projectile.timeLeft < Projectile.ai[0])
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(6, 6), DustID.Snow, Projectile.velocity, Scale: Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 0.3f, 0.3f));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SnowDebuff>(), 30);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<SnowDebuff>(), 30);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Snow, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.15f, 0.25f), Scale: Main.rand.NextFloat(1.4f, 1.6f));
                dust.noGravity = true;
            }

            return true;
        }

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 4; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Snow, Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.6f, 0.8f), Scale: Main.rand.NextFloat(1.4f, 1.6f));
                    dust.noGravity = true;
                }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, mainTex.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class SnowdropBloom : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles_Shoot + Name;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 70;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.netImportant = true;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            int timer = 70 - Projectile.timeLeft;
            float factor = timer * 0.157f;   //魔法数字
            Projectile.rotation = MathF.Sin(factor) * 0.4f;

            if (timer == 6 || timer == 26 || timer == 46)
            {
                if (Projectile.IsOwnedByLocalPlayer() && Helper.FindClosestEnemy(Projectile.Center, 600, npc => npc.active && !npc.friendly && npc.CanBeChasedBy()) is not null)
                {
                    Vector2 center = Projectile.Top + ((Projectile.rotation + 1.57f).ToRotationVector2() * 40);
                    Vector2 dir = (center - Projectile.Top).SafeNormalize(Vector2.Zero);

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), center, dir * 7, ModContent.ProjectileType<SnowSpirit>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner);
                    for (int i = 0; i < 2; i++)
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SnowdropPetal>(), dir.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(1f, 2f));
                }
            }

            Projectile.velocity *= 0.99f;

            Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 0.6f, 0.3f));
            if (timer < 10)
                Projectile.ai[0] += 0.07f;
            else if (timer > 60)
                Projectile.ai[0] -= 0.07f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;
            Vector2 origin = new(28, 0);

            Main.spriteBatch.Draw(mainTex, center, mainTex.Frame(1, 2, 0, 0), lightColor * Projectile.ai[0], Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(mainTex, center, mainTex.Frame(1, 2, 0, 1), Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class SnowdropBud : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles_Shoot + Name;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 1;
            Projectile.friendly = true;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SnowdropPetal>(), -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.15f, 0.2f));
        }

        public override void AI()
        {
            Projectile.UpdateFrameNormally(6, 6);

            Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 0.6f, 0.3f));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.HasBuff<SnowDebuff>())
            {
                target.DelBuff(target.FindBuffIndex(ModContent.BuffType<SnowDebuff>()));
                if (Projectile.IsOwnedByLocalPlayer())
                {
                    float vel = Main.rand.NextFloat(2.5f, 3.5f);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(Main.rand.Next(-16, 16), Main.rand.Next(0, 16)), Vector2.UnitY * vel, ModContent.ProjectileType<SnowdropBloom>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack, Projectile.owner);
                }
                if (VisualEffectSystem.HitEffect_Dusts)
                    for (int j = 0; j < 6; j++)
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SnowdropPetal>(), Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-1f, 1f)) * Main.rand.NextFloat(0.5f, 1.5f));
            }
            else
                SpawnSnowDust();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SpawnSnowDust();
            return true;
        }

        public void SpawnSnowDust()
        {
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 4; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Snow, Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)) * Main.rand.NextFloat(0.15f, 0.25f), Scale: Main.rand.NextFloat(1.4f, 1.6f));
                    dust.noGravity = true;
                }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, mainTex.Frame(1, 7, 0, Projectile.frame), lightColor, Projectile.rotation, new Vector2(28, 13), Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class SnowSpirit : ModProjectile, IDrawPrimitive
    {
        BasicEffect effect;
        private Trail trail;

        public SnowSpirit()
        {
            if (Main.dedServ)
            {
                return;
            }

            Main.QueueMainThreadAction(() =>
            {
                effect = new BasicEffect(Main.instance.GraphicsDevice);
                effect.VertexColorEnabled = true;
            });
        }

        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.timeLeft = 180;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.oldPos = new Vector2[24];
            for (int i = 0; i < 24; i++)
                Projectile.oldPos[i] = Projectile.Center;
        }

        public override void AI()
        {
            //抄自叶绿弹，看不太懂它在写一些什么玩意
            float velLength = Projectile.velocity.Length();
            float num185 = Projectile.localAI[0];
            if (num185 == 0f)
            {
                Projectile.localAI[0] = velLength;
                num185 = velLength;
            }

            float num186 = Projectile.position.X;
            float num187 = Projectile.position.Y;
            float chasingLength = 900f;
            bool flag5 = false;
            int targetIndex = 0;
            if (Projectile.ai[1] == 0f)
            {
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(Projectile) && (Projectile.ai[1] == 0f || Projectile.ai[1] == i + 1))
                    {
                        float targetX = Main.npc[i].Center.X;
                        float targetY = Main.npc[i].Center.Y;
                        float num193 = Math.Abs(Projectile.Center.X - targetX) + Math.Abs(Projectile.Center.Y - targetY);
                        if (num193 < chasingLength)
                        {
                            chasingLength = num193;
                            num186 = targetX;
                            num187 = targetY;
                            flag5 = true;
                            targetIndex = i;
                        }
                    }
                }

                if (flag5)
                    Projectile.ai[1] = targetIndex + 1;

                flag5 = false;
            }

            if (Projectile.ai[1] > 0f)
            {
                int targetIndex2 = (int)(Projectile.ai[1] - 1f);
                if (Main.npc[targetIndex2].active && Main.npc[targetIndex2].CanBeChasedBy(this, ignoreDontTakeDamage: true) && !Main.npc[targetIndex2].dontTakeDamage)
                {
                    float num195 = Main.npc[targetIndex2].Center.X;
                    float num196 = Main.npc[targetIndex2].Center.Y;
                    if (Math.Abs(Projectile.Center.X - num195) + Math.Abs(Projectile.Center.Y - num196) < 1000f)
                    {
                        flag5 = true;
                        num186 = Main.npc[targetIndex2].Center.X;
                        num187 = Main.npc[targetIndex2].Center.Y;
                    }
                }
                else
                    Projectile.ai[1] = 0f;
            }

            if (flag5)
            {
                float num197 = num185;
                Vector2 center = Projectile.Center;
                float num198 = num186 - center.X;
                float num199 = num187 - center.Y;
                float dis2Target = MathF.Sqrt((num198 * num198) + (num199 * num199));
                dis2Target = num197 / dis2Target;
                num198 *= dis2Target;
                num199 *= dis2Target;
                int chase = 24;

                Projectile.velocity.X = ((Projectile.velocity.X * (chase - 1)) + num198) / chase;
                Projectile.velocity.Y = ((Projectile.velocity.Y * (chase - 1)) + num199) / chase;
            }


            trail ??= new Trail(Main.instance.GraphicsDevice, 24, new ArrowheadTrailGenerator(4), factor => Helper.Lerp(0, 2, factor), factor =>
            {
                if (factor.X > 0.7f)
                    return Color.Lerp(new Color(152, 192, 70, 60), Color.White, (factor.X - 0.7f) / 0.3f);

                return Color.Lerp(new Color(0, 0, 0, 0), new Color(152, 192, 70, 60), factor.X / 0.7f);
            });

            for (int i = 0; i < 23; i++)
                Projectile.oldPos[i] = Projectile.oldPos[i + 1];

            Projectile.oldPos[23] = Projectile.Center + Projectile.velocity;
            trail.TrailPositions = Projectile.oldPos;
        }

        public void DrawPrimitives()
        {
            if (effect == null)
                return;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            trail?.DrawTrail(effect);
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
