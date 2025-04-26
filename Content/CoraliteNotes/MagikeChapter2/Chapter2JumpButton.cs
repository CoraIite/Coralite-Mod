using Coralite.Content.UI.BookUI;
using Coralite.Core;
using Coralite.Core.Loaders;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter2
{
    public class Chapter2Jump : PageJumpButton
    {
        private LocalizedText Text { get; set; }

        public Chapter2Jump(LocalizedText text, Func<int> GetPage) : base(() => CoraliteNoteUIState.BookPanel, GetPage)
        {
            Text = text;
            SetImage(ModContent.Request<Texture2D>(AssetDirectory.NoteMagikeS2 + "MagikeButton"));
            SetHoverImage(ModContent.Request<Texture2D>(AssetDirectory.NoteMagikeS2 + "MagikeButton_Outline"));

            OnSuccessChangePage += Chapter1Jump_OnSuccessChangePage;

            scaleActive = 1.3f;
            scaleInactive = 1.1f;
        }

        private void Chapter1Jump_OnSuccessChangePage()
        {
            UILoader.GetUIState<CoraliteNoteUIState>().Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            const float r = 40;
            Vector2 pos = GetDimensions().Position() + new Vector2(r, Height.Pixels / 2);

            float scale = GetScale();

            spriteBatch.Draw(texture.Value, pos, null, Color.White, 0, texture.Size() / 2, 1f, 0, 0);
            Color textColor = Color.White;
            if (IsMouseHovering)
            {
                spriteBatch.Draw(borderTexture.Value, pos, null, Color.White, 0, borderTexture.Size() / 2, 1f, 0, 0);
                textColor = Coralite.CrystallinePurple;
            }

            pos += new Vector2(r, 4);
            Helpers.Helper.DrawText(spriteBatch, Text.Value, GetDimensions().Width
                , pos, new Vector2(0, 0.5f), Vector2.One * scale, new Color(40, 40, 40), textColor, out _, true);
        }
    }
}
