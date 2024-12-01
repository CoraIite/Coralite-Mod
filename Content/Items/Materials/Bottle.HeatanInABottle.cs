using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ObjectData;

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
            //MagikeCraftRecipe.CreateRecipe<HeatanInABottle>(60)
            //    .SetMainItem(ItemID.Bottle)
            //    .AddIngredient(ItemID.LivingFireBlock, 20)
            //    .AddIngredient<EmpyrosPowder>(7)
            //    .Register();
        }
    }

    public class HeatanInABottleTile() : BaseBottleTile(DustID.FlameBurst, new Color(254, 121, 2), AssetDirectory.Materials)
    {
    }
}
