using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class SunflowerGun : ModItem
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(19, 4);
            Item.DefaultToRangedWeapon(ProjectileType<SunflowerGunBullet>(), AmmoID.Bullet, 26, 10f, true);

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 0, 30, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = CoraliteSoundID.NoUse_BlowgunPlus_Item65;

            Item.useTurn = false;
            Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<SunflowerGunHeldProj>(), damage, knockback, player.whoAmI);
            Vector2 targetDir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero);
            Projectile.NewProjectile(source, player.Center, targetDir * Main.rand.NextFloat(6f, 8f), ProjectileType<SunflowerGunBullet>(), (int)(damage * (5 / 12f)), knockback / 2, player.whoAmI);

            for (int i = 0; i < 2; i++)
                Projectile.NewProjectile(source, player.Center, targetDir.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(6f, 8f), ProjectileType<SunflowerGunBullet>(), (int)(damage * (5 / 12f)), knockback / 2, player.whoAmI);

            Helper.PlayPitched(CoraliteSoundID.Gun2_Item40, player.Center);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Sunflower)
            .AddIngredient(ItemID.JungleSpores, 10)
            .AddIngredient(ItemID.Vine, 5)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }

    [VaultLoaden(AssetDirectory.HyacinthSeriesItems)]
    public class SunflowerGunHeldProj : BaseGunHeldProj
    {
        public SunflowerGunHeldProj() : base(0.6f, 20, -8, AssetDirectory.HyacinthSeriesItems) { }

        public static ATex SunflowerGunFire { get; private set; }

        public override void ModifyAI(float factor)
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.6f, 0.6f, 0.1f));
            if (Projectile.timeLeft != MaxTime && Projectile.timeLeft % 2 == 0)
            {
                Projectile.frame++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);

            if (Projectile.frame > 3)
                return false;

            Texture2D effect = SunflowerGunFire.Value;
            Rectangle frameBox = effect.Frame(1, 4, 0, Projectile.frame);
            //SpriteEffects effects = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float rot = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
            float n = rot - DirSign * MathHelper.PiOver2;

            Main.spriteBatch.Draw(effect, Projectile.Center + rot.ToRotationVector2() * 18 /*+ n.ToRotationVector2() * 4*/ - Main.screenPosition, frameBox, Color.Lerp(lightColor, Color.White, 0.5f)
                , rot, new Vector2(0, frameBox.Height / 2), Projectile.scale, 0, 0f);
            return false;
        }
    }

    public class SunflowerGunBullet : ModProjectile
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 100;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = 1;
                Projectile.timeLeft = (int)(Projectile.knockBack * 20);
                Projectile.rotation = Main.rand.NextFloat(6.282f);
                Projectile.netUpdate = true;
            }

            if (Projectile.timeLeft < 30)
            {
                Projectile.velocity *= 0.97f;
                Projectile.ai[1] = Projectile.timeLeft / 30f;
                Lighting.AddLight(Projectile.Center, new Vector3(0.8f, 0.8f, 0) * Projectile.ai[1]);
            }
            else if (Projectile.timeLeft % 3 == 0)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8)
                    , DustType<SunflowerDust>(), -Projectile.velocity * 0.2f, Scale: 1.2f);
                dust.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_Dusts && !VaultUtils.isServer)
            {
                int type = DustType<SunflowerSeed>();
                for (int i = 0; i < 3; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8)
                        , type, Projectile.velocity.RotatedBy((i - 1) * 0.2f).SafeNormalize(Vector2.Zero) * 2);
                    dust.noGravity = true;
                    dust.frame = new Rectangle(0, i, 1, 3);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 90);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Color drawColor = lightColor;
            //if (Projectile.timeLeft < 30)
            //    drawColor = new Color(200, 200, 200, 200) * Projectile.ai[1];

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, new Vector2(4, 4), 1.4f, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class SunflowerDust() : BaseVanillaDust(DustID.JungleGrass, AssetDirectory.HyacinthSeriesItems)
    {

    }

    public class SunflowerSeed : ModDust
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            //dust.frame = new Rectangle(0, Main.rand.Next(3), 1, 3);
            dust.color = Color.White;
        }

        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, new Vector3(0.15f, 0.15f, 0));
            dust.position += dust.velocity;
            dust.rotation += 0.1f;
            dust.velocity *= 0.99f;
            dust.velocity.Y += 0.02f;

            if (dust.fadeIn > 30)
                dust.color *= 0.84f;

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
            Texture2D.Value.QuickCenteredDraw(Main.spriteBatch, dust.frame, dust.position - Main.screenPosition, c, dust.rotation, scale: dust.scale);

            return false;
        }
    }
}
