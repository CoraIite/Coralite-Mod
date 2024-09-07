using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Tools.MiniColumns
{
    public class MiniBrilliantColumn : MagikeChargeableItem
    {
        public MiniBrilliantColumn() : base(500, Item.sellPrice(0, 0, 10, 0)
            , ModContent.RarityType<CrystallineMagikeRarity>(), 600, AssetDirectory.MagikeTools)
        {
        }

        public override void SetDefs()
        {
            Item.GetMagikeItem().magikeSendable = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MiniDemoniteColumn>()
                .AddIngredient<CrystallineMagike>(4)
                .AddCondition(MagikeSystem.LearnedMagikeAdvanced, () => MagikeSystem.learnedMagikeAdvanced)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<MiniCrimtaneColumn>()
                .AddIngredient<CrystallineMagike>(4)
                .AddCondition(MagikeSystem.LearnedMagikeAdvanced, () => MagikeSystem.learnedMagikeAdvanced)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
