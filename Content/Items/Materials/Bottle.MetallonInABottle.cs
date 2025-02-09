using Coralite.Content.Items.Steel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    /// <summary>
    /// 瓶中刚
    /// </summary>
    public class MetallonInABottle : BaseMaterial, IMagikeCraftable
    {
        public MetallonInABottle() : base(9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.Gray, AssetDirectory.Materials) { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<MetallonInABottleTile>());
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe(ItemID.Bottle, ModContent.ItemType<MetallonInABottle>(), MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 6))
                .AddIngredient<SteelBar>()
                .AddIngredient(ItemID.CobaltBar)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.Bottle, ModContent.ItemType<MetallonInABottle>(), MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 6))
                .AddIngredient<SteelBar>()
                .AddIngredient(ItemID.PalladiumBar)
                .Register();
        }
    }

    public class MetallonInABottleTile() : BaseBottleTile(DustID.Platinum, new Color(150, 150, 150), AssetDirectory.Materials)
    {
    }
}
