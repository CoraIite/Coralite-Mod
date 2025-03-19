using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public abstract class BaseChalcedonyFurniture<T> : BasePlaceableItem where T : ModTile
    {
        public BaseChalcedonyFurniture() : base(0, ItemRarityID.White, ModContent.TileType<T>(), AssetDirectory.MagikeSeries2Item)
        {
        }
    }

    public class ChalcedonyBathtub : BaseChalcedonyFurniture<ChalcedonyBathtubTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafChalcedony>(12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class ChalcedonyBed : BaseChalcedonyFurniture<ChalcedonyBedTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafChalcedony>(12)
                .AddIngredient(ItemID.Silk, 12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class ChalcedonyBigDroplight : BaseChalcedonyFurniture<ChalcedonyBigDroplightTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafChalcedony>(6)
                .AddIngredient(ItemID.Torch, 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class ChalcedonyBookcase : BaseChalcedonyFurniture<ChalcedonyBookcaseTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafChalcedony>(20)
                .AddIngredient(ItemID.Book, 10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class ChalcedonyCandelabra : BaseChalcedonyFurniture<ChalcedonyCandelabraTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafChalcedony>(8)
                .AddIngredient(ItemID.Torch, 3)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class ChalcedonyCandle : BaseChalcedonyFurniture<ChalcedonyCandleTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafChalcedony>(4)
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class ChalcedonyChair : BaseChalcedonyFurniture<ChalcedonyChairTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafChalcedony>(4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class ChalcedonyClock : BaseChalcedonyFurniture<ChalcedonyClockTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafChalcedony>(12)
                .AddRecipeGroup(RecipeGroupID.IronBar, 3)
                .AddIngredient(ItemID.Glass, 6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class ChalcedonyDoor : BaseChalcedonyFurniture<ChalcedonyDoorClosedTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafChalcedony>(6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class ChalcedonyDresser : BaseChalcedonyFurniture<ChalcedonyDresserTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafChalcedony>(16)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class ChalcedonyDroplight : BaseChalcedonyFurniture<ChalcedonyDroplightTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafChalcedony>(6)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class ChalcedonyFloorLamp : BaseChalcedonyFurniture<ChalcedonyFloorLampTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafChalcedony>(6)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class ChalcedonyPiano : BaseChalcedonyFurniture<ChalcedonyPianoTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafChalcedony>(12)
                .AddIngredient(ItemID.Bone, 20)
                .AddIngredient(ItemID.Book)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class ChalcedonyPlatform : BaseChalcedonyFurniture<ChalcedonyPlatformTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient<LeafChalcedony>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class ChalcedonySink : BaseChalcedonyFurniture<ChalcedonySinkTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafChalcedony>(12)
                .AddIngredient(ItemID.WaterBucket)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class ChalcedonySofa : BaseChalcedonyFurniture<ChalcedonySofaTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafChalcedony>(8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class ChalcedonyTable : BaseChalcedonyFurniture<ChalcedonyTableTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafChalcedony>(8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class ChalcedonyToilet : BaseChalcedonyFurniture<ChalcedonyToiletTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafChalcedony>(6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class ChalcedonyWorkBench : BaseChalcedonyFurniture<ChalcedonyWorkBenchTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafChalcedony>(10)
                .Register();
        }
    }

    public class ChalcedonyChest : BaseChalcedonyFurniture<ChalcedonyChestTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafChalcedony>(10)
                .AddRecipeGroup(RecipeGroupID.IronBar, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
