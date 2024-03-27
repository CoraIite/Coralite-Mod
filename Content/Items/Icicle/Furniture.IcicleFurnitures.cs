using Coralite.Content.Tiles.Icicle;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public abstract class BaseIcicleFurniture<T> : BasePlaceableItem where T : ModTile
    {
        public BaseIcicleFurniture() : base(0, ItemRarityID.Blue, ModContent.TileType<T>(), AssetDirectory.IcicleItems)
        {
        }
    }

    public class IcicleStoneBlock : BaseIcicleFurniture<IcicleStoneTile>
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.maxStack = Item.CommonMaxStack;
        }

        public override void AddRecipes()
        {
            CreateRecipe(200)
                .AddIngredient<IcicleCrystal>()
                .AddIngredient(ItemID.StoneBlock, 100)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

    public class IcicleBathtub : BaseIcicleFurniture<IcicleBathtubTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleStoneBlock>(12)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

    public class IcicleBed : BaseIcicleFurniture<IcicleBedTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleStoneBlock>(12)
                .AddIngredient(ItemID.Silk, 12)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

    public class IcicleBigDroplight : BaseIcicleFurniture<IcicleBigDroplightTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleStoneBlock>(6)
                .AddIngredient(ItemID.Torch, 4)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

    public class IcicleBookcase : BaseIcicleFurniture<IcicleBookcaseTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleStoneBlock>(20)
                .AddIngredient(ItemID.Book, 10)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

    public class IcicleCandelabra : BaseIcicleFurniture<IcicleCandelabraTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleStoneBlock>(8)
                .AddIngredient(ItemID.Torch, 3)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

    public class IcicleCandle : BaseIcicleFurniture<IcicleCandleTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleStoneBlock>(4)
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

    public class IcicleChair : BaseIcicleFurniture<IcicleChairTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleStoneBlock>(4)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

    public class IcicleChest : BaseIcicleFurniture<IcicleChestTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleStoneBlock>(8)
                .AddRecipeGroup(RecipeGroupID.IronBar, 2)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

    public class IcicleClock : BaseIcicleFurniture<IcicleClockTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleStoneBlock>(12)
                .AddRecipeGroup(RecipeGroupID.IronBar, 3)
                .AddIngredient(ItemID.Glass, 6)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

    public class IcicleDoor : BaseIcicleFurniture<IcicleDoorClosedTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleStoneBlock>(6)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

    public class IcicleDresser : BaseIcicleFurniture<IcicleDresserTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleStoneBlock>(16)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

    public class IcicleDroplight : BaseIcicleFurniture<IcicleDroplightTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleStoneBlock>(6)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

    public class IcicleFloorLamp : BaseIcicleFurniture<IcicleFloorLampTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleStoneBlock>(6)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

    public class IciclePiano : BaseIcicleFurniture<IciclePianoTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleStoneBlock>(12)
                .AddIngredient(ItemID.Bone, 20)
                .AddIngredient(ItemID.Book)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

    public class IciclePlatform : BaseIcicleFurniture<IciclePlatformTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient<IcicleStoneBlock>()
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

    public class IcicleSink : BaseIcicleFurniture<IcicleSinkTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleStoneBlock>(12)
                .AddIngredient(ItemID.WaterBucket)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

    public class IcicleSofa : BaseIcicleFurniture<IcicleSofaTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleStoneBlock>(8)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

    public class IcicleTable : BaseIcicleFurniture<IcicleTableTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleStoneBlock>(8)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

    public class IcicleToilet : BaseIcicleFurniture<IcicleToiletTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleStoneBlock>(6)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

    public class IcicleWorkBench : BaseIcicleFurniture<IcicleWorkBenchTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleStoneBlock>(10)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

}
