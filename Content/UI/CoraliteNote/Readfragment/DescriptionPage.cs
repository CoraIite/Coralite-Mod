using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.UI.CoraliteNote.Readfragment
{
    public class DescriptionPage : KnowledgePage
    {
        public static LocalizedText Description { get; private set; }

        public override void OnInitialize()
        {
            Description = this.GetLocalization(nameof(Description));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Helper.DrawText(spriteBatch, Description.Value, PageWidth, Center, Vector2.One / 2, Vector2.One
                , new Color(40, 40, 40), Coralite.MagicCrystalPink, out _);
        }
    }
}
