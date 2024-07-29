using Coralite.Content.UI;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.MagikeSystem.Tiles
{
    public abstract class BaseLensTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeLensTiles + Name;
        public virtual string TopTextureName => AssetDirectory.MagikeLensTiles + Name + "_Top";

        public Asset<Texture2D> TopTexture;
        public const int FrameWidth = 18 * 2;
        public const int FrameHeight = 18 * 3;
        public const int halfWidth = 16 * 2 / 2;
        public const int halfHeight = 16 * 3 / 2;
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
            if (MagikeHelper.TryGetEntityWithTopLeft(x, y, out MagikeGenerator generator))
                generator.Kill(x, y);
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
            Vector2 offScreen = new Vector2(Main.offScreenRange);
            if (Main.drawToScreen)
                offScreen = Vector2.Zero;

            //检查物块它是否真的存在
            Point p = new Point(i, j);
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

            // 一些数学魔法，使其随着时间的推移平稳地上下移动
            Vector2 drawPos = worldPos + offScreen - Main.screenPosition;
            if (MagikeHelper.TryGetEntityWithTopLeft(i, j, out IMagikeContainer container))
            {
                if (container.Active)   //如果处于活动状态那么就会上下移动，否则就落在底座上
                {
                    const float TwoPi = (float)Math.PI * 2f;
                    float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);
                    drawPos += new Vector2(0f, offset * 4f);
                }
                else
                    drawPos += new Vector2(0, halfHeight - 16);
            }

            // 绘制主帖图
            spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);

            // 绘制周期性发光效果
            //float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 2f) * 0.3f + 0.7f;
            //Color effectColor = color;
            //effectColor.A = 0;
            //effectColor = effectColor * 0.1f * scale;
            //for (float m = 0f; m < 1f; m += 355f / (678f * (float)Math.PI))
            //    spriteBatch.Draw(texture, drawPos + (TwoPi * m).ToRotationVector2() * (6f + offset * 2f), frame, effectColor, 0f, origin, 1f, effects, 0f);
        }
    }


    public abstract class BaseCostItemLensTile : BaseLensTile
    {
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            MagikeItemSlotPanel.visible = false;
            UILoader.GetUIState<MagikeItemSlotPanel>().Recalculate();

            base.KillMultiTile(i, j, frameX, frameY);
        }

        public override bool RightClick(int i, int j)
        {
            if (MagikeHelper.TryGetEntity(i, j, out MagikeGenerator_FromMagItem generator))
            {
                MagikeItemSlotPanel.visible = true;
                MagikeItemSlotPanel.tileEntity = generator;
                UILoader.GetUIState<MagikeItemSlotPanel>().Recalculate();
            }

            return true;
        }
    }
}
