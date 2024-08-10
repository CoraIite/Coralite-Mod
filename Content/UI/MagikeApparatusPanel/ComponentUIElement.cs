using Coralite.Core.Systems.CoraliteActorComponent;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Coralite.Content.UI.MagikeApparatusPanel
{
    public abstract class ComponentUIElement(Component component):UIElement
    {

    }

    public class ComponentUIElementText<TComponent>:UIElement 
        where TComponent : Component 
    {
        private readonly TComponent component;
        private readonly Func<TComponent,string> text;

        public ComponentUIElementText(Func<TComponent,string> text,TComponent component,UIElement parent)
        {
            this.text = text;
            this.component = component;

            float width = parent.GetInnerDimensions().Width;

            string text2 = FontAssets.MouseText.Value.CreateWrappedText(text(component), width);

            TextSnippet[] textSnippets = [.. ChatManager.ParseMessage(text2, Color.White)];
            ChatManager.ConvertNormalSnippets(textSnippets);

           Vector2 textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, textSnippets, Vector2.One, width);

            Width.Set(0, 1);
            Height.Set(textSize.Y, 0);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Helpers.Helper.DrawTextQuick(spriteBatch, text(component), Parent.GetInnerDimensions().Width, GetDimensions().Position(), Vector2.Zero, out Vector2 textSize);
            if (Height.Pixels!=textSize.Y)
            {
                Height.Pixels = textSize.Y;
                Recalculate();
            }
        }
    }
}
