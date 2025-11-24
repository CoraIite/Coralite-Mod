using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.Tiles
{
    public abstract class BasePedestalTile(int width, int height, Color mapColor, int dustType, int minPick = 0)
        : BaseMagikeTile(width, height, mapColor, dustType, minPick)
    {
        public const float ItemSize = 36;

        public override CoraliteSetsSystem.MagikeTileType PlaceType => CoraliteSetsSystem.MagikeTileType.FourWayNormal;

        public override void QuickLoadAsset(MALevel level)
        {

        }

        public override void PreDrawExtra(SpriteBatch spriteBatch, Rectangle tileRect, Vector2 offset, Color lightColor, float rotation, MagikeTP entity)
        {
            Vector2 selfCenter = tileRect.Center();
            Vector2 drawPos = selfCenter + offset - Main.screenPosition;

            //虽然一般不会没有 但是还是检测一下
            if (!entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
                return;

            if (!container.Items[0].IsAir)
                MagikeHelper.DrawItem(spriteBatch, container.Items[0], drawPos + (rotation.ToRotationVector2()
                    * (18 + (ItemSize / 2) + (MathF.Sin(Main.GlobalTimeWrappedHourly + ((tileRect.X + tileRect.Y) * 0.1f)) * 6))), ItemSize, lightColor);
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
        public virtual void DrawTopTex(SpriteBatch spriteBatch, Texture2D tex, Vector2 drawPos, Color lightColor, MALevel level)
        {
            spriteBatch.Draw(tex, drawPos, null, lightColor, 0, tex.Size() / 2, 1f, 0, 0f);
        }
    }
}
