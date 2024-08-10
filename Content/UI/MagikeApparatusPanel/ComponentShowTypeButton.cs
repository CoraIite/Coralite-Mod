using Coralite.Core.Loaders;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.UI;

namespace Coralite.Content.UI.MagikeApparatusPanel
{
    public class ComponentShowTypeButton : UIElement
    {
        private float _scale;

        public ComponentShowTypeButton()
        {
            Asset<Texture2D> buttonTex = MagikeSystem.GetUIShowTypeButton();
            Vector2 size = buttonTex.Frame(2, 1, 0, 0).Size();
            Width.Set(size.X, 0);
            Height.Set(size.Y, 0);

            PaddingLeft = 2;
            PaddingRight = 2;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);

        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            //切换显示方式
            MagikeApparatusPanel.CurrentComponentShowType++;
            if (MagikeApparatusPanel.CurrentComponentShowType > MagikeApparatusPanel.ComponentShowType.VerticalBar)
                MagikeApparatusPanel.CurrentComponentShowType = MagikeApparatusPanel.ComponentShowType.Grid;

            UILoader.GetUIState<MagikeApparatusPanel>().Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetInnerDimensions().Center();

            int frameX = (int)MagikeApparatusPanel.CurrentComponentShowType;

            if (IsMouseHovering)
            {
                _scale = Helper.Lerp(_scale, 1.2f, 0.2f);
                //设置鼠标文本

            }
            else
                _scale = Helper.Lerp(_scale, 1f, 0.2f);

            Texture2D tex = MagikeSystem.GetUIShowTypeButton().Value;
            var frameBox = tex.Frame(2, 1, frameX, 0);

            spriteBatch.Draw(tex, pos, frameBox, Color.White, 0, frameBox.Size() / 2, _scale, 0, 0);
        }

    }
}
