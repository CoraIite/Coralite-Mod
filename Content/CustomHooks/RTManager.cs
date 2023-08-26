using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Coralite.Content.CustomHooks
{
    public class RTManager:HookGroup
    {
        // 应该是安全的
        public override SafetyLevel Safety => SafetyLevel.Safe;

        public override void Load()
        {
            On_Main.UpdateDisplaySettings += On_Main_UpdateDisplaySettings;
        }

        public override void Unload()
        {
            On_Main.UpdateDisplaySettings -= On_Main_UpdateDisplaySettings;
        }

        private void On_Main_UpdateDisplaySettings(On_Main.orig_UpdateDisplaySettings orig, Main self)
        {
            orig.Invoke(self);

            Main.QueueMainThreadAction(() =>
            {
                if (EndCapture.screen is null || EndCapture.screen.Width != Main.screenWidth || EndCapture.screen.Height != Main.screenHeight)
                {
                    EndCapture.screen = new Microsoft.Xna.Framework.Graphics.RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                }
            });
        }
    }
}
