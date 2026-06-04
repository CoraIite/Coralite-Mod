using Coralite.Content.UI.BookUI;
using Coralite.Core;
using Coralite.Core.Loaders;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace Coralite.Content.CoraliteNotes
{
    public class CoraliteNotePanel : UIBookPanel
    {
        public CoraliteNotePanel() : base(ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "BookPanel", AssetRequestMode.ImmediateLoad)
            , 28, 80, 40, 30)
        {
            PanelBackTex = ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "BookPanelBack", AssetRequestMode.ImmediateLoad);
        }

        public override void InitPageGroups()
        {
            //初始只包含目录
            pageGroups = [new Readfragment.GroupReadfragment()];
            //按照整理过的顺序遍历知识并添加书页集
            foreach (var series in KnowledgeLoader.SortedKnowledgeSerieses)
                foreach (var knowledge in series.ContainedKnowledges)
                {
                    var group = knowledge.GetUIPageGroup();
                    if (group == null)//使用默认的组
                        pageGroups.Add(new UIPageGroup(knowledge, knowledge.GetUIPages()));
                    else
                        pageGroups.Add(group);
                }

            //pageGroups =
            //    [
            //        new RedJade.GroupRedJade(),
            //        new IceDragonChapter1.GroupIceDragonChapter1(),
            //        new ThunderChapter1.GroupThunderChapter1(),

            //        new SlimeChapter1.GroupSlimeChapter1(),
            //        new NightmareChapter.GroupNightmare(),

            //        new MagikeChapter1.GroupMagikeChapter1(),
            //        new MagikeInterstitial1.GroupMagikeInterstitial1(),
            //        new MagikeChapter2.GroupMagikeChapter2(),

            //        new SwordChapter.GroupSwordChapter(),
            //        new FlyingShieldChapter.GroupFlyingShield(),
            //        new FlowerGunChapter.GroupFlowerGun(),
            //        new DashBowChapter.GroupDashBowChapter(),
            //        new LandOfTheLustrousChapter.GroupLandOfTheLustrous(),

            //        new CoraliteActivities.GroupCoraliteActivities(),
            //    ];
        }
    }
}
