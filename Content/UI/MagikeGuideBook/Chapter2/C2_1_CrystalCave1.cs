using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter2
{
    public class C2_1_CrystalCave1 : FragmentPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        public static LocalizedText Date { get; private set; }
        public static LocalizedText CrystalCave1 { get; private set; }

        public override void OnInitialize()
        {
            Date = this.GetLocalization("Date", () => "【泰拉历235年11月15日】");
            CrystalCave1 = this.GetLocalization("CrystalCave1", () => "    探索这个泰拉大陆是一件令人兴奋的事情，你能找到蚁狮们的巢穴，丛林中的蜥蜴人神庙，或者是充满骷髅的地牢，又或者是影子们的地城......哦，我似乎说的太多了，这些有趣的地形都不是重点，重要的是，能够采集到“魔力晶体”的魔力水晶洞。我决定加入探险队，根据珊瑚的启示，寻找这个特殊的地形。");
        }

        public override bool CanShowInBook => MagikeSystem.MagikeCave_1;

        protected override void DrawOthers(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "Note1").Value;
            spriteBatch.Draw(mainTex, BottomRight+new Vector2(0,-60), null, Color.White, 0, mainTex.Size(), 1, 0, 0);

            Vector2 pos = Position + new Vector2(0, 60);
            Utils.DrawBorderStringBig(spriteBatch, Date.Value, pos, Coralite.Instance.MagicCrystalPink
                , 0.8f, 0f, 0f);

            pos += new Vector2(0, 60);

            //文字段1
            Helper.DrawText(spriteBatch, CrystalCave1.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out _);
        }
    }
}
