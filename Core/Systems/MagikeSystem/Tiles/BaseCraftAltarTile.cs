using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.Tiles
{
    public abstract class BaseCraftAltarTile(int width, int height, Color mapColor, int dustType, int minPick = 0, bool topSoild = false)
        : BaseMagikeTile(width, height, mapColor, dustType, minPick, topSoild)
    {
        /// <summary>
        /// 悬浮位置相对于中心的偏移
        /// </summary>
        /// <param name="alternate"></param>
        /// <returns></returns>
        public virtual Vector2 GetFloatingOffset(float rotation, MALevel level)
        {
            return Vector2.Zero;
        }

        /// <summary>
        /// 静止位置相对于中心的偏移
        /// </summary>
        /// <param name="alternate"></param>
        /// <returns></returns>
        public virtual Vector2 GetRestOffset(float rotation, MALevel level)
        {
            return Vector2.Zero;
        }

        public override void DrawExtraTex(SpriteBatch spriteBatch, Texture2D tex, Rectangle tileRect, Vector2 offset, Color lightColor, float rotation, MagikeTP entity, MALevel level)
        {
            if (!entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
                return;

            Item item = GetFirstItem(container);
            Vector2 center = tileRect.Center() + offset;
            Rectangle frameBox;

            //没有物品就静置在基座上
            if (item == null)
            {
                Vector2 restOffset = GetRestOffset(rotation, level);
                frameBox = tex.Frame(3, 1);

                spriteBatch.Draw(tex, center + restOffset, frameBox, lightColor, rotation + 1.57f, frameBox.Size() / 2, 1, 0, 0);
                return;
            }

            if (!entity.TryGetComponent(MagikeComponentID.MagikeFactory, out CraftAltar altar))
                return;

            //有物品直接悬浮
            Vector2 floatingOffset = GetFloatingOffset(rotation, level);

            Color c = lightColor;

            //正在合成时就转动
            if (altar.IsWorking && altar.ChosenResipe != null)
            {
                frameBox = tex.Frame(3, 1, 2);
                rotation = (float)Main.timeForVisualEffects * 0.1f;
                c *= (float)altar.RequiredMagike / altar.ChosenResipe.magikeCost;
            }
            else
            {
                frameBox = tex.Frame(3, 1, 1);
            }

            spriteBatch.Draw(tex, center + floatingOffset, frameBox, lightColor, rotation + 1.57f, frameBox.Size() / 2, 1, 0, 0);

            //绘制物品
            MagikeHelper.DrawItem(spriteBatch, item, center + floatingOffset, Math.Min(tileRect.Width, tileRect.Height), c);
        }

        private static Item GetFirstItem(ItemContainer container)
        {
            Item i = null;
            foreach (var item in container.Items)
            {
                if (item.IsAir)
                    continue;

                i = item;
                break;
            }

            return i;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            if (!MagikeHelper.TryGetEntity(i, j, out MagikeTP entity))
                return true;

            if (!entity.TryGetComponent(MagikeComponentID.MagikeFactory, out MagikeFactory factory))
                return true;

            return !factory.IsWorking;
        }
    }
}
