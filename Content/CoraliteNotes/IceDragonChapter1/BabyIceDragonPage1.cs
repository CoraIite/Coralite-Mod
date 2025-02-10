using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
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
            Utils.DrawBorderStringBig(spriteBatch, Title.Value, Center + new Vector2(0, -PageWidth / 2), Coralite.IcicleCyan, 1, 0.5f, 0.5f);

            Vector2 pos = Position + new Vector2(PageWidth / 2, 140);

            Helper.DrawTextParagraph(spriteBatch, Description.Value, PageWidth, new Vector2(Position.X, pos.Y), out _);

            Texture2D tex = CoraliteAssets.IceDragon1.BabyIceDragon.Value;
            float scale1 = 1f;

            //绘制图
            pos = Position + new Vector2(PageWidth / 2, PageHeight  - tex.Height * scale1 / 2);
            tex.QuickCenteredDraw(spriteBatch, pos, scale: scale1);

        }
    }
}
