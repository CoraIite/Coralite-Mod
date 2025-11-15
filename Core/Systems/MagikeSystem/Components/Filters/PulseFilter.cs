using Coralite.Core.Systems.MagikeSystem.TileEntities;
using System.Collections.Generic;
using System.Linq;

namespace Coralite.Core.Systems.MagikeSystem.Components.Filters
{
    /// <summary>
    /// 脉冲滤镜：减少计时器CD
    /// </summary>
    public abstract class PulseFilter : MagikeFilter/*, IUIShowable*/
    {
        public abstract int TimerBonus { get; }

        public override bool CanInsert_SpecialCheck(MagikeTP entity, ref string text)
        {
            bool hasTimer = false;

            foreach (var c in entity.ComponentsCache)
                if (c is ITimerTriggerComponent)
                {
                    hasTimer = true;
                    break;
                }

            if (!hasTimer)
            {
                text = MagikeSystem.GetFilterText(MagikeSystem.FilterID.TimerNotFound);
                return false;
            }

            return true;
        }

        public override void ChangeComponentValues(MagikeComponent component)
        {
            if (component is ITimerTriggerComponent timer)
            {
                timer.DelayBonus = 1f;

                float bonus = 0;

                foreach (MagikeFilter filter in ((List<MagikeComponent>)(Entity.Components[MagikeComponentID.MagikeFilter])).Cast<MagikeFilter>())
                {
                    if (filter is PulseFilter pf)
                        bonus += pf.TimerBonus;
                }

                timer.DelayBonus -= bonus * 0.01f;
            }
        }

        public override void RestoreComponentValues(MagikeComponent component)
        {
            if (component is ITimerTriggerComponent timer)
            {
                timer.DelayBonus = 1f;

                float bonus = 0;

                foreach (MagikeFilter filter in ((List<MagikeComponent>)(Entity.Components[MagikeComponentID.MagikeFilter])).Cast<MagikeFilter>())
                {
                    if (filter is PulseFilter pf)
                        bonus += pf.TimerBonus;
                }

                timer.DelayBonus -= bonus * 0.01f;
            }
        }

        #region UI部分

        //public void ShowInUI(UIElement parent)
        //{
        //    UIElement title = this.AddTitle(MagikeSystem.UITextID.PulseFilterName, parent);

        //    UIList list =
        //    [
        //        this.NewTextBar(c =>
        //        MagikeSystem.GetUIText(MagikeSystem.UITextID.PulseBonus)+TimerBonus+"%", parent),
        //        new FilterText(c =>
        //            $"  ▶ [i:{c.ItemType}]",this,parent),
        //        //取出按钮
        //        new FilterRemoveButton(Entity, this)
        //    ];

        //    list.SetSize(0, 0, 1, 1);
        //    list.SetTopLeft(title.Height.Pixels + 8, 0);

        //    list.QuickInvisibleScrollbar();

        //    parent.Append(list);
        //}

        #endregion
    }
}
