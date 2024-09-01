using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.Tiles
{
    public abstract class BaseColumnTile(int width, int height, Color mapColor, int dustType, int minPick = 0, bool topSoild = true)
        : BaseMagikeTile(width, height, mapColor, dustType, minPick, topSoild)
    {
        public override void DrawExtraTex(SpriteBatch spriteBatch, Texture2D tex, Rectangle tileRect, Vector2 offset, Color lightColor, float rotation, MagikeTileEntity entity, MagikeApparatusLevel level)
        {
            Vector2 bottomLeft = tileRect.BottomLeft();
            Vector2 drawPos = bottomLeft + offset;

            //虽然一般不会没有 但是还是检测一下
            if (!entity.TryGetComponent(MagikeComponentID.MagikeContainer, out MagikeContainer container))
                return;

            float percent = (float)container.Magike / container.MagikeMax;

            for (int i = 0; i < tileRect.Width / 2; i++)
            {
                int currentHeight = Math.Clamp(
                   (int)(tex.Height * (percent + (0.04f * MathF.Sin((((float)Main.timeForVisualEffects + tileRect.X + tileRect.Y) * 0.1f) + (i * 0.3f)))))
                    , 0, tex.Height);

                Rectangle frameBox = new(i * 2, tex.Height - currentHeight, 2, currentHeight);
                var origin = new Vector2(0, frameBox.Height);
                spriteBatch.Draw(tex, drawPos + new Vector2(i * 2, 0), frameBox, lightColor, 0, origin, 1f, 0, 0f);
            }
        }
    }
}
