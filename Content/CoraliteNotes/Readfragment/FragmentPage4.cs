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
    [AutoLoadTexture(Path = AssetDirectory.NoteReadfragment)]
    public class FragmentPage4 : KnowledgePage
    {
        public static LocalizedText CollectJourney { get; set; }
        public static LocalizedText CollectJourneyDescription { get; set; }

        public static ATex CollectJourneyTex { get; set; }

        public FixedUIGrid SlotGrid;

        public override void OnInitialize()
        {
            CollectJourney = this.GetLocalization(nameof(CollectJourney));
            CollectJourneyDescription = this.GetLocalization(nameof(CollectJourneyDescription));
            SlotGrid = new FixedUIGrid();
            AddMagikeGenerationButton();
        }

        public override void Recalculate()
        {
            RemoveAllChildren();

            int height = CollectJourneyTex.Height();
            height += 20;

            var t = new TitleElement(CollectJourneyTex, CollectJourney, height, new Vector2(), Color.LightCoral);
            Append(t);

            Vector2 textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, CollectJourneyDescription.Value, Vector2.One, PageWidth);

            SlotGrid.SetSize(new Vector2(0, PageHeight - height - textSize.Y - 20), 1, 0);
            SlotGrid.SetTopLeft(height + textSize.Y + 20, 0);
            Append(SlotGrid);

            base.Recalculate();
        }

        public void AddMagikeGenerationButton()
        {
            SlotGrid.Clear();

            SlotGrid.Add(new KnowledgeButten<FlyingShieldChapter.FlyingShieldKnowledge>(KnowledgeButtonType.Normal));
            SlotGrid.Add(new KnowledgeButten<FlowerGunChapter.FlowerGunKnowledge>(KnowledgeButtonType.Normal));
            SlotGrid.Add(new KnowledgeButten<LandOfTheLustrousChapter.LandOfTheLustrousKnowledge>(KnowledgeButtonType.Normal));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, CollectJourneyTex.Height() + 30);

            Utils.DrawBorderString(spriteBatch, CollectJourneyDescription.Value, pos, Color.White, anchorx: 0.5f, anchory: 0f);
        }
    }
}
