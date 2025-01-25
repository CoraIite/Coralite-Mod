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
    /// 瓶中灼
    /// </summary>
    public class HeatanInABottle : BaseMaterial, IMagikeCraftable
    {
        public HeatanInABottle() : base(9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.Yellow, AssetDirectory.Materials) { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<HeatanInABottleTile>());
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateRecipe(ItemID.Bottle, ModContent.ItemType<HeatanInABottle>(), MagikeHelper.CalculateMagikeCost(MALevel.Hellstone, 6))
                .AddIngredient(ItemID.Hellstone)
                .AddIngredient(ItemID.Fireblossom)
                .Register();
        }
    }

    public class HeatanInABottleTile() : BaseBottleTile(DustID.FlameBurst, new Color(254, 121, 2), AssetDirectory.Materials)
    {
    }
}
