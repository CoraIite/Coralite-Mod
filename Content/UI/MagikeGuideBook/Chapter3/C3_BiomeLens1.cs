using Coralite.Content.UI.UILib;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter3
{
    public class C3_BiomeLens1 : UIPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        public static LocalizedText TitleName { get; private set; }
        public static LocalizedText BiomeLens1 { get; private set; }
        public static LocalizedText ForestLens { get; private set; }
        public static LocalizedText MarbleLens { get; private set; }
        public static LocalizedText GraniteLens { get; private set; }

        public override void OnInitialize()
        {
            TitleName = this.GetLocalization("TitleName", () => "环境透镜");
            BiomeLens1 = this.GetLocalization("BiomeLens1", () => "    这些环境透镜只有在你处于特定环境中时才能放置下来。放置后它会自动工作，不需要消耗任何东西就能生产魔能。");
            ForestLens = this.GetLocalization("ForestLens", () => "    森林透镜需要被放置在地表森林。清晨的树林中传来鸟鸣声，太阳花随着太阳的升起而开放。墨绿色的晶体，漂浮在爬满青苔的森之石上。");
            MarbleLens = this.GetLocalization("MarbleLens", () => "    大理石透镜需要被放置在地底的大理石环境中。地底的白色洞窟内传来敲打的声音，穿着战甲的骷髅正漫无目的地游走着。白色的八面体石头包裹着中心的晶体，漂浮在同样是白色的底座上。");
            GraniteLens = this.GetLocalization("GraniteLens", () => "    花岗岩透镜需要被放置在地底的花岗岩环境之中。地底的深蓝色洞窟内传来流水的声响，蓝色的神秘能量驱动着花岗精四处飞翔。深蓝色的石头包裹着中心的晶体，漂浮在深蓝色的底座上。");
        }

        public override bool CanShowInBook => true;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            //Vector2 pos = PageTop + new Vector2(0, 60);
            //Utils.DrawBorderStringBig(spriteBatch, TitleName.Value, pos, Coralite.MagicCrystalPink
            //    , 0.8f, 0.5f, 0.5f);

            //pos += new Vector2(0, 60);

            ////文字段1
            //Helpers.Helper.DrawText(spriteBatch, BiomeLens1.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
            //    , new Color(40, 40, 40), Color.White, out Vector2 textSize);

            //pos += new Vector2(0, textSize.Y + 20);

            ////森林透镜
            //Helpers.Helper.DrawText(spriteBatch, ForestLens.Value, PageWidth - 100, new Vector2(Position.X + 100, pos.Y), Vector2.Zero, Vector2.One
            //    , new Color(40, 40, 40), Color.White, out textSize);

            //Texture2D mainTex = TextureAssets.Item[ModContent.ItemType<ForestLens>()].Value;
            //spriteBatch.Draw(mainTex, new Vector2(Position.X + 50, pos.Y + textSize.Y / 2), null, Color.White, 0, mainTex.Size() / 2, 1, 0, 0);

            //pos += new Vector2(0, textSize.Y + 30);

            ////大理石透镜
            //Helpers.Helper.DrawText(spriteBatch, MarbleLens.Value, PageWidth - 100, new Vector2(Position.X + 100, pos.Y), Vector2.Zero, Vector2.One
            //    , new Color(40, 40, 40), Color.White, out textSize);

            //mainTex = TextureAssets.Item[ModContent.ItemType<MarbleLens>()].Value;
            //spriteBatch.Draw(mainTex, new Vector2(Position.X + 50, pos.Y + textSize.Y / 2), null, Color.White, 0, mainTex.Size() / 2, 1, 0, 0);

            //pos += new Vector2(0, textSize.Y + 30);

            ////花岗岩透镜
            //Helpers.Helper.DrawText(spriteBatch, GraniteLens.Value, PageWidth - 100, new Vector2(Position.X + 100, pos.Y), Vector2.Zero, Vector2.One
            //    , new Color(40, 40, 40), Color.White, out textSize);

            //mainTex = TextureAssets.Item[ModContent.ItemType<GraniteLens>()].Value;
            //spriteBatch.Draw(mainTex, new Vector2(Position.X + 50, pos.Y + textSize.Y / 2), null, Color.White, 0, mainTex.Size() / 2, 1, 0, 0);
        }
    }
}
