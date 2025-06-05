using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class ThyphionRelic : BaseRelicItem
    {
        public ThyphionRelic() : base(ModContent.TileType<ThyphionRelicTile>(), AssetDirectory.ThyphionSeriesItems) { }
    }

    public class ThyphionRelicTile : BaseBossRelicTile
    {
        public override string Texture => AssetDirectory.MiscTiles + "ThyphionRelicPedestal";
        public override string RelicTextureName => AssetDirectory.MiscTiles + Name;

        public override bool CanDrop(int i, int j) => true;

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return
            [
                new(ModContent.ItemType<ThyphionRelic>())
            ];
        }

        public override bool RightClick(int i, int j)
        {
            Thyphion.Skin = !Thyphion.Skin;
            return true;
        }

        public static float GetRot()
        {
            float time = Main.GlobalTimeWrappedHourly % MathHelper.Pi;

            float ease = (MathF.Sin(MathF.Cos(time) + 0.7f) + 0.29552f) / 1.29552f;

            return (1-ease)*MathHelper.TwoPi;
        }

        public override void DrawRelicTop(SpriteBatch spriteBatch, Texture2D texture, Vector2 offScreen, Point p, Tile tile)
        {
            //绘制底部
            Rectangle frame = texture.Frame(3, 1, 2, 0);

            Vector2 origin = frame.Size() / 2f;
            Vector2 worldPos = p.ToWorldCoordinates(24f, 72f);

            Color color = Lighting.GetColor(p.X, p.Y);

            bool direction = tile.TileFrameY / FrameHeight != 0;
            SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            const float TwoPi = (float)Math.PI * 2f;
            float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);

            Vector2 drawPos = worldPos + offScreen - Main.screenPosition + new Vector2(0f, -38f);

            //绘制云/树枝
            spriteBatch.Draw(texture, drawPos , frame, color, 0, origin, 1f, effects, 0f);
            drawPos += new Vector2(0f, offset * 4f);

            frame = texture.Frame(3, 1, 0, 0);
            // 绘制球体
            spriteBatch.Draw(texture, drawPos+new Vector2(-2,0), frame, color, GetRot(), origin, 1f, effects, 0f);
            frame = texture.Frame(3, 1, 1, 0);


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