using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Disturbance
{
    public class TheWhirlpoolOfPeopleAndThings : ModItem
    {
        public override string Texture => AssetDirectory.DisturbanceItems + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("人于物的牵绊漩涡");
            Tooltip.SetDefault("螺旋，收束，合而为一\n[未实现内容]");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 150;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.mana = 14;
            Item.knockBack = 8;
            Item.crit = 0;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Red;
            // Item.shoot = ProjectileType<StarBookProj1>();

            Item.useTurn = true;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.autoReuse = true;
            Item.channel = false;
        }
    }
}
