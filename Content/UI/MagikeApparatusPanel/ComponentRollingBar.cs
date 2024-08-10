using Coralite.Core.Loaders;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.UI;

namespace Coralite.Content.UI.MagikeApparatusPanel
{
    public class ComponentRollingBar(Action<int> setIndex, Func<int> getIndex) : UIRollingBar(setIndex, getIndex)
    {
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = MagikeSystem.GetUIRollingBar().Value;

            Vector2 center = GetDimensions().Center();
            float halfWidth = mainTex.Width / 2;

            if (Elements != null && Elements.Count > 0)
                halfWidth += Elements[0].GetOuterDimensions().Width / 2;

            for (int i = -1; i < 2; i += 2)
            {
                Rectangle frame = mainTex.Frame(2, 1, i > 0 ? 1 : 0);
                var origin = frame.Size() / 2;

                Vector2 pos = center + new Vector2(i * halfWidth, 0);

                spriteBatch.Draw(mainTex, pos, frame, Color.White, 0, origin, 1, 0, 0);
            }
        }
    }

    public class ComponentButtonAlpha(int index) : UIAlphaDrawElement
    {
        public override void DrawSelfAlpha(SpriteBatch spriteBatch, Vector2 center, float alpha)
        {
            if (MagikeApparatusPanel.CurrentEntity == null)
                return;

            Vector2 pos = GetInnerDimensions().Position();

            int id = MagikeApparatusPanel.CurrentEntity.ComponentsCache[index].ID;
            int frameY = MagikeApparatusPanel.CurrentShowComponentIndex == index ? 1 : 0;

            Texture2D tex = MagikeSystem.GetComponentButton().Value;
            var frameBox = tex.Frame(MagikeComponentID.Count, 2, id, frameY);

            spriteBatch.Draw(tex, pos, frameBox, Color.White * alpha, 0, Vector2.Zero, 1, 0, 0);
        }
    }

    public class ComponentButton : UIElement
    {
        private int index;
        private float _scale;

        public ComponentButton(int index)
        {
            Asset<Texture2D> buttonTex = MagikeSystem.GetComponentButton();
            Vector2 size = buttonTex.Frame(MagikeComponentID.Count, 2, 
               0, 0).Size();
            Width.Set(size.X, 0);
            Height.Set(size.Y, 0);

            //PaddingLeft = 4;
            //PaddingRight = 4;
            this.index = index;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            MagikeApparatusPanel.CurrentShowComponentIndex = index;

            UILoader.GetUIState<MagikeApparatusPanel>().ResetComponentPanel();

            base.LeftClick(evt);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (MagikeApparatusPanel.CurrentEntity == null)
                return;

            Vector2 pos = GetInnerDimensions().Center();

            int id = MagikeApparatusPanel.CurrentEntity.ComponentsCache[index].ID;
            int frameY = MagikeApparatusPanel.CurrentShowComponentIndex == index ? 1 : 0;

            Texture2D tex = MagikeSystem.GetComponentButton().Value;
            var frameBox = tex.Frame(MagikeComponentID.Count, 2, id, frameY);


            if (IsMouseHovering)
            {
                _scale = Helper.Lerp(_scale, 1.2f, 0.2f);

            }
            else
                _scale = Helper.Lerp(_scale, 1f, 0.2f);


            spriteBatch.Draw(tex, pos, frameBox, Color.White, 0, frameBox.Size()/2, _scale, 0, 0);
        }
    }
}
