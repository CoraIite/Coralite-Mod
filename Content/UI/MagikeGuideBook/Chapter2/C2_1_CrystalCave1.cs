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
        public static LocalizedText CrystalCave2 { get; private set; }
        public static LocalizedText CrystalCave3 { get; private set; }

        public override void OnInitialize()
        {
            Date = this.GetLocalization("Date", () => "【泰拉历235年11月15日】");
            CrystalCave1 = this.GetLocalization("CrystalCave1", () => "    索这个泰拉大陆是一件令人兴奋的事情，能找到蚁狮们的巢穴，丛林中的蜥蜴人神庙，或者是充满骷髅的地牢，又或者是影子们的地城。这些有趣的地形都不是重点，重要的是，能够采集到“魔力晶体”的魔力水晶洞。");
            CrystalCave2 = this.GetLocalization("CrystalCave2", () => "    在深思熟虑后，我决定加入魔能探险队，根据珊瑚的启示，寻找这个特殊的地形。说到那个叫珊瑚的，前两天听说是突然出现在了国王大厅里，吓得卫兵们以为是刺客，全跑过来了。就在那时她拿出了个奇妙的橙色晶体，一下子释放出了惊人的能量把大厅天花板打出了个大洞。当时我正巧路过，听到了巨大的响声后偷偷向门缝内瞄了一眼，似乎是个粉色头发的人，头上还长者蓝色的角。");
            CrystalCave3 = this.GetLocalization("CrystalCave3", () => "    很快地解释清楚后，国王就发布了紧急通知，根据珊瑚的说法，组建探险队前往地底调查并寻找称为魔力水晶洞的地方。真不知道这个叫“魔能”的玩意到底靠不靠谱，万一是被骗了怎么办。但是有一线希望就得抓住，在现在这个混乱的情形下可没那么多可选项。");
        }

        public override bool CanShowInBook => MagikeSystem.MagikeCave_1;

        protected override void DrawOthers(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "Note1").Value;
            spriteBatch.Draw(mainTex, BottomRight + new Vector2(2, -58), null, new Color(50, 0, 28), 0, mainTex.Size(), 1, 0, 0);
            spriteBatch.Draw(mainTex, BottomRight+new Vector2(0,-60), null, Color.White, 0, mainTex.Size(), 1, 0, 0);

            Vector2 pos = Position + new Vector2(0, 60);
            Utils.DrawBorderStringBig(spriteBatch, Date.Value, pos, Coralite.Instance.MagicCrystalPink
                , 0.8f, 0f, 0f);

            pos += new Vector2(0, 60);

            //文字段1
            Helper.DrawText(spriteBatch, CrystalCave1.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out Vector2 textSize);

            pos += new Vector2(0, textSize.Y + 10);

            //文字段2
            Helper.DrawText(spriteBatch, CrystalCave2.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out textSize);

            pos += new Vector2(0, textSize.Y + 10);

            //文字段3
            Helper.DrawText(spriteBatch, CrystalCave3.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out _);
        }
    }
}
