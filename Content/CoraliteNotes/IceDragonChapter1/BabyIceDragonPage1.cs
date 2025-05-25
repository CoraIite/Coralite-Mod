using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.IceDragonChapter1
{
    public class BabyIceDragonPage1 : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText Description { get; private set; }

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            Description = this.GetLocalization(nameof(Description));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawTitle(spriteBatch, Title, Coralite.IcicleCyan);

            Vector2 pos = Position + new Vector2(PageWidth / 2, TitleHeight);

            DrawParaNormal(spriteBatch, Description, pos.Y, out _);

            Texture2D tex = CoraliteAssets.IceDragon1.BabyIceDragon.Value;
            float scale1 = 0.9f;

            //绘制图
            tex.QuickBottomDraw(spriteBatch, Bottom, scale: scale1);
        }
    }
}
