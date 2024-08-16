using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ObjectData;
using Terraria.UI;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class BiomeProducer : MagikeActiveProducer, IUIShowable
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

        #region 生产逻辑部分

        /// <summary>
        /// 检测底座上的物块<br></br>
        /// 底座上必定有物块
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public abstract bool CheckTile(Tile tile);

        /// <summary>
        /// 检测透镜所处的背景墙
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public abstract bool CheckWall(Tile tile);

        public override bool CanProduce_SpecialCheck()
        {
            Point16 point = (Entity as MagikeTileEntity).Position;

            GetMagikeAlternateData(point.X, point.Y, out TileObjectData data, out MagikeAlternateStyle alternate);

            //检测自身范围内的墙壁
            for (int i = 0; i < data.Width; i++)
                for (int j = 0; j < data.Height; j++)
                    if (!CheckWall(Framing.GetTileSafely(point.X + i, point.Y + j)))
                        return false;

            //检测底座上的物块
            switch (alternate)
            {
                case MagikeAlternateStyle.Bottom:
                    for (int i = 0; i < data.Width; i++)
                        if (!CheckTile(Framing.GetTileSafely(point.X + i, point.Y + data.Height)))
                            return false;
                    break;
                case MagikeAlternateStyle.Top:
                    for (int i = 0; i < data.Width; i++)
                        if (!CheckTile(Framing.GetTileSafely(point.X + i, point.Y - 1)))
                            return false;
                    break;
                case MagikeAlternateStyle.Left:
                    for (int i = 0; i < data.Height; i++)
                        if (!CheckTile(Framing.GetTileSafely(point.X - 1, point.Y + i)))
                            return false;
                    break;
                case MagikeAlternateStyle.Right:
                    for (int i = 0; i < data.Height; i++)
                        if (!CheckTile(Framing.GetTileSafely(point.X + data.Width, point.Y + i)))
                            return false;
                    break;
                default:
                    return false;
            }

            return true;
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
                this.NewTextBar(c =>
                {
                    float timer= MathF.Round(c.Timer/60f,1);
                    float delay= MathF.Round(c.ProductionDelay/60f,1);
                    float delayBase= MathF.Round(c.ProductionDelayBase/60f,1);
                    float DelayBonus= c.ProductionDelayBonus;

                    return $"  ▶ {timer} / {delay} ({delayBase} * {DelayBonus})";
                },parent),
                //生产量
                this.NewTextBar(c => MagikeSystem.GetUIText(MagikeSystem.UITextID.ProduceAmount), parent),
                this.NewTextBar(c => 
                    $"  ▶ {c.Throughput} ({c.ThroughputBase} * {c.ProductionDelayBonus})", parent),
                //生产条件
                this.NewTextBar(c => MagikeSystem.GetUIText(MagikeSystem.UITextID.ProduceCondition), parent),
                this.NewTextBar(c => MagikeSystem.GetUIText(c.ProduceCondition()), parent),
            ];

            list.SetTopLeft(title.Height.Pixels + 8, 0);
            list.SetSize(0, -list.Top.Pixels, 1, 1);

            parent.Append(list);
        }

        #endregion
    }
}
