using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class SandWall : BaseFlyingShieldItem<SandWallGuard>
    {
        public SandWall() : base(Item.sellPrice(0, 1), ItemRarityID.LightRed, AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<SandWallProj>();
            Item.knockBack = 6;
            Item.shootSpeed = 14;
            Item.damage = 50;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AncientBattleArmorMaterial)
                .AddIngredient(ItemID.SandstoneBrick, 20)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class SandWallProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "SandWall";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 46;
        }

        public override void SetOtherValues()
        {
            flyingTime = 15;
            backTime = 4;
            backSpeed = 12;
            trailCachesLength = 6;
            trailWidth = 20 / 2;
        }

        public override Color GetColor(float factor)
        {
            return new Color(255, 206, 146) * factor;
        }
    }

    public class SandWallGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.scale = 1.1f;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.25f;
            distanceAdder = 1.8f;
            StrongGuard = 0.05f;
            scalePercent = 1.8f;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            SoundEngine.PlaySound(CoraliteSoundID.Dig, Projectile.Center);
            Projectile.NewProjectileFromThis<SandWallStorm>(Projectile.Center, Projectile.rotation.ToRotationVector2() * 10, Projectile.damage, Projectile.knockBack);
        }

        public override void DrawSelf(Texture2D mainTex, Vector2 pos, float rotation, Color lightColor, Vector2 scale, SpriteEffects effect)
        {
            Rectangle frameBox;
            Vector2 rotDir = Projectile.rotation.ToRotationVector2();
            Vector2 dir = rotDir * (DistanceToOwner / (Projectile.width * scalePercent));
            Color c = lightColor * 0.6f;
            c.A = lightColor.A;

            frameBox = mainTex.Frame(3, 1, 0, 0);
            Vector2 origin2 = frameBox.Size() / 2;

            //绘制基底
            Main.spriteBatch.Draw(mainTex, pos - dir * 5, frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上部
            frameBox = mainTex.Frame(3, 1, 1, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 3, frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 7, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上上部
            frameBox = mainTex.Frame(3, 1, 2, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 11, frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 15, frameBox, lightColor, rotation, origin2, scale, effect, 0);
        }

        public override float GetWidth()
        {
            return Projectile.width / 2;
        }
    }

    public class SandWallStorm : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        ref float State => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.tileCollide = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage()
        {
            if (State == 0)
                return base.CanDamage();

            return false;
        }

        public override void AI()
        {
            float num1043 = 10f;
            float num1044 = 10f;
            Point point7 = Projectile.Center.ToTileCoordinates();
            Collision.ExpandVertically(point7.X, point7.Y, out var topY, out var bottomY, (int)num1043, (int)num1044);
            topY++;
            bottomY--;
            Vector2 value20 = new Vector2(point7.X, topY) * 16f + new Vector2(8f);
            Vector2 value21 = new Vector2(point7.X, bottomY) * 16f + new Vector2(8f);
            Vector2 vector164 = Vector2.Lerp(value20, value21, 0.5f);
            Vector2 vector165 = new Vector2(0f, value21.Y - value20.Y);
            vector165.X = vector165.Y * 0.2f;
            Projectile.width = (int)(vector165.X * 0.65f);
            Projectile.height = (int)vector165.Y;
            Projectile.Center = vector164;

            Projectile.SpawnTrailDust(DustID.Sand, Main.rand.NextFloat(0.2f, 0.5f), noGravity: false);

            Projectile.ai[0]++;
            if (Projectile.ai[0] > 180)
                Projectile.Kill();

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (State > 0)
                return false;

            State = 1;
            Projectile.ai[0] = 180 - 30f;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (State > 0)
                return;
            Projectile.damage = (int)(Projectile.damage * 0.9f);
            Projectile.ai[2]++;

            if (Projectile.damage < 20 || Projectile.ai[2] > 10)
            {
                Projectile.velocity *= 0;
                State = 1;
                Projectile.ai[0] = 180 - 30f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float num290 = 180f;

            float num291 = 10f;
            float num292 = 10f;
            float num293 = Projectile.ai[0];
            float num294 = MathHelper.Clamp(num293 / 30f, 0f, 1f);
            if (num293 > num290 - 30f)
                num294 = MathHelper.Lerp(1f, 0f, (num293 - (num290 - 30f)) / 30f);

            Point point5 = Projectile.Center.ToTileCoordinates();
            Collision.ExpandVertically(point5.X, point5.Y, out var topY, out var bottomY, (int)num291, (int)num292);
            topY++;
            bottomY--;
            float num295 = 0.2f;
            Vector2 value77 = new Vector2(point5.X, topY) * 16f + new Vector2(8f);
            Vector2 value78 = new Vector2(point5.X, bottomY) * 16f + new Vector2(8f);
            Vector2.Lerp(value77, value78, 0.5f);
            Vector2 vector69 = new Vector2(0f, value78.Y - value77.Y);
            vector69.X = vector69.Y * num295;

            Main.instance.LoadProjectile(656);
            Texture2D value79 = TextureAssets.Projectile[656].Value;
            Rectangle rectangle19 = value79.Frame();
            Vector2 origin19 = rectangle19.Size() / 2f;
            float num296 = -(float)Math.PI / 50f * num293;
            Vector2 spinningpoint4 = Vector2.UnitY.RotatedBy(num293 * 0.1f);
            float num297 = 0f;
            float num298 = 5.1f;
            Color value80 = new Color(212, 192, 100);
            for (float i = (int)value78.Y; i > (int)value77.Y; i -= num298)
            {
                num297 += num298;
                float num300 = num297 / vector69.Y;
                float num301 = num297 * ((float)Math.PI * 2f) / -20f;
                float num302 = num300 - 0.15f;
                Vector2 position19 = spinningpoint4.RotatedBy(num301);
                Vector2 vector70 = new Vector2(0f, num300 + 1f);
                vector70.X = vector70.Y * num295;
                Color color77 = Color.Lerp(Color.Transparent, value80, num300 * 2f);
                if (num300 > 0.5f)
                    color77 = Color.Lerp(Color.Transparent, value80, 2f - num300 * 2f);

                color77.A = (byte)((float)(int)color77.A * 0.5f);
                color77 *= num294;
                position19 *= vector70 * 100f;
                position19.Y = 0f;
                position19.X = 0f;
                position19 += new Vector2(value78.X, i) - Main.screenPosition;
                Main.EntitySpriteDraw(value79, position19, rectangle19, color77, num296 + num301, origin19, 1f + num302, SpriteEffects.None);
            }

            return false;
        }
    }
}
