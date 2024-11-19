using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class WhatIsMagikePage : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText Description { get; private set; }

        private float _scale = 0.3f;

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            Description = this.GetLocalization(nameof(Description));
        }

        public override void Recalculate()
        {
            _scale = 0.5f;
            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 60);
            Utils.DrawBorderStringBig(spriteBatch, Title.Value, pos, Coralite.MagicCrystalPink
                , 0.8f, 0.5f, 0.5f);

            pos += new Vector2(0, 60);

            Helper.DrawText(spriteBatch, Description.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out Vector2 textSize);

            var tex = CoraliteAssets.MagikeChapter1.WhatIsMagike.Value;
            pos.Y += textSize.Y + 20 + tex.Height / 2;
             
            Rectangle rect = Utils.CenteredRectangle(pos, tex.Size());
            Vector2 pos2 = Main.MouseWorld - Main.screenPosition;
            if (rect.Contains((int)pos2.X, (int)pos2.Y))
                _scale = Helper.Lerp(_scale, 1f, 0.15f);
            else
                _scale = Helper.Lerp(_scale, 0.8f, 0.15f);

            spriteBatch.Draw(tex, pos + Vector2.One * Helper.Lerp(0, 20, (_scale - 0.8f) / 0.2f), null,
                Color.DarkGray * 0.75f, 0, tex.Size() / 2, _scale, 0, 0);
            spriteBatch.Draw(tex, pos, null, Color.White, 0, tex.Size() / 2, _scale, 0, 0);
        }
    }
}
