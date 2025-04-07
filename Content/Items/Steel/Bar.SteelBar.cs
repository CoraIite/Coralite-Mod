using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Coralite.Content.Items.Steel
{
    public class SteelBar : BaseMaterial, IMagikeCraftable
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

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe(ItemID.IronOre, ModContent.ItemType<SteelBar>()
                , MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 3, 45), 2)
                .AddIngredient(ItemID.TitaniumOre)
                .AddIngredient(ItemID.Coal)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.IronOre, ModContent.ItemType<SteelBar>()
                , MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 3, 45), 2)
                .AddIngredient(ItemID.AdamantiteOre)
                .AddIngredient(ItemID.Coal)
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
