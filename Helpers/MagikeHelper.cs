using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Particles;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using Terraria.UI;

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
        public static bool TryGetEntity(int i, int j, out MagikeTileEntity entity)
        {
            entity = null;

            Point16? topLeft = ToTopLeft(i, j);
            if (!topLeft.HasValue)
                return false;

            if (!TryGetEntity(topLeft.Value, out MagikeTileEntity tempEntity))
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
        public static bool TryGetEntity(Point16 position, out MagikeTileEntity entity)
        {
            entity = null;
            if (!TileEntity.ByPosition.TryGetValue(position, out TileEntity tileEntity))
                return false;

            if (tileEntity is not MagikeTileEntity entity1)
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
        public static bool TryGetEntityWithComponent(int i, int j, int componentType, out MagikeTileEntity entity)
        {
            entity = null;

            if (!TryGetEntity(i, j, out MagikeTileEntity tempEntity))
                return false;

            if (!tempEntity.HasComponent(componentType))
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
        public static bool TryGetEntityWithComponent<T>(int i, int j, int componentType, out MagikeTileEntity entity)
            where T : Component
        {
            entity = null;

            if (!TryGetEntityWithComponent(i, j, componentType, out MagikeTileEntity tempEntity))
                return false;

            if (!tempEntity.Components.Contains(componentType))
                return false;

            if (MagikeComponentID.IsSingleton(componentType))
            {
                if (tempEntity.Components[componentType] is not T)
                    return false;
            }
            else
            {
                if (!(tempEntity.Components[componentType] as List<Component>).Any(c => c is T))
                    return false;
            }

            entity = tempEntity;
            return true;
        }

        /// <summary>
        /// 使用两个位置数位置获取物块左上角的位置
        /// 只在没有物块时返回<see cref="null"/>
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static Point16? ToTopLeft(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (!tile.HasTile)
                return null;

            TileObjectData data = TileObjectData.GetTileData(tile);

            if (data == null)
                return new Point16(i, j);

            if (!Main.tileSolidTop[tile.TileType])
                GetMagikeAlternateData(i, j, out data, out _);

            int frameX = tile.TileFrameX;
            int frameY = tile.TileFrameY;
            if (data != null)
            {
                frameX %= data.Width * (data.CoordinateWidth + data.CoordinatePadding);
                frameY %= data.Height * 18;
            }

            int x = frameX / 18;
            int y = frameY / 18;
            return new Point16(i - x, j - y);
        }

        /// <summary>
        /// 获取魔能仪器的<see cref="TileObjectData"/>，对于没有这个的会返回默认值<br></br>
        /// 使用<see cref="Main.tileSolidTop"/>来判断是否有特殊的摆放形式
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="alternateData">正常0，挂天花板1，底座在左2，底座在右3</param>
        /// <param name="alternate"></param>
        public static void GetMagikeAlternateData(int i, int j, out TileObjectData alternateData, out MagikeAlternateStyle alternate)
        {
            Tile t = Main.tile[i, j];
            TileObjectData tileData = TileObjectData.GetTileData(t.TileType, 0, 0);

            if (tileData == null)
            {
                alternateData = null;
                alternate = 0;
                return;
            }

            if (Main.tileSolidTop[t.TileType])
            {
                alternateData = tileData;
                alternate = MagikeAlternateStyle.None;
                return;
            }

            int width = tileData.Width;
            int height = tileData.Height;
            int y1 = t.TileFrameY / (tileData.CoordinateWidth + tileData.CoordinatePadding);


            if (y1 < height)
            {
                alternate = MagikeAlternateStyle.Bottom;
                alternateData = tileData;
            }
            else if (y1 < height * 2)
            {
                alternate = MagikeAlternateStyle.Top;
                alternateData = TileObjectData.GetTileData(t.TileType, 0, (int)alternate + 1);
            }
            else if (y1 < height * 2 + width)
            {
                alternate = MagikeAlternateStyle.Left;
                alternateData = TileObjectData.GetTileData(t.TileType, 0, (int)alternate + 1);
            }
            else
            {
                alternate = MagikeAlternateStyle.Right;
                alternateData = TileObjectData.GetTileData(t.TileType, 0, (int)alternate + 1);
            }
        }

        public enum MagikeAlternateStyle
        {
            None = -1,
            Bottom,
            Top,
            Left,
            Right,
        }

        /// <summary>
        /// 尝试获取魔能仪器的等级
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static bool TryGetMagikeApparatusLevel(Point16 topLeft, out MagikeApparatusLevel level)
        {
            level = MagikeApparatusLevel.None;

            Tile targetTile = Framing.GetTileSafely(topLeft);
            GetMagikeAlternateData(topLeft.X, topLeft.Y, out TileObjectData alternateData, out _);

            if (alternateData == null)
                return false;

            //计算当前帧图
            MagikeApparatusLevel? chooseLevel = MagikeSystem.FrameToLevel(targetTile.TileType, targetTile.TileFrameX / alternateData.CoordinateFullWidth);

            if (!chooseLevel.HasValue)
                return false;

            level = chooseLevel.Value;
            return true;
        }

        /// <summary>
        /// 检测是实体否能升级
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="incomeLevel"></param>
        /// <returns></returns>
        public static bool CheckUpgrageable(this IEntity Entity,MagikeApparatusLevel incomeLevel)
        {
            int tileType = (Entity as MagikeTileEntity).TileType;

            if (!MagikeSystem.MagikeApparatusLevels.TryGetValue(tileType, out var keyValuePairs))
                return false;

            if (!keyValuePairs.ContainsKey(incomeLevel))
                return false;

            return true;
        }

        /// <summary>
        /// 生成菱形特效
        /// </summary>
        /// <param name="pos"></param>
        public static void SpawnLozengeParticle(Point16 pos)
        {
            Point16? topLeft = ToTopLeft(pos.X, pos.Y);
            if (topLeft.HasValue)
            {
                GetMagikeAlternateData(pos.X, pos.Y, out TileObjectData data, out _);
                Point16 size = data == null ? new Point16(1) : new Point16(data.Width, data.Height);

                if (TryGetMagikeApparatusLevel(topLeft.Value, out MagikeApparatusLevel level))
                    MagikeLozengeParticle.Spawn(Helper.GetMagikeTileCenter(topLeft.Value), size, MagikeSystem.GetColor(level));
            }
        }

        /// <summary>
        /// 生成菱形特效
        /// </summary>
        /// <param name="topLeft"></param>
        public static void SpawnLozengeParticle_WithTopLeft(Point16 topLeft)
        {
            GetMagikeAlternateData(topLeft.X, topLeft.Y, out TileObjectData data, out _);
            Point16 size = data == null ? new Point16(1) : new Point16(data.Width, data.Height);

            if (TryGetMagikeApparatusLevel(topLeft, out MagikeApparatusLevel level))
                MagikeLozengeParticle.Spawn(Helper.GetMagikeTileCenter(topLeft), size, MagikeSystem.GetColor(level));
        }

        public static void DrawRectangleFrame(SpriteBatch spriteBatch, Point16 p1, Point16 p2, Color color)
        {
            int x = Math.Min(p1.X, p2.X);
            int y = Math.Min(p1.Y, p2.Y);

            DrawRectangleFrame(spriteBatch, new Rectangle(x, y, Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y)), color);
        }

        /// <summary>
        /// 绘制一个矩形区域
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="rect"></param>
        public static void DrawRectangleFrame(SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            int width = rect.Width + 1;
            int height = rect.Height + 1;

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    if (!GetFrame(i, j, width, height, out Point framePoint))
                        continue;

                    Texture2D mainTex = MagikeSystem.SelectFrame.Value;
                    Rectangle frame = mainTex.Frame(4, 4, framePoint.X, framePoint.Y);

                    Vector2 pos = new Vector2(rect.X + i, rect.Y + j) * 16 - Main.screenPosition;

                    spriteBatch.Draw(mainTex, pos, frame, color);
                }
        }

        private static bool GetFrame(int currentX, int currentY, int maxX, int maxY, out Point framePoint)
        {
            framePoint = new Point();
            bool maxXIsZero = maxX == 1;
            bool maxYIsZero = maxY == 1;

            bool topBar = currentY == 0;
            bool bottomBar = currentY == maxY - 1;

            bool leftBar = currentX == 0;
            bool rightBar = currentX == maxX - 1;

            if (maxXIsZero && maxYIsZero)
            {
                //对应贴图为右下角的单块
                framePoint = new Point(3, 3);
                return true;
            }

            if (maxXIsZero)
            {
                //对应贴图为最右方的一条
                framePoint.X = 3;

                if (topBar)//右上
                    framePoint.Y = 0;
                else if (bottomBar)//右边下
                    framePoint.Y = 2;
                else//右边中间
                    framePoint.Y = 1;

                return true;
            }

            if (maxYIsZero)
            {
                //对应贴图为最下方的一条
                framePoint.Y = 3;

                if (leftBar)//左下
                    framePoint.X = 0;
                else if (rightBar)//下面中
                    framePoint.X = 2;
                else//下面右
                    framePoint.X = 1;

                return true;
            }

            //正常部分，中心不绘制

            if (topBar)//最上面一条
            {
                framePoint.Y = 0;

                if (leftBar)//左边拐角
                    framePoint.X = 0;
                else if (rightBar)//右边拐角
                    framePoint.X = 2;
                else//上面一条
                    framePoint.X = 1;

                return true;
            }
            else if (bottomBar)//最下面一条
            {
                framePoint.Y = 2;

                if (leftBar)//左边拐角
                    framePoint.X = 0;
                else if (rightBar)//右边拐角
                    framePoint.X = 2;
                else//上面一条
                    framePoint.X = 1;

                return true;
            }
            else if (leftBar)
            {
                framePoint.X = 0;
                framePoint.Y = 1;
                return true;
            }
            else if (rightBar)
            {
                framePoint.X = 2;
                framePoint.Y = 1;
                return true;
            }

            return false;
        }

        public static void SpawnDustOnSend(Point16 selfPos, Point16 targetPos, int dustType = DustID.Teleporter)
        {
            if (!TryGetMagikeApparatusLevel(selfPos, out MagikeApparatusLevel level))
                return;

            Color dustColor = MagikeSystem.GetColor(level);
            Vector2 selfCenter = Helper.GetTileCenter(selfPos);
            Vector2 targetCenter = Helper.GetTileCenter(targetPos);

            Vector2 dir = (targetCenter - selfCenter).SafeNormalize(Vector2.Zero);
            float length = Vector2.Distance(selfCenter, targetCenter);

            while (length > 0)
            {
                Dust dust = Dust.NewDustPerfect(selfCenter + dir * length, dustType, dir * 0.2f, newColor: dustColor);
                dust.noGravity = true;
                length -= 8;
            }
        }

        /// <summary>
        /// 获取魔能容器组件的魔能上限应该有的颜色<br></br>
        /// 上限大于基础值就为蓝色，否则为红色，不变是白色
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static string GetMagikeContainerMaxColorCode(this MagikeContainer container)
        {
            if (container.MagikeMaxExtra > 0)
                return "80d3ff";//蓝色
            else if (container.MagikeMaxExtra < 0)
                return "ff1919";//红色
            else
                return "ffffff";
        }

        /// <summary>
        /// 添加标题
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="component"></param>
        /// <param name="id"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static UIElement AddTitle<TComponent>(this TComponent component, MagikeSystem.UITextID id,UIElement parent)
            where TComponent : Component
        {
            UIElement title = new ComponentUIElementText<TComponent>(c =>
                 MagikeSystem.GetUIText(id), component, parent, new Vector2(1.3f));
            parent.Append(title);

            return title;
        }

        /// <summary>
        /// 新建一个文字UI元素
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="component"></param>
        /// <param name="textFunc"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static ComponentUIElementText<TComponent> NewTextBar<TComponent>(this TComponent component, Func<TComponent, string> textFunc, UIElement parent)
            where TComponent : Component
        {
            return new ComponentUIElementText<TComponent>(textFunc, component, parent);
        }






        //public static void SpawnDustOnSend(int selfWidth, int selfHeight, Point16 Position, IMagikeContainer container, Color dustColor, int dustType = DustID.Teleporter)
        //{
        //    Tile tile = Framing.GetTileSafely(container.GetPosition);
        //    TileObjectData data = TileObjectData.GetTileData(tile);
        //    int xOffset = data == null ? 16 : data.Width * 8;
        //    int yOffset = data == null ? 24 : data.Height * 8;

        //    Vector2 selfPos = Position.ToWorldCoordinates(selfWidth * 8, selfHeight * 8);
        //    Vector2 receiverPos = container.GetPosition.ToWorldCoordinates(xOffset, yOffset);
        //    Vector2 dir = selfPos.DirectionTo(receiverPos);
        //    float length = Vector2.Distance(selfPos, receiverPos);

        //    while (length > 0)
        //    {
        //        Dust dust = Dust.NewDustPerfect(selfPos + dir * length, dustType, dir * 0.2f, newColor: dustColor);
        //        dust.noGravity = true;
        //        length -= 8;
        //    }
        //}

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
            Point16 position = new(i - x, j - y);

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
            Point16 position = new(top, left);

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
        //public static void ShowMagikeNumber(int i, int j)
        //{
        //    if (TryGetEntity(i, j, out IMagikeContainer magikeContainer))
        //        Main.instance.MouseText(magikeContainer.Magike + " / " + magikeContainer.MagikeMax, 0, 0, -1, -1, -1, -1);
        //}

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
