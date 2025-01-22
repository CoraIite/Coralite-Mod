using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.Readfragment
{
    /// <summary>
    /// 读取的页
    /// </summary>
    public class ReadPage : KnowledgePage
    {
        public static LocalizedText Description { get; private set; }
        public static LocalizedText HowToUse { get; private set; }

        public override void OnInitialize()
        {
            Description = this.GetLocalization(nameof(Description));
            HowToUse = this.GetLocalization(nameof(HowToUse));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Helper.DrawText(spriteBatch, Description.Value, PageWidth, Center, Vector2.One / 2, Vector2.One
                , new Color(40, 40, 40), Coralite.MagicCrystalPink, out Vector2 size);

            Helper.DrawText(spriteBatch, HowToUse.Value, PageWidth, Center + new Vector2(0, size.Y + 80), Vector2.One / 2, Vector2.One
                , new Color(40, 40, 40), Main.DiscoColor, out _,true);
        }
    }
}
