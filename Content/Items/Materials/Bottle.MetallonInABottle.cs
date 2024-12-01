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
            //PolymerizeRecipe.CreateRecipe<FreosanInABottle>(300)
            //    .SetMainItem<>
        }
    }

    public class MetallonInABottleTile() : BaseBottleTile(DustID.Platinum, new Color(150, 150, 150), AssetDirectory.Materials)
    {
    }
}
