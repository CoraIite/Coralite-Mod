using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Content.Items.LandOfTheLustrousSeries.Accessories;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.LandOfTheLustrousChapter
{
    internal class LandOfTheLustrousPage1 : ItemShowPage
    {
        public static LocalizedText Gems { get; private set; }

        public override void OnInitialize()
        {
            Gems = this.GetLocalization(nameof(Gems));
            AddImages();
        }

        public override void AddImages()
        {
            int x = -120;
            int y = 40;

            //初级宝石原石
            ItemShowImage i0 = NewImage<PrimaryRoughGemstone>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Ball)
                .SetColor(Color.LightGray);

            const int length = 120;
            const float perRot = MathHelper.Pi / 4;
            float rot = -MathHelper.PiOver2;
            ItemShowImage i0_1 = NewImage<Pyrope>(new Vector2(x, y) + rot.ToRotationVector2() * length, Readfragment.KnowledgeButtonType.None)
                .SetColor(PyropeProj.brightC)
                .SetReverse();
            rot -= perRot;

            ItemShowImage i0_2 = NewImage(ItemID.Amethyst, new Vector2(x, y) + rot.ToRotationVector2() * length, Readfragment.KnowledgeButtonType.None)
                .SetColor(AmethystLaser.brightC)
                .SetReverse();
            rot -= perRot;

            ItemShowImage i0_3 = NewImage<Aquamarine>(new Vector2(x, y) + rot.ToRotationVector2() * length, Readfragment.KnowledgeButtonType.None)
                .SetColor(AquamarineProj.brightC)
                .SetReverse();
            rot -= perRot;

            ItemShowImage i0_4 = NewImage<PinkDiamond>(new Vector2(x, y) + rot.ToRotationVector2() * length, Readfragment.KnowledgeButtonType.None)
                .SetColor(PinkDiamondProj.brightC)
                .SetReverse();
            rot -= perRot;

            ItemShowImage i0_5 = NewImage(ItemID.Diamond, new Vector2(x, y) + rot.ToRotationVector2() * length, Readfragment.KnowledgeButtonType.None)
                .SetColor(DiamondProj.brightC)
                .SetReverse();

            i0_1.AddChainedElement(i0);
            i0_2.AddChainedElement(i0);
            i0_3.AddChainedElement(i0);
            i0_4.AddChainedElement(i0);
            i0_5.AddChainedElement(i0);

            x = -20;
            y += 20;
            NewImage<Phosphophyllite>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Coral, CoraliteConditions.DownedNightmarePlantera)
                .SetColor(Color.Green);

            x += 100;
            y += 20;

            ItemShowImage i1 = NewImage<SeniorRoughGemstone>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Ball, CoraliteConditions.UnlockCrystallineSkyIsland)
                .SetColor(Color.LightGray);

            rot = -MathHelper.PiOver2;
            const float perRot2 = MathHelper.Pi / 7;
            const int length2 = 160;

            ItemShowImage i1_1 = NewImage<Zumurud>(new Vector2(x, y) + rot.ToRotationVector2() * length2, Readfragment.KnowledgeButtonType.None, CoraliteConditions.UnlockCrystallineSkyIsland)
                .SetColor(ZumurudProj.brightC)
                .SetReverse();
            rot += perRot2;

            ItemShowImage i1_2 = NewImage(ItemID.WhitePearl, new Vector2(x, y) + rot.ToRotationVector2() * length2, Readfragment.KnowledgeButtonType.None, CoraliteConditions.UnlockCrystallineSkyIsland)
                .SetColor(PearlProj.brightC)
                .SetReverse();
            rot += perRot2;

            ItemShowImage i1_3 = NewImage(ItemID.Ruby, new Vector2(x, y) + rot.ToRotationVector2() * length2, Readfragment.KnowledgeButtonType.None, CoraliteConditions.UnlockCrystallineSkyIsland)
                .SetColor(RubyProj.brightC)
                .SetReverse();
            rot += perRot2;

            ItemShowImage i1_4 = NewImage<Peridot>(new Vector2(x, y) + rot.ToRotationVector2() * length2, Readfragment.KnowledgeButtonType.None, CoraliteConditions.UnlockCrystallineSkyIsland)
                .SetColor(PeridotProj.brightC)
                .SetReverse();
            rot += perRot2;

            ItemShowImage i1_5 = NewImage(ItemID.Sapphire, new Vector2(x, y) + rot.ToRotationVector2() * length2, Readfragment.KnowledgeButtonType.None, CoraliteConditions.UnlockCrystallineSkyIsland)
                .SetColor(SapphireProj.brightC)
                .SetReverse();
            rot += perRot2;

            ItemShowImage i1_6 = NewImage<Tourmaline>(new Vector2(x, y) + rot.ToRotationVector2() * length2, Readfragment.KnowledgeButtonType.None, CoraliteConditions.UnlockCrystallineSkyIsland)
                .SetColor(TourmalineProj.brightC)
                .SetReverse();
            rot += perRot2;

            ItemShowImage i1_7 = NewImage(ItemID.Topaz, new Vector2(x, y) + rot.ToRotationVector2() * length2, Readfragment.KnowledgeButtonType.None, CoraliteConditions.UnlockCrystallineSkyIsland)
                .SetColor(TopazProj.brightC)
                .SetReverse();
            rot += perRot2;

            ItemShowImage i1_8 = NewImage<Zircon>(new Vector2(x, y) + rot.ToRotationVector2() * length2, Readfragment.KnowledgeButtonType.None, CoraliteConditions.UnlockCrystallineSkyIsland)
                .SetColor(ZirconProj.brightC)
                .SetReverse();

            rot += 0.4f;
            ItemShowImage i1_9 = NewImage<SilkAgate>(new Vector2(x, y) + rot.ToRotationVector2() * length2 * 1.2f, Readfragment.KnowledgeButtonType.Ball, CoraliteConditions.UnlockCrystallineSkyIsland)
                .SetColor(Color.Orange)
                .SetReverse();

            i1_1.AddChainedElement(i1);
            //i1_2.AddChainedElement(i1);//珍珠开不出来
            i1_3.AddChainedElement(i1);
            i1_4.AddChainedElement(i1);
            i1_5.AddChainedElement(i1);
            i1_6.AddChainedElement(i1);
            i1_7.AddChainedElement(i1);
            i1_8.AddChainedElement(i1);
            i1_9.AddChainedElement(i1);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawParaNormal(spriteBatch, Gems, Position.Y + 40, out _);
        }
    }
}
