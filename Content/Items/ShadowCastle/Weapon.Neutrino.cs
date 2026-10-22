using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.ShadowCastle
{
    public class Neutrino : ModItem
    {
        public override string Texture => AssetDirectory.ShadowCastleItems + Name;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
            Item.useAnimation =  Item.useTime = 45; // The item's use time in ticks (60 ticks == 1 second.)
            Item.knockBack = 5.5f; 
            Item.damage = 30; 
            Item.noUseGraphic = true; 
            Item.shoot = ModContent.ProjectileType<ExampleAdvancedFlailProjectile>(); // The flail projectile
            Item.shootSpeed = 12f; // The speed of the projectile measured in pixels per frame.
            Item.UseSound = SoundID.Item1; // The sound that this item makes when used
            Item.rare = ItemRarityID.Green; // The color of the name of your item
            Item.value = Item.sellPrice(gold: 1, silver: 50); // Sells for 1 gold 50 silver
            Item.DamageType = DamageClass.MeleeNoSpeed; // Deals melee damage
            Item.channel = true;
            Item.noMelee = true;
        }

    }


}
