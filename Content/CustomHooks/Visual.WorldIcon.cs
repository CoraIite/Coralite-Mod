using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Coralite.Content.CustomHooks
{
    public class WorldIcon : HookGroup
    {
        public override void Load()
        {
            On_AWorldListItem.GetIconElement += On_AWorldListItem_GetIconElement;
        }

        public override void Unload()
        {
            On_AWorldListItem.GetIconElement -= On_AWorldListItem_GetIconElement;
        }

        private UIElement On_AWorldListItem_GetIconElement(On_AWorldListItem.orig_GetIconElement orig, AWorldListItem self)
        {
            if (self.Data.WorldGenModsRecorded)
            {
                if (self.Data.TryGetHeaderData<CoraliteWorld>(out var data))
                {
                    if (data.GetBool(CoraliteWorld.DigDigDigSaveKey))
                    {
                        string c = self.Data.HasCorruption ? "Corruption" : "Crimson";
                        string hallow = self.Data.IsHardMode ? "Hallow" : "";

                        Asset<Texture2D> tex = ModContent.Request<Texture2D>(AssetDirectory.WorldIcon + "IconDig" + c + hallow,AssetRequestMode.ImmediateLoad);
                        return new UIImage(tex)
                        {
                            Left = new StyleDimension(4f, 0f)
                        };
                    }
                }
            }

            return orig.Invoke(self);
        }
    }
}
