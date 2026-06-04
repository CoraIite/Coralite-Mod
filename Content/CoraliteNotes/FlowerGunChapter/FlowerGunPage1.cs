using Coralite.Content.Items.HyacinthSeries;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.FlowerGunChapter
{
    public class FlowerGunPage1 : ItemShowPage
    {
        public static LocalizedText Accessories { get; private set; }

        public override void OnInitialize()
        {
            Accessories = this.GetLocalization(nameof(Accessories));
            AddImages();
        }

        public override void AddImages()
        {
            int x = -120;
            int y = -180;

            ItemShowImage i0_1 = NewImage<PollenGunpowder>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Rune, CoraliteConditions.DownedRediancie)
                .SetColor(Colors.RarityBlue);

            x += 120;

            ItemShowImage i0_2 = NewImage<RoseGunpowder>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Rune, Condition.Hardmode)
                .SetColor(Colors.RarityRed);
            i0_1.AddChainedElement(i0_2);

            x += 120;

            ItemShowImage i0_3 = NewImage<MidasGunpowder>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Rune, Condition.DownedPumpking)
                .SetColor(Colors.RarityYellow);
            i0_2.AddChainedElement(i0_3);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            //DrawParaNormal(spriteBatch, Arrows, Position.Y + 40, out _);
            DrawParaNormal(spriteBatch, Accessories, Position.Y + 40, out _);
        }
    }

}
