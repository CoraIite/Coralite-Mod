using Coralite.Content.UI;
using Coralite.Core.Loaders;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.UI;

namespace Coralite.Content.CustomHooks
{
    public class WorldSettings : HookGroup
    {
        public override void Load()
        {
            On_UIWorldSelect.NewWorldClick += On_UIWorldSelect_NewWorldClick;
            On_UIWorldCreation.Click_GoBack += On_UIWorldCreation_Click_GoBack;
            On_UIWorldCreation.Click_NamingAndCreating += On_UIWorldCreation_Click_NamingAndCreating; 
        }

        public override void Unload()
        {
            On_UIWorldSelect.NewWorldClick -= On_UIWorldSelect_NewWorldClick;
            On_UIWorldCreation.Click_GoBack -= On_UIWorldCreation_Click_GoBack;
            On_UIWorldCreation.Click_NamingAndCreating -= On_UIWorldCreation_Click_NamingAndCreating; 
        }

        /// <summary>
        /// 创建世界
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="evt"></param>
        /// <param name="listeningElement"></param>
        private void On_UIWorldCreation_Click_NamingAndCreating(On_UIWorldCreation.orig_Click_NamingAndCreating orig, UIWorldCreation self, Terraria.UI.UIMouseEvent evt, Terraria.UI.UIElement listeningElement)
        {
            //CoraliteWorldSettings.visible = false;
            //UILoader.GetUIState<CoraliteWorldSettings>().Recalculate();

            orig.Invoke(self, evt, listeningElement);
        }

        /// <summary>
        /// 返回选择页面
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="evt"></param>
        /// <param name="listeningElement"></param>
        private void On_UIWorldCreation_Click_GoBack(On_UIWorldCreation.orig_Click_GoBack orig, UIWorldCreation self, Terraria.UI.UIMouseEvent evt, Terraria.UI.UIElement listeningElement)
        {
            //CoraliteWorldSettings.visible = false;
            //UILoader.GetUIState<CoraliteWorldSettings>().Recalculate();
            orig.Invoke(self, evt, listeningElement);
        }

        /// <summary>
        /// 选择世界页面的点击新建世界按钮
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="evt"></param>
        /// <param name="listeningElement"></param>
        private void On_UIWorldSelect_NewWorldClick(On_UIWorldSelect.orig_NewWorldClick orig, UIWorldSelect self, Terraria.UI.UIMouseEvent evt, Terraria.UI.UIElement listeningElement)
        {
            //CoraliteWorldSettings.visible = true;
            //UILoader.GetUIState<CoraliteWorldSettings>().Recalculate();
            orig.Invoke(self, evt, listeningElement);

            UIState state = Main.MenuUI.CurrentState;
            CoraliteWorldSettings.OnInitialize(state);
        }
    }
}
