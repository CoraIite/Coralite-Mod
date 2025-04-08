using Coralite.Content.WorldGeneration;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.BaseItems
{
    /// <summary>
    /// 魔鸟
    /// </summary>
    public abstract class Mabird : ModItem
    {
        /// <summary>
        /// 魔鸟的最远飞行距离
        /// </summary>
        public int SendLength { get; }
        /// <summary>
        /// 一次能抓多少物品
        /// </summary>
        public int CatchStack { get; }
        /// <summary>
        /// 飞行速度
        /// </summary>
        public float Speed { get; }

        /// <summary>
        /// 休息时间
        /// </summary>
        public const int RestTime = 60 * 5;

        public MabirdAIState State { get; set; }
        public short Timer { get; set; }

        public enum MabirdAIState : byte
        {
            Rest,
            /// <summary>
            /// 去拿物品
            /// </summary>
            FlyToGetItem,
            /// <summary>
            /// 去放物品
            /// </summary>
            FlyToReleaseItem,
            /// <summary>
            /// 去把物品放回原位
            /// </summary>
            FlyToPutItemBack,
            /// <summary>
            /// 归巢
            /// </summary>
            FlyBack
        }

        /// <summary>
        /// 从哪拿物品
        /// </summary>
        public MabirdTarget GetItemPos { get; private set; }
        /// <summary>
        /// 放物品
        /// </summary>
        public MabirdTarget ReleaseItemPos { get; private set; }

        public Vector2 Center { get; private set; }
        public Vector2 Velocity { get; private set; }
        /// <summary>
        /// 白名单物品
        /// </summary>
        public Item WhiteListItem { get; set; }

        private byte frameY;
        private byte frameCounter;
        private Item catchItem;

        public override void SetStaticDefaults()
        {
            CoraliteSets.IsMabird.Add(Type);
        }

        public void UpdateMabird(Vector2 center)
        {
            try
            {
                switch (State)
                {
                    default:
                    case MabirdAIState.Rest://啥也不干
                        Timer++;
                        if (Timer > RestTime)
                        {
                            Timer = 0;
                            TurnToGetItem(center);
                        }
                        return;
                    case MabirdAIState.FlyToGetItem://飞向目标点1
                        {
                            UpdateBaseValue();

                            if (Vector2.Distance(center, GetItemPos.Pos) > Speed * 1.5f)
                                break;

                            //到达指定位置，开始拿物品
                            Timer = 0;
                            if (CatchItem())//如果抓到了物品
                            {
                                if (HasEmptySlot(ReleaseItemPos))//并且位置2能够容纳物品
                                    TurnToReleaseItem();//放物品到位置2
                                else
                                    TurnToPutItemBack();//物品放回位置1
                            }
                            else
                                TurnToBack(center);//没抓到直接返回
                        }
                        break;
                    case MabirdAIState.FlyToReleaseItem://飞向目标点2
                        {
                            UpdateBaseValue();

                            if (Vector2.Distance(center, ReleaseItemPos.Pos) > Speed * 1.5f)
                                break;

                            if (HasEmptySlot(ReleaseItemPos))//并且位置2能够容纳物品
                            {
                                PutItemIn(ReleaseItemPos);
                                TurnToBack(center);//放完直接返回
                            }
                            else
                                TurnToPutItemBack();//物品放回位置1
                        }
                        break;
                    case MabirdAIState.FlyToPutItemBack://返回位置1，把物品放回去
                        {
                            UpdateBaseValue();

                            if (Vector2.Distance(center, GetItemPos.Pos) > Speed * 1.5f)
                                break;

                            if (HasEmptySlot(GetItemPos))//位置1能继续放
                            {
                                PutItemIn(GetItemPos);

                                if (!catchItem.IsAir)
                                {
                                    Item.NewItem(catchItem.GetSource_DropAsItem(), Center, catchItem);
                                    catchItem.TurnToAir();
                                }
                            }

                            TurnToBack(center);//放完直接返回
                        }
                        break;
                    case MabirdAIState.FlyBack:
                        {
                            UpdateBaseValue();

                            if (Vector2.Distance(center, Center) < Speed * 1.5f)//到达指定位置，开始拿物品
                            {
                                Timer = 0;
                                State = MabirdAIState.Rest;
                            }
                        }
                        break;
                }
            }
            catch
            {
                State = MabirdAIState.Rest;
                GetItemPos = null;
                ReleaseItemPos = null;
            }
        }

        #region 状态切换

        public void TurnToGetItem(Vector2 center)
        {
            if (GetItemPos == null || ReleaseItemPos == null)
            {
                Timer = RestTime;
                return;
            }

            State = MabirdAIState.FlyToGetItem;

            if (GetItemPos.chestIndex == -1)
                GetItemPos.FindChest();
            if (ReleaseItemPos.chestIndex == -1)
                ReleaseItemPos.FindChest();

            Center = center;
            Velocity = (GetItemPos.Pos - Center).SafeNormalize(Vector2.Zero) * Speed;
        }

        public void TurnToReleaseItem()
        {
            State = MabirdAIState.FlyToReleaseItem;

            Velocity = (ReleaseItemPos.Pos - Center).SafeNormalize(Vector2.Zero) * Speed;
        }

        public void TurnToPutItemBack()
        {
            State = MabirdAIState.FlyToPutItemBack;

            Velocity = (GetItemPos.Pos - Center).SafeNormalize(Vector2.Zero) * Speed;
        }

        public void TurnToBack(Vector2 center)
        {
            State = MabirdAIState.FlyBack;
            Velocity = (center - Center).SafeNormalize(Vector2.Zero) * Speed;
        }

        #endregion

        #region 各种帮助方法

        /// <summary>
        /// 抓取物品，从一个物品容器中拿物品，会直接设置<see cref="catchItem"/>
        /// </summary>
        /// <returns></returns>
        private bool CatchItem()
        {
            Point p = GetItemPos.TopLeft;
            Tile t = Framing.GetTileSafely(p);
            if (!t.HasTile)
                return false;

            int? whiteListType = WhiteListItem == null ? WhiteListItem.type : null;

            if (MagikeHelper.TryGetEntityWithTopLeft(new Point16(p), out var magikeTP))
            {
                if (magikeTP.TryGetComponent(MagikeComponentID.ItemGetOnlyContainer, out GetOnlyItemContainer container))
                {
                    //找一下只读物品容器，有只读就只招只读
                    Item getItem = container.GetItem(CatchStack, whiteListType);
                    if (getItem != null)
                    {
                        catchItem = getItem;
                        return true;
                    }
                }
                else if (magikeTP.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container2))
                {
                    //找一下物品容器
                    Item getItem = container2.GetItem(CatchStack, whiteListType);
                    if (getItem != null)
                    {
                        catchItem = getItem;
                        return true;
                    }
                }

                return false;
            }

            if (Main.tileContainer[t.TileType] && GetItemPos.chestIndex != -1)
            {
                int index = GetItemPos.chestIndex;

                Chest chest = Main.chest[index];
                if (Chest.IsLocked(p.X, p.Y))
                    return false;

                Item getItem = chest.GetItem(CatchStack, whiteListType);
                if (getItem != null)
                {
                    catchItem = getItem;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检测一个位置上的东西是否有空位置
        /// </summary>
        /// <param name="topLeft"></param>
        /// <returns></returns>
        private bool HasEmptySlot(MabirdTarget target)
        {
            Tile t = Framing.GetTileSafely(target.TopLeft);
            if (!t.HasTile)
                return false;

            if (MagikeHelper.TryGetEntityWithTopLeft(new Point16(target.TopLeft), out var magikeTP))
            {
                if (magikeTP.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
                {
                    //找一下物品容器，不找只读
                    if (container.CanAddItem(catchItem.type, catchItem.stack))
                        return true;
                }

                return false;
            }

            if (Main.tileContainer[t.TileType] && target.chestIndex != -1)
            {
                int index = target.chestIndex;
                if (index == -1)
                    return false;

                if (Main.chest[index].CanAddItem(catchItem.type, catchItem.stack))
                    return true;
            }

            return false;
        }

        private void PutItemIn(MabirdTarget target)
        {
            Tile t = Framing.GetTileSafely(target.TopLeft);
            if (!t.HasTile)
                return;

            if (MagikeHelper.TryGetEntityWithTopLeft(new Point16(target.TopLeft), out var magikeTP))
            {
                if (magikeTP.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
                    container.AddItem(catchItem);

                return;
            }

            if (Main.tileContainer[t.TileType] && target.chestIndex != -1)
            {
                int index = target.chestIndex;//箱子的傻逼8000遍历
                if (index == -1)
                    return;

                Chest chest = Main.chest[index];
                if (Chest.IsLocked(target.TopLeft.X, target.TopLeft.Y))
                    return;

                chest.AddItem(catchItem);
            }
        }

        public void UpdateBaseValue()
        {
            Center += Velocity;

            if (++frameCounter > 4)
            {
                frameCounter = 0;
                frameY++;
            }

            if (frameY > 4)
                frameY = 0;
        }

        #endregion

        public void DrawMabird(SpriteBatch spriteBatch)
        {
            if (State == MabirdAIState.Rest)//休息时不绘制
                return;

            Texture2D tex = MagikeAssets.Mabird.Value;

            Rectangle frameBox = tex.Frame(1, 5, 0, frameY);
            Color color = Lighting.GetColor(Center.ToTileCoordinates());
            spriteBatch.Draw(tex, Center - Main.screenPosition, frameBox
                , color, 0, frameBox.Size() / 2, 1, Velocity.X > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            if (catchItem != null && !catchItem.IsAir)//绘制下面吊着的物品
                MagikeHelper.DrawItem(spriteBatch, catchItem, Center + new Vector2(0, 24), 48, color);
        }

        /// <summary>
        /// 传入的可以不是左上角
        /// </summary>
        /// <param name="p"></param>
        public void SetGetItemPos(Point p)
        {
            Point16? p2 = MagikeHelper.ToTopLeft(p.X, p.Y);
            GetItemPos = new MabirdTarget(p2 == null ? p : p2.Value.ToPoint());
        }

        public void SetReleaseItemPos(Point p)
        {
            Point16? p2 = MagikeHelper.ToTopLeft(p.X, p.Y);
            ReleaseItemPos = new MabirdTarget(p2 == null ? p : p2.Value.ToPoint());
        }

        public override ModItem Clone(Item newEntity)
        {
            return base.Clone(newEntity);
        }

        public override void SaveData(TagCompound tag)
        {
            bool save = false;
            BitsByte b1 = new BitsByte();

            if (GetItemPos != null)
            {
                b1[0] = true;
                save = true;
                GetItemPos.Save(tag, nameof(GetItemPos));
            }

            if (ReleaseItemPos != null)
            {
                b1[1] = true;
                save = true;
                ReleaseItemPos.Save(tag, nameof(ReleaseItemPos));
            }

            if (WhiteListItem != null)
            {
                b1[2] = true;
                save = true;
                ItemIO.Save(WhiteListItem);
            }

            if (catchItem != null && !catchItem.IsAir)
            {
                b1[3] = true;
                save = true;
                ItemIO.Save(catchItem);
            }

            if (save)
                tag.Add("MabirdValues", b1);

            tag.Add(nameof(State), (byte)State);
            tag.Add(nameof(Timer), Timer);

            tag.Add(nameof(Center), Center);
            tag.Add(nameof(Velocity), Velocity);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet("MabirdValues", out BitsByte b1))
            {
                if (b1[0])
                    GetItemPos = MabirdTarget.Load(tag, nameof(GetItemPos));
                if (b1[1])
                    ReleaseItemPos = MabirdTarget.Load(tag, nameof(ReleaseItemPos));
                if (b1[2])
                    WhiteListItem = ItemIO.Load(tag);
                if (b1[3])
                    catchItem = ItemIO.Load(tag);
            }

            State = (MabirdAIState)tag.Get<byte>(nameof(State));
            Timer = tag.Get<short>(nameof(Timer));

            Center = tag.Get<Vector2>(nameof(Center));
            Velocity = tag.Get<Vector2>(nameof(Velocity));
        }
    }

    /// <summary>
    /// 魔鸟的目标点
    /// </summary>
    public record class MabirdTarget
    {
        public Point TopLeft;
        public Vector2 Pos;
        public int chestIndex = -1;

        public MabirdTarget(Point topLeft)
        {
            TopLeft = topLeft;
            Pos = Helper.GetMagikeTileCenter(TopLeft);
        }

        public void FindChest()
        {
            chestIndex = Chest.FindChest(TopLeft.X, TopLeft.Y);
        }

        public void Save(TagCompound tag, string preName)
        {
            tag.Add(preName + nameof(TopLeft), TopLeft);
        }

        public static MabirdTarget Load(TagCompound tag, string preName)
        {
            return new MabirdTarget(tag.Get<Point>(preName + nameof(TopLeft)));
        }
    }
}
