using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class PartJumpPage:KnowledgePage
    {
        public static LocalizedText[] ParaTitles { get; private set; }

        public override void OnInitialize()
        {
            ParaTitles = [
                this.GetLocalization("P1Title"),
                this.GetLocalization("P2Title"),
                this.GetLocalization("P3Title"),
                this.GetLocalization("P4Title"),
                this.GetLocalization("P5Title"),
                this.GetLocalization("P6Title"),
                this.GetLocalization("P7Title"),
                this.GetLocalization("P8Title"),
                ];
        }

        public override void Recalculate()
        {
            RemoveAllChildren();

            Append(new Chapter1Jump(ParaTitles[0], () => CoraliteNoteUIState.BookPanel.GetPageIndex<WhatIsMagikePage>()));
            Append(new Chapter1Jump(ParaTitles[1], () => CoraliteNoteUIState.BookPanel.GetPageIndex<PlaceFirstLens>()));
            Append(new Chapter1Jump(ParaTitles[2], () => CoraliteNoteUIState.BookPanel.GetPageIndex<StoneMaker>()));
            Append(new Chapter1Jump(ParaTitles[3], () => CoraliteNoteUIState.BookPanel.GetPageIndex<WhatIsMagikePage>()));
            Append(new Chapter1Jump(ParaTitles[4], () => CoraliteNoteUIState.BookPanel.GetPageIndex<WhatIsMagikePage>()));
            Append(new Chapter1Jump(ParaTitles[5], () => CoraliteNoteUIState.BookPanel.GetPageIndex<WhatIsMagikePage>()));
            Append(new Chapter1Jump(ParaTitles[6], () => CoraliteNoteUIState.BookPanel.GetPageIndex<WhatIsMagikePage>()));
            Append(new Chapter1Jump(ParaTitles[7], () => CoraliteNoteUIState.BookPanel.GetPageIndex<WhatIsMagikePage>()));

            float width = PageWidth;

            for (int i = 0; i < Elements.Count; i++)
            {
                var element = Elements[i];
                element.Width.Set(width, 0);
                element.Height.Set(60, 0);
                element.Top.Set(80 + (i * 60), 0);
                element.Left.Set(5, 0);
            }

            base.Recalculate();
        }
    }
}
