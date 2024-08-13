using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Items;
using Terraria.ID;

namespace Coralite.Content.Items.ShadowCastle
{
    public class MercuryScepter : ModItem, IVariantItem
    {
        public override string Texture => AssetDirectory.ShadowCastleItems + Name;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.AquaScepter;
            ItemID.Sets.ShimmerTransformToItem[ItemID.AquaScepter] = Type;
        }

        public void AddVarient()
        {
            ItemVariants.AddVariant(ModContent.ItemType<MercuryScepter>(), ItemVariants.StrongerVariant, Condition.RemixWorld);
        }

        public override void SetDefaults()
        {
            Item.staff[Type] = true;
            Item.mana = 7;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 16;
            Item.useTime = 8;
            Item.knockBack = 7f;
            Item.damage = 30;
            Item.shoot = ModContent.ProjectileType<MercuryStream>();
            Item.shootSpeed = 12.5f;
            Item.UseSound = CoraliteSoundID.WaterShoot_Item13;
            Item.noMelee = true;
            Item.autoReuse = true;

            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, 1, 75);
            Item.DamageType = DamageClass.Magic;
            if (Item.Variant == ItemVariants.StrongerVariant)
            {
                Item.value = Item.sellPrice(0, 5);
                Item.rare = ItemRarityID.Yellow;
                Item.damage = 90;
                Item.useAnimation = 10;
                Item.useTime = 5;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Main.NewText(Item.useTime);
            if (Main.myPlayer == player.whoAmI)
            {
                float min;
                float max;
                if (player.direction > 0)
                {
                    min = -0.15f;
                    max = 0.05f;
                }
                else
                {
                    min = -0.05f;
                    max = 0.15f;
                }

                Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(min, max)), type,
                    damage, knockback, player.whoAmI, ai1: Main.rand.NextFloat(0.1f, 0.3f));
            }
            return false;
        }
    }

    /// <summary>
    /// 使用ai1传入每帧下降多少
    /// </summary>
    public class MercuryStream : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 5;
            Projectile.extraUpdates = 2;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            if (Main.remixWorld)
            {
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 12;
            }
        }

        public override void AI()
        {
            float scaleLess = 0.01f;

            Projectile.scale -= scaleLess;
            if (Projectile.scale <= 0f)
                Projectile.Kill();

            if (Projectile.ai[0] > 3f)
            {
                Projectile.velocity.Y += Projectile.ai[1];
                for (int i = 0; i < 1; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        float num133 = Projectile.velocity.X / 3f * j;
                        float num134 = Projectile.velocity.Y / 3f * j;
                        int offset = 4;
                        int index = Dust.NewDust(new Vector2(Projectile.position.X + offset, Projectile.position.Y + offset), Projectile.width - offset * 2, Projectile.height - offset * 2
                            , Main.rand.NextBool(1, 5) ? DustID.GemDiamond : DustID.SilverCoin, 0f, 0f, 100, default, 1.3f);
                        Dust dust = Main.dust[index];
                        dust.noGravity = true;
                        dust.velocity *= 0.3f;
                        dust.velocity += Projectile.velocity * 0.5f;
                        dust.position.X -= num133;
                        dust.position.Y -= num134;
                    }

                    if (Main.rand.NextBool(8))
                    {
                        int offset = 6;
                        int index = Dust.NewDust(new Vector2(Projectile.position.X + offset, Projectile.position.Y + offset), Projectile.width - offset * 2, Projectile.height - offset * 2
                            , Main.rand.NextBool(1, 5) ? DustID.GemDiamond : DustID.SilverCoin, 0f, 0f, 100, default, 0.75f);
                        Dust dust2 = Main.dust[index];
                        dust2.velocity *= 0.5f;
                        dust2.velocity += Projectile.velocity * 0.5f;
                    }
                }
            }
            else
            {
                Projectile.ai[0] += 1f;
            }

        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
