using Coralite.Content.Raritys;
using Coralite.Content.Tiles.Magike;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class CrystalStaff : MagikeChargeableItem, ISpecialPlaceable
    {
        public CrystalStaff() : base(500, Item.sellPrice(0, 0, 10, 0)
            , ModContent.RarityType<MagicCrystalRarity>(), -1, AssetDirectory.MagikeSeries1Item)
        { }

        public override bool CanUseItem(Player player)
        {
            return MagikeHelper.CheckMagike(1, Item, player);
        }

        public bool CanPlace(Player player)
        {
            return MagikeHelper.TryCosumeMagike(1, Item, player);
        }

        public override void SetDefs()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 5;
            Item.useAnimation = 20;
            Item.UseSound = CoraliteSoundID.MagicStaff_Item8;
            Item.createTile = ModContent.TileType<CrystalFrame>();
            Item.autoReuse = true;
        }
    }
}
