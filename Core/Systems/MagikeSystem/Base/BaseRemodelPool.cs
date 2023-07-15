using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Coralite.Content.UI;
using Coralite.Core.Loaders;

namespace Coralite.Core.Systems.MagikeSystem.Base
{
    public abstract class BaseRemodelPool : ModTile
    {
        public override string Texture => AssetDirectory.MagikeTiles + Name;
        public virtual string ExtraTextureName => AssetDirectory.MagikeTiles + Name + "_Top";

        public Asset<Texture2D> ExtraTexture;
        public const int FrameWidth = 18 * 3;
        public const int FrameHeight = 18 * 2;
        public const int halfWidth = 16 * 3 / 2;
        public const int halfHeight = 16 * 2 / 2;
        public readonly int HorizontalFrames = 1;
        public readonly int VerticalFrames = 1;

        public override void Load()
        {
            if (!Main.dedServ)
                ExtraTexture = ModContent.Request<Texture2D>(ExtraTextureName);
        }

        public override void Unload()
        {
            ExtraTexture = null;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 2;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            SoundEngine.PlaySound(CoraliteSoundID.DigStone_Tink, new Vector2(i, j) * 16);
            int x = i - frameX / 18;
            int y = j - frameY / 18;
            if (MagikeHelper.TryGetEntityWithTopLeft(x, y, out MagikeFactory_RemodelPool pool))
                pool.Kill(x, y);
        }

        public override bool RightClick(int i, int j)
        {
            if (MagikeHelper.TryGetEntity(i, j, out MagikeFactory_RemodelPool pool))
            {
                MagikeRemodelUI.visible = true;
                MagikeRemodelUI.remodelPool = pool;
                UILoader.GetUIState<MagikeRemodelUI>().Recalculate();
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

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            //由于该物块在其工作表上没有悬停部分，因此我们必须自己对其进行动画处理
            //因此，我们将瓷砖的左上角注册为“特殊点”
            //这使我们能够在SpecialDraw中绘制内容
            if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0)
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
        }
    }
}
