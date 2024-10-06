using Coralite.Helpers;
using System;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.Components.Producers
{
    public abstract class ProducerByTime : MagikeActiveProducer, IUIShowable
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
        /// 检测当前时间
        /// </summary>
        /// <returns></returns>
        public abstract bool CheckDayTime();

        public override bool CanProduce_SpecialCheck()
            => CheckDayTime();

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
