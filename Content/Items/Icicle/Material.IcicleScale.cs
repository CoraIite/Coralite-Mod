using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.IceDragonChapter1;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.KeySystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    internal class IcicleScale : BaseMaterial, IConsultableItem
    {
        public IcicleScale() : base(Item.CommonMaxStack, Item.sellPrice(0, 0, 40), ItemRarityID.Green, AssetDirectory.IcicleItems)
        {
        }

        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<IceDragon1Knowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<IciclePage1>();
    }
}
