using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.Readfragment
{
    [AutoLoadTexture(Path = AssetDirectory.NoteReadfragment)]
    public class DescriptionPage : KnowledgePage
    {
        public static LocalizedText Description { get; private set; }
        public static LocalizedText HowToUse { get; private set; }

        public static ATex IconPage { get; private set; }

        public override void OnInitialize()
        {
            Description = this.GetLocalization(nameof(Description));
            HowToUse = this.GetLocalization(nameof(HowToUse));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            IconPage.Value.QuickCenteredDraw(spriteBatch, Center, scale: 1.2f);

            Texture2D mainTex = ModContent.Request<Texture2D>("Coralite/icon").Value;

            spriteBatch.Draw(mainTex, Center + new Vector2(0, -115), null, Color.White, 0, mainTex.Size() / 2, 2f, 0, 0);

            Vector2 pos = Center + new Vector2(0, 70);
            Helper.DrawText(spriteBatch, Description.Value, PageWidth, pos, Vector2.One / 2, Vector2.One
                , Coralite.TextShadowColor, Coralite.MagicCrystalPink, out Vector2 size);

            pos.Y += size.Y + 20;
            Helper.DrawText(spriteBatch, HowToUse.Value, PageWidth, pos, Vector2.One / 2, Vector2.One
                , Coralite.TextShadowColor, Coralite.MagicCrystalPink, out _);
        }
    }
}
