using Terraria;
using Coralite.Content.Items.Weapons_Magic;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Coralite.Core;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Weapons_Shoot
{
    public class WoodWax : ModItem
    {
        public override string Texture => AssetDirectory.Weapons_Shoot + Name;

        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.reuseDelay = 25;
            Item.mana = 18;
            Item.knockBack = 3;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 0, 30, 0);
            Item.rare = ItemRarityID.Blue;
            //Item.shoot = ProjectileType<MagicalAshesProj>();
            Item.useAmmo = AmmoID.Bullet;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {


            return false;
        }

    }
}
