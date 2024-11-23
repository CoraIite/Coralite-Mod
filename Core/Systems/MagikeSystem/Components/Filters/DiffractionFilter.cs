using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components.Filters
{
    /// <summary>
    /// 衍射滤镜，增加发送器连接量
    /// </summary>
    public abstract class DiffractionFilter : MagikeFilter, IUIShowable
    {
        public abstract byte ConnectMaxBonus { get; }

        public override bool CanInsert_SpecialCheck(MagikeTP entity, ref string text)
        {
            if (!entity.TryGetComponent<MagikeLinerSender>(MagikeComponentID.MagikeSender, out _))
            {
                text = MagikeSystem.GetFilterText(MagikeSystem.FilterID.MagikeLinerSenderNotFound);
                return false;
            }

            return true;
        }

        public override void ChangeComponentValues(MagikeComponent component)
        {
            if (component is MagikeLinerSender sender)
                sender.MaxConnectExtra += ConnectMaxBonus;
        }

        public override void RestoreComponentValues(MagikeComponent component)
        {
            if (component is MagikeLinerSender sender)
                sender.MaxConnectExtra -= ConnectMaxBonus;
        }

        #region UI部分

        public void ShowInUI(UIElement parent)
        {
            UIElement title = this.AddTitle(MagikeSystem.UITextID.DiffractionFilterName, parent);

            UIList list =
            [
                this.NewTextBar(c =>
                MagikeSystem.GetUIText(MagikeSystem.UITextID.DiffractionBonus)+ConnectMaxBonus, parent),
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
