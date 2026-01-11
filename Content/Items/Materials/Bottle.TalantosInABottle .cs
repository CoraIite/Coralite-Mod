using Coralite.Content.Items.Gels;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    /// <summary>
    /// 瓶中包容
    /// </summary>
    public class TalantosInABottle : BaseMaterial, IMagikeCraftable
    {
        public TalantosInABottle() : base(9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.Cyan, AssetDirectory.Materials) { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<TalantosInABottleTile>());
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe(ItemID.Bottle, ModContent.ItemType<TalantosInABottle>(), MagikeHelper.CalculateMagikeCost<EmperorLevel>(6))
                .AddIngredient(ItemID.WaterBucket)
                .AddIngredient<EmperorGel>()
                .Register();
        }
    }

    public class TalantosInABottleTile() : BaseBottleTile(DustID.GreenMoss, new Color(93, 167, 155), AssetDirectory.Materials)
    {
    }
}
