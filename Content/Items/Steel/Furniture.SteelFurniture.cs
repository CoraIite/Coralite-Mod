using Coralite.Content.Tiles.Steel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Steel
{
    public class SteelBrick() : BasePlaceableItem(Item.sellPrice(0, 0, 20), ItemRarityID.LightRed
    , ModContent.TileType<SteelBrickTile>(), AssetDirectory.SteelItems)
    {
        public override void AddRecipes()
        {
            CreateRecipe(20)
                .AddIngredient<B9Alloy>()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SteelBrickWallItem : ModItem
    {
        public override string Texture => AssetDirectory.SteelItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<SteelBrickWall>());
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<B9AlloyTileItem>()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public abstract class BaseSteelFurniture<T> : BasePlaceableItem where T : ModTile
    {
        public BaseSteelFurniture() : base(0, ItemRarityID.LightRed, ModContent.TileType<T>(), AssetDirectory.SteelItems)
        {
        }
    }

    public class SteelBathtub : BaseSteelFurniture<SteelBathtubTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SteelBed : BaseSteelFurniture<SteelBedTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(6)
                .AddIngredient(ItemID.Silk, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SteelBigDroplight : BaseSteelFurniture<SteelBigDroplightTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(3)
                .AddIngredient(ItemID.Torch, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SteelBookcase : BaseSteelFurniture<SteelBookcaseTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(6)
                .AddIngredient(ItemID.Book, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SteelCandelabra : BaseSteelFurniture<SteelCandelabraTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(3)
                .AddIngredient(ItemID.Torch, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SteelCandle : BaseSteelFurniture<SteelCandleTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>()
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SteelChair : BaseSteelFurniture<SteelChairTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SteelClock : BaseSteelFurniture<SteelClockTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(6)
                .AddRecipeGroup(RecipeGroupID.IronBar, 3)
                .AddIngredient(ItemID.Glass, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SteelDoor : BaseSteelFurniture<SteelDoorClosedTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SteelDresser : BaseSteelFurniture<SteelDresserTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SteelDroplight : BaseSteelFurniture<SteelDroplightTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(3)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SteelFloorLamp : BaseSteelFurniture<SteelFloorLampTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(3)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SteelPiano : BaseSteelFurniture<SteelPianoTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(6)
                .AddIngredient(ItemID.Bone, 20)
                .AddIngredient(ItemID.Book)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SteelPlatform : BaseSteelFurniture<SteelPlatformTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe(8)
                .AddIngredient<SteelBar>()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SteelSink : BaseSteelFurniture<SteelSinkTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(6)
                .AddIngredient(ItemID.WaterBucket)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SteelSofa : BaseSteelFurniture<SteelSofaTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SteelBench : BaseSteelFurniture<SteelBenchTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SteelTable : BaseSteelFurniture<SteelTableTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SteelToilet : BaseSteelFurniture<SteelToiletTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SteelWorkBench : BaseSteelFurniture<SteelWorkBenchTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(3)
                .Register();
        }
    }

    public class SteelChest : BaseSteelFurniture<SteelChestTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
