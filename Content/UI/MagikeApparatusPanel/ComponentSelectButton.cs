using Coralite.Core.Systems.MagikeSystem;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.UI;

namespace Coralite.Content.UI.MagikeApparatusPanel
{
    /// <summary>
    /// 完全和贴图配合的类
    /// </summary>
    public class ComponentSelectButton : UIElement
    {
        private int slot;

        public ComponentSelectButton(int slot)
        {
            Asset<Texture2D> buttonTex = MagikeSystem.GetUIApparatusButton();
            Vector2 size = buttonTex.Frame(MagikeComponentID.Count + 1, 2, slot, 0).Size();
            Width.Set(size.X, 0);
            Height.Set(size.Y, 0);

            PaddingLeft = 6;
            PaddingRight = 6;

            this.slot = slot;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);

        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

             

            if (slot == 0)//第零个为操控所有的
            {
                if (MagikeApparatusPanel.ShowComponents[slot])
                    Array.Fill(MagikeApparatusPanel.ShowComponents, false);
                else
                    Array.Fill(MagikeApparatusPanel.ShowComponents, true);
            }
            else
            {
                if (MagikeApparatusPanel.ShowComponents[slot])
                    MagikeApparatusPanel.ShowComponents[slot] = false;
                else
                    MagikeApparatusPanel.ShowComponents[slot] = true;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetInnerDimensions().Position();

            Vector2 scale = Vector2.One;
            int frameY = MagikeApparatusPanel.ShowComponents[slot] ? 1 : 0;

            if (IsMouseHovering)
            {
                float factor = MathF.Sin((int)Main.timeForVisualEffects * 0.1f) * 0.05f;
                scale.X = 1.05f + factor;
                scale.Y = 1.05f - factor;

                //设置鼠标文本

            }

            Texture2D tex = MagikeSystem.GetUIApparatusButton().Value;
            var frameBox = tex.Frame(MagikeComponentID.Count + 1, 2, slot, frameY);

            spriteBatch.Draw(tex, pos, frameBox, Color.White, 0, Vector2.Zero, scale, 0, 0);
        }
    }
}
