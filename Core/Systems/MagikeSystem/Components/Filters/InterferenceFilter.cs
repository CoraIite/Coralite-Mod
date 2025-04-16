using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using System.Collections.Generic;
using System.Linq;

namespace Coralite.Core.Systems.MagikeSystem.Components.Filters
{
    /// <summary>
    /// 干涉滤镜，增加发送量
    /// </summary>
    public abstract class InterferenceFilter : MagikeFilter/*, IUIShowable*/
    {
        public abstract int UnitDeliveryBonus { get; }

        public override bool CanInsert_SpecialCheck(MagikeTP entity, ref string text)
        {
            if (!entity.HasComponent(MagikeComponentID.MagikeSender))
            {
                text = MagikeSystem.GetFilterText(MagikeSystem.FilterID.MagikeSenderNotFound);
                return false;
            }

            return true;
        }

        public override void ChangeComponentValues(MagikeComponent component)
        {
            if (component is MagikeSender sender)
            {
                sender.UnitDeliveryBonus = 1f;

                float bonus = 0;

                foreach (MagikeFilter filter in ((List<MagikeComponent>)(Entity.Components[MagikeComponentID.MagikeFilter])).Cast<MagikeFilter>())
                {
                    if (filter is InterferenceFilter iff)
                        bonus += iff.UnitDeliveryBonus;
                }

                sender.UnitDeliveryBonus += bonus * 0.01f;
            }
        }

        public override void RestoreComponentValues(MagikeComponent component)
        {
            if (component is MagikeSender sender)
            {
                sender.UnitDeliveryBonus = 1f;

                float bonus = 0;

                foreach (MagikeFilter filter in ((List<MagikeComponent>)(Entity.Components[MagikeComponentID.MagikeFilter])).Cast<MagikeFilter>())
                {
                    if (filter is InterferenceFilter iff)
                        bonus += iff.UnitDeliveryBonus;
                }

                sender.UnitDeliveryBonus += bonus * 0.01f;
            }
        }

        #region UI部分

        //public void ShowInUI(UIElement parent)
        //{
        //    UIElement title = this.AddTitle(MagikeSystem.UITextID.InterferenceFilterName, parent);

        //    UIList list =
        //    [
        //        this.NewTextBar(c =>
        //        MagikeSystem.GetUIText(MagikeSystem.UITextID.InterferenceBonus)+UnitDeliveryBonus+"%", parent),
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
