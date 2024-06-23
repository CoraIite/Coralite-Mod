using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Items.FlyingShields
{
    public class AncientFurnace : ModItem
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AncientFurnaceTile>());
            Item.rare = ItemRarityID.Red;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Furnace)
                .AddIngredient(ItemID.GlassKiln)
                .AddIngredient(ItemID.Hellforge)
                .AddIngredient(ItemID.AdamantiteForge)
                .AddIngredient(ItemID.LunarCraftingStation)
                .AddIngredient(ItemID.LunarBar, 10)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Furnace)
                .AddIngredient(ItemID.GlassKiln)
                .AddIngredient(ItemID.Hellforge)
                .AddIngredient(ItemID.TitaniumForge)
                .AddIngredient(ItemID.LunarCraftingStation)
                .AddIngredient(ItemID.LunarBar, 10)
                .Register();
        }
    }

    public class AncientFurnaceTile : ModTile
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            AdjTiles = new int[]
            {
                TileID.Furnaces,
                TileID.Hellforge,
                TileID.GlassKiln,
                TileID.AdamantiteForge,
                TileID.LunarCraftingStation
            };

            DustType = DustID.Ambient_DarkBrown;
            MinPick = 160;

            AddMapEntry(Color.DarkGray, CreateMapEntryName());
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
