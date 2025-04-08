using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

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
        public int Timer { get; set; }

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

        public void UpdateMabird(Vector2 center)
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
                        Center += Velocity;
                        UpdateFrame();

                        if (Vector2.Distance(center, GetItemPos.Pos) < Speed * 1.5f)//到达指定位置，开始拿物品
                        {
                            Timer = 0;
                            if ( /* TODO: 增加位置2检测*/  CatchItem())//如果抓到了物品并且位置2能够容纳物品
                            {
                                State = MabirdAIState.FlyToReleaseItem;
                            }
                            else
                                TurnToBack(center);
                        }
                    }
                    break;
                case MabirdAIState.FlyToReleaseItem:
                    {
                        UpdateFrame();

                    }
                    break;
                case MabirdAIState.FlyToPutItemBack:
                    {
                        UpdateFrame();

                    }
                    break;
                case MabirdAIState.FlyBack:
                    {
                        UpdateFrame();
                        Center += Velocity;

                        if (Vector2.Distance(center, Center) < Speed * 1.5f)//到达指定位置，开始拿物品
                        {
                            Timer = 0;
                            State = MabirdAIState.Rest;
                        }
                    }
                    break;
            }
        }

        private void TurnToGetItem(Vector2 center)
        {
            State = MabirdAIState.FlyToGetItem;

            Center = center;
            Velocity = (GetItemPos.Pos - center).SafeNormalize(Vector2.Zero) * Speed;
        }

        public void TurnToBack(Vector2 center)
        {
            State = MabirdAIState.FlyBack;
            Velocity = (center - Center).SafeNormalize(Vector2.Zero) * Speed;
        }

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

            if (Main.tileContainer[t.TileType])
            {
                int index = Chest.FindChest(p.X, p.Y);//箱子的傻逼8000遍历
                if (index == -1)
                    return false;

                Chest chest = Main.chest[index];
                Item getItem = chest.GetItem(CatchStack, whiteListType);
                if (getItem != null)
                {
                    catchItem = getItem;
                    return true;
                }
            }

            return false;
        }

        public void UpdateFrame()
        {
            if (++frameCounter > 4)
            {
                frameCounter = 0;
                frameY++;
            }

            if (frameY > 4)
                frameY = 0;
        }

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

        public void SetGetItemPos(Point p)
        {

        }

        public void SetReleaseItemPos(Point p)
        {

        }

        public override ModItem Clone(Item newEntity)
        {
            return base.Clone(newEntity);
        }
    }

    /// <summary>
    /// 魔鸟的目标点
    /// </summary>
    public record struct MabirdTarget
    {
        public Point TopLeft;
        public Vector2 Pos;

        public MabirdTarget(Point topLeft)
        {
            TopLeft = topLeft;
            Pos = Helper.GetMagikeTileCenter(TopLeft);
        }

        public void Save()
        {

        }

        public static void Load()
        {

        }
    }
}
