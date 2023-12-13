using Coralite.Content.UI.UILib;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter1
{
    public class C1_2_Origin1 : UIPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        public static LocalizedText _2_Name { get; private set; }
        public static LocalizedText Origin1 { get; private set; }
        public static LocalizedText Origin2 { get; private set; }

        public override void OnInitialize()
        {
            _2_Name = this.GetLocalization("_2_Name", () => "1.2 魔能的来源");
            Origin1 = this.GetLocalization("Origin1", () => "    魔能会出现在一些天然矿物中，比如魔力晶体。通过神奇的地质运动，最初的魔能团在逐渐冷却降温后结成晶体，这些晶体团接触到石头并与之融合，形成了拥有较好的魔能传导能力的玄武岩，将晶体包裹在其中。");
            Origin2 = this.GetLocalization("Origin2", () => "    除了天然的魔能，使用各类素材制造而出的各种透镜也拥有制造魔能的能力，只不过它们需要一些特殊的条件来进行生产。");
        }

        public override bool CanShowInBook => true;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 60);
            Utils.DrawBorderStringBig(spriteBatch, _2_Name.Value, pos, Coralite.Instance.MagicCrystalPink
                , 0.8f, 0.5f, 0.5f);

            pos += new Vector2(0, 60);

            //文字段1
            Helpers.Helper.DrawText(spriteBatch, Origin1.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out Vector2 textSize);

            pos += new Vector2(0, textSize.Y + 30);

            //插图1
            Texture2D mainTex1 = ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "C1_2_Origin1").Value;
            spriteBatch.Draw(mainTex1, pos, null, Color.White, 0, new Vector2(mainTex1.Width / 2, 0), 1, 0, 0);
           
            pos += new Vector2(0, mainTex1.Height + 30);

            //文字段2
            Helpers.Helper.DrawText(spriteBatch, Origin2.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out textSize);

            pos += new Vector2(0, textSize.Y + 30);

            Texture2D mainTex2 = ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "C1_2_Origin2").Value;
            spriteBatch.Draw(mainTex2, pos, null, Color.White, 0, new Vector2(mainTex2.Width / 2, 0), 1, 0, 0);
        }
    }
}
