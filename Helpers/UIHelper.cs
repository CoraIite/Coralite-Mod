using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Coralite.Helpers
{
    public partial class Helper
    {
        public static void SetCenter(this UIElement element,Vector2 center)
        {
            element.Top.Set(center.X - element.Width.Pixels, 0);
            element.Left.Set(center.Y - element.Height.Pixels, 0);
        }

        /// <summary>
        /// 快速绘制，黑底白字，缩放为1
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="originText"></param>
        /// <param name="maxWidth"></param>
        /// <param name="position"></param>
        /// <param name="origin"></param>
        public static void DrawTextQuick(SpriteBatch spriteBatch, string originText, float maxWidth, Vector2 position, Vector2 origin, out Vector2 textSize)
        {
            DrawText(spriteBatch, originText, maxWidth, position, origin, Vector2.One, Color.Black, Color.White, out textSize);
        }

        /// <summary>
        /// 在UI中绘制文本
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="position"></param>
        /// <param name="maxWidth"></param>
        public static void DrawText(SpriteBatch spriteBatch, string originText, float maxWidth, Vector2 position, Vector2 origin, Vector2 scale, Color shadowColor, Color textColor, out Vector2 textSize)
        {
            string text = FontAssets.MouseText.Value.CreateWrappedText(originText, maxWidth);

            TextSnippet[] textSnippets = ChatManager.ParseMessage(text, Color.White).ToArray();
            ChatManager.ConvertNormalSnippets(textSnippets);

            textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, textSnippets, scale, maxWidth);

            foreach (Vector2 direction in ChatManager.ShadowDirections)
            {
                ChatManager.DrawColorCodedStringShadow(spriteBatch, FontAssets.MouseText.Value, textSnippets, position + direction,
                    shadowColor, 0f, origin, scale, maxWidth);
            }
            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, textSnippets,
                position, textColor, 0f, origin, scale, out _, maxWidth, false);
        }

    }
}
