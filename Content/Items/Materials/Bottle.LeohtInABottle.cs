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
    /// 瓶中光
    /// </summary>
    public class LeohtInABottle : BaseMaterial, IMagikeCraftable
    {
        public LeohtInABottle() : base(9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.Orange, AssetDirectory.Materials) { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<LeohtInABottleTile>());
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateRecipe(ItemID.Bottle, ModContent.ItemType<LeohtInABottle>(), MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 6))
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.FallenStar)
                .Register();
        }
    }

    public class LeohtInABottleTile() : BaseBottleTile(DustID.OrangeTorch, new Color(208, 86, 29), AssetDirectory.Materials)
    {
    }
}
