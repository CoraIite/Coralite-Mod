using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Items.Materials
{
    /// <summary>
    /// 瓶中调和
    /// </summary>
    public class ConcileInABottle : BaseMaterial, IMagikeCraftable
    {
        public ConcileInABottle() : base(9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.Pink, AssetDirectory.Materials) { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<ConcileInABottleTile>());
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeCraftRecipe.CreateRecipe(ItemID.Bottle, ModContent.ItemType<ConcileInABottle>(), MagikeHelper.CalculateMagikeCost(MALevel.Glistent, 6))
                .AddIngredient(ItemID.IronAnvil)
                .AddIngredient(ItemID.TinkerersWorkshop)
                .Register();

            MagikeCraftRecipe.CreateRecipe(ItemID.Bottle, ModContent.ItemType<ConcileInABottle>(), MagikeHelper.CalculateMagikeCost(MALevel.Glistent, 6))
                .AddIngredient(ItemID.LeadAnvil)
                .AddIngredient(ItemID.TinkerersWorkshop)
                .Register();
        }
    }

    public class ConcileInABottleTile() : BaseBottleTile(DustID.PinkFairy, Coralite.MagicCrystalPink, AssetDirectory.Materials)
    {
    }
}
