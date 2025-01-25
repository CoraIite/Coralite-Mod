using Coralite.Content.Items.Icicle;
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
    /// 瓶中冻
    /// </summary>
    public class FreosanInABottle : BaseMaterial, IMagikeCraftable
    {
        public FreosanInABottle() : base(9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.Cyan, AssetDirectory.Materials) { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<FreosanInABottleTile>());
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateRecipe(ItemID.Bottle, ModContent.ItemType<FreosanInABottle>(), MagikeHelper.CalculateMagikeCost(MALevel.Hellstone, 6))
                .AddIngredient<IcicleBreath>()
                .AddIngredient(ItemID.IceBlock)
                .Register();
        }
    }

    public class FreosanInABottleTile() : BaseBottleTile(DustID.ApprenticeStorm, Coralite.IcicleCyan, AssetDirectory.Materials)
    {
    }
}
