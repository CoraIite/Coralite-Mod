using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.ZephyrkiloSeries
{
    public class Afterglow : ModItem
    {
        public override string Texture => AssetDirectory.ZephyrkiloSeriesItems + Name;

        public override void SetDefaults()
        {
            Item.useAmmo = AmmoID.Arrow;
            Item.damage = 11;
            Item.shootSpeed = 7f;
            Item.knockBack = 0;
            Item.shoot = ProjectileID.PurificationPowder;

            Item.DamageType = DamageClass.Ranged;
            Item.rare = ItemRarityID.Blue;
            Item.useTime = Item.useAnimation = 27;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(0, 0, 10);

            Item.useTurn = false;
            Item.noMelee = true;
            Item.autoReuse = false;

            Item.UseSound = CoraliteSoundID.Bow_Item5;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
            {
                type = ProjectileID.FireArrow;
                velocity = velocity.SafeNormalize(Vector2.Zero) * (velocity.Length() * 1);
            }
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-1, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BorealWoodBow)
                .AddIngredient(ItemID.Torch, 99)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
