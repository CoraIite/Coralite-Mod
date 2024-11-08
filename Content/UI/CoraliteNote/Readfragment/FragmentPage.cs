using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;
using Terraria.UI;

namespace Coralite.Content.UI.CoraliteNote.Readfragment
{
    public class FragmentPage : KnowledgePage
    {
        public static LocalizedText Fragments {  get; set; }

        public override void OnInitialize()
        {
            Fragments=this.GetLocalization(nameof(Fragments));
        }

        public override void Recalculate()
        {

        }

        public void AddFragments(FixedUIGrid grid)
        {
            //遍历知识，把所有的知识都加入进去
        }
    }

    public class FragmentSlot:UIElement
    {
        public int KnowledgeID;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            //绘制背景板

            //绘制对应的图标
            Texture2D tex = CoraliteContent.GetKKnowledge(KnowledgeID).Texture2D.Value;
        }
    }
}
