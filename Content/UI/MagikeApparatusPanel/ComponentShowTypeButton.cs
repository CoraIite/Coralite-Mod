using Coralite.Core.Systems.MagikeSystem;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.UI;

namespace Coralite.Content.UI.MagikeApparatusPanel
{
    public class ComponentShowTypeButton:UIElement
    {
        public ComponentShowTypeButton()
        {
            Asset<Texture2D> buttonTex = MagikeSystem.GetUIApparatusButton();
            Vector2 size = buttonTex.Frame(2, 1, 0, 0).Size();
            Width.Set(size.X, 0);
            Height.Set(size.Y, 0);

            PaddingLeft = 4;
            PaddingRight = 4;
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
            if (MagikeApparatusPanel.CurrentComponentShowType>MagikeApparatusPanel.ComponentShowType.VerticalBar)
                MagikeApparatusPanel.CurrentComponentShowType = MagikeApparatusPanel.ComponentShowType.Grid;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetInnerDimensions().Position();

            float scale = 1;
            int frameX = (int)MagikeApparatusPanel.CurrentComponentShowType;

            if (IsMouseHovering)
            {
                scale = 1.2f;
                //设置鼠标文本

            }

            Texture2D tex = MagikeSystem.GetUIApparatusButton().Value;
            var frameBox = tex.Frame(2, 1, frameX, 0);

            spriteBatch.Draw(tex, pos, frameBox, Color.White, 0, Vector2.Zero, scale, 0, 0);
        }

    }
}
