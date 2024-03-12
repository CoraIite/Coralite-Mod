using Coralite.Content.Tiles.ShadowCastle;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.ShadowCastle
{
    public abstract class BaseMercuryFurniture<T> : BasePlaceableItem where T:ModTile
    {
        public BaseMercuryFurniture() : base(0, ItemRarityID.Blue, ModContent.TileType<T>(), AssetDirectory.ShadowCastleItems)
        {
        }
    }

    public class MercuryBathtub : BaseMercuryFurniture<MercuryBathtubTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowBrick>(12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class MercuryBed: BaseMercuryFurniture<MercuryBedTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowBrick>(12)
                .AddIngredient(ItemID.Silk, 12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class MercuryBigDroplight : BaseMercuryFurniture<MercuryBigDroplightTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowBrick>(6)
                .AddIngredient(ItemID.Torch, 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class MercuryBookcase : BaseMercuryFurniture<MercuryBookcaseTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowBrick>(20)
                .AddIngredient(ItemID.Book, 10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class MercuryCandelabra : BaseMercuryFurniture<MercuryCandelabraTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowBrick>(8)
                .AddIngredient(ItemID.Torch,3)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class MercuryCandle : BaseMercuryFurniture<MercuryCandleTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowBrick>(4)
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class MercuryChair : BaseMercuryFurniture<MercuryChairTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowBrick>(4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class MercuryClock : BaseMercuryFurniture<MercuryClockTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowBrick>(12)
                .AddRecipeGroup(RecipeGroupID.IronBar, 3)
                .AddIngredient(ItemID.Glass, 6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class MercuryDoor : BaseMercuryFurniture<MercuryDoorClosedTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowBrick>(6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class MercuryDresser : BaseMercuryFurniture<MercuryDresserTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowBrick>(16)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class MercuryDroplight2 : BaseMercuryFurniture<MercuryDroplight2Tile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowBrick>(6)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class MercuryFloorLamp : BaseMercuryFurniture<MercuryFloorLampTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowBrick>(6)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class MercuryPiano : BaseMercuryFurniture<MercuryPianoTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowBrick>(12)
                .AddIngredient(ItemID.Bone, 20)
                .AddIngredient(ItemID.Book)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class MercurySink : BaseMercuryFurniture<MercurySinkTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowBrick>(12)
                .AddIngredient(ItemID.WaterBucket)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class MercurySofa : BaseMercuryFurniture<MercurySofaTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowBrick>(8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class MercuryTable : BaseMercuryFurniture<MercuryTableTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowBrick>(8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class MercuryToilet : BaseMercuryFurniture<MercuryToiletTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowBrick>(6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class MercuryWorkBench : BaseMercuryFurniture<MercuryWorkBenchTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowBrick>(10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
