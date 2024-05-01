using Coralite.Content.DamageClasses;
using Coralite.Core;
using Terraria;

namespace Coralite.Content.Items.Accessories
{
    public class MaintenanceFluid : ModItem
    {
        public override string Texture => AssetDirectory.Accessories + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetDamage<YujianDamage>().Flat += 2f;
        }
    }
}
