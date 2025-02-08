using Coralite.Core.Loaders;
using Coralite.Helpers;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class PartJumpPage : KnowledgePage
    {
        public static LocalizedText[] ParaTitles { get; private set; }

        public UIList buttonList;

        public override void OnInitialize()
        {
            buttonList = new UIList();

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

            buttonList.Add(new Chapter1Jump(ParaTitles[0], () => CoraliteNoteUIState.BookPanel.GetPageIndex<WhatIsMagikePage>()));
            buttonList.Add(new Chapter1Jump(ParaTitles[1], () => CoraliteNoteUIState.BookPanel.GetPageIndex<PlaceFirstLens>()));
            buttonList.Add(new Chapter1Jump(ParaTitles[2], () => CoraliteNoteUIState.BookPanel.GetPageIndex<StoneMaker>()));
            buttonList.Add(new Chapter1Jump(ParaTitles[3], () => CoraliteNoteUIState.BookPanel.GetPageIndex<CraftAltar>()));
            buttonList.Add(new Chapter1Jump(ParaTitles[4], () => CoraliteNoteUIState.BookPanel.GetPageIndex<ExpandProductionLine>()));
            buttonList.Add(new Chapter1Jump(ParaTitles[5], () => CoraliteNoteUIState.BookPanel.GetPageIndex<INeedMoreMagike>()));
            //buttonList.Add(new Chapter1Jump(ParaTitles[6], () => CoraliteNoteUIState.BookPanel.GetPageIndex<WhatIsMagikePage>()));
            //buttonList.Add(new Chapter1Jump(ParaTitles[7], () => CoraliteNoteUIState.BookPanel.GetPageIndex<WhatIsMagikePage>()));
        }

        public override void Recalculate()
        {
            RemoveAllChildren();

            buttonList.SetSize(-20, -40, 1, 1);
            buttonList.SetTopLeft(0, 10);
            float width = PageWidth;
            FixedUIScrollbar scrollbar = new FixedUIScrollbar(UILoader.GetUserInterface<CoraliteNoteUIState>());
            scrollbar.SetTopLeft(20, 0);
            scrollbar.SetSize(20, PageHeight - 80);
            buttonList.SetScrollbar(scrollbar);

            Append(buttonList);
            Append(scrollbar);

            foreach (var element in buttonList)
            {
                element.Width.Set(width, 0);
                element.Height.Set(45, 0);
            }

            base.Recalculate();
        }
    }
}
