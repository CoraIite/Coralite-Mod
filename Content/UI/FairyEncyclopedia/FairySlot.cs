using Coralite.Core.Systems.FairyCatcherSystem;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using Terraria.UI;

namespace Coralite.Content.UI.FairyEncyclopedia
{
    public class FairySlot:UIElement
    {
        private Fairy _fairy;

        /// <summary>
        /// 自身在UIGrid里的索引
        /// </summary>
        public readonly int index; 



        public override void Update(GameTime gameTime)
        {
            if (IsMouseHovering)//鼠标在上面，开始更新自身的样子
            {
                UpdateFairy();
            }
            else
            {

            }
            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {



            base.DrawSelf(spriteBatch);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            //切换到个体面板
            base.LeftClick(evt);
        }



        public void UpdateFairy()
        {
            if (_fairy == null)
                return;
            if (++_fairy.frameCounter > 6)
            {
                _fairy.frameCounter = 0;
                if (++_fairy.frame.Y > _fairy.VerticalFrames)
                    _fairy.frame.Y = 0;
            }
        }
    }
}
