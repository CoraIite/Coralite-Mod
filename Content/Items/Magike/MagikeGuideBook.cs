using Coralite.Content.Raritys;
using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike
{
    public class MagikeGuideBook : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetDefaults()
        {
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
            Item.useAnimation = Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            //Item.noUseGraphic = true;
            //Item.UseSound = CoraliteSoundID.IceMagic_Item28;
            //Item.consumable = true;
        }

        public override void RightClick(Player player)
        {
        }
    }
}
