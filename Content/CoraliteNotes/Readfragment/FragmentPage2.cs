using Coralite.Content.UI;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.Readfragment
{
    [AutoLoadTexture(Path = AssetDirectory.NoteReadfragment)]
    public class FragmentPage2 : KnowledgePage
    {
        public static LocalizedText MagikeGeneration { get; set; }
        //public static LocalizedText WonderKnowledge { get; set; }

        public static ATex MagikeGenerationTex { get; set; }
        //public static ATex WonderKnowledgeTex { get; set; }

        public FixedUIGrid SlotGrid1;
        //public FixedUIGrid SlotGrid2;

        public override void OnInitialize()
        {
            MagikeGeneration = this.GetLocalization(nameof(MagikeGeneration));
            //WonderKnowledge = this.GetLocalization(nameof(WonderKnowledge));
            SlotGrid1 = new FixedUIGrid();
            //SlotGrid2 = new FixedUIGrid();
        }

        public override void Recalculate()
        {
            RemoveAllChildren();

            int height = MagikeGenerationTex.Height();// Math.Max(MagikeGenerationTex.Height(), WonderKnowledgeTex.Height());
            height += 20;

            float halfPageHeight = PageHeight / 2;

            var t = new TitleElement(MagikeGenerationTex, MagikeGeneration, height, new Vector2(), Color.LightCoral);
            Append(t);

            AddMagikeGenerationButton();
            SlotGrid1.SetSize(new Vector2(0, halfPageHeight - height), 1, 0);
            SlotGrid1.SetTopLeft(height, 0);
            Append(SlotGrid1);

            //t = new TitleElement(WonderKnowledgeTex, WonderKnowledge, height, new Vector2(), Color.LightCoral);
            //t.SetTopLeft(halfPageHeight, 0);
            //Append(t);

            //AddWonderKnowledgeButton();
            //SlotGrid2.SetSize(new Vector2(0, halfPageHeight - height), 1, 0);
            //SlotGrid2.SetTopLeft(halfPageHeight + height, 0);
            //Append(SlotGrid2);

            base.Recalculate();
        }

        public void AddMagikeGenerationButton()
        {
            SlotGrid1.Clear();

            SlotGrid1.Add(new KnowledgeButten<MagikeChapter1.MagikeS1Knowledge>(KnowledgeButtonType.Reel));
            SlotGrid1.Add(new KnowledgeButten<MagikeInterstitial1.MagikeInterstitial1Knowledge>(KnowledgeButtonType.Rune));
            SlotGrid1.Add(new KnowledgeButten<MagikeChapter2.MagikeS2Knowledge>(KnowledgeButtonType.Reel));
        }

        //public void AddWonderKnowledgeButton()
        //{
        //    SlotGrid2.Clear();

        //    SlotGrid2.Add(new KnowledgeButten<SlimeChapter1.Slime1Knowledge>(KnowledgeButtonType.Ball));
        //}

        public static void AddFragments(FixedUIGrid grid)
        {
            //遍历知识，把所有的知识都加入进去
            List<KeyKnowledge> knowledges = [];

            foreach (var item in KeyKnowledgeLoader.knowledgesF)
            {
                knowledges.Add(item.Value);
            }

            knowledges.Sort((k1, k2) => k1.Type.CompareTo(k2.Type));

            foreach (var item in knowledges)
            {
                var slot = new FragmentSlot(item.InnerType);
                grid.Add(slot);
            }
        }
    }
}
