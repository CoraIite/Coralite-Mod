using Coralite.Core.Loaders;
using Coralite.Core.Systems.MagikeSystem;
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
        where TComponent : MagikeComponent
    {
        protected readonly TComponent component;
        private readonly Func<TComponent, string> text;
        private Vector2 scale;

        public ComponentUIElementText(Func<TComponent, string> text, TComponent component, UIElement parent, Vector2? scale = null)
        {
            this.text = text;
            this.component = component;

            float width = parent.GetInnerDimensions().Width + 50;

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
            Helpers.Helper.DrawText(spriteBatch, text(component), Parent.GetInnerDimensions().Width + 52, GetDimensions().Position() + new Vector2(2, 4)
                , new Vector2(0), scale, new Color(0, 0, 0, 150), Color.White, out Vector2 textSize);
            if (Height.Pixels != textSize.Y || Width.Pixels != textSize.X)
            {
                Width.Pixels = textSize.X;
                Height.Pixels = textSize.Y;
                UILoader.GetUIState<MagikeApparatusPanel>().ComponentPanel.Recalculate();
            }
        }
    }

    public class ComponentUIElementText : UIElement
    {
        private readonly Func<string> text;
        private Vector2 scale;

        public ComponentUIElementText(Func<string> text, UIElement parent, Vector2? scale = null)
        {
            this.text = text;

            float width = parent.GetInnerDimensions().Width + 50;

            string text2 = FontAssets.MouseText.Value.CreateWrappedText(text(), width);

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
            Helpers.Helper.DrawText(spriteBatch, text(), Parent.GetInnerDimensions().Width + 52, GetDimensions().Position() + new Vector2(2, 4)
                , new Vector2(0), scale, new Color(0, 0, 0, 150), Color.White, out Vector2 textSize);
            if (Height.Pixels != textSize.Y || Width.Pixels != textSize.X)
            {
                Width.Pixels = textSize.X;
                Height.Pixels = textSize.Y;
                UILoader.GetUIState<MagikeApparatusPanel>().ComponentPanel.Recalculate();
            }
        }
    }
}
