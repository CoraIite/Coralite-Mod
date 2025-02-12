using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Misc_Magic
{
    public class Taurus:ModItem
    {
        public override string Texture => AssetDirectory.Misc_Magic + Name;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(50, 6f);
            Item.DefaultToMagicWeapon(10, 12, 15, true);
            Item.mana = 5;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(0, 1);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = CoraliteSoundID.Swing_Item1;

            Item.useTurn = false;
            Item.noUseGraphic = true;
        }
    }
}
