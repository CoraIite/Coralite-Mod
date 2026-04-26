using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.UI;

namespace Coralite.Content.CoraliteNotes
{
    public class DangerousButton : UIElement, INoteLineDraw
    {
        public List<DangerousButton> PrevNodes;
        public List<DangerousButton> PostNodes;

        public readonly KnowledgeButtonType buttonType;
        public bool canShow;
        public bool reverseLine;
        public int DangerousLevel;

        public Color lineColor = Color.White;

        public bool[] flags;
        public int index;

        public DangerousButton(KnowledgeButtonType buttonType, bool[] flags,int index)
        {
            this.buttonType = buttonType;
            this.SetSize(80, 80);
            this.flags = flags;
            this.index = index;
        }

        public void DrawLine(SpriteBatch spriteBatch)
        {
        }

        public void AddPrevNode(DangerousButton element)
        {
            PrevNodes ??= [];
            PrevNodes.Add(element);

            element.PostNodes ??= [];
            element.PostNodes.Add(this);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);


        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            
        }
    }
}
