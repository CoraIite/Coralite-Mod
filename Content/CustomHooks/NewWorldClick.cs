using Terraria.GameContent.UI.States;

namespace Coralite.Content.CustomHooks
{
    public class NewWorldClick : HookGroup
    {
        public override void Load()
        {
            On_UIWorldSelect.NewWorldClick += On_UIWorldSelect_NewWorldClick;
        }

        private void On_UIWorldSelect_NewWorldClick(On_UIWorldSelect.orig_NewWorldClick orig, UIWorldSelect self, Terraria.UI.UIMouseEvent evt, Terraria.UI.UIElement listeningElement)
        {
            orig.Invoke(self, evt, listeningElement);
        }
    }
}
