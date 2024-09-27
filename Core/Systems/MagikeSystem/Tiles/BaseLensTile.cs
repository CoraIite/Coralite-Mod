using Coralite.Core.Systems.MagikeSystem.Components.Producers;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.Tiles
{
    public abstract class BaseLensTile(Color mapColor, int dustType, int width = 2, int height = 3, int minPick = 0)
        : BaseMagikeTile(width, height, mapColor, dustType, minPick)
    {
        public override string Texture => AssetDirectory.MagikeLensTiles + Name;

        public override void DrawExtraTex(SpriteBatch spriteBatch, Texture2D tex, Rectangle tileRect, Vector2 offset, Color lightColor, float rotation, MagikeTileEntity entity, MagikeApparatusLevel level)
        {
            Vector2 selfCenter = tileRect.Center();
            Vector2 drawPos = selfCenter + offset;
            int halfHeight = Math.Max(tileRect.Height / 2, tileRect.Width / 2);

            //虽然一般不会没有 但是还是检测一下
            if (!entity.TryGetComponent(MagikeComponentID.MagikeProducer, out MagikeProducer producer))
                return;

            bool canProduce = producer.CanProduce();

            if (canProduce)
            {
                const float TwoPi = (float)Math.PI * 2f;
                float offset2 = (float)Math.Sin((Main.GlobalTimeWrappedHourly + tileRect.X + tileRect.Y) * TwoPi / 5f);
                drawPos += new Vector2(0f, offset2 * 4f);
            }
            else
                drawPos -= rotation.ToRotationVector2() * (halfHeight - ((tileRect.Width > tileRect.Height ? tex.Width : tex.Height) / 2) - 4);

            // 绘制主帖图
            DrawTopTex(spriteBatch, tex, drawPos, lightColor, level,canProduce);
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
        public virtual void DrawTopTex(SpriteBatch spriteBatch, Texture2D tex, Vector2 drawPos, Color lightColor, MagikeApparatusLevel level,bool canProduce)
        {
            spriteBatch.Draw(tex, drawPos, null, lightColor, 0, tex.Size() / 2, 1f, 0, 0f);
        }
    }
}
