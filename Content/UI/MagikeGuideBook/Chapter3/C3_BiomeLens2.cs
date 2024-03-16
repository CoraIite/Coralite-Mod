using Coralite.Content.Items.Magike.BiomeLens;
using Coralite.Content.UI.UILib;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter3
{
    public class C3_BiomeLens2 : UIPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        public static LocalizedText HellLens { get; private set; }
        public static LocalizedText GlowingMushroomLens { get; private set; }
        public static LocalizedText BoneLens { get; private set; }

        public override void OnInitialize()
        {
            HellLens = this.GetLocalization("HellLens", () => "    地狱透镜需要被放置在地狱环境中。比岩浆更深的地方，有一个巨大的空间，那是恶魔们的住所。被雕刻成恶魔头颅形状的黑曜石包裹着中心的晶体，漂浮在狱岩石底座之上。");
            GlowingMushroomLens = this.GetLocalization("GlowingMushroomLens", () => "    夜光菇透镜需要被放置在夜光蘑菇环境中。地底深处亮起的蓝色光芒，这是地下为数不多的光源。据说一种稀有的饵料就生活在这个地方。夜光蘑菇的菌丝包裹着中心的水晶，漂浮在菌柄上。");
            BoneLens = this.GetLocalization("BoneLens", () => "    骸骨透镜需要呗放置在地牢环境中。深邃的回廊中传来哀嚎声，这是地牢的幽魂在讨论他们的午饭。扭曲的骨头上镶嵌着水晶，漂浮在地牢砖块之上。");
        }

        public override bool CanShowInBook => NPC.downedBoss2;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 60);

            //地狱透镜
            Helpers.Helper.DrawText(spriteBatch, HellLens.Value, PageWidth - 100, new Vector2(Position.X + 100, pos.Y), Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out Vector2 textSize);

            Texture2D mainTex = TextureAssets.Item[ModContent.ItemType<HellLens>()].Value;
            spriteBatch.Draw(mainTex, new Vector2(Position.X + 50, pos.Y + textSize.Y / 2), null, Color.White, 0, mainTex.Size() / 2, 1, 0, 0);

            pos += new Vector2(0, textSize.Y + 30);

            if (NPC.downedBoss3)
            {
                //夜光菇透镜
                Helpers.Helper.DrawText(spriteBatch, GlowingMushroomLens.Value, PageWidth - 100, new Vector2(Position.X + 100, pos.Y), Vector2.Zero, Vector2.One
                    , new Color(40, 40, 40), Color.White, out textSize);

                mainTex = TextureAssets.Item[ModContent.ItemType<GlowingMushroomLens>()].Value;
                spriteBatch.Draw(mainTex, new Vector2(Position.X + 50, pos.Y + textSize.Y / 2), null, Color.White, 0, mainTex.Size() / 2, 1, 0, 0);

                pos += new Vector2(0, textSize.Y + 30);

                //骸骨透镜
                Helpers.Helper.DrawText(spriteBatch, BoneLens.Value, PageWidth - 100, new Vector2(Position.X + 100, pos.Y), Vector2.Zero, Vector2.One
                    , new Color(40, 40, 40), Color.White, out textSize);

                mainTex = TextureAssets.Item[ModContent.ItemType<BoneLens>()].Value;
                spriteBatch.Draw(mainTex, new Vector2(Position.X + 50, pos.Y + textSize.Y / 2), null, Color.White, 0, mainTex.Size() / 2, 1, 0, 0);

            }

        }
    }
}
