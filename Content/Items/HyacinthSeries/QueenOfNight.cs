using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
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
            Item.SetWeaponValues(74, 2, 6);
            Item.DefaultToRangedWeapon(ProjectileType<QueenOfNightSpilitProj>(), AmmoID.Bullet, 40, 12.5f, true);

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
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<QueenOfNightHeldProj>(), 0, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity, ProjectileType<QueenOfNightSpilitProj>(), damage, knockback, player.whoAmI);

            Vector2 dir = velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 4; i++)
            {
                QueenOfNightHeldProj.SpawnLeafDustLine(DustID.RedTorch, dir.RotateByRandom(-0.3f, 0.3f), position + dir * 40
                    , Main.rand.NextFloat(3, 4), Main.rand.NextFloat(5, 7), Main.rand.Next(8, 12)
                    , Main.rand.NextFloat(0.2f, 0.4f) * Main.rand.NextFromList(-1, 1));
            }

            return false;
        }

        public override void AddRecipes()
        {
            //CreateRecipe()
            //    .AddIngredient(ItemID.OnyxBlaster)
            //    .AddIngredient<FragmentsOfLight>(2)
            //    .AddTile(TileID.MythrilAnvil)
            //    .Register();
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.HyacinthSeriesItems)]
    public class QueenOfNightHeldProj : BaseGunHeldProj
    {
        public QueenOfNightHeldProj() : base(0.45f, 24, -12, AssetDirectory.HyacinthSeriesItems) { }

        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;
        public static ATex QueenOfNightFire { get; private set; }

        private int frame2;
        private int frameCounter2;

        protected override float HeldPositionY => -8;

        public override void ModifyAI(float factor)
        {
            if (++frameCounter2 > 2)
            {
                frameCounter2 = 0;
                if (frame2 < 5)
                    frame2++;
            }

            if (++Projectile.frameCounter > 1)
            {
                Projectile.frameCounter = 0;
                if (Projectile.frame < 20)
                    Projectile.frame++;
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

            if (frame2 > 5)
                return false;

            Texture2D effect = QueenOfNightFire.Value;
            Rectangle frameBox = effect.Frame(1, 5, 0, frame2);

            float rot = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
            float n = rot - DirSign * MathHelper.PiOver2;

            Main.spriteBatch.Draw(effect, Projectile.Center + rot.ToRotationVector2() * 34 - n.ToRotationVector2() * 4 - Main.screenPosition, frameBox, Color.Lerp(lightColor, Color.White, 0.5f)
                , rot, new Vector2(0, frameBox.Height / 2), Projectile.scale * 0.8f, 0, 0f);
            return false;
        }
    }

    public class QueenOfNightSpilitProj : ModProjectile
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
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
            Projectile.UpdateFrameNormally(3, 9);

            switch (State)
            {
                case 0://飞行
                    {
                        Timer++;
                        if (Timer > 20)
                        {
                            Timer = 0;
                            State = 1;
                            Projectile.tileCollide = false;
                        }

                        for (int i = 0; i < 2; i++)
                        {
                            Projectile.SpawnTrailDust(DustID.Granite, Main.rand.NextFloat(-0.2f, 0.4f), 100);
                            Projectile.SpawnTrailDust(4f, DustID.RedTorch, Main.rand.NextFloat(-0.1f, 0.1f));
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
                                    , dir * 12, damage, Projectile.knockBack / 2,i2);
                                dir = dir.RotatedBy(totalAngle / 6);
                            }
                        SpilitDust();

                        Projectile.tileCollide = false;

                        Boom();
                    }
                    break;
                case 2://消失
                    {
                        Timer++;
                        if (Timer > 4)
                            Projectile.Kill();
                    }
                    break;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public void SpilitDust()
        {
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.Zero);

            for (int i = 0; i < 32; i++)
            {
               Dust d= Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 12)
                    , DustID.RedTorch, Helper.NextVec2Dir() * Main.rand.NextFloat(4, 7), Scale: Main.rand.NextFloat(1, 2));

                d.noGravity = true;
               d= Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 12)
                    , DustID.Granite, Helper.NextVec2Dir() * Main.rand.NextFloat(2, 5), Alpha: 100, Scale: Main.rand.NextFloat(1, 1.5f));
                d.noGravity = true;
            }

            for (int i = 0; i < 8; i++)
            {
                var p = LightTrailParticle_NoPrimitive.Spawn(Projectile.Center, dir.RotateByRandom(-0.6f, 0.6f) * Main.rand.NextFloat(2f, 5f),
                     Color.DarkRed, Main.rand.NextFloat(0.1f, 0.2f));

                p.targetColor = Color.DarkRed with { A=0};
                p.noGravity = true;
            }
        }

        public void Boom()
        {
            State = 2;
            Timer = 0;

            Projectile.velocity = Vector2.Zero;
            int size = 120;
            Projectile.Resize(size, size);
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
            if (State == 0)
                State = 1;
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
            Projectile.QuickDraw(frame, (Color.White * 0.4f) with { A = 0 }, 0, 1.1f);
            Projectile.QuickDraw(frame, lightColor, 0);

            return false;
        }
    }

    public class QueenOfNightSmallProj : ModProjectile
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + "QueenOfNightSpilitProj";

        public ref float Timer => ref Projectile.ai[0];

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
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;

            for (int i = 0; i < 2; i++)
            {
                Projectile.SpawnTrailDust(DustID.Granite, Main.rand.NextFloat(-0.2f, 0.4f), 100);
                Projectile.SpawnTrailDust(4f, DustID.RedTorch, Main.rand.NextFloat(-0.1f, 0.1f));
            }

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
                        Projectile.NewProjectileFromThis(Projectile.Center, dir.RotatedBy(i * 0.2f) * speed, projType
                            , (int)(damage * 0.3f), knockback);
                    }
                }

                Projectile.Resize(50, 50);
                Projectile.timeLeft = 2;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Timer<15)
                Timer = 15;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 12)
                      , DustID.RedTorch, Helper.NextVec2Dir() * Main.rand.NextFloat(2.5f, 5), Scale: Main.rand.NextFloat(1, 2));
                d.noGravity = true;

                d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 12)
                     , DustID.Granite, Helper.NextVec2Dir() * Main.rand.NextFloat(1, 3), Alpha: 100, Scale: Main.rand.NextFloat(1, 1.5f));
                d.noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.tileCollide = false;
            Projectile.velocity =Vector2.Zero;

            Projectile.Resize(40, 40);
            Projectile.timeLeft = 2;

            Timer = 20;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Timer >11)
                return false;

            Texture2D mainTex = Projectile.GetTexture();

            var pos = Projectile.Center - Main.screenPosition;
            var frame = mainTex.Frame(1, 9, 0, Projectile.frame);
            Vector2 exScale = new Vector2(1, 0.6f);

            Main.spriteBatch.Draw(mainTex, pos, frame, (Color.White * 0.4f) with { A = 0 }, Projectile.rotation,
                frame.Size() / 2, Projectile.scale * exScale * 1.1f, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, frame, lightColor, Projectile.rotation,
                frame.Size() / 2, Projectile.scale * exScale, 0, 0);

            return false;
        }
    }
}
