using Coralite.Core;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.Misc_Magic
{
    public class Taurus : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Magic + Name;

        public int shootCount;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(50, 6f);
            Item.DefaultToMagicWeapon(ModContent.ProjectileType<TaurusStar>(), 12, 12, true);
            Item.mana = 5;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(0, 1);
            Item.rare = ItemRarityID.LightRed;
            
            Item.useTurn = false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            shootCount++;
            Helper.PlayPitched(CoraliteSoundID.MagicShoot_Item9, position,pitch:-0.5f);

            if (shootCount > 7)
            {
                shootCount = 0;
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<TaurusMeteor>(), damage, knockback, player.whoAmI);
                return false;
            }

            if (Main.rand.NextBool(3))
            {
                for (int i = -1; i < 2; i+=2)
                    ShootSmallStar(player, source, position, velocity, type, (int)(damage*0.6f), knockback, i);
            }
            else
                ShootSmallStar(player, source, position, velocity, type, damage, knockback, Main.rand.NextFromList(-1, 1));

            //Helper.PlayPitched(CoraliteSoundID.LaserShoot2_Item75, position);
            return false;
        }

        private static void ShootSmallStar(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback, int dir)
        {
            int time = Main.rand.Next(5, 30);
            float Rot = time * 0.003f;

            Projectile.NewProjectile(source, position, velocity.RotatedBy(dir * Rot)*Main.rand.NextFloat(0.9f,1.1f), type, damage, knockback, player.whoAmI, Rot, time, -dir);
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-20, -8);
        }
    }

    /// <summary>
    /// 使用ai0传入转角，ai1传入最大时间，ai2传入初次旋转应该转向哪
    /// </summary>
    public class TaurusStar : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Magic + Name;

        public ref float RotateAngle => ref Projectile.ai[0];
        public ref float MaxTimer => ref Projectile.ai[1];
        public ref float RecordRotDir => ref Projectile.ai[2];
        public ref float Timer => ref Projectile.localAI[0];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 12);
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 12;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = Projectile.MaxUpdates * 85;
        }

        public override void AI()
        {
            if (Projectile.localAI[1] < 1)
            {
                Projectile.localAI[1] += 0.05f;
                if (Projectile.localAI[1] > 1)
                    Projectile.localAI[1] = 1;
            }

            Projectile.rotation += 0.1f;
            Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 0.6f, 0.6f));
            Timer++;
            if (Timer > MaxTimer)//每隔一段时间转动一下
            {
                Timer = 0;
                Projectile.velocity = Projectile.velocity.RotatedBy(RotateAngle * 2 * RecordRotDir);
                RecordRotDir *= -1;

                float range = Main.rand.NextFloat(0, 1);
                for (int i = 0; i < 4; i++)
                {
                    Vector2 dir = (i * MathHelper.PiOver2).ToRotationVector2();
                    for (int j = 0; j < 3; j++)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Clentaminator_Cyan, dir * (0.5f + (j * (0.75f + (0.25f * range))))
                            , Scale: 0.6f + (range * 0.2f) - (j * 0.1f));
                        d.noGravity = true;
                    }
                }
            }

            if (Main.rand.NextBool(3))
                Projectile.SpawnTrailDust(DustID.Clentaminator_Cyan, Main.rand.NextFloat(0.05f, 0.4f), Scale: 0.5f);
        }

        public override void OnKill(int timeLeft)
        {
            Helper.PlayPitched(CoraliteSoundID.Hit_Item10, Projectile.Center);
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir2 = (i * MathHelper.PiOver2).ToRotationVector2();
                for (int j = 0; j < 5; j++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Clentaminator_Cyan, dir2 * (1 + (j * 0.8f)), Scale: 1.3f - (j * 0.12f));
                    d.noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor *= Projectile.localAI[1];
            Projectile.DrawShadowTrailsSacleStep(lightColor, 0.5f, 0.5f / 12, 0, 12, 1, 0.8f / 12, null, 0);

            Projectile.QuickDraw(lightColor, 0);
            return false;
        }
    }

    public class TaurusMeteor : BaseHeldProj
    {
        public override string Texture => AssetDirectory.Misc_Magic + Name;

        public int trailCachesLength = 20;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.timeLeft = 160;
            Projectile.penetrate = 5;
            Projectile.extraUpdates = 2;
        }

        public override void Initialize()
        {
            Projectile.oldPos = new Vector2[trailCachesLength];
            Projectile.oldRot = new float[trailCachesLength];

            for (int i = 0; i < trailCachesLength; i++)
            {
                Projectile.oldPos[i] = Projectile.Center;
                Projectile.oldRot[i] = Projectile.rotation;
            }
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, Coralite.IcicleCyan.ToVector3());

            Projectile.UpdateOldPosCache(addVelocity: true);
            Projectile.UpdateOldRotCache();

            Projectile.SpawnTrailDust(8f, DustID.Clentaminator_Cyan, -Main.rand.NextFloat(0.1f, 0.4f), Scale: Main.rand.NextFloat(0.6f, 0.8f));
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.noGravity)
                target.velocity.Y -= target.knockBackResist * 10;

            Vector2 dir = Projectile.rotation.ToRotationVector2();
            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, dir.RotateByRandom(-0.4f, 0.4f) * Main.rand.NextFloat(2f, 6f)
                    , Scale: Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = false;
            }

            for (int i = 0; i < 6; i++)
            {
                Vector2 offset = dir.RotateByRandom(-0.5f, 0.5f);
                Dust d = Dust.NewDustPerfect(Projectile.Center + (offset * Main.rand.Next(16, 32)), DustID.GoldFlame, offset * Main.rand.NextFloat(2f, 4f)
                    , Scale: Main.rand.NextFloat(2.5f, 3f));
                d.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = (i * MathHelper.PiOver2).ToRotationVector2();
                for (int j = 0; j < 6; j++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Clentaminator_Cyan, dir * (1 + (j * 0.8f)), Scale: 1.6f - (j * 0.15f));
                    d.noGravity = true;
                }
            }

            SoundEngine.PlaySound(CoraliteSoundID.Hit_Item10, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrails();
            Texture2D mainTex = Projectile.GetTexture();

            var pos = Projectile.Center - Main.screenPosition;
            var origin = mainTex.Size() / 2;

            Main.spriteBatch.Draw(mainTex, pos, null, lightColor, Projectile.rotation, origin
                , Projectile.scale, Projectile.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
            return false;
        }

        public virtual void DrawTrails()
        {
            Texture2D Texture = CoraliteAssets.Trail.CircleA.Value;

            List<ColoredVertex> bars = new();

            for (int i = 0; i < trailCachesLength; i++)
            {
                float factor = (float)i / trailCachesLength;
                Vector2 Center = Projectile.oldPos[i];
                Vector2 normal = (Projectile.oldRot[i] + MathHelper.PiOver2).ToRotationVector2();
                Vector2 Top = Center - Main.screenPosition + (normal * 5 * factor);
                Vector2 Bottom = Center - Main.screenPosition - (normal * 5 * factor);

                var Color = new Color(20, 255, 199, 0) * factor;
                bars.Add(new(Top, Color, new Vector3(factor, 0, 1)));
                bars.Add(new(Bottom, Color, new Vector3(factor, 1, 1)));
            }

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            //Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
        }
    }
}
