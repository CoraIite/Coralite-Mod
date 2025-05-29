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
    /// 瓶中异变
    /// </summary>
    public class MutatusInABottle : BaseMaterial, IMagikeCraftable
    {
        public MutatusInABottle() : base(9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.Purple, AssetDirectory.Materials) { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<MutatusInABottleTile>());
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe(ItemID.Bottle, ModContent.ItemType<MutatusInABottle>(), MagikeHelper.CalculateMagikeCost(MALevel.Crimson, 6))
                .AddIngredient(ItemID.EbonstoneBlock, 10)
                .AddIngredient(ItemID.ShadowScale)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.Bottle, ModContent.ItemType<MutatusInABottle>(), MagikeHelper.CalculateMagikeCost(MALevel.Crimson, 6))
                .AddIngredient(ItemID.CrimstoneBlock, 10)
                .AddIngredient(ItemID.TissueSample)
                .Register();
        }
    }

    public class MutatusInABottleTile() : BaseBottleTile(DustID.BubbleBurst_Purple, new Color(125, 68, 117), AssetDirectory.Materials)
    {
    }
}
