using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class Naclite : BaseMaterial
    {
        public Naclite() : base( 9999, Item.sellPrice(0, 0, 0, 25), ItemRarityID.Green, AssetDirectory.Materials) { }
    }
}
