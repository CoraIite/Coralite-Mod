using Coralite.Core;
using Coralite.Core.Systems.BossSystems;
using Coralite.Core.Systems.WorldValueSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.RedJade
{
    public class RedJadeItemPage : ItemShowPage
    {
        public static LocalizedText RedJadeWeapons { get; private set; }

        public override void OnInitialize()
        {
            RedJadeWeapons = this.GetLocalization(nameof(RedJadeWeapons));
            AddImages();
        }

        public override void AddImages()
        {
            int yBase = 40;
            ItemShowImage i0_0 = NewImage<Items.RedJades.RedJade>(new Vector2(0, yBase), Readfragment.KnowledgeButtonType.Wild, CoraliteConditions.DownedRediancie)
                .SetColor(Coralite.RedJadeRed);
            ItemShowImage i1 = NewImage<Items.RedJades.MagicCraftStation>(new Vector2(0, 220 + yBase), Readfragment.KnowledgeButtonType.Normal, CoraliteConditions.DownedRediancie)
                .SetColor(Coralite.RedJadeRed);

            ItemShowImage i0_1 = NewImage<Items.RedJades.RedJadeBlade>(new Vector2(-200, -120 + yBase), Readfragment.KnowledgeButtonType.Wild, CoraliteConditions.DownedRediancie)
                .SetColor(Coralite.RedJadeRed);
            ItemShowImage i0_2 = NewImage<Items.RedJades.RedJadeStaff>(new Vector2(-140, 10 + yBase), Readfragment.KnowledgeButtonType.Wild, CoraliteConditions.DownedRediancie)
                .SetColor(Coralite.RedJadeRed);
            ItemShowImage i0_3 = NewImage<Items.RedJades.RedJadeShield>(new Vector2(0, -140 + yBase), Readfragment.KnowledgeButtonType.Wild, CoraliteConditions.DownedRediancie)
                .SetColor(Coralite.RedJadeRed);
            ItemShowImage i0_4 = NewImage<Items.RedJades.RedJadeShrine>(new Vector2(120, 5 + yBase), Readfragment.KnowledgeButtonType.Wild, CoraliteConditions.DownedRediancie)
                .SetColor(Coralite.RedJadeRed);
            ItemShowImage i0_5 = NewImage<Items.RedJades.RedJadeSpear>(new Vector2(200, -120 + yBase), Readfragment.KnowledgeButtonType.Wild, CoraliteConditions.DownedRediancie)
                .SetColor(Coralite.RedJadeRed);
            ItemShowImage i0_6 = NewImage<Items.RedJades.RedJadeArrow>(new Vector2(-100, 120 + yBase), Readfragment.KnowledgeButtonType.Wild, CoraliteConditions.DownedRediancie)
                .SetColor(Coralite.RedJadeRed);
            ItemShowImage i0_7 = NewImage<Items.RedJades.RedJadeBullet>(new Vector2(100, 100 + yBase), Readfragment.KnowledgeButtonType.Wild, CoraliteConditions.DownedRediancie)
                .SetColor(Coralite.RedJadeRed);

            ItemShowImage i0_8 = NewImage<Items.RedJades.RedJadeTong>(new Vector2(-100, -180 + yBase), Readfragment.KnowledgeButtonType.Wild, CoraliteConditions.DownedRediancie)
                .SetColor(Coralite.RedJadeRed);

            ItemShowImage i0_9 = NewImage<Items.RedJades.SmallFirecracker>(new Vector2(100, -180 + yBase), Readfragment.KnowledgeButtonType.Wild, CoraliteConditions.DownedRediancie)
                .SetColor(Coralite.RedJadeRed);

            i0_0.AddChainedElement(i1);
            i0_0.AddChainedElement(i0_1);
            i0_0.AddChainedElement(i0_2);
            i0_0.AddChainedElement(i0_3);
            i0_0.AddChainedElement(i0_4);
            i0_0.AddChainedElement(i0_5);
            i0_0.AddChainedElement(i0_6);
            i0_0.AddChainedElement(i0_7);
            i0_0.AddChainedElement(i0_8);
            i0_0.AddChainedElement(i0_9);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (WorldValueSystem.Flag<DownedRediancie>())
                DrawParaNormal(spriteBatch, RedJadeWeapons, Position.Y + 40, out _);
            else
                DrawParaNormal(spriteBatch, CoraliteConditions.DownedRediancie.Description, Position.Y + 40, out _);
        }
    }
}
