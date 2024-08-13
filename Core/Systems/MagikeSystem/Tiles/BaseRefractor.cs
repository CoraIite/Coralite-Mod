using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.MagikeSystem.Tiles
{
    public abstract class OldBaseRefractorTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeRefractorTiles + Name;
        public virtual string TopTextureName => AssetDirectory.MagikeRefractorTiles + Name + "_Top";

        public Asset<Texture2D> TopTexture;
        public const int FrameWidth = 18;
        public const int FrameHeight = 18 * 2;
        public const int halfWidth = 16 / 2;
        public const int halfHeight = 16 * 2 / 2;
        public readonly int HorizontalFrames = 1;
        public readonly int VerticalFrames = 1;

        public override void Load()
        {
            if (!Main.dedServ)
                TopTexture = ModContent.Request<Texture2D>(TopTextureName);
        }

        public override void Unload()
        {
            TopTexture = null;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 2;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            SoundEngine.PlaySound(CoraliteSoundID.GlassBroken_Shatter, new Vector2(i, j) * 16);
            int x = i - frameX / 18;
            int y = j - frameY / 18;
            if (MagikeHelper.TryGetEntityWithTopLeft(x, y, out MagikeSender_Line sender))
                sender.Kill(x, y);
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

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //这是特定于照明模式的，如果您手动绘制瓷砖，请始终包含此内容
            Vector2 offScreen = new(Main.offScreenRange);
            if (Main.drawToScreen)
                offScreen = Vector2.Zero;

            //检查物块它是否真的存在
            Point p = new(i, j);
            Terraria.Tile tile = Main.tile[p.X, p.Y];
            if (tile == null || !tile.HasTile)
                return;

            //获取初始绘制参数
            Texture2D texture = TopTexture.Value;

            // 根据项目的地点样式拾取图纸上的框架
            int frameY = tile.TileFrameX / FrameWidth;
            Rectangle frame = texture.Frame(HorizontalFrames, VerticalFrames, 0, frameY);

            Vector2 origin = frame.Size() / 2f;
            Vector2 worldPos = p.ToWorldCoordinates(halfWidth, halfHeight);

            Color color = Lighting.GetColor(p.X, p.Y);

            //这与我们之前注册的备用磁贴数据有关
            bool direction = tile.TileFrameY / FrameHeight != 0;
            SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 drawPos = worldPos + offScreen - Main.screenPosition;
            float rotation = 0;
            if (MagikeHelper.TryGetEntityWithTopLeft(i, j, out MagikeSender_Line sender))
            {
                if (sender.receiverPoints[0] == Point16.NegativeOne)
                    drawPos += new Vector2(0, halfHeight - 8);
                else
                    rotation = (sender.receiverPoints[0].ToWorldCoordinates() - p.ToWorldCoordinates()).ToRotation() + 1.57f;
            }

            // 绘制主帖图
            spriteBatch.Draw(texture, drawPos, frame, color, rotation, origin, 1f, effects, 0f);
        }

    }
}
