using Coralite.Core;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.ShadowCastle
{
    public class ShadowDart : ModItem
    {
        public override string Texture => AssetDirectory.ShadowCastleItems + Name;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.WaterBolt;
            ItemID.Sets.ShimmerTransformToItem[ItemID.WaterBolt] = Type;
        }

        public override void SetDefaults()
        {
            Item.damage = 28;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.mana = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 4;
            Item.shootSpeed = 13;

            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<ShadowDartProj>();
            Item.UseSound = CoraliteSoundID.ScutlixMount_Item90;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 aimPos = Main.MouseWorld;
            velocity = velocity.RotatedBy(Main.rand.NextFromList(-1f, 1, 3.141f, 0f) + Main.rand.NextFloat(-0.5f, 0.5f));

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, aimPos.X, aimPos.Y);

            return false;
        }
    }

    public class ShadowDartProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public Vector2 TargetPos => new(Projectile.ai[0], Projectile.ai[1]);

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 2;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                if (Projectile.localAI[1] < 21)
                {
                    if (Vector2.Distance(Projectile.Center, TargetPos) < 40)
                    {
                        Projectile.localAI[1] = 22;
                    }
                    Projectile.velocity =
                        (Projectile.velocity.ToRotation()
                        .AngleLerp((TargetPos - Projectile.Center).ToRotation(), Projectile.localAI[1] / 20).ToRotationVector2()
                        * Projectile.velocity.Length());
                    Projectile.localAI[1]++;
                }

                for (int i = 0; i < 3; i++)
                    Projectile.SpawnTrailDust(DustID.ShadowbeamStaff, Main.rand.NextFloat(0.1f, 0.8f)
                        , Scale: Main.rand.NextFloat(1, 1.5f));
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.ShadowbeamStaff, Helper.NextVec2Dir(4, 10), Scale: Main.rand.NextFloat(1, 1.4f));
                    d.noGravity = true;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Shadowflame, Helper.NextVec2Dir(0.5f, 5), Scale: Main.rand.NextFloat(1, 1.4f));
                d.noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.tileCollide = false;
                Projectile.velocity *= 0;
                Projectile.Resize(6 * 16, 6 * 16);
                Projectile.localAI[0] = 1;
            }
            return false;
        }
    }
}
