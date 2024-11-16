//using Coralite.Content.Raritys;
//using Coralite.Content.UI.MagikeGuideBook;
//using Coralite.Core;
//using Coralite.Core.Loaders;
//using Terraria;
//using Terraria.ID;

//namespace Coralite.Content.Items.Magike
//{
//    public class MagikeGuideBook : ModItem
//    {
//        public override string Texture => AssetDirectory.MagikeItems + Name;

//        public override void SetDefaults()
//        {
//            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
//            Item.useAnimation = Item.useTime = 30;
//            Item.useStyle = ItemUseStyleID.HoldUp;
//            //Item.noUseGraphic = true;
//            //Item.UseSound = CoraliteSoundID.IceMagic_Item28;
//            //Item.consumable = true;
//        }

//        public override bool CanRightClick() => true;

//        public override void RightClick(Player player)
//        {
//            //UILoader.GetUIState<MagikeGuideBookUI>().Recalculate();
//            //UILoader.GetUIState<MagikeGuideBookUI>().OpenBook();

//            //Main.playerInventory = false;
//        }

//        public override bool ConsumeItem(Player player) => false;
//    }
//}
