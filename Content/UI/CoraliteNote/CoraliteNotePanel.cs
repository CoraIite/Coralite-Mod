using Coralite.Content.UI.CoraliteNote.Readfragment;
using Coralite.Content.UI.UILib;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace Coralite.Content.UI.CoraliteNote
{
    public class CoraliteNotePanel : UI_BookPanel
    {
        public CoraliteNotePanel() : base(ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "BookPanel", AssetRequestMode.ImmediateLoad)
            , 38, 100, 50, 50, 1)
        {

        }

        public override void InitPageGroups()
        {
            pageGroups = 
                [
                    new GroupReadfragment()
                ];
        }
    }
}
