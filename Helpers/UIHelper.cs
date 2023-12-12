using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace Coralite.Helpers
{
    public partial class Helper
    {
        /// <summary>
        /// 快速绘制，黑底白字，缩放为1
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="originText"></param>
        /// <param name="maxWidth"></param>
        /// <param name="position"></param>
        /// <param name="origin"></param>
        public static void DrawTextQuick(SpriteBatch spriteBatch, string originText, float maxWidth, Vector2 position, Vector2 origin)
        {
            DrawText(spriteBatch, originText, maxWidth, position, origin, Vector2.One, Color.Black, Color.White);
        }

        /// <summary>
        /// 在UI中绘制文本
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="position"></param>
        /// <param name="maxWidth"></param>
        public static void DrawText(SpriteBatch spriteBatch, string originText, float maxWidth, Vector2 position, Vector2 origin, Vector2 scale, Color shadowColor, Color textColor)
        {
            string text = FontAssets.MouseText.Value.CreateWrappedText(originText, maxWidth);

            TextSnippet[] textSnippets = ChatManager.ParseMessage(text, Color.White).ToArray();
            ChatManager.ConvertNormalSnippets(textSnippets);

            foreach (Vector2 direction in ChatManager.ShadowDirections)
            {
                ChatManager.DrawColorCodedStringShadow(spriteBatch, FontAssets.MouseText.Value, textSnippets, position + direction,
                    shadowColor, 0f, origin, Vector2.One, maxWidth);
            }
            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, textSnippets,
                position, textColor, 0f, origin, Vector2.One, out _, maxWidth, false);
        }

    }
}
