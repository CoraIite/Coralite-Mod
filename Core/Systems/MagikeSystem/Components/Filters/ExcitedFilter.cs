using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.Components.Producers;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components.Filters
{
    /// <summary>
    /// 激发滤镜，增加魔能生产器产量
    /// </summary>
    public abstract class ExcitedFilter : MagikeFilter, IUIShowable
    {
        public abstract int ProduceBonus { get; }

        public override bool CanInsert_SpecialCheck(MagikeTP entity, ref string text)
        {
            if (!entity.HasComponent(MagikeComponentID.MagikeProducer))
            {
                text = MagikeSystem.GetFilterText(MagikeSystem.FilterID.MagikeProducerNotFound);
                return false;
            }

            return true;
        }

        public override void ChangeComponentValues(MagikeComponent component)
        {
            if (component is MagikeProducer producer)
            {
                producer.ThroughputBonus = 1f;

                float bonus = 0;

                foreach (MagikeFilter filter in ((List<MagikeComponent>)(Entity.Components[MagikeComponentID.MagikeFilter])).Cast<MagikeFilter>())
                {
                    if (filter is ExcitedFilter pf)
                        bonus += pf.ProduceBonus;
                }

                producer.ThroughputBonus += bonus * 0.01f;
            }
        }

        public override void RestoreComponentValues(MagikeComponent component)
        {
            if (component is MagikeProducer producer)
            {
                producer.ThroughputBonus = 1f;

                float bonus = 0;

                foreach (MagikeFilter filter in ((List<MagikeComponent>)(Entity.Components[MagikeComponentID.MagikeFilter])).Cast<MagikeFilter>())
                {
                    if (filter is ExcitedFilter pf)
                        bonus += pf.ProduceBonus;
                }

                producer.ThroughputBonus += bonus * 0.01f;
            }
        }

        #region UI部分

        public void ShowInUI(UIElement parent)
        {
            UIElement title = this.AddTitle(MagikeSystem.UITextID.ExcitedFilterName, parent);

            UIList list =
            [
                this.NewTextBar(c =>
                MagikeSystem.GetUIText(MagikeSystem.UITextID.ExcitedBonus)+ProduceBonus+"%", parent),
                new FilterText(c =>
                    $"  ▶ [i:{c.ItemType}]",this,parent),
                //取出按钮
                new FilterRemoveButton(Entity, this)
            ];

            list.SetSize(0, 0, 1, 1);
            list.SetTopLeft(title.Height.Pixels + 8, 0);

            list.QuickInvisibleScrollbar();

            parent.Append(list);
        }

        #endregion
    }
}
