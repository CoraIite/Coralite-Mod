using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike.Tools
{
    public class CrystalStaff : BaseMagikeChargeableItem, IMagikeContainerItemPlaceable
    {
        public CrystalStaff() : base(150, Item.sellPrice(0, 0, 10, 0)
            , ModContent.RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeTools)
        { }

        public bool CanPlace(Player player)
        {
            return Item.TryCosumeMagike(1) || player.TryCosumeMagike(1);
        }

        public override void SetDefs()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 20;
            Item.UseSound = CoraliteSoundID.MagicStaff_Item8;
            Item.createTile = ModContent.TileType<CrystalFrame>();
            Item.autoReuse = true;
        }
    }

    public class CrystalFrame : ModTile
    {
        public override string Texture => AssetDirectory.MagikeTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileBlockLight[Type] = false;
            Main.tileSolid[Type] = true;
            Main.tileNoFail[Type] = true;

            DustType = DustID.CrystalSerpent_Pink;
            HitSound = CoraliteSoundID.DigStone_Tink;
            AddMapEntry(Coralite.Instance.MagicCrystalPink);
        }

        public override bool CanDrop(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 5;
        }
    }
}
