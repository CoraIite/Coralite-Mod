using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.Tiles
{
    public abstract class BasePrismTile(int width, int height, Color mapColor, int dustType, int frameCount, int minPick = 0, bool topSoild = false)
        : BaseMagikeTile(width, height, mapColor, dustType, minPick, topSoild)
    {
        public override CoraliteSets.MagikeTileType PlaceType => CoraliteSets.MagikeTileType.FourWayNormal;

        public override void DrawExtraTex(SpriteBatch spriteBatch, Texture2D tex, Rectangle tileRect, Vector2 offset, Color lightColor, float rotation, MagikeTP entity, MALevel level)
        {
            Vector2 selfCenter = tileRect.Center();
            Vector2 drawPos = selfCenter + offset;
            int halfHeight = Math.Max(tileRect.Height / 2, tileRect.Width / 2);
            float rotationTop = 0;//rotation + MathHelper.PiOver2;

            //虽然一般不会没有 但是还是检测一下
            if (!entity.TryGetComponent(MagikeComponentID.MagikeSender, out MagikeLinerSender senderComponent))
                return;

            Rectangle frameBox;

            if (senderComponent.IsEmpty())
            {
                drawPos -= rotation.ToRotationVector2() * (halfHeight - 10);
                frameBox = tex.Frame(1, frameCount, 0, 0);
            }
            else
            {
                drawPos += rotation.ToRotationVector2() * (halfHeight - 6);
                int frame = ((tileRect.X / 10) + tileRect.Y + ((int)Main.timeForVisualEffects / 4)) % frameCount;
                frameBox = tex.Frame(1, frameCount, 0, frame);
            }

            // 绘制主帖图
            spriteBatch.Draw(tex, drawPos, frameBox, lightColor, rotationTop, frameBox.Size() / 2, 1f, 0, 0f);
        }
    }

}
