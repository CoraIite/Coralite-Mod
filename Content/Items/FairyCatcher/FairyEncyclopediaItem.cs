using Coralite.Content.UI.FairyEncyclopedia;
using Coralite.Core;
using Coralite.Core.Loaders;
using Terraria;

namespace Coralite.Content.Items.FairyCatcher
{
    public class FairyEncyclopediaItem:ModItem
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public override bool CanRightClick() => true;
        public override bool ConsumeItem(Player player) => false;

        public override void RightClick(Player player)
        {
            FairyEncyclopedia.visible = true;
            UILoader.GetUIState<FairyEncyclopedia>().SetToAllShow();
            UILoader.GetUIState<FairyEncyclopedia>().Recalculate();
        }
    }
}
