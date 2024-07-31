using Coralite.Content.UI;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Terraria.Audio;

namespace Coralite.Core.Systems.MagikeSystem.Tiles
{
    public abstract class BaseItemSiphonTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeFactoryTiles + Name;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 2;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            MagikeItemSiphonUI.visible = false;
            UILoader.GetUIState<MagikeItemSiphonUI>().Recalculate();

            SoundEngine.PlaySound(CoraliteSoundID.DigStone_Tink, new Vector2(i, j) * 16);
            if (MagikeHelper.TryGetEntity(i, j, out MagikeItemSiphon siphon))
                siphon.Kill(siphon.Position.X, siphon.Position.Y);
        }

        public override bool RightClick(int i, int j)
        {
            if (MagikeHelper.TryGetEntity(i, j, out MagikeItemSiphon siphon))
            {
                MagikeItemSiphonUI.visible = true;
                MagikeItemSiphonUI.siphon = siphon;
                UILoader.GetUIState<MagikeItemSiphonUI>().Recalculate();
            }

            return true;
        }

        public override void HitWire(int i, int j)
        {
            if (MagikeHelper.TryGetEntity(i, j, out MagikeFactory pool))
                pool.StartWork();
        }

        public override void MouseOver(int i, int j)
        {
            MagikeHelper.ShowMagikeNumber(i, j);
        }

    }
}
