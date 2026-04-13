using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace Coralite.Content.UI.NewKnowledgeUnlock
{
    public class NewKnowledgeBar : UIElement
    {
        public PRTGroup group;
        public int Timer;

        

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);


        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Timer++;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
        }
    }
}
