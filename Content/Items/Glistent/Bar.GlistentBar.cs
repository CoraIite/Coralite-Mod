using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Coralite.Content.Items.Glistent
{
    public class GlistentBar : BaseMaterial  //, IMagikePolymerizable
    {
        public GlistentBar() : base(9999, Item.sellPrice(0, 0, 5, 50), ItemRarityID.Green, AssetDirectory.GlistentItems) { }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 59;

            //ItemTrader.ChlorophyteExtractinator.AddOption_OneWay(Type, 5, ItemID.ChlorophyteBar, 2);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<GlistentBarTile>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>()
                .AddIngredient(ItemID.CrimtaneBar)
                .AddIngredient(ItemID.Diamond)
                .AddTile<MagicCraftStation>()
                .Register();

            CreateRecipe()
                .AddIngredient<LeafStone>()
                .AddIngredient(ItemID.DemoniteBar)
                .AddIngredient(ItemID.Diamond)
                .AddTile<MagicCraftStation>()
                .Register();

            Recipe r = CreateRecipe();
            r.ReplaceResult(ItemID.LivingLoom);
            r.AddIngredient<LeafStone>()
                .AddRecipeGroup(RecipeGroupID.Wood, 12)
                .AddIngredient(ItemID.Acorn)
                .AddTile<MagicCraftStation>()
                .Register();
        }

        //public void AddMagikePolymerizeRecipe()
        //{
        //    PolymerizeRecipe.CreateRecipe<GlistentBar>(50)
        //        .SetMainItem<LeafStone>()
        //        .AddIngredient(ItemID.CrimtaneBar, 2)
        //        .AddIngredient(ItemID.Diamond)
        //        .Register();

        //    PolymerizeRecipe.CreateRecipe<GlistentBar>(50)
        //        .SetMainItem<LeafStone>()
        //        .AddIngredient(ItemID.DemoniteBar, 2)
        //        .AddIngredient(ItemID.Diamond)
        //        .Register();
        //}
    }

    public class GlistentBarTile : ModTile
    {
        public override string Texture => AssetDirectory.GlistentItems + Name;

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

            DustType = DustID.GreenTorch;

            AddMapEntry(new Color(150, 150, 150), Language.GetText("MapObject.MetalBar")); // localized text for "Metal Bar"
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }

}
