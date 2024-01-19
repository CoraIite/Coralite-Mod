using Coralite.Content.Items.ShadowCastle;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Tiles.ShadowCastle
{
    public class MercuryChestTile : BaseLockableChest
    {
        public override string Texture => AssetDirectory.ShadowCastleTiles + Name;

        public override int KeyType => ModContent.ItemType<ShadowMagneticCard>();
        public override int ChestItemType => ModContent.ItemType<MercuryChest>();
        public override string ChestName => "水银箱";

        public override void AddToMap()
        {
            AddMapEntry(Color.MediumPurple, this.GetLocalization("MapEntry0"), MapChestName);
            AddMapEntry(Color.MediumPurple, this.GetLocalization("MapEntry1"), MapChestName);
            DustType = DustID.Shadowflame;
        }
    }
}
