using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;

namespace Coralite.Core.Systems.MagikeSystem.Tiles
{
    public abstract class BasePedestalTile(int width, int height, Color mapColor, int dustType, int minPick = 0)
        : BaseMagikeTile(width, height, mapColor, dustType, minPick)
    {
        public const float ItemSize = 36;

        public override void QuickLoadAsset(MagikeApparatusLevel level)
        {

        }

        public override void DrawExtra(SpriteBatch spriteBatch, Rectangle tileRect, Vector2 offset, Color lightColor, float rotation, MagikeTileEntity entity)
        {
            Vector2 selfCenter = tileRect.Center();
            Vector2 drawPos = selfCenter + offset - Main.screenPosition;

            //虽然一般不会没有 但是还是检测一下
            if (!entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
                return;

            if (!container.Items[0].IsAir)
                DrawItem(spriteBatch, container.Items[0], drawPos + rotation.ToRotationVector2()
                    * (18 + ItemSize / 2 + MathF.Sin(Main.GlobalTimeWrappedHourly + (tileRect.X + tileRect.Y) * 0.1f) * 6));
        }

        public static void DrawItem(SpriteBatch spriteBatch, Item i, Vector2 pos)
        {
            int type = i.type;

            Main.instance.LoadItem(type);
            Texture2D itemTex = TextureAssets.Item[type].Value;
            Rectangle rectangle2;

            if (Main.itemAnimations[type] != null)
                rectangle2 = Main.itemAnimations[type].GetFrame(itemTex, -1);
            else
                rectangle2 = itemTex.Frame();

            Vector2 origin = rectangle2.Size() / 2;
            float itemScale = 1f;

            if (rectangle2.Width > ItemSize || rectangle2.Height > ItemSize)
            {
                if (rectangle2.Width > ItemSize)
                    itemScale = ItemSize / rectangle2.Width;
                else
                    itemScale = ItemSize / rectangle2.Height;
            }

            spriteBatch.Draw(itemTex, pos, new Rectangle?(rectangle2), i.GetAlpha(Color.White), 0f, origin, itemScale, 0, 0f);
            if (i.color != default)
                spriteBatch.Draw(itemTex, pos, new Rectangle?(rectangle2), i.GetColor(Color.White), 0f, origin, itemScale, 0, 0f);
        }

        /// <summary>
        /// 可以根据不同等级自定义绘制
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="tex"></param>
        /// <param name="drawPos"></param>
        /// <param name="lightColor"></param>
        /// <param name="rotation"></param>
        /// <param name="level"></param>
        public virtual void DrawTopTex(SpriteBatch spriteBatch, Texture2D tex, Vector2 drawPos, Color lightColor, MagikeApparatusLevel level)
        {
            spriteBatch.Draw(tex, drawPos, null, lightColor, 0, tex.Size() / 2, 1f, 0, 0f);
        }
    }
}
