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
            //PolymerizeRecipe.CreateRecipe<FreosanInABottle>(300)
            //    .SetMainItem<>
        }
    }

    public class FreosanInABottleTile() : BaseBottleTile(DustID.ApprenticeStorm, Coralite.IcicleCyan, AssetDirectory.Materials)
    {
    }
}
