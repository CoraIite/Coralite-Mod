using Coralite.Content.Items.Materials;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.MagikeSystem.Particles;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class QueenOfNight : ModItem
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(76, 2);
            Item.DefaultToRangedWeapon(ProjectileType<QueenOfNightSpilitProj>(), AmmoID.Bullet, 46, 5f, true);

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 8, 0, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = CoraliteSoundID.Shotgun_Item36;

            Item.useTurn = false;
            Item.noUseGraphic = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += new Vector2(0, -4);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, velocity, ProjectileType<QueenOfNightHeldProj>(), 0, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity, ProjectileType<QueenOfNightSpilitProj>(), damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.OnyxBlaster)
                .AddIngredient(ItemID.QuadBarrelShotgun)
                .AddIngredient<FragmentsOfLight>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.HyacinthSeriesItems)]
    public class QueenOfNightHeldProj : BaseGunHeldProj
    {
        public QueenOfNightHeldProj() : base(0.45f, 24, -12, AssetDirectory.HyacinthSeriesItems) { }

        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;
        public static ATex QueenOfNightFire { get; private set; }
        public static ATex QueenOfNightFire2 { get; private set; }

        private int frame2;
        private int frameCounter2;

        private int frameX3;
        private int frame3;
        private int frameCounter3;

        protected override float HeldPositionY => -8;

        public override void InitializeGun()
        {
            base.InitializeGun();
            frameX3 = Main.rand.Next(4);
        }

        public override void ModifyAI(float factor)
        {
            Lighting.AddLight(Projectile.Center, Color.DarkRed.ToVector3() * 0.75f);

            if (++frameCounter2 > 2)
            {
                frameCounter2 = 0;
                if (frame2 < 5)
                    frame2++;
            }

            if (++frameCounter3 > 3)
            {
                frameCounter3 = 0;
                if (frame3 < 6)
                    frame3++;
            }

            if (++Projectile.frameCounter > 1)
            {
                Projectile.frameCounter = 0;
                if (Projectile.frame < 20)
                {
                    Projectile.frame++;
                    if (Projectile.frame ==9)
                    {
                        Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.Zero);
                        for (int i = 0; i < 5; i++)
                            Dust.NewDustPerfect(Projectile.Center, DustType<QueenOfNightPetal>()
                                , -dir.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(1, 4f), Scale: Main.rand.NextFloat(0.6f, 1f));
                    }
                }
            }
        }

        public override float Ease()
        {
            float x = 1.465f * Projectile.timeLeft / MaxTime;
            return x * MathF.Sin(x * x * x) / 1.186f;
        }

        public override void GetFrame(Texture2D mainTex, out Rectangle? frame, out Vector2 origin)
        {
            frame = mainTex.Frame(1, 21, 0, Projectile.frame);
            origin = frame.Value.Size() / 2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dustType"></param>
        /// <param name="baseDir">基础速度角度</param>
        /// <param name="pos"></param>
        /// <param name="speed">粒子速度</param>
        /// <param name="posMover">每次生成的位置移动量</param>
        /// <param name="count">数量</param>
        /// <param name="totalAngle">总角度</param>
        public static void SpawnLeafDustLine(int dustType, Vector2 baseDir, Vector2 pos, float speed, float posMover, int count, float totalAngle)
        {
            for (int i = 0; i < count; i++)
            {
                float scale = 2f - 1f / count * i;
                Dust d = Dust.NewDustPerfect(pos + baseDir * i * posMover, dustType, baseDir.RotateByRandom(-0.2f, 0.2f) * speed, Scale: scale);
                d.noGravity = true;
                baseDir = baseDir.RotatedBy(totalAngle / count);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);

            if (frame2 < 5)
            {
                Texture2D effect = QueenOfNightFire.Value;
                Rectangle frameBox = effect.Frame(1, 5, 0, frame2);

                float rot = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
                float n = rot - DirSign * MathHelper.PiOver2;

                Main.spriteBatch.Draw(effect, Projectile.Center + rot.ToRotationVector2() * 34 - n.ToRotationVector2() * 4 - Main.screenPosition, frameBox, Color.Lerp(lightColor, Color.White, 0.8f)
                    , rot, new Vector2(0, frameBox.Height / 2), Projectile.scale, 0, 0f);
            }

            if (frame3 < 6)
            {
                Texture2D effect = QueenOfNightFire2.Value;
                Rectangle frameBox = effect.Frame(4, 6, frameX3, frame3);

                float rot = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
                float n = rot - DirSign * MathHelper.PiOver2;

                Main.spriteBatch.Draw(effect, Projectile.Center - rot.ToRotationVector2() * 38 - n.ToRotationVector2() * 4 - Main.screenPosition, frameBox, Color.Lerp(lightColor, Color.White, 0.8f)
                    , rot, new Vector2(0, frameBox.Height / 2), Projectile.scale, 0, 0f);
            }
            return false;
        }
    }

    public class QueenOfNightSpilitProj : ModProjectile
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float ExRot => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 4;
            Projectile.localNPCHitCooldown = -1;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.UpdateFrameNormally(8, 9);
            Lighting.AddLight(Projectile.Center, Color.DarkRed.ToVector3() * 0.75f);

            switch (State)
            {
                case 0://飞行
                    {
                        Timer++;
                        if (Timer > 46)
                        {
                            Timer = 0;
                            State = 1;
                            Projectile.tileCollide = false;
                        }

                        for (int i = 0; i < 2; i++)
                        {
                            ExRot += 0.15f;
                            ExRot %= MathHelper.TwoPi;
                            float length = 24;
                            float a = (length * MathF.Sqrt(3)) / 2;
                            float b = length / 2;

                            float r = QueenOfNightSmallProj.CalculateR(ExRot, a, b);
                            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.Zero) * 0.1f;
                            Vector2 velocity = dir.RotatedBy(ExRot) * r;
                            Vector2 center = Projectile.Center + i * Projectile.velocity / 2;
                            Dust d = Dust.NewDustPerfect(center + velocity * 3, DustID.RedTorch, velocity, Scale: Main.rand.NextFloat(1, 2));
                            d.noGravity = true;

                            float exRot2 = ExRot + MathHelper.TwoPi / 3;
                            exRot2 %= MathHelper.TwoPi;
                            velocity = dir.RotatedBy(exRot2) * r;
                            d = Dust.NewDustPerfect(center + velocity * 2, DustID.Granite, velocity, Alpha: 100, Scale: Main.rand.NextFloat(1, 1.5f));
                            d.noGravity = true;

                            exRot2 += MathHelper.TwoPi / 3;
                            exRot2 %= MathHelper.TwoPi;
                            velocity = dir.RotatedBy(exRot2) * r;
                            d = Dust.NewDustPerfect(center + velocity * 3, DustID.CrimsonSpray, velocity, Alpha: 100, Scale: Main.rand.NextFloat(0.8f, 1f));
                            d.noGravity = true;
                        }
                    }
                    break;
                case 1://生成分裂弹幕
                    {
                        //分裂6个
                        float totalAngle = 6 * 0.15f;
                        Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(-totalAngle / 2);
                        int damage = (int)(Projectile.damage * 0.75f);
                        if (Projectile.IsOwnedByLocalPlayer())
                            for (int i = 0; i < 6; i++)
                            {
                                int i2 = i switch//一个比较愚蠢的写法，总之是让中间的弹幕非得更远
                                {
                                    0 or 5 => 4,
                                    1 or 4 => -5,
                                    _ => -10,
                                };
                                Projectile.NewProjectileFromThis<QueenOfNightSmallProj>(Projectile.Center
                                    , dir * 12, damage, Projectile.knockBack / 2, i2);
                                dir = dir.RotatedBy(totalAngle / 6);
                            }

                        Projectile.tileCollide = false;

                        Boom();
                    }
                    break;
                case 2://消失
                    {
                        Timer++;
                        if (Timer > 5)
                            Projectile.Kill();
                    }
                    break;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public void SpilitDust()
        {
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.Zero);

            for (int i = 0; i < 40; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 12)
                     , DustID.RedTorch, Helper.NextVec2Dir() * Main.rand.NextFloat(5f, 8.5f), Scale: Main.rand.NextFloat(1.5f, 2.5f));

                d.noGravity = true;
                d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 12)
                     , DustID.Smoke, Helper.NextVec2Dir() * Main.rand.NextFloat(2, 5), Alpha: 0,Color.Black, Scale: Main.rand.NextFloat(1, 2f));
                d.noGravity = true;
                d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4, 4)
                     , DustID.Clentaminator_Red, Helper.NextVec2Dir() * Main.rand.NextFloat(1, 2), Alpha: 100, Scale: Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = true;
            }

            for (int i = 0; i < 10; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 12)
                     , DustType<QueenOfNightPetal>(), Helper.NextVec2Dir() * Main.rand.NextFloat(1, 2.5f), Scale: Main.rand.NextFloat(0.6f, 1f));

                d.noGravity = true;
            }

            var p2 = PRTLoader.NewParticle<MagikeLozengeParticle>(Projectile.Center, Vector2.Zero, Color.DarkRed, 0.75f);
            p2.Rotation = Projectile.rotation+MathHelper.PiOver2;

            for (int i = 0; i < 8; i++)
            {
                var p = LightTrailParticle_NoPrimitive.Spawn(Projectile.Center, dir.RotateByRandom(-0.8f, 0.8f) * Main.rand.NextFloat(2f, 5f),
                     Color.DarkRed, Main.rand.NextFloat(0.1f, 0.2f));

                p.targetColor = Color.Transparent;//Color.DarkRed with { A = 0 };
                p.noGravity = true;
            }
        }

        public void Boom()
        {
            State = 2;
            Timer = 0;

            SpilitDust();
            Projectile.velocity = Vector2.Zero;
            int size = 165;
            Projectile.Resize(size, size);
            Helper.PlayPitched(CoraliteSoundID.Boom_Item14, Projectile.Center);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (State == 0)
                State = 1;
            Projectile.tileCollide = false;
            Timer = 0;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Boom();
            Projectile.tileCollide = false;
            Timer = 0;

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (State > 0)
                return false;

            Texture2D mainTex = Projectile.GetTexture();

            var frame = mainTex.Frame(1, 9, 0, Projectile.frame);
            Projectile.QuickDraw(frame, (Color.White * 0.8f) with { A = 0 }, 0, 1.2f);
            Projectile.QuickDraw(frame, lightColor, 0);

            return false;
        }
    }

    public class QueenOfNightSmallProj : ModProjectile
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + "QueenOfNightSpilitProj";

        public ref float Timer => ref Projectile.ai[0];
        public ref float RecordDir => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.localNPCHitCooldown = -1;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                RecordDir = Projectile.velocity.ToRotation();
            }

            Lighting.AddLight(Projectile.Center, Color.DarkRed.ToVector3() * 0.75f);
            Projectile.UpdateFrameNormally(5, 9);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;

            Projectile.SpawnTrailDust(6f, DustID.Granite, Main.rand.NextFloat(-0.2f, 0.4f), 100);
            Projectile.SpawnTrailDust(2f, DustID.RedTorch, Main.rand.NextFloat(-0.1f, 0.1f));

            if (Timer == 16)
            {
                //分裂
                if (Projectile.IsOwnedByLocalPlayer())
                {
                    Main.player[Projectile.owner].PickAmmo(ContentSamples.ItemsByType[ItemType<QueenOfNight>()]
                        , out int projType, out float speed, out int damage, out float knockback, out _, true);

                    Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    for (int i = -1; i < 2; i += 2)
                    {
                        Projectile.NewProjectileFromThis(Projectile.Center, dir.RotatedBy(i * 0.1f) * speed * 2, projType
                            , (int)(damage * 0.3f), knockback);
                    }
                }

                Helper.PlayPitched(CoraliteSoundID.LiteBoom_Item118 , Projectile.Center);
                Projectile.Resize(50, 50);
                Projectile.timeLeft = 2;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Timer < 15)
                Timer = 15;
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 dir = RecordDir.ToRotationVector2();
            var p2 = PRTLoader.NewParticle<MagikeLozengeParticle>(Projectile.Center, Vector2.Zero, Color.DarkRed, 0.35f);
            p2.Rotation = Projectile.rotation + MathHelper.PiOver2;

            LozengeDust(Projectile.Center, DustID.Clentaminator_Red, 24, dir * 0.07f, 14, () => 1);
            LozengeDust(Projectile.Center, DustID.Granite, 24, dir * 0.12f, 18, () => Main.rand.NextFloat(1, 1.5f), 150);

            //LozengeDust(Projectile.Center, DustID.RedTorch, 24, dir * 0.2f, 28, () => Main.rand.NextFloat(1, 2f));
        }

        public static void LozengeDust(Vector2 pos, int dustType, int length, Vector2 dir, int count, Func<float> scale, int alpha = 0)
        {
            float a = (length * MathF.Sqrt(3)) / 2;
            float b = length / 2;

            for (int i = 0; i < count; i++)
            {
                float theta = MathHelper.TwoPi * i / count;
                float r = CalculateR(theta, a, b);
                Dust d = Dust.NewDustPerfect(pos, dustType, dir.RotatedBy(theta) * r, Alpha: alpha, Scale: scale());
                d.noGravity = true;
            }
        }

        /// <summary>
        /// 以下内容由AI生成
        /// </summary>
        /// <param name="theta"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="DivideByZeroException"></exception>
        public static float CalculateR(float theta, float a, float b)
        {
            // 计算分母部分
            float denominator = MathF.Abs(MathF.Cos(theta) / a) + MathF.Abs(MathF.Sin(theta) / b);

            // 如果分母为0，返回一个很大的值或者抛出异常，取决于具体需求
            if (denominator == 0)
            {
                throw new DivideByZeroException("Denominator is zero, which is not allowed.");
            }

            // 计算 r 值
            float r = 1 / denominator;

            return r;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.tileCollide = false;
            Projectile.velocity = oldVelocity.SafeNormalize(Vector2.Zero);

            Projectile.Resize(40, 40);
            Projectile.timeLeft = 2;

            Timer = 20;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Timer > 15)
                return false;

            Texture2D mainTex = Projectile.GetTexture();

            var pos = Projectile.Center - Main.screenPosition;
            var frame = mainTex.Frame(1, 9, 0, Projectile.frame);
            Vector2 exScale = new Vector2(1, 0.6f);

            Main.spriteBatch.Draw(mainTex, pos, frame, (Color.White * 0.8f) with { A = 0 }, Projectile.rotation,
                frame.Size() / 2, Projectile.scale * exScale * 1.2f, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, frame, lightColor, Projectile.rotation,
                frame.Size() / 2, Projectile.scale * exScale, 0, 0);

            return false;
        }
    }

    public class QueenOfNightPetal : ModDust
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            dust.frame = new Rectangle(0, Main.rand.Next(15), 1, 16);
            dust.color = Color.White;
        }

        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, new Vector3(0.2f, 0.1f, 0.1f));
            dust.position += dust.velocity;
            dust.rotation += 0.02f;
            dust.velocity *= 0.99f;
            dust.velocity.Y += 0.02f;

            if (dust.fadeIn > 30)
                dust.color *= 0.84f;

            if (dust.fadeIn % 2 == 0)
            {
                dust.frame.Y++;
                if (dust.frame.Y > 15)
                    dust.frame.Y = 0;
            }

            if (!dust.noGravity && dust.velocity.Y < 5)
            {
                dust.velocity.Y += 0.05f;
            }

            dust.fadeIn++;
            if (dust.fadeIn > 45)
                dust.active = false;
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Color c = Lighting.GetColor(dust.position.ToTileCoordinates());
            c *= dust.color.A / 255f;
            Texture2D.Value.QuickCenteredDraw(Main.spriteBatch, dust.frame, dust.position - Main.screenPosition, c, dust.rotation,scale:dust.scale);

            return false;
        }
    }
}
