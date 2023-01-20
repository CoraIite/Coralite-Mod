using Coralite.Content.Projectiles.StarsProjectiles;
using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.StarsItems
{
    public class StarBook : ModItem
    {
        public override string Texture => AssetDirectory.StarsItems + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星之书");

            Tooltip.SetDefault("汇聚群星的能量");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 75;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.mana = 22;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 8;
            Item.crit = 0;

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ProjectileType<StarBookProj1>();

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.channel = true;
        }
    }
}
