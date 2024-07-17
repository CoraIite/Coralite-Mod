using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public abstract class BaseBasaltFurniture<T> : BasePlaceableItem where T : ModTile
    {
        public BaseBasaltFurniture() : base(0, ModContent.RarityType<MagicCrystalRarity>(), ModContent.TileType<T>(), AssetDirectory.MagikeSeries1Item)
        {
        }
    }

    public class BasaltBathtub : BaseBasaltFurniture<BasaltBathtubTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BasaltBed : BaseBasaltFurniture<BasaltBedTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(12)
                .AddIngredient(ItemID.Silk, 12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BasaltBigDroplight : BaseBasaltFurniture<BasaltBigDroplightTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(6)
                .AddIngredient(ItemID.Torch, 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BasaltBookcase : BaseBasaltFurniture<BasaltBookcaseTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(20)
                .AddIngredient(ItemID.Book, 10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BasaltCandelabra : BaseBasaltFurniture<BasaltCandelabraTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(8)
                .AddIngredient(ItemID.Torch, 3)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BasaltCandle : BaseBasaltFurniture<BasaltCandleTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(4)
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BasaltChair : BaseBasaltFurniture<BasaltChairTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BasaltClock : BaseBasaltFurniture<BasaltClockTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(12)
                .AddRecipeGroup(RecipeGroupID.IronBar, 3)
                .AddIngredient(ItemID.Glass, 6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BasaltDoor : BaseBasaltFurniture<BasaltDoorClosedTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BasaltDresser : BaseBasaltFurniture<BasaltDresserTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(16)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BasaltDroplight : BaseBasaltFurniture<BasaltDroplightTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(6)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BasaltFloorLamp : BaseBasaltFurniture<BasaltFloorLampTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(6)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BasaltPiano : BaseBasaltFurniture<BasaltPianoTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(12)
                .AddIngredient(ItemID.Bone, 20)
                .AddIngredient(ItemID.Book)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BasaltPlatform : BaseBasaltFurniture<BasaltPlatformTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient<Basalt>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BasaltSink : BaseBasaltFurniture<BasaltSinkTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(12)
                .AddIngredient(ItemID.WaterBucket)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BasaltSofa : BaseBasaltFurniture<BasaltSofaTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BasaltTable : BaseBasaltFurniture<BasaltTableTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BasaltToilet : BaseBasaltFurniture<BasaltToiletTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BasaltWorkBench : BaseBasaltFurniture<BasaltWorkBenchTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
