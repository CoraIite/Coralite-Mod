using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Shadow
{
    public class ShadowSickle : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetDefaults()
        {
            Item.height = Item.width = 40;
            Item.damage = 24;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.knockBack = 6f;
            Item.reuseDelay = 20;
            Item.mana = 9;
            Item.crit = 10;

            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item9;

            Item.autoReuse = true;
            Item.useTurn = true;
            Item.noMelee = true;
            Item.channel = true;

            Item.shoot = ModContent.ProjectileType<ShadowSwordProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }

        public override void AddRecipes()
        {
            
        }
    }
}