using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Helpers
{
    public static class MagikeHelper
    {
        public static MagikeItem GetMagikeItem(this Item item)
        {
            return item.GetGlobalItem<MagikeItem>();
        }

        public static void SpawnDustOnSend(int selfWidth, int selfHeight, Point16 Position, IMagikeContainer container, Color dustColor, int dustType = DustID.Teleporter)
        {
            Tile tile = Framing.GetTileSafely(container.GetPosition);
            TileObjectData data = TileObjectData.GetTileData(tile);
            int xOffset = data == null ? 16 : data.Width * 8;
            int yOffset = data == null ? 24 : data.Height * 8;

            Vector2 selfPos = Position.ToWorldCoordinates(selfWidth * 8, selfHeight * 8);
            Vector2 receiverPos = container.GetPosition.ToWorldCoordinates(xOffset, yOffset);
            Vector2 dir = selfPos.DirectionTo(receiverPos);
            float length = Vector2.Distance(selfPos, receiverPos);

            while (length > 0)
            {
                Dust dust = Dust.NewDustPerfect(selfPos + dir * length, dustType, dir * 0.2f, newColor: dustColor);
                dust.noGravity = true;
                length -= 8;
            }
        }

        public static void SpawnDustOnItemSend(int selfWidth, int selfHeight, Point16 Position, Color dustColor, int dustType = DustID.VilePowder)
        {
            Tile tile = Framing.GetTileSafely(Position);
            TileObjectData data = TileObjectData.GetTileData(tile);
            int xOffset = data == null ? 16 : data.Width * 8;
            int yOffset = data == null ? 24 : data.Height * 8;

            Vector2 selfPos = Position.ToWorldCoordinates(selfWidth * 8, selfHeight * 8);
            Vector2 receiverPos = Position.ToWorldCoordinates(xOffset, yOffset);
            Vector2 dir = selfPos.DirectionTo(receiverPos);
            float length = Vector2.Distance(selfPos, receiverPos);

            while (length > 0)
            {
                Dust dust = Dust.NewDustPerfect(selfPos + dir * length, dustType, dir * 0.2f, newColor: dustColor);
                dust.noGravity = true;
                length -= 8;
            }

            for (int i = 0; i < 16; i++)
            {
                Dust dust = Dust.NewDustPerfect(receiverPos, dustType, (i * MathHelper.TwoPi / 16).ToRotationVector2() * Main.rand.NextFloat(2, 3), newColor: dustColor);
                dust.noGravity = true;
            }
        }


        public static void SpawnDustOnGenerate(int selfWidth, int selfHeight, Point16 Position, Color dustColor, int dustType = DustID.LastPrism)
        {
            Vector2 position = Position.ToWorldCoordinates(selfWidth * 8, selfHeight * 8);
            for (int i = 0; i < 16; i++)
            {
                Dust dust = Dust.NewDustPerfect(position, dustType, (i * MathHelper.TwoPi / 16).ToRotationVector2() * Main.rand.NextFloat(2, 3), newColor: dustColor);
                dust.noGravity = true;
            }
        }

        /// <summary>
        /// 根据当前物块的帧图获取到物块左上角，之后根据位置尝试获取指定类型的TileEntity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool TryGetEntity<T>(int i, int j, out T entity) where T : class
        {
            Tile tile = Framing.GetTileSafely(i, j);
            TileObjectData data = TileObjectData.GetTileData(tile);
            int frameX = tile.TileFrameX;
            int frameY = tile.TileFrameY;
            if (data != null)
            {
                frameX %= data.Width * 18;
                frameY %= data.Height * 18;
            }

            int x = frameX / 18;
            int y = frameY / 18;
            Point16 position = new Point16(i - x, j - y);

            if (TileEntity.ByPosition.ContainsKey(position) && TileEntity.ByPosition[position] is T tEntity)
            {
                entity = tEntity;
                return true;
            }

            entity = null;
            return false;
        }

        public static bool TryGetEntityWithTopLeft<T>(int top, int left, out T entity) where T : class
        {
            Point16 position = new Point16(top, left);

            if (TileEntity.ByPosition.ContainsKey(position) && TileEntity.ByPosition[position] is T tEntity)
            {
                entity = tEntity;
                return true;
            }

            entity = null;
            return false;
        }

        /// <summary>
        /// 获取到当前鼠标上的魔能物块并显示魔能物块实体中的魔能量/魔能最大值
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public static void ShowMagikeNumber(int i, int j)
        {
            if (TryGetEntity(i, j, out IMagikeContainer magikeContainer))
                Main.instance.MouseText(magikeContainer.Magike + " / " + magikeContainer.MagikeMax, 0, 0, -1, -1, -1, -1);
        }
    }
}
