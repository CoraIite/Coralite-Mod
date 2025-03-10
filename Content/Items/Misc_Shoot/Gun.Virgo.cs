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
            Item.SetWeaponValues(50, 6f);
            Item.DefaultToRangedWeapon(ProjectileType<VirgoHeldProj>(), AmmoID.Bullet, 28, 12.5f);

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 1);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = CoraliteSoundID.Bow2_Item102;

            Item.useTurn = false;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += new Vector2(0, -8);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<VirgoHeldProj>(), damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity, ProjectileType<VirgoFeather>(), damage, knockback, player.whoAmI);
            return false;
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
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0] < 28 && (Projectile.ai[0] % 4 == 0))
            {
                float range = Main.rand.NextFloat(0, 1);

                for (int i = 0; i < 4; i++)
                {
                    Vector2 dir = (i * MathHelper.PiOver2).ToRotationVector2();
                    for (int j = 0; j < 3; j++)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Clentaminator_Cyan, dir * (1 + (j * (0.55f + (0.25f * range))))
                            , Scale: 0.8f + (range * 0.4f) - (j * 0.15f));
                        d.noGravity = true;
                    }
                }

                Projectile.NewProjectileFromThis<VirgoBullet>(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero) * (2.5f+Projectile.ai[0] / 4 * (2.5f / 7)),
                    (int)(Projectile.damage * 0.36f), Projectile.knockBack / 5);
            }

            Projectile.ai[0]++;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir2 = (i * MathHelper.PiOver2).ToRotationVector2();
                for (int j = 0; j < 6; j++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Clentaminator_Cyan, dir2 * (1 + (j * 0.8f)), Scale: 1.6f - (j * 0.15f));
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
            Projectile.ignoreWater = true;
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
