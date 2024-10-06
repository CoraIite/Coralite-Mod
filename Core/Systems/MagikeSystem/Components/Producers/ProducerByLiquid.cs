using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ObjectData;
using Terraria.UI;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.Components.Producers
{
    public abstract class ProducerByLiquid : MagikeActiveProducer, IUIShowable
    {
        /// <summary>
        /// 名称
        /// </summary>
        /// <returns></returns>
        public abstract MagikeSystem.UITextID ApparatusName();

        /// <summary>
        /// 生产条件
        /// </summary>
        /// <returns></returns>
        public abstract MagikeSystem.UITextID ProduceCondition();

        /// <summary>
        /// 液体类型
        /// </summary>
        public virtual int LiquidType { get => 0; }

        /// <summary>
        /// 每次消耗液体的量
        /// </summary>
        public virtual byte CostCount { get => 256 / 32; }

        #region 生产逻辑部分

        /// <summary>
        /// 检测当前液体
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckLiquid(Tile tile)
        {
            if (tile.LiquidAmount < CostCount)
                return false;

            if (tile.LiquidType != LiquidType)
                return false;

            return true;
        }

        /// <summary>
        /// 减少液体
        /// </summary>
        /// <param name="tile"></param>
        public virtual void ReduceLiquid(Tile tile)
        {
            tile.LiquidAmount -= CostCount;
        }

        public override bool CanProduce_SpecialCheck()
        {
            Point16 point = (Entity as MagikeTileEntity).Position;

            GetMagikeAlternateData(point.X, point.Y, out TileObjectData data, out MagikeAlternateStyle alternate);

            //检测底座上的物块液体
            switch (alternate)
            {
                case MagikeAlternateStyle.Bottom:
                    for (int i = 0; i < data.Width; i++)
                        if (!CheckLiquid(Framing.GetTileSafely(point.X + i, point.Y + data.Height - 1)))
                            return false;
                    break;
                case MagikeAlternateStyle.Top:
                    for (int i = 0; i < data.Width; i++)
                        if (!CheckLiquid(Framing.GetTileSafely(point.X + i, point.Y)))
                            return false;
                    break;
                case MagikeAlternateStyle.Left:
                    for (int i = 0; i < data.Height; i++)
                        if (!CheckLiquid(Framing.GetTileSafely(point.X, point.Y + i)))
                            return false;
                    break;
                case MagikeAlternateStyle.Right:
                    for (int i = 0; i < data.Height; i++)
                        if (!CheckLiquid(Framing.GetTileSafely(point.X + data.Width - 1, point.Y + i)))
                            return false;
                    break;
                default:
                    return false;
            }

            return true;
        }

        public override void Produce()
        {
            Point16 point = (Entity as MagikeTileEntity).Position;

            GetMagikeAlternateData(point.X, point.Y, out TileObjectData data, out MagikeAlternateStyle alternate);

            //检测底座上的物块液体
            switch (alternate)
            {
                case MagikeAlternateStyle.Bottom:
                    for (int i = 0; i < data.Width; i++)
                        ReduceLiquid(Framing.GetTileSafely(point.X + i, point.Y + data.Height - 1));
                    break;
                case MagikeAlternateStyle.Top:
                    for (int i = 0; i < data.Width; i++)
                        ReduceLiquid(Framing.GetTileSafely(point.X + i, point.Y));
                    break;
                case MagikeAlternateStyle.Left:
                    for (int i = 0; i < data.Height; i++)
                        ReduceLiquid(Framing.GetTileSafely(point.X, point.Y + i));
                    break;
                case MagikeAlternateStyle.Right:
                    for (int i = 0; i < data.Height; i++)
                        ReduceLiquid(Framing.GetTileSafely(point.X + data.Width - 1, point.Y + i));
                    break;
                default:
                    return;
            }

            base.Produce();
        }

        #endregion

        #region UI部分

        public void ShowInUI(UIElement parent)
        {
            UIElement title = this.AddTitle(ApparatusName(), parent);

            UIList list =
            [
                //生产时间
                this.NewTextBar(c => MagikeSystem.GetUIText(MagikeSystem.UITextID.ProduceTime), parent),
                this.NewTextBar(ConnectLengthText,parent),

                //生产量
                this.NewTextBar(c => MagikeSystem.GetUIText(MagikeSystem.UITextID.ProduceAmount), parent),
                this.NewTextBar(ThroughputText, parent),

                //生产条件
                this.NewTextBar(c => MagikeSystem.GetUIText(MagikeSystem.UITextID.ProduceCondition), parent),
                this.NewTextBar(c => MagikeSystem.GetUIText(c.ProduceCondition()), parent),
            ];

            list.SetTopLeft(title.Height.Pixels + 8, 0);
            list.SetSize(0, -list.Top.Pixels, 1, 1);
            list.QuickInvisibleScrollbar();

            parent.Append(list);
        }

        #endregion

    }
}
