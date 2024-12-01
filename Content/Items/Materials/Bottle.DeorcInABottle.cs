using Coralite.Content.Items.Shadow;
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
    /// 瓶中阴
    /// </summary>
    public class DeorcInABottle : BaseMaterial, IMagikeCraftable
    {
        public DeorcInABottle() : base(9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.LightPurple, AssetDirectory.Materials) { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<DeorcInABottleTile>());
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeCraftRecipe.CreateRecipe(ItemID.Bottle, ModContent.ItemType<DeorcInABottle>(), MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 6))
                .AddIngredient(ItemID.SoulofNight)
                .AddIngredient<ShadowCrystal>()
                .Register();
        }
    }

    public class DeorcInABottleTile() : BaseBottleTile(DustID.Shadowflame, new Color(162, 95, 234), AssetDirectory.Materials)
    {
    }
}
