using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Misc_Shoot
{
    public class Virgo : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.knockBack = 6;
            Item.shootSpeed = 12;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ProjectileType<VirgoHeldProj>();
            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = CoraliteSoundID.Bow2_Item102;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += new Vector2(0, -8);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<VirgoHeldProj>(), damage, knockback, player.whoAmI);
                if (type == ProjectileID.Bullet)
                {
                    Projectile.NewProjectile(source, position, velocity, ProjectileType<VirgoFeather>(), damage, knockback, player.whoAmI);
                    return false;
                }
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TitaniumBar, 12)
                .AddIngredient(ItemID.SoulofFlight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.AdamantiteBar, 12)
                .AddIngredient(ItemID.SoulofFlight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class VirgoHeldProj : BaseGunHeldProj
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;

        public VirgoHeldProj() : base(0.25f, 16, -8, AssetDirectory.Misc_Shoot) { }

        public override void ModifyAI(float factor)
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.2f, 0.5f));
        }
    }

    public class VirgoFeather : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 80 && (Projectile.ai[0] % 10 == 0))
            {
                float range = Main.rand.NextFloat(0, 1);

                for (int i = 0; i < 4; i++)
                {
                    Vector2 dir = (i * MathHelper.PiOver2).ToRotationVector2();
                    for (int j = 0; j < 3; j++)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Clentaminator_Cyan, dir * (1 + j * (0.55f + 0.25f * range))
                            , Scale: 0.8f + range * 0.4f - j * 0.15f);
                        d.noGravity = true;
                    }
                }

                Projectile.NewProjectileFromThis<VirgoBullet>(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero) * 4.5f,
                    (int)(Projectile.damage * 0.4f), Projectile.knockBack / 5);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir2 = (i * MathHelper.PiOver2).ToRotationVector2();
                for (int j = 0; j < 6; j++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Clentaminator_Cyan, dir2 * (1 + j * 0.8f), Scale: 1.6f - j * 0.15f);
                    d.noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color c = Color.Cyan;
            c.A = 50;
            Projectile.DrawShadowTrails(c, 0.7f, 0.7f / 8, 0, 8, 1, 0, Projectile.scale * 0.8f);
            Projectile.QuickDraw(lightColor, 0);
            return false;
        }
    }

    public class VirgoBullet : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = Projectile.height = 8;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.extraUpdates = 2;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Clentaminator_Cyan, Helper.NextVec2Dir(0.5f, 1.5f)
                    , Scale: Main.rand.NextFloat(0.8f, 1f));
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}
