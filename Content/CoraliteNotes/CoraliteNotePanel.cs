using Coralite.Content.UI.UILib;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace Coralite.Content.CoraliteNotes
{
    public class CoraliteNotePanel : UI_BookPanel
    {
        public CoraliteNotePanel() : base(ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "BookPanel", AssetRequestMode.ImmediateLoad)
            , 38, 100, 50, 50)
        {

        }

        public override void InitPageGroups()
        {
            pageGroups =
                [
                    new Readfragment.GroupReadfragment(),
                    new RedJade.GroupRedJade(),
                    new MagikeChapter1.GroupMagikeChapter1(),
                    new IceDragonChapter1.GroupIceDragonChapter1(),
                ];
        }
    }
}
