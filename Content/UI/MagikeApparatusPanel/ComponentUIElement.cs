using Coralite.Core.Loaders;
using Coralite.Core.Systems.CoraliteActorComponent;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Coralite.Content.UI.MagikeApparatusPanel
{
    //public abstract class ComponentUIElement(Component component):UIElement
    //{

    //}

    public class ComponentUIElementText<TComponent> : UIElement
        where TComponent : Component
    {
        protected readonly TComponent component;
        private readonly Func<TComponent, string> text;
        private Vector2 scale;

        public ComponentUIElementText(Func<TComponent, string> text, TComponent component, UIElement parent, Vector2? scale = null)
        {
            this.text = text;
            this.component = component;

            float width = parent.GetInnerDimensions().Width;

            string text2 = FontAssets.MouseText.Value.CreateWrappedText(text(component), width);

            TextSnippet[] textSnippets = [.. ChatManager.ParseMessage(text2, Color.White)];
            ChatManager.ConvertNormalSnippets(textSnippets);

            Vector2 textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, textSnippets, scale ?? Vector2.One, width);

            Width.Set(textSize.X, 0);
            Height.Set(textSize.Y, 0);

            if (scale.HasValue)
                this.scale = scale.Value;
            else
                this.scale = Vector2.One;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Helpers.Helper.DrawText(spriteBatch, text(component), Parent.GetInnerDimensions().Width, GetDimensions().Position() + new Vector2(0, 4)
                , new Vector2(0.5f), scale, new Color(0, 0, 0, 150), Color.White, out Vector2 textSize);
            if (Height.Pixels != textSize.Y || Width.Pixels != textSize.X)
            {
                Width.Pixels = textSize.X;
                Height.Pixels = textSize.Y;
                UILoader.GetUIState<MagikeApparatusPanel>().ComponentPanel.Recalculate();
            }
        }
    }
}
