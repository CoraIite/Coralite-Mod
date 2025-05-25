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
    public class FragmentPage2 : KnowledgePage
    {
        public static LocalizedText WonderKnowledge { get; set; }
        public static LocalizedText WonderKnowledgeDescription { get; set; }

        public static ATex WonderKnowledgeTex { get; set; }

        public FixedUIGrid SlotGrid;

        public override void OnInitialize()
        {
            WonderKnowledge = this.GetLocalization(nameof(WonderKnowledge));
            WonderKnowledgeDescription = this.GetLocalization(nameof(WonderKnowledgeDescription));
            SlotGrid = new FixedUIGrid();
            AddMagikeGenerationButton();
        }

        public override void Recalculate()
        {
            RemoveAllChildren();

            int height = WonderKnowledgeTex.Height();
            height += 20;

            var t = new TitleElement(WonderKnowledgeTex, WonderKnowledge, height, new Vector2(), Color.LightCoral);
            Append(t);

            Vector2 textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, WonderKnowledgeDescription.Value, Vector2.One, PageWidth);
            
            SlotGrid.SetSize(new Vector2(0, PageHeight - height - textSize.Y - 20), 1, 0);
            SlotGrid.SetTopLeft(height + textSize.Y + 20, 0);
            Append(SlotGrid);

            base.Recalculate();
        }

        public void AddMagikeGenerationButton()
        {
            SlotGrid.Clear();

            SlotGrid.Add(new KnowledgeButten<SlimeChapter1.Slime1Knowledge>(KnowledgeButtonType.Ball));
            SlotGrid.Add(new KnowledgeButten<NightmareChapter.NightmareKnowledge>(KnowledgeButtonType.Ball));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, WonderKnowledgeTex.Height() + 30);

            Utils.DrawBorderString(spriteBatch, WonderKnowledgeDescription.Value, pos, Color.White, anchorx: 0.5f, anchory: 0f);
        }
    }
}
