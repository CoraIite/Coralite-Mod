using Coralite.Content.Tiles.Thunder;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Thunder
{
    public abstract class BaseThunderFurniture<T> : BasePlaceableItem where T : ModTile
    {
        public BaseThunderFurniture() : base(0, ItemRarityID.Yellow, ModContent.TileType<T>(), AssetDirectory.ThunderItems)
        {
        }
    }

    public class ThunderStoneBlock : BaseThunderFurniture<ThunderStoneTile>
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.maxStack = Item.CommonMaxStack;
        }

        public override void AddRecipes()
        {
            CreateRecipe(200)
                .AddIngredient<ZapCrystal>()
                .AddIngredient(ItemID.StoneBlock, 100)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderBathtub : BaseThunderFurniture<ThunderBathtubTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThunderStoneBlock>(12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderBed : BaseThunderFurniture<ThunderBedTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThunderStoneBlock>(12)
                .AddIngredient(ItemID.Silk, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderBigDroplight : BaseThunderFurniture<ThunderBigDroplightTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThunderStoneBlock>(6)
                .AddIngredient(ItemID.Torch, 4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderBookcase : BaseThunderFurniture<ThunderBookcaseTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThunderStoneBlock>(20)
                .AddIngredient(ItemID.Book, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderCandelabra : BaseThunderFurniture<ThunderCandelabraTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThunderStoneBlock>(8)
                .AddIngredient(ItemID.Torch, 3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderCandle : BaseThunderFurniture<ThunderCandleTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThunderStoneBlock>(4)
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderChair : BaseThunderFurniture<ThunderChairTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThunderStoneBlock>(4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderChest : BaseThunderFurniture<ThunderChestTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThunderStoneBlock>(8)
                .AddRecipeGroup(RecipeGroupID.IronBar, 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderClock : BaseThunderFurniture<ThunderClockTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThunderStoneBlock>(12)
                .AddRecipeGroup(RecipeGroupID.IronBar, 3)
                .AddIngredient(ItemID.Glass, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderDoor : BaseThunderFurniture<ThunderDoorClosedTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThunderStoneBlock>(6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderDresser : BaseThunderFurniture<ThunderDresserTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThunderStoneBlock>(16)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderDroplight : BaseThunderFurniture<ThunderDroplightTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThunderStoneBlock>(6)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderFloorLamp : BaseThunderFurniture<ThunderFloorLampTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThunderStoneBlock>(6)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderPiano : BaseThunderFurniture<ThunderPianoTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThunderStoneBlock>(12)
                .AddIngredient(ItemID.Bone, 20)
                .AddIngredient(ItemID.Book)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderPlatform : BaseThunderFurniture<ThunderPlatformTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient<ThunderStoneBlock>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderSink : BaseThunderFurniture<ThunderSinkTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThunderStoneBlock>(12)
                .AddIngredient(ItemID.WaterBucket)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderSofa : BaseThunderFurniture<ThunderSofaTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThunderStoneBlock>(8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderTable : BaseThunderFurniture<ThunderTableTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThunderStoneBlock>(8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderToilet : BaseThunderFurniture<ThunderToiletTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThunderStoneBlock>(6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderWorkBench : BaseThunderFurniture<ThunderWorkBenchTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThunderStoneBlock>(10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
