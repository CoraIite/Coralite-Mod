using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Misc_Magic
{
    public class MagicalAshes : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Magic + Name;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("蕴魔灰烬");

            // Tooltip.SetDefault("它尝起来是甜的");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 8;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.reuseDelay = 25;
            Item.mana = 18;
            Item.knockBack = 6;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.sellPrice(0, 0, 1, 0);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ProjectileType<MagicalAshesProj>();

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = -1; i < 2; i++)
            {
                Projectile.NewProjectile(source, player.Center + player.direction * Vector2.UnitX * 16f, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX).RotatedBy(i * MathHelper.PiOver4 / 2) * 8f, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BambooBlock, 30)
                .Register();
        }
    }

    public class MagicalAshesProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles_Magic + Name;

        internal ref float Stoped => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("蕴魔灰烬");

            Main.projFrames[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 24;
            Projectile.alpha = 0;
            Projectile.scale = 1f;

            Projectile.damage = 20;
            Projectile.penetrate = 3;
            Projectile.aiStyle = -1;

            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.timeLeft = 300;
        }

        public override bool PreAI()
        {
            if (Projectile.timeLeft > 60 && Projectile.alpha < 200)
                Projectile.alpha += 2;

            if (Projectile.timeLeft % 5 == 0)
            {
                switch (Projectile.frame)
                {
                    case 0:
                        Projectile.frame = 1;
                        break;
                    case 1:
                        Projectile.frame = 0;
                        break;
                    default:
                        goto case 0;
                }
            }

            return true;
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server && Projectile.timeLeft % 5 == 0)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32), 261, -Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(1f) * 5f, 0, default, Main.rand.Next(1, 3));
                dust.noGravity = true;
            }

            Projectile.velocity *= 0.95f;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            if (Projectile.velocity.Length() < 0.05f && Stoped == 0)
            {
                Stoped++;
                Projectile.timeLeft = 60;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity *= 0.8f;
            Projectile.alpha += 20;
            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32), 261, -Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(1f), 0, default, Main.rand.Next(1, 3));
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Stoped++;
            Projectile.velocity *= 0.1f;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            Projectile.timeLeft = 15;
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24), 261, Main.rand.NextVector2Circular(3, 3), 100, default, Main.rand.Next(1, 3));
            }
        }
    }
}
