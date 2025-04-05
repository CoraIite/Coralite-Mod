using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public abstract class BaseSkarnBrickFurniture<T> : BasePlaceableItem where T : ModTile
    {
        public BaseSkarnBrickFurniture() : base(0, ModContent.RarityType<CrystallineMagikeRarity>(), ModContent.TileType<T>(), AssetDirectory.MagikeSeries2Item)
        {
        }
    }

    public class SkarnBrickBathtub : BaseSkarnBrickFurniture<SkarnBrickBathtubTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SkarnBrick>(6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SkarnBrickBed : BaseSkarnBrickFurniture<SkarnBrickBedTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SkarnBrick>(6)
                .AddIngredient(ItemID.Silk, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SkarnBrickBigDroplight : BaseSkarnBrickFurniture<SkarnBrickBigDroplightTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SkarnBrick>(3)
                .AddIngredient(ItemID.Torch, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SkarnBrickBookcase : BaseSkarnBrickFurniture<SkarnBrickBookcaseTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SkarnBrick>(6)
                .AddIngredient(ItemID.Book, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SkarnBrickCandelabra : BaseSkarnBrickFurniture<SkarnBrickCandelabraTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SkarnBrick>(3)
                .AddIngredient(ItemID.Torch, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SkarnBrickCandle : BaseSkarnBrickFurniture<SkarnBrickCandleTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SkarnBrick>()
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SkarnBrickChair : BaseSkarnBrickFurniture<SkarnBrickChairTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SkarnBrick>()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SkarnBrickClock : BaseSkarnBrickFurniture<SkarnBrickClockTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SkarnBrick>(6)
                .AddRecipeGroup(RecipeGroupID.IronBar, 3)
                .AddIngredient(ItemID.Glass, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SkarnBrickDoor : BaseSkarnBrickFurniture<SkarnBrickDoorClosedTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SkarnBrick>(3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SkarnBrickDresser : BaseSkarnBrickFurniture<SkarnBrickDresserTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SkarnBrick>(6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SkarnBrickDroplight : BaseSkarnBrickFurniture<SkarnBrickDroplightTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SkarnBrick>(3)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SkarnBrickFloorLamp : BaseSkarnBrickFurniture<SkarnBrickFloorLampTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SkarnBrick>(3)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SkarnBrickPiano : BaseSkarnBrickFurniture<SkarnBrickPianoTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SkarnBrick>(6)
                .AddIngredient(ItemID.Bone, 20)
                .AddIngredient(ItemID.Book)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SkarnBrickSink : BaseSkarnBrickFurniture<SkarnBrickSinkTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SkarnBrick>(6)
                .AddIngredient(ItemID.WaterBucket)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SkarnBrickSofa : BaseSkarnBrickFurniture<SkarnBrickSofaTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SkarnBrick>(8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SkarnBrickBench : BaseSkarnBrickFurniture<SkarnBrickBenchTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SkarnBrick>(8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SkarnBrickTable : BaseSkarnBrickFurniture<SkarnBrickTableTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SkarnBrick>(3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SkarnBrickToilet : BaseSkarnBrickFurniture<SkarnBrickToiletTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SkarnBrick>(3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SkarnBrickWorkBench : BaseSkarnBrickFurniture<SkarnBrickWorkBenchTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SkarnBrick>(3)
                .Register();
        }
    }
}
