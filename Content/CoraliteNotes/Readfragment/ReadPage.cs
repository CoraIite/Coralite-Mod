using Coralite.Core.Attributes;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.Readfragment
{
    /// <summary>
    /// 读取的页
    /// </summary>
    [AutoLoadTexture(Path = AssetDirectory.NoteReadfragment)]
    public class ReadPage : KnowledgePage
    {
        public static LocalizedText Description { get; private set; }
        public static LocalizedText HowToUse { get; private set; }

        public static ATex ReadPageTex { get; set; }

        public override void OnInitialize()
        {
            Description = this.GetLocalization(nameof(Description));
            HowToUse = this.GetLocalization(nameof(HowToUse));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 40);
            Helper.DrawText(spriteBatch, Description.Value, PageWidth, pos, Vector2.One / 2, Vector2.One
                , Coralite.TextShadowColor, Coralite.MagicCrystalPink, out Vector2 size);

            pos.Y += size.Y + 40;
            Helper.DrawText(spriteBatch, HowToUse.Value, PageWidth, pos, Vector2.One / 2, Vector2.One
                , Coralite.TextShadowColor, Main.DiscoColor, out _, true);

            spriteBatch.Draw(ReadPageTex.Value, Bottom+new Vector2(0,-50), null, Color.White, 0, new Vector2(ReadPageTex.Width() / 2, ReadPageTex.Height())
                , 1.1f, 0, 0);
        }
    }
}
