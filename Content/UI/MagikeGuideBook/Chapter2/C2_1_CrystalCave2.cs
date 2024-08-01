using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter2
{
    public class C2_1_CrystalCave2 : FragmentPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        public static LocalizedText Date { get; private set; }
        public static LocalizedText CrystalCave1 { get; private set; }

        public override void OnInitialize()
        {
            Date = this.GetLocalization("Date", () => "【泰拉历235年11月16日】");
            CrystalCave1 = this.GetLocalization("CrystalCave1", () => "    今天从王国北方的山洞向地底进发，不知到是不是因为那个的影响，怪物出现的频率变高了。洞窟里的各种史莱姆和骷髅已经杀掉2名队友了，我得时刻保持警惕。队伍内的气氛也很紧张，这样下去搞不好任务要失败了。");
        }

        public override bool CanShowInBook => MagikeSystem.MagikeCave_2;

        protected override void DrawOthers(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "Note2").Value;
            spriteBatch.Draw(mainTex, BottomRight + new Vector2(2, -58), null, new Color(50, 0, 28), 0, mainTex.Size(), 1, 0, 0);
            spriteBatch.Draw(mainTex, BottomRight + new Vector2(0, -60), null, Color.White, 0, mainTex.Size(), 1, 0, 0);

            Vector2 pos = Position + new Vector2(0, 60);
            Utils.DrawBorderStringBig(spriteBatch, Date.Value, pos, Coralite.MagicCrystalPink
                , 0.8f, 0f, 0f);

            pos += new Vector2(0, 60);

            //文字段1
            Helper.DrawText(spriteBatch, CrystalCave1.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out Vector2 textSize);

            pos += new Vector2(0, textSize.Y + 10);
        }
    }
}
