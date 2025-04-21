using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeInterstitial1
{
    [AutoLoadTexture(Path = AssetDirectory.CoraliteNote + "MagikeInterstitial1/")]
    public class MagikeInterstitial1Page : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText Description { get; private set; }

        public static ATex SkyIslandUnlockGuide { get; private set; }

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            Description = this.GetLocalization(nameof(Description));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Utils.DrawBorderStringBig(spriteBatch, Title.Value, Center + new Vector2(0, -PageWidth / 2), Coralite.CrystallinePurple, 1, 0.5f, 0.5f);
            Vector2 pos = Position + new Vector2(PageWidth / 2, 140);

            Helper.DrawTextParagraph(spriteBatch, Description.Value, PageWidth, new Vector2(Position.X, pos.Y), out _);

            Texture2D tex = SkyIslandUnlockGuide.Value;
            float scale1 = 1f;

            //绘制图
            spriteBatch.Draw(tex, Bottom + new Vector2(0, -40), null, Color.White, 0, new Vector2(tex.Width / 2, tex.Height), scale1, 0, 0);
        }
    }
}
