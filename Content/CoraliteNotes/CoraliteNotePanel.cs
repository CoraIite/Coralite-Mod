using Coralite.Content.UI.BookUI;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace Coralite.Content.CoraliteNotes
{
    public class CoraliteNotePanel : UI_BookPanel
    {
        public CoraliteNotePanel() : base(ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "BookPanel", AssetRequestMode.ImmediateLoad)
            , 28, 80, 40, 30)
        {
            PanelBackTex = ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "BookPanelBack", AssetRequestMode.ImmediateLoad);
        }

        public override void InitPageGroups()
        {
            pageGroups =
                [
                    new Readfragment.GroupReadfragment(),

                    new RedJade.GroupRedJade(),
                    new IceDragonChapter1.GroupIceDragonChapter1(),
                    new ThunderChapter1.GroupThunderChapter1(),

                    new SlimeChapter1.GroupSlimeChapter1(),
                    new NightmareChapter.GroupNightmare(),

                    new MagikeChapter1.GroupMagikeChapter1(),
                    new MagikeInterstitial1.GroupMagikeInterstitial1(),
                    new MagikeChapter2.GroupMagikeChapter2(),

                    new SwordChapter.GroupSwordChapter(),
                    new FlyingShieldChapter.GroupFlyingShield(),
                    new FlowerGunChapter.GroupFlowerGun(),
                    new DashBowChapter.GroupDashBowChapter(),
                    new LandOfTheLustrousChapter.GroupLandOfTheLustrous(),

                    new CoraliteActivities.GroupCoraliteActivities(),
                ];
        }
    }
}
