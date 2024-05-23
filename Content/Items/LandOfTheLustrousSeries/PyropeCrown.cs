using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class PyropeCrown : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(0, 1));
            Item.SetWeaponValues(20, 4);
            Item.useTime = Item.useAnimation = 20;
            Item.mana = 10;

            Item.shoot = ModContent.ProjectileType<PyropeCrownProj>();

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;


            if (player.ownedProjectileCounts[type] < 1)
                Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
            else
            {

            }

            return false;
        }
    }

    public class PyropeCrownProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "PyropeCrown";

        public override void AI()
        {
            if (Owner.HeldItem.type == ModContent.ItemType<PyropeCrown>())
                Projectile.timeLeft = 2;


        }
    }

    public class PyropeProj : ModProjectile
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "EquilateralHexagonProj1";
    }
}
