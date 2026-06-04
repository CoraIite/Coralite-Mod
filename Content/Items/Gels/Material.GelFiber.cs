using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.SlimeChapter1;
using Coralite.Content.Tiles.Gel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.KeySystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Gels
{
    public class GelFiber : BaseMaterial,IConsultableItem
    {
        public GelFiber() : base(9999, 0, ItemRarityID.White, AssetDirectory.GelItems) { }

        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<Slime1Knowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<SlimePage1>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<GelFiberTile>());
        }
    }
}
