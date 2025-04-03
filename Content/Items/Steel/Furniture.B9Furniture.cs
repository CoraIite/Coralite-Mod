using Coralite.Content.Tiles.Steel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;

namespace Coralite.Content.Items.Steel
{
    public abstract class BaseB9AlloyFurniture<T> : BasePlaceableItem where T : ModTile
    {
        public BaseB9AlloyFurniture() : base(0, ItemRarityID.LightRed, ModContent.TileType<T>(), AssetDirectory.SteelItems)
        {
        }
    }

    public class B9AlloyBathtub : BaseB9AlloyFurniture<B9AlloyBathtubTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class B9AlloyBed : BaseB9AlloyFurniture<B9AlloyBedTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(6)
                .AddIngredient(ItemID.Silk, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class B9AlloyBigDroplight : BaseB9AlloyFurniture<B9AlloyBigDroplightTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(3)
                .AddIngredient(ItemID.Torch, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class B9AlloyBookcase : BaseB9AlloyFurniture<B9AlloyBookcaseTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(6)
                .AddIngredient(ItemID.Book, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class B9AlloyCandelabra : BaseB9AlloyFurniture<B9AlloyCandelabraTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(3)
                .AddIngredient(ItemID.Torch, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class B9AlloyCandle : BaseB9AlloyFurniture<B9AlloyCandleTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>()
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class B9AlloyChair : BaseB9AlloyFurniture<B9AlloyChairTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class B9AlloyClock : BaseB9AlloyFurniture<B9AlloyClockTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(6)
                .AddRecipeGroup(RecipeGroupID.IronBar, 3)
                .AddIngredient(ItemID.Glass, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class B9AlloyDoor : BaseB9AlloyFurniture<B9AlloyDoorClosedTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class B9AlloyDresser : BaseB9AlloyFurniture<B9AlloyDresserTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class B9AlloyDroplight : BaseB9AlloyFurniture<B9AlloyDroplightTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(3)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class B9AlloyFloorLamp : BaseB9AlloyFurniture<B9AlloyFloorLampTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(3)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class B9AlloyPiano : BaseB9AlloyFurniture<B9AlloyPianoTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(6)
                .AddIngredient(ItemID.Bone, 20)
                .AddIngredient(ItemID.Book)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class B9AlloyPlatform : BaseB9AlloyFurniture<B9AlloyPlatformTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe(8)
                .AddIngredient<B9Alloy>()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class B9AlloySink : BaseB9AlloyFurniture<B9AlloySinkTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(6)
                .AddIngredient(ItemID.WaterBucket)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class B9AlloySofa : BaseB9AlloyFurniture<B9AlloySofaTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class B9AlloyTable : BaseB9AlloyFurniture<B9AlloyTableTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class B9AlloyToilet : BaseB9AlloyFurniture<B9AlloyToiletTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class B9AlloyWorkBench : BaseB9AlloyFurniture<B9AlloyWorkBenchTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(3)
                .Register();
        }
    }

    public class B9AlloyChest : BaseB9AlloyFurniture<B9AlloyChestTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
