using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Tools.MiniColumns
{
    public class MiniCrystalColumn : MagikeChargeableItem
    {
        public MiniCrystalColumn() : base(150, Item.sellPrice(0, 0, 10, 0)
            , ModContent.RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeTools)
        {
        }

        public override void SetDefs()
        {
            Item.GetMagikeItem().magikeSendable = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystal>(10)
                .AddTile(TileID.Anvils)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
                .Register();
        }
    }
}
