using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Coralite.Content.Items.Steel
{
    public class SteelBar : BaseMaterial  //, IMagikePolymerizable
    {
        public SteelBar() : base(9999, Item.sellPrice(0, 0, 20), ItemRarityID.LightRed, AssetDirectory.SteelItems) { }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 59;

            //ItemTrader.ChlorophyteExtractinator.AddOption_OneWay(Type, 5, ItemID.ChlorophyteBar, 2);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<SteelBarTile>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.IronBar, 3)
                .AddTile(TileID.AdamantiteForge)
                .Register();
        }

        //public void AddMagikePolymerizeRecipe()
        //{
        //    PolymerizeRecipe.CreateRecipe<SteelBar>(300)
        //        .SetMainItem(ItemID.IronBar, 12)
        //        .AddIngredient(ItemID.Coal, 6)
        //        .AddIngredient<HeatanInABottle>()
        //        .AddCondition(HardModeCondition.Instance)
        //        .Register();
        //}
    }

    public class SteelBarTile : ModTile
    {
        public override string Texture => AssetDirectory.SteelItems + Name;

        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 1100;
            Main.tileSolid[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            DustType = DustID.PlatinumCoin;

            AddMapEntry(new Color(150, 150, 150), Language.GetText("MapObject.MetalBar")); // localized text for "Metal Bar"
        }
    }
}
