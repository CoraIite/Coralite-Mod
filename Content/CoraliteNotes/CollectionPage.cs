using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.CoraliteNotes
{
    public class CollectionPage : KnowledgePage
    {
        public void DrawCollectText(SpriteBatch spriteBatch, bool[] collects, Vector2 center)
        {
            bool allCollect = true;
            foreach (var c in collects)
                if (!c)
                {
                    allCollect = false;
                    break;
                }

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
