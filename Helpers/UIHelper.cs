using Coralite.Content.CoraliteNotes;
using Coralite.Content.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;
using static System.Net.Mime.MediaTypeNames;

namespace Coralite.Helpers
{
    public partial class Helper
    {
        public static void SetCenter(this UIElement element, Vector2 center)
        {
            element.Left.Set(center.X - (element.Width.Pixels / 2), 0);
            element.Top.Set(center.Y - (element.Height.Pixels / 2), 0);
        }

        /// <summary>
        /// 获取宽度+左右的间隔
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static float OutsideWidth(this UIElement element)
            => element.Width.Pixels + element.PaddingLeft + element.PaddingRight;

        /// <summary>
        /// 获取宽度+左右的间隔
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static float OutsideHeight(this UIElement element)
            => element.Height.Pixels + element.PaddingTop + element.PaddingBottom;

        public static void SetSize(this UIElement element, float widthPixel, float heightPixel, float widthPercent = 0, float heightPercent = 0)
        {
            element.Width.Set(widthPixel, widthPercent);
            element.Height.Set(heightPixel, heightPercent);
        }

        public static void SetSize(this UIElement element, Vector2 size, float widthPercent = 0, float heightPercent = 0)
        {
            element.Width.Set(size.X, widthPercent);
            element.Height.Set(size.Y, heightPercent);
        }

        public static void SetTopLeft(this UIElement element, float topPixel, float leftPixel, float topPercent = 0, float leftPercent = 0)
        {
            element.Top.Set(topPixel, topPercent);
            element.Left.Set(leftPixel, leftPercent);
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
        /// 根据物品type获取对应的贴图与帧
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="tex"></param>
        /// <param name="frameBox"></param>
        public static void GetItemTexAndFrame(int itemType, out Texture2D tex, out Rectangle frameBox)
        {
            Main.instance.LoadItem(itemType);
            tex = TextureAssets.Item[itemType].Value;

            if (Main.itemAnimations[itemType] != null)
                frameBox = Main.itemAnimations[itemType].GetFrame(tex, -1);
            else
                frameBox = tex.Frame();
        }

        /// <summary>
        /// 在UI中绘制文本
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="position"></param>
        /// <param name="maxWidth"></param>
        public static void DrawText(SpriteBatch spriteBatch, string originText, float maxWidth, Vector2 position, Vector2 origin, Vector2 scale, Color shadowColor, Color textColor, out Vector2 textSize, bool useIncomeColor = false)
        {
            string text = FontAssets.MouseText.Value.CreateWrappedText(originText, maxWidth);

            TextSnippet[] textSnippets = [.. ChatManager.ParseMessage(text, Color.White)];
            ChatManager.ConvertNormalSnippets(textSnippets);

            textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, textSnippets, scale, maxWidth);
            origin *= textSize;

            foreach (Vector2 direction in ChatManager.ShadowDirections)
            {
                //ChatManager.DrawColorCodedStringShadow(spriteBatch, FontAssets.MouseText.Value, textSnippets, position,
                //shadowColor, 0f, origin, scale, maxWidth, 2f);
                ChatManager.DrawColorCodedStringShadow(spriteBatch, FontAssets.MouseText.Value, textSnippets, position + direction,
                        shadowColor, 0f, origin, scale, maxWidth, 1.5f);
                //ChatManager.DrawColorCodedStringShadow(spriteBatch, FontAssets.MouseText.Value, textSnippets, position,
                //    shadowColor, 0f, origin, scale, maxWidth, 1f);
            }

            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, textSnippets,
                position, textColor, 0f, origin, scale, out _, maxWidth, useIncomeColor);
        }

        /// <summary>
        /// 按照一段的方式绘制文字
        /// </summary>
        public static void DrawTextParagraph(SpriteBatch spriteBatch, string originText, float maxWidth, Vector2 position, out Vector2 textSize, Vector2? scale = null, Color? shadowColor = null, Color? textColor = null)
          => DrawText(spriteBatch, originText, maxWidth, position, Vector2.Zero
                , scale ?? Vector2.One
                , shadowColor ?? Coralite.TextShadowColor
                , textColor ?? Color.White
                , out textSize);

        /// <summary>
        /// 鼠标的屏幕位置是否包含在矩形中
        /// </summary>
        /// <param name="tect"></param>
        /// <returns></returns>
        public static bool MouseScreenInRect(this Rectangle rect)
        {
            Vector2 pos = Main.MouseWorld - Main.screenPosition;
            return rect.Contains((int)pos.X, (int)pos.Y);
        }

        public static void DrawMouseOverScaleTex(SpriteBatch spriteBatch, Texture2D tex, Vector2 pos, ScaleController scale, float offset, Color darkColor, bool fadeWithOriginScale = false, Color? selfColor = null)
        {
            Vector2 origin = tex.Size() / 2;
            spriteBatch.Draw(tex, pos + Vector2.One * Lerp(0, offset, scale.ScalePercent), null,
               darkColor, 0, origin, fadeWithOriginScale ? scale.targetScale : scale.Scale, 0, 0);
            spriteBatch.Draw(tex, pos, null, selfColor ?? Color.White, 0, origin, scale.Scale, 0, 0);
        }

        public static void QuickInvisibleScrollbar(this UIList list)
        {
            var scrollbar = new UIScrollbar();
            scrollbar.SetTopLeft(5000, 5000);
            list.SetScrollbar(scrollbar);
        }

        public static void QuickInvisibleScrollbar(this FixedUIGrid grid)
        {
            var scrollbar = new UIScrollbar();
            scrollbar.SetTopLeft(5000, 5000);
            grid.SetScrollbar(scrollbar);
        }
    }
}
