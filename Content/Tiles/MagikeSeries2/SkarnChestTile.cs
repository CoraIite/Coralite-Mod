using Coralite.Content.Dusts;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class SkarnChestTile : BaseLockableChest
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override int KeyType => ModContent.ItemType<SkarnKey>();
        public override int ChestItemType => ModContent.ItemType<SkarnChest>();
        public override string ChestName => "矽卡箱";

        public static LocalizedText TryUnlock { get; private set; }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            TryUnlock = this.GetLocalization("TryUnlock");
        }

        public override void AddToMap()
        {
            AddMapEntry(Color.Gold, this.GetLocalization("MapEntry0"), MapChestName);
            AddMapEntry(Color.Gold, this.GetLocalization("MapEntry1"), MapChestName);
            DustType = ModContent.DustType<SkarnDust>();
        }

        public override void OnFillToUnlock(int i, int j)
        {
            Main.NewText(TryUnlock.Value, Coralite.CrystallinePurple);
        }
    }
}
