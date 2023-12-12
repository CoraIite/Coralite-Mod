using Coralite.Content.UI.UILib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter1
{
    public class C1_1_2_Property : UIPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        //第一章 1.1.2魔能的性质
        public static LocalizedText _1_2_Name { get; private set; }
        public static LocalizedText Property1 { get; private set; }
        public static LocalizedText Property2 { get; private set; }

        public override void OnInitialize()
        {
            _1_2_Name = this.GetLocalization("_1_2_Name", () => "1.1.2 魔能的性质");
            Property1 = this.GetLocalization("Property1", () => "    看到你生命值下方的那个蓝色五角星或其他样式的蓝色条条了吗，那个是你拥有的魔力值，当然，这和魔能没什么关系。");
            Property2 = this.GetLocalization("Property2", () => "    魔能是一种能量，可以粗略地看作压缩过的魔力，通过一些装置或物品中的特殊机构，可以消耗魔能来驱动这些物件。");
        }

        public override bool CanShowInBook => true;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = Position + new Vector2(0, 100);
            Utils.DrawBorderString(spriteBatch, _1_2_Name.Value, pos, Coralite.Instance.MagicCrystalPink, 1, 0f, 00f);


        }
    }
}
