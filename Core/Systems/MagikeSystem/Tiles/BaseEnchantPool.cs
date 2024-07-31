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
using Terraria.GameContent;

namespace Coralite.Core.Systems.MagikeSystem.Tiles
{
    public abstract class BaseEnchantPool : ModTile
    {
        public override string Texture => AssetDirectory.MagikeEnchantPoolTiles + Name;
        public virtual string ExtraTextureName => AssetDirectory.MagikeEnchantPoolTiles + Name + "_Top";

        public Asset<Texture2D> ExtraTexture;
        public const int FrameWidth = 18 * 3;
        public const int FrameHeight = 18 * 2;
        public const int halfWidth = 16 * 2 / 2;
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
            MagikeEnchantUI.visible = false;
            UILoader.GetUIState<MagikeEnchantUI>().Recalculate();

            SoundEngine.PlaySound(CoraliteSoundID.DigStone_Tink, new Vector2(i, j) * 16);
            int x = i - frameX / 18;
            int y = j - frameY / 18;
            if (MagikeHelper.TryGetEntityWithTopLeft(x, y, out MagikeFactory_EnchantPool pool))
                pool.Kill(x, y);
        }

        public override bool RightClick(int i, int j)
        {
            if (MagikeHelper.TryGetEntity(i, j, out MagikeFactory_EnchantPool pool))
            {
                MagikeEnchantUI.visible = true;
                MagikeEnchantUI.tileEntity = pool;
                UILoader.GetUIState<MagikeEnchantUI>().Recalculate();
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

            Texture2D texture = ExtraTexture.Value;

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
            if (MagikeHelper.TryGetEntityWithTopLeft(i, j, out MagikeFactory_EnchantPool pool))
            {
                if (pool.magike > 0) //大于0时才会绘制
                {
                    spriteBatch.Draw(texture, drawPos + new Vector2(0, 2), frame, color, 0f, origin, 1f, effects, 0f);
                }

                if (pool.containsItem is not null && !pool.containsItem.IsAir)
                {
                    int type = pool.containsItem.type;
                    float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 5f);
                    Vector2 pos = drawPos + new Vector2(0f, offset * 4f - halfHeight * 2);

                    Main.instance.LoadItem(type);
                    Texture2D itemTex = TextureAssets.Item[type].Value;
                    Rectangle rectangle2;

                    if (Main.itemAnimations[type] != null)
                        rectangle2 = Main.itemAnimations[type].GetFrame(itemTex, -1);
                    else
                        rectangle2 = itemTex.Frame();

                    Vector2 origin2 = rectangle2.Size() / 2;
                    float itemScale = 1f;
                    const float pixelWidth = 16 * 2;      //同样的魔法数字，是物品栏的长和宽（去除了边框的）
                    const float pixelHeight = 16 * 3;
                    if (rectangle2.Width > pixelWidth || rectangle2.Height > pixelHeight)
                    {
                        if (rectangle2.Width > pixelWidth)
                            itemScale = pixelWidth / rectangle2.Width;
                        else
                            itemScale = pixelHeight / rectangle2.Height;
                    }

                    spriteBatch.Draw(itemTex, pos, new Rectangle?(rectangle2), pool.containsItem.GetAlpha(Color.White), 0f, origin2, itemScale, 0, 0f);
                    if (pool.containsItem.color != default(Color))
                        spriteBatch.Draw(itemTex, pos, new Rectangle?(rectangle2), pool.containsItem.GetColor(Color.White), 0f, origin2, itemScale, 0, 0f);
                }
            }
        }
    }
}
