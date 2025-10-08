using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter2
{
    [VaultLoaden(AssetDirectory.NoteMagikeS2)]
    public class MabirdNestConnect : KnowledgePage
    {
        public static LocalizedText ConnectDescription { get; private set; }
        public static LocalizedText ConnectDescription2 { get; private set; }
        public static LocalizedText ConnectDescription3 { get; private set; }

        public static ATex MabirdNestConnectTex { get; private set; }
        public static ATex MabirdNestConnectTex2 { get; private set; }

        public override void OnInitialize()
        {
            ConnectDescription = this.GetLocalization(nameof(ConnectDescription));
            ConnectDescription2 = this.GetLocalization(nameof(ConnectDescription2));
            ConnectDescription3 = this.GetLocalization(nameof(ConnectDescription3));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 60);

            //描述段1
            Helper.DrawTextParagraph(spriteBatch, ConnectDescription.Value, PageWidth, new Vector2(Position.X, pos.Y), out Vector2 textSize);
            pos.Y += textSize.Y;

            float scale = 0.5f;
            pos.Y += MabirdNestConnectTex.Height() / 2 * scale;
            MabirdNestConnectTex.Value.QuickCenteredDraw(spriteBatch, pos, scale: scale);
            pos.Y += 20 + MabirdNestConnectTex.Height() / 2 * scale;

            //描述段2
            Helper.DrawTextParagraph(spriteBatch, ConnectDescription2.Value, PageWidth, new Vector2(Position.X, pos.Y), out textSize);

            scale = 0.5f;
            pos.Y += textSize.Y + MabirdNestConnectTex2.Height() / 2 * scale;

            MabirdNestConnectTex2.Value.QuickCenteredDraw(spriteBatch, pos, scale: scale);
            pos.Y += 20 + MabirdNestConnectTex2.Height() / 2 * scale;

            //描述段3
            Helper.DrawTextParagraph(spriteBatch, ConnectDescription3.Value, PageWidth, new Vector2(Position.X, pos.Y), out textSize);
        }
    }
}
