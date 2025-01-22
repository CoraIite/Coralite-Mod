using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class PolymerizeCraft2 : KnowledgePage
    {
        public static LocalizedText ConnectToPedestal { get; private set; }
        public static LocalizedText CheckAltar { get; private set; }

        public override void OnInitialize()
        {
            ConnectToPedestal = this.GetLocalization(nameof(ConnectToPedestal));
            CheckAltar = this.GetLocalization(nameof(CheckAltar));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 20);


            //描述段1
            Helper.DrawTextParagraph(spriteBatch, ConnectToPedestal.Value, PageWidth, new Vector2(Position.X, pos.Y), out Vector2 textSize);
            ATex tex1 = CoraliteAssets.MagikeChapter1.ConnectToPedestal;
            pos.Y += textSize.Y + 10 + tex1.Height()  / 2;

            //绘制图1
            tex1.Value.QuickCenteredDraw(spriteBatch, pos);
            pos.Y += 20 + tex1.Height() / 2;

            float scale2 = 0.8f;

            //描述段2
            Helper.DrawTextParagraph(spriteBatch, CheckAltar.Value, PageWidth, new Vector2(Position.X, pos.Y), out textSize);
            ATex tex2 = CoraliteAssets.MagikeChapter1.CheckAltar;
            pos.Y += textSize.Y + 10 + tex2.Height() * scale2 / 2;

            //绘制图2
            tex2.Value.QuickCenteredDraw(spriteBatch, pos, scale: scale2);
        }
    }
}
