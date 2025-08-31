using Coralite.Content.UI;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI.Chat;

namespace Coralite.Content.CoraliteNotes.Readfragment
{
    [VaultLoaden(AssetDirectory.NoteReadfragment)]
    public class FragmentPage3 : KnowledgePage
    {
        public static LocalizedText MagikeGeneration { get; set; }
        public static LocalizedText MagikeGenerationDescription { get; set; }

        public static ATex MagikeGenerationTex { get; set; }

        public FixedUIGrid SlotGrid;

        public override void OnInitialize()
        {
            MagikeGeneration = this.GetLocalization(nameof(MagikeGeneration));
            MagikeGenerationDescription = this.GetLocalization(nameof(MagikeGenerationDescription));
            SlotGrid = new FixedUIGrid();
            AddMagikeGenerationButton();
        }

        public override void Recalculate()
        {
            RemoveAllChildren();

            int height = MagikeGenerationTex.Height();
            height += 20;

            var t = new TitleElement(MagikeGenerationTex, MagikeGeneration, height, new Vector2(), Color.LightCoral);
            Append(t);

            Vector2 textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, MagikeGenerationDescription.Value, Vector2.One, PageWidth);

            SlotGrid.SetSize(new Vector2(0, PageHeight - height - textSize.Y - 20), 1, 0);
            SlotGrid.SetTopLeft(height + textSize.Y + 20, 0);
            Append(SlotGrid);

            base.Recalculate();
        }

        public void AddMagikeGenerationButton()
        {
            SlotGrid.Clear();

            SlotGrid.Add(new KnowledgeButten<MagikeChapter1.MagikeS1Knowledge>(KnowledgeButtonType.Reel));
            SlotGrid.Add(new KnowledgeButten<MagikeInterstitial1.MagikeInterstitial1Knowledge>(KnowledgeButtonType.Coral));
            SlotGrid.Add(new KnowledgeButten<MagikeChapter2.MagikeS2Knowledge>(KnowledgeButtonType.Reel));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, MagikeGenerationTex.Height() + 30);

            Utils.DrawBorderString(spriteBatch, MagikeGenerationDescription.Value, pos, Color.White, anchorx: 0.5f, anchory: 0f);
        }
    }
}
