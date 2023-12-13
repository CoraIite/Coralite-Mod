using Coralite.Content.UI.UILib;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter1
{
    public class C1_4_2_ClassifyOfInstrument : UIPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        public static LocalizedText _4_2_Name { get; private set; }
        public static LocalizedText Classify1 { get; private set; }
        public static LocalizedText Classify2 { get; private set; }
        public static LocalizedText Classify3 { get; private set; }
        public static LocalizedText Classify4 { get; private set; }

        public override void OnInitialize()
        {
            _4_2_Name = this.GetLocalization("_4_2_Name", () => "1.4.2 魔能仪器的分类");
            Classify1 = this.GetLocalization("Classify1", () => "    生产类仪器：透镜，通过各种各样的条件来生产魔能。");
            Classify2 = this.GetLocalization("Classify2", () => "    传递类仪器：广角镜，拥有更强的传导能力，体积也较小。另外还有多面镜这类特殊的广角镜，它们牺牲了一些传递距离换来了更多的连接数量。");
            Classify3 = this.GetLocalization("Classify3", () => "    存储类仪器：水晶柱，内部空间比想象中的大许多，可以使魔能在其内部循环流动，一般作为存储魔能的仪器。");
            Classify4 = this.GetLocalization("Classify4", () => "    以上3种是最基础的魔能仪器，都需要放置在地上之后才能开始工作，除此之外还有统称为“魔能工厂”的仪器，可以进行物品重塑，注魔，聚合，或者是射出攻击性弹幕之类的特殊工作。");
        }

        public override bool CanShowInBook => true;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "C1_4_2_ClassifyOfInstrument").Value;
            spriteBatch.Draw(mainTex, Bottom + new Vector2(0, -60), null, Color.White, 0, new Vector2(mainTex.Width / 2, mainTex.Height), PageWidth / mainTex.Width, 0, 0);

            Vector2 pos = Position + new Vector2(0, 30);
            Utils.DrawBorderString(spriteBatch, _4_2_Name.Value, pos, Coralite.Instance.MagicCrystalPink
                , 1, 0f, 00f);
            pos = PageTop + new Vector2(0, 60 + 30);

            Helpers.Helper.DrawText(spriteBatch, Classify1.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out Vector2 textSize);

            pos += new Vector2(0, textSize.Y + 10);

            Helpers.Helper.DrawText(spriteBatch, Classify2.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out textSize);

            pos += new Vector2(0, textSize.Y + 10);

            Helpers.Helper.DrawText(spriteBatch, Classify3.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out textSize);

            pos += new Vector2(0, textSize.Y + 10);

            Helpers.Helper.DrawText(spriteBatch, Classify4.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out _);
        }
    }
}
