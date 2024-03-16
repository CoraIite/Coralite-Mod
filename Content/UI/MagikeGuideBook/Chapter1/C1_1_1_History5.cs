using Coralite.Content.UI.UILib;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter1
{
    public class C1_1_1_History5 : UIPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        public static LocalizedText History5 { get; private set; }

        public override void OnInitialize()
        {
            History5 = this.GetLocalization("History5", () => "    正在阅读这本书的人，我想你就是那个特异点，我期待你的到来。这本书将包含你想了解的魔能知识，因为书也拥有魔能，所以你无法理解的事物暂时不会向你展示，不断扩充知识解锁书中的内容吧，祝你好运！");
        }

        public override bool CanShowInBook => true;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = Position + new Vector2(0, 40);
            Helpers.Helper.DrawText(spriteBatch, History5.Value, PageWidth, pos, Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out _);


        }
    }
}
