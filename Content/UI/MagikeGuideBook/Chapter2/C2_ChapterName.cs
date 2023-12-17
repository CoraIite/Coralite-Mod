using Coralite.Content.UI.UILib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter2
{
    public class C2_ChapterName : UIPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        public static LocalizedText ChapterName { get; private set; }

        public C2_ChapterName()
        {
            //Append(new C1_History_Jump());
        }

        public override void Recalculate()
        {
            float widdth = PageWidth;

            for (int i = 0; i < Elements.Count; i++)
            {
                var element = Elements[i];
                element.Width.Set(widdth, 0);
                element.Height.Set(50, 0);
                element.Top.Set(120 + i * 50, 0);
                element.Left.Set(10, 0);
            }

            base.Recalculate();
        }

        public override void OnInitialize()
        {
            ChapterName = this.GetLocalization("ChapterName", () => "第二章 探索发现");
        }

        public override bool CanShowInBook => true;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Utils.DrawBorderStringBig(spriteBatch, ChapterName.Value, Center + new Vector2(0, -PageWidth / 2), Coralite.Instance.MagicCrystalPink, 1, 0.5f, 0.5f);
        }
    }
}