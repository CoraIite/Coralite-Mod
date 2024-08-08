using Coralite.Core.Systems.MagikeSystem;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

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

            Texture2D tex = MagikeSystem.GetUIApparatusButton().Value;
            var frameBox = tex.Frame(MagikeComponentID.Count + 1, 2, id, frameY);

            spriteBatch.Draw(tex, pos, frameBox, Color.White * alpha, 0, Vector2.Zero, 1, 0, 0);
        }
    }

    public class ComponentButton
    {

    }
}
