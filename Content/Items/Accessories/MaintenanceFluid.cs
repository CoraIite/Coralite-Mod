using Coralite.Content.DamageClasses;
using Coralite.Core;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Accessories
{
    public class MaintenanceFluid : ModItem
    {
        public override string Texture => AssetDirectory.Accessories + Name;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("护剑精油");
            /* Tooltip.SetDefault("给你的剑定期保养一下吧！\n" +
                "念力伤害 +2"); */
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<YujianDamage>().Flat += 2f;
        }
    }
}
