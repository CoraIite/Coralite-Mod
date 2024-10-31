﻿using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.MagikeSystem.Tiles
{
    public abstract class BaseRefractorTile(int width, int height, Color mapColor, int dustType, int minPick = 0, bool topSoild = false)
        : BaseMagikeTile(width, height, mapColor, dustType, minPick, topSoild)
    {
        public override void DrawExtraTex(SpriteBatch spriteBatch, Texture2D tex, Rectangle tileRect, Vector2 offset, Color lightColor, float rotation, MagikeTP entity, MALevel level)
        {
            Vector2 selfCenter = tileRect.Center();
            Vector2 drawPos = selfCenter + offset;
            int halfHeight = Math.Max(tileRect.Height / 2, tileRect.Width / 2);
            float rotationTop = rotation + MathHelper.PiOver2;

            //虽然一般不会没有 但是还是检测一下
            if (!entity.TryGetComponent(MagikeComponentID.MagikeSender, out MagikeLinerSender senderComponent))
                return;

            if (senderComponent.IsEmpty())
                drawPos -= rotation.ToRotationVector2() * (halfHeight - 8);
            else
            {
                Point16 p = senderComponent.FirstConnector();
                Vector2 targetPos = Helper.GetMagikeTileCenter(p);
                rotationTop = (targetPos - selfCenter).ToRotation() + MathHelper.PiOver2;
            }

            // 绘制主帖图
            spriteBatch.Draw(tex, drawPos, null, lightColor, rotationTop, tex.Size() / 2, 1f, 0, 0f);
        }
    }
}
