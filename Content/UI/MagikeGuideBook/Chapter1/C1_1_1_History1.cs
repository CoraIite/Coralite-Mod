using Coralite.Content.UI.UILib;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter1
{
    public class C1_1_1_History1 : UIPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        //第一章 1.1什么是魔能
        public static LocalizedText _1_Name { get; private set; }
        //第一章 1.1.1魔能的历史
        public static LocalizedText _1_1_Name { get; private set; }

        public static LocalizedText History1 { get; private set; }

        public override void OnInitialize()
        {
            _1_Name = this.GetLocalization("_1_Name", () => "1.1 什么是魔能");
            _1_1_Name = this.GetLocalization("_1_1_Name", () => "1.1.1 魔能的历史");
            History1 = this.GetLocalization("History1", () => "    在很久很久之前，泰拉大陆的科技水平并不发达，并且由于各个种族之间的战争，导致世界一片混沌。就是在这样的时代，我来到了这里。在这些种族间，有我曾经的同族：人。它们相对于龙族，妖精族而言十分弱小，但是在它们身上，我看到了曾经的自己。于是我决定帮助他们，带领他们发掘蕴含着魔能的水晶矿物。");
        }

        public override bool CanShowInBook => true;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "C1_1_1_History1").Value;
            spriteBatch.Draw(mainTex, Bottom + new Vector2(0, -60), null, Color.White, 0, new Vector2(mainTex.Width / 2, mainTex.Height), PageWidth / mainTex.Width, 0, 0);

            Vector2 pos = PageTop + new Vector2(0, 60);
            Utils.DrawBorderStringBig(spriteBatch, _1_Name.Value, pos, Coralite.Instance.MagicCrystalPink
                , 0.8f, 0.5f, 0.5f);
            pos = Position + new Vector2(0, 100);
            Utils.DrawBorderString(spriteBatch, _1_1_Name.Value, pos, Coralite.Instance.MagicCrystalPink
                , 1, 0f, 00f);
            pos += new Vector2(0, 60);
            Helpers.Helper.DrawText(spriteBatch, History1.Value, PageWidth, pos, Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out _);
        }
    }
}
