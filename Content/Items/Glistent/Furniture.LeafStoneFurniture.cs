using Coralite.Content.Tiles.Glistent;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;

namespace Coralite.Content.Items.Glistent
{
    public class LeafStoneBrick : ModItem
    {
        public override string Texture => AssetDirectory.GlistentItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LeafStoneBrickTile>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LeafStoneWallItem : ModItem
    {
        public override string Texture => AssetDirectory.GlistentItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<LeafStoneWall>());
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<LeafStone>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public abstract class BasePlushFurniture<T> : BasePlaceableItem where T : ModTile
    {
        public BasePlushFurniture() : base(0, ItemRarityID.Green, ModContent.TileType<T>(), AssetDirectory.GlistentItems)
        {
        }
    }

    public class LeafStoneBathtub : BasePlushFurniture<LeafStoneBathtubTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LeafStoneBed : BasePlushFurniture<LeafStoneBedTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(12)
                .AddIngredient(ItemID.Silk, 12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LeafStoneBigDroplight : BasePlushFurniture<LeafStoneBigDroplightTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(6)
                .AddIngredient(ItemID.Torch, 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LeafStoneBookcase : BasePlushFurniture<LeafStoneBookcaseTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(20)
                .AddIngredient(ItemID.Book, 10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LeafStoneCandelabra : BasePlushFurniture<LeafStoneCandelabraTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(8)
                .AddIngredient(ItemID.Torch, 3)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LeafStoneCandle : BasePlushFurniture<LeafStoneCandleTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(4)
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LeafStoneChair : BasePlushFurniture<LeafStoneChairTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LeafStoneClock : BasePlushFurniture<LeafStoneClockTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(12)
                .AddRecipeGroup(RecipeGroupID.IronBar, 3)
                .AddIngredient(ItemID.Glass, 6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LeafStoneDoor : BasePlushFurniture<LeafStoneDoorClosedTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LeafStoneDresser : BasePlushFurniture<LeafStoneDresserTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(16)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LeafStoneDroplight : BasePlushFurniture<LeafStoneDroplightTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(6)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LeafStoneFloorLamp : BasePlushFurniture<LeafStoneFloorLampTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(6)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LeafStonePiano : BasePlushFurniture<LeafStonePianoTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(12)
                .AddIngredient(ItemID.Bone, 20)
                .AddIngredient(ItemID.Book)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LeafStonePlatform : BasePlushFurniture<LeafStonePlatformTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient<LeafStone>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LeafStoneSink : BasePlushFurniture<LeafStoneSinkTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(12)
                .AddIngredient(ItemID.WaterBucket)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LeafStoneSofa : BasePlushFurniture<LeafStoneSofaTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LeafStoneTable : BasePlushFurniture<LeafStoneTableTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LeafStoneToilet : BasePlushFurniture<LeafStoneToiletTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LeafStoneWorkBench : BasePlushFurniture<LeafStoneWorkBenchTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(10)
                .Register();
        }
    }

    public class LeafStoneChest : BasePlushFurniture<LeafStoneChestTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(10)
                .AddRecipeGroup(RecipeGroupID.IronBar, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
