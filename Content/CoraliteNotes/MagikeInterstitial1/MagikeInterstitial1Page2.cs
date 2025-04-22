using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeInterstitial1
{
    [AutoLoadTexture(Path = AssetDirectory.CoraliteNote + "MagikeInterstitial1/")]
    public class MagikeInterstitial1Page2 : KnowledgePage
    {
        public static LocalizedText Description { get; private set; }

        public static ATex SkyIslandEnemyWarn { get; private set; }

        public override void OnInitialize()
        {
            Description = this.GetLocalization(nameof(Description));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = Position + new Vector2(PageWidth / 2, 20);

            Helper.DrawTextParagraph(spriteBatch, Description.Value, PageWidth, new Vector2(Position.X, pos.Y), out _);

            Texture2D tex = SkyIslandEnemyWarn.Value;
            float scale1 = 1.15f;

            //绘制图
            spriteBatch.Draw(tex, Bottom + new Vector2(0, -40), null, Color.White, 0, new Vector2(tex.Width / 2, tex.Height), scale1, 0, 0);
        }
    }
}
