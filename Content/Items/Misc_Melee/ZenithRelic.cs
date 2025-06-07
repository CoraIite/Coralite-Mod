using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Items.Misc_Melee
{
    public class ZenithRelic : BaseRelicItem
    {
        public ZenithRelic() : base(ModContent.TileType<ZenithRelicTile>(), AssetDirectory.Misc_Melee) { }
    }

    public class ZenithRelicTile : BaseBossRelicTile
    {
        public override string Texture => AssetDirectory.MiscTiles + "ZenithRelicPedestal";
        public override string RelicTextureName => AssetDirectory.MiscTiles + Name;

        public override bool CanDrop(int i, int j) => true;

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return
            [
                new(ModContent.ItemType<ZenithRelic>())
            ];
        }

        public override void DrawRelicTop(SpriteBatch spriteBatch, Texture2D texture, Vector2 offScreen, Point p, Tile tile)
        {
            //绘制底部
            Rectangle frame = texture.Frame(2, 1, 1, 0);

            Vector2 origin = frame.Size() / 2f;
            Vector2 worldPos = p.ToWorldCoordinates(24f, 69f);

            Color color = Lighting.GetColor(p.X, p.Y);

            bool direction = tile.TileFrameY / FrameHeight != 0;
            SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawPos = worldPos + offScreen - Main.screenPosition + new Vector2(0, -40);

            // 绘制底座
            spriteBatch.Draw(texture, drawPos + new Vector2(0, 5), frame, color, 0f, origin, 1f, effects, 0f);

            //绘制悬浮
            const float TwoPi = (float)Math.PI * 2f;
            float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);

            frame = texture.Frame(2, 1, 0, 0);
            drawPos = worldPos + offScreen - Main.screenPosition + new Vector2(0f, -38f) + new Vector2(0f, offset * 4f);

            spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);

            // 绘制周期性发光效果
            float scale = ((float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 2f) * 0.3f) + 0.7f;
            Color effectColor = color;
            effectColor.A = 0;
            effectColor = effectColor * 0.1f * scale;
            for (float m = 0f; m < 1f; m += 355f / (678f * (float)Math.PI))
                spriteBatch.Draw(texture, drawPos + ((TwoPi * m).ToRotationVector2() * (6f + (offset * 2f))), frame, effectColor, 0f, origin, 1f, effects, 0f);
        }
    }
}
