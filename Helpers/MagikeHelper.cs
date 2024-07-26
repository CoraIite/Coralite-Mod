using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.ObjectData;

namespace Coralite.Helpers
{
    public static class MagikeHelper
    {
        public static MagikeItem GetMagikeItem(this Item item)
        {
            return item.GetGlobalItem<MagikeItem>();
        }

        /// <summary>
        /// 从实体上获取魔能容器，请使用<see cref="IsMagikeContainer(IEntity)"/>来检测是否为魔能容器
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Core.Systems.MagikeSystem.Components.MagikeContainer GetMagikeContainer(this IEntity entity)
            => entity.GetSingleComponent(MagikeComponentID.MagikeContainer) as Core.Systems.MagikeSystem.Components.MagikeContainer;

        /// <summary>
        /// 检测实体是否是一个魔能容器
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool IsMagikeContainer(this IEntity entity)
            => entity.HasComponent(MagikeComponentID.MagikeContainer);

        /// <summary>
        /// 尝试从<see cref="TileEntity"/>种获取<see cref="IEntity"/><br></br>
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool TryGetEntity(int i, int j, out BaseMagikeTileEntity entity)
        {
            entity = null;

            Point16? topLeft = ToTopLeft(i, j);
            if (!topLeft.HasValue)
                return false;

            if (!TryGetEntity(topLeft.Value, out BaseMagikeTileEntity tempEntity))
                return false;

            entity = tempEntity;
            return true;
        }

        /// <summary>
        /// 尝试从<see cref="TileEntity"/>种获取<see cref="IEntity"/><br></br>
        /// 必须传入的为左上角的位置才行
        /// </summary>
        /// <param name="position"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool TryGetEntity(Point16 position, out BaseMagikeTileEntity entity)
        {
            entity = null;
            if (!TileEntity.ByPosition.TryGetValue(position, out TileEntity tileEntity))
                return false;

            if (tileEntity is not BaseMagikeTileEntity entity1)
                return false;

            entity = entity1;
            return true;
        }

        /// <summary>
        /// 尝试获取带指定组件的实体
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="componentType"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool TryGetEntityWithComponent(int i, int j, int componentType, out BaseMagikeTileEntity entity)
        {
            entity = null;

            if (!TryGetEntity(i, j, out BaseMagikeTileEntity tempEntity))
                return false;

            if (!((IEntity)tempEntity).HasComponent(componentType))
                return false;

            entity = tempEntity;
            return true;
        }

        /// <summary>
        /// 尝试获取带指定组件的实体，同时限定基类
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="componentType"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool TryGetEntityWithComponent<T>(int i, int j, int componentType, out BaseMagikeTileEntity entity)
            where T : Component
        {
            entity = null;

            if (!TryGetEntityWithComponent(i, j, componentType, out BaseMagikeTileEntity tempEntity))
                return false;

            if (!tempEntity.Components[componentType].Any(c => c is T))
                return false;

            entity = tempEntity;
            return true;
        }

        /// <summary>
        /// 使用两个位置数位置获取物块左上角的位置
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static Point16? ToTopLeft(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (!tile.HasTile)
                return null;

            TileObjectData data= TileObjectData.GetTileData(tile);

            if (data == null)
                return new Point16(i, j);

            if (!Main.tileSolidTop[tile.TileType])
                GetMagikeAlternateData(i, j, out data, out _);

            int frameX = tile.TileFrameX;
            int frameY = tile.TileFrameY;
            if (data != null)
            {
                frameX %= data.Width * 18;
                frameY %= data.Height * 18;
            }

            int x = frameX / 18;
            int y = frameY / 18;
            return new Point16(i - x, j - y);
        }

        public static void GetMagikeAlternateData(int i, int j, out TileObjectData alternateData, out int alternate)
        {
            Tile t = Main.tile[i, j];
            TileObjectData tileData = TileObjectData.GetTileData(t.TileType, 0, 0);

            int width = tileData.Width;
            int height = tileData.Height;
            int y1 = t.TileFrameY / 18;

            alternate = 0;

            if (y1 < height)
                alternateData = tileData;
            else if (y1 < height * 2)
            {
                alternate = 1;
                alternateData = TileObjectData.GetTileData(t.TileType, 0, alternate + 1);
            }
            else if (y1 < height * 2 + width)
            {
                alternate = 2;
                alternateData = TileObjectData.GetTileData(t.TileType, 0, alternate + 1);
            }
            else
            {
                alternate = 3;
                alternateData = TileObjectData.GetTileData(t.TileType, 0, alternate + 1);
            }
        }

        public static int? GetFrameX(Point16 topLeft)
        {
            Tile tile = Framing.GetTileSafely(topLeft);
            if (!tile.HasTile)
                return null;

            TileObjectData data = TileObjectData.GetTileData(tile);

            return tile.TileFrameX / (data.Width * 18);
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

            if (TileEntity.ByPosition.TryGetValue(position, out TileEntity value) && value is T tEntity)
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

            if (TileEntity.ByPosition.TryGetValue(position, out TileEntity value) && value is T tEntity)
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

        /// <summary>
        /// 消耗玩家背包中的魔能，前提是玩家背包内拥有可提供魔能的物品
        /// </summary>
        /// <param name="player"></param>
        /// <param name="howMany"></param>
        /// <returns></returns>
        public static bool TryCosumeMagike(this Player player, int howMany)
        {
            for (int i = 0; i < 58; i++)
            {
                Item item = player.inventory[i];
                if (!item.IsAir && item.TryGetGlobalItem(out MagikeItem mi) && mi.magikeSendable && mi.magike >= howMany)
                {
                    mi.magike -= howMany;
                    return true;
                }
            }

            if (player.useVoidBag())
                for (int i = 0; i < player.bank4.item.Length; i++)
                {
                    Item item = player.bank4.item[i];
                    if (!item.IsAir && item.TryGetGlobalItem(out MagikeItem mi) && mi.magikeSendable && mi.magike >= howMany)
                    {
                        mi.magike -= howMany;
                        return true;
                    }
                }

            return false;
        }

        /// <summary>
        /// 尝试消耗物品自身的魔能
        /// </summary>
        /// <param name="item"></param>
        /// <param name="howMany"></param>
        /// <returns></returns>
        public static bool TryCosumeMagike(this Item item, int howMany)
        {
            if (item.TryGetGlobalItem(out MagikeItem mi) && mi.magike >= howMany)
            {
                mi.magike -= howMany;
                return true;
            }

            return false;
        }

        //public static void DrawFragmentPageBackground(SpriteBatch spriteBatch,Vector2 center)
        //{
        //    Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "Fragment").Value;
        //    spriteBatch.Draw(mainTex, center, null, Color.White, 0, mainTex.Size() / 2, 1, 0, 0);
        //}
    }
}
