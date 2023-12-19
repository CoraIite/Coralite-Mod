using Coralite.Content.Items.Magike.BasicLens;
using Coralite.Content.UI.UILib;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter3
{
    public class C3_BasicInstrument : UIPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        public static LocalizedText TitleName { get; private set; }
        public static LocalizedText BasicInstrument1 { get; private set; }
        public static LocalizedText BasicInstrument2 { get; private set; }

        public override void OnInitialize()
        {
            TitleName = this.GetLocalization("TitleName", () => "基本仪器");
            BasicInstrument1 = this.GetLocalization("Origin1", () => "    这些最为基础的透镜需要消耗蕴含魔能的物品来生产魔能，或者可以简单理解为将物品转化为了魔能。蕴含魔能的物品会在物品提示中写明这个物品包含多少的魔能。");
            BasicInstrument2 = this.GetLocalization("Origin2", () => "    右键以打开它的面板，可以将物品放入其中，稍等一会后它就会自动的开始生产魔能并存储在自身中。");
        }

        public override bool CanShowInBook => true;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 60);
            Utils.DrawBorderStringBig(spriteBatch, TitleName.Value, pos, Coralite.Instance.MagicCrystalPink
                , 0.8f, 0.5f, 0.5f);

            pos += new Vector2(0, 60);

            //文字段1
            Helpers.Helper.DrawText(spriteBatch, BasicInstrument1.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out Vector2 textSize);

            pos += new Vector2(0, textSize.Y + 20);

            //插图1
            Vector2 itemPos = pos + new Vector2(20, 0);
            Texture2D mainTex1 = TextureAssets.Item[ModContent.ItemType<CrystalLens>()].Value;
            spriteBatch.Draw(mainTex1, itemPos, null, Color.White, 0, new Vector2(mainTex1.Width / 2, 0), 1, 0, 0);
            itemPos += new Vector2(80, 0);

            if (WorldGen.crimson)
                mainTex1 = TextureAssets.Item[ModContent.ItemType<CrimtaneLens>()].Value;
            else
                mainTex1 = TextureAssets.Item[ModContent.ItemType<DemoniteLens>()].Value;

            spriteBatch.Draw(mainTex1, itemPos, null, Color.White, 0, new Vector2(mainTex1.Width / 2, 0), 1, 0, 0);


            if (MagikeSystem.learnedMagikeAdvanced)
            {
                itemPos += new Vector2(80, 0);
                mainTex1 = TextureAssets.Item[ModContent.ItemType<BrilliantLens>()].Value;
                spriteBatch.Draw(mainTex1, itemPos, null, Color.White, 0, new Vector2(mainTex1.Width / 2, 0), 1, 0, 0);
                itemPos += new Vector2(80, 0);
                mainTex1 = TextureAssets.Item[ModContent.ItemType<FeatheredLens>()].Value;
                spriteBatch.Draw(mainTex1, itemPos, null, Color.White, 0, new Vector2(mainTex1.Width / 2, 0), 1, 0, 0);

                if (NPC.downedMoonlord)
                {
                    itemPos += new Vector2(80, 0);
                    mainTex1 = TextureAssets.Item[ModContent.ItemType<SplendorLens>()].Value;
                    spriteBatch.Draw(mainTex1, itemPos, null, Color.White, 0, new Vector2(mainTex1.Width / 2, 0), 1, 0, 0);
                }
            }

            pos += new Vector2(0, mainTex1.Height + 20);

            //文字段2
            Helpers.Helper.DrawText(spriteBatch, BasicInstrument2.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out textSize);

            pos += new Vector2(0, textSize.Y + 30);

            Texture2D mainTex2 = ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "C1_2_Origin2").Value;
            spriteBatch.Draw(mainTex2, pos, null, Color.White, 0, new Vector2(mainTex2.Width / 2, 0), 1, 0, 0);
        }
    }
}
