using Coralite.Content.Raritys;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class MagicCrystalPolarizedFilter : PolarizedFilterItem
    {
        public MagicCrystalPolarizedFilter() : base(Item.sellPrice(0, 0, 10), ModContent.RarityType<MagicCrystalRarity>())
        {
        }

        public override MagikeFilter GetFilterComponent() => new MagicCrystalPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystal>(6)
                .AddIngredient<Basalt>(4)
                .AddTile(TileID.Anvils)
                .Register();
            
        }
    }

    public class MagicCrystalPolarizedFilterComponent : PolarizedFilter
    {
        public override MagikeApparatusLevel UpgradeLevel => MagikeApparatusLevel.MagicCrystal;

        public override int ItemType => ModContent.ItemType<MagicCrystalPolarizedFilter>();
    }
}
