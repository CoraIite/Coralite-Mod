using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core.Prefabs.Items;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Refractors
{
    public class BasicRefractor : BaseMagikePlaceableItem
    {
        public BasicRefractor() : base(TileType<CrystalRefractorTile>(), Item.sellPrice(0, 0, 5, 0)
            , RarityType<MagicCrystalRarity>(), 25, AssetDirectory.MagikeRefractors)
        { }

        public override int MagikeMax => 1;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystal>(5)
                .AddIngredient<Basalt>(10)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
