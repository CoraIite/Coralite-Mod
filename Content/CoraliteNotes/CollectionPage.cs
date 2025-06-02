using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.CoraliteNotes
{
    public class CollectionPage : KnowledgePage
    {
        public void DrawCollectTip(SpriteBatch spriteBatch, bool[] collects)
        {
            if (collects.AllTrue())
                return;

            Utils.DrawBorderString(spriteBatch, CoraliteNoteSystem.HowToCollect.Value
                , PageTop + new Vector2(0, 10), Main.hslToRgb(Main.GlobalTimeWrappedHourly/2 % 1, 1f, 0.95f), anchorx: 0.5f, anchory: 0.5f);
        }

        public void DrawCollectText(SpriteBatch spriteBatch, bool[] collects, Vector2 center)
        {
            bool allCollect = collects.AllTrue();

            string text = CoraliteNoteSystem.CollectProgress.Value;
            Color color = Color.White;

            if (allCollect)
            {
                text = CoraliteNoteSystem.AllCollect.Value;
                color = Color.Coral;
            }

            Utils.DrawBorderString(spriteBatch, text, center, color, anchorx: 0.5f, anchory: 0.5f);
        }

        public void DrawCollectProgress(SpriteBatch spriteBatch, bool[] collects, Vector2 center)
        {
            int collected = 0;

            foreach (var c in collects)
                if (c)
                    collected++;

            Color color = Color.White;
            if (collected == collects.Length)
            {
                color = Color.Coral;
            }

            Utils.DrawBorderString(spriteBatch, $"{collected} / {collects.Length}", center, color, anchorx: 0.5f, anchory: 0.5f);
        }
    }
}
