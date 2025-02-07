using Coralite.Content.Tiles.Plush;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;

namespace Coralite.Content.Items.Plush
{
    public class BlackPlushBlock : ModItem
    {
        public override string Texture => AssetDirectory.PlushItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<BlackPlushTile>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackPlush>(2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BlackPlushWallItem : ModItem
    {
        public override string Texture => AssetDirectory.PlushItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<BlackPlushWall>());
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<BlackPlushBlock>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public abstract class BasePlushFurniture<T> : BasePlaceableItem where T : ModTile
    {
        public BasePlushFurniture() : base(0, ItemRarityID.Blue, ModContent.TileType<T>(), AssetDirectory.PlushItems)
        {
        }
    }

    public class BlackPlushBathtub : BasePlushFurniture<BlackPlushBathtubTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackPlushBlock>(12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BlackPlushBed : BasePlushFurniture<BlackPlushBedTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackPlushBlock>(12)
                .AddIngredient(ItemID.Silk, 12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BlackPlushBigDroplight : BasePlushFurniture<BlackPlushBigDroplightTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackPlushBlock>(6)
                .AddIngredient(ItemID.Torch, 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BlackPlushBookcase : BasePlushFurniture<BlackPlushBookcaseTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackPlushBlock>(20)
                .AddIngredient(ItemID.Book, 10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BlackPlushCandelabra : BasePlushFurniture<BlackPlushCandelabraTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackPlushBlock>(8)
                .AddIngredient(ItemID.Torch, 3)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BlackPlushCandle : BasePlushFurniture<BlackPlushCandleTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackPlushBlock>(4)
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BlackPlushChair : BasePlushFurniture<BlackPlushChairTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackPlushBlock>(4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BlackPlushClock : BasePlushFurniture<BlackPlushClockTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackPlushBlock>(12)
                .AddRecipeGroup(RecipeGroupID.IronBar, 3)
                .AddIngredient(ItemID.Glass, 6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BlackPlushDoor : BasePlushFurniture<BlackPlushDoorClosedTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackPlushBlock>(6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BlackPlushDresser : BasePlushFurniture<BlackPlushDresserTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackPlushBlock>(16)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BlackPlushDroplight : BasePlushFurniture<BlackPlushDroplightTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackPlushBlock>(6)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BlackPlushFloorLamp : BasePlushFurniture<BlackPlushFloorLampTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackPlushBlock>(6)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BlackPlushPiano : BasePlushFurniture<BlackPlushPianoTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackPlushBlock>(12)
                .AddIngredient(ItemID.Bone, 20)
                .AddIngredient(ItemID.Book)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BlackPlushPlatform : BasePlushFurniture<BlackPlushPlatformTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient<BlackPlushBlock>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BlackPlushSink : BasePlushFurniture<BlackPlushSinkTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackPlushBlock>(12)
                .AddIngredient(ItemID.WaterBucket)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BlackPlushSofa : BasePlushFurniture<BlackPlushSofaTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackPlushBlock>(8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BlackPlushTable : BasePlushFurniture<BlackPlushTableTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackPlushBlock>(8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BlackPlushToilet : BasePlushFurniture<BlackPlushToiletTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackPlushBlock>(6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BlackPlushWorkBench : BasePlushFurniture<BlackPlushWorkBenchTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackPlushBlock>(10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BlackPlushChest : BasePlushFurniture<BlackPlushChestTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackPlushBlock>(10)
                .AddRecipeGroup(RecipeGroupID.IronBar, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
