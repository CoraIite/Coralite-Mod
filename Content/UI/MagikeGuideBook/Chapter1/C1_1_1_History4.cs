using Coralite.Content.UI.UILib;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter1
{
    public class C1_1_1_History4 : UIPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        public static LocalizedText History4 { get; private set; }

        public override void OnInitialize()
        {
            History4 = this.GetLocalization("History4", () => "    像往常那样，我打开了传送门，前往了其他未踏足过的世界。就在不久前，我观测到这里即将发生的变化，或许事情会变得有趣起来。因此，我写下了这本书，并利用能力批量复制，之后交给了妖精族制造的水晶生命体。");
        }

        public override bool CanShowInBook => true;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = Position + new Vector2(0, 40);
            Helpers.Helper.DrawText(spriteBatch, History4.Value, PageWidth, pos, Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out _);


        }
    }
}
