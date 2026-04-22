using Coralite.Content.Items.Icicle;
using Coralite.Core;
using Coralite.Core.Systems.BossSystems;
using Coralite.Core.Systems.WorldValueSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.IceDragonChapter1
{
    public class IciclePage1 : ItemShowPage
    {
        public static LocalizedText IcicleWeapons { get; private set; }

        public override void OnInitialize()
        {
            IcicleWeapons = this.GetLocalization(nameof(IcicleWeapons));
            AddImages();
        }

        public override void AddImages()
        {
            float yBase = -120;
            NewIcicleImage<IcicleScale>(new Vector2(-80, yBase));
            NewIcicleImage<IcicleCrystal>(new Vector2(0, yBase - 10));
            NewIcicleImage<IcicleBreath>(new Vector2(80, yBase));

            yBase += 80;
            NewMark(new Vector2(0, yBase), ItemShowMark.MarkType.Arrow, Coralite.IcicleCyan, MathHelper.PiOver2);

            float x = -180;
            float y = 60;

            NewMark(new Vector2(x, y - 60), ItemShowMark.MarkType.NodeBig, Coralite.IcicleCyan);
            NewIcicleImage<IcicleHelmet>(new Vector2(x, y));
            y += 80;
            NewIcicleImage<IcicleBreastplate>(new Vector2(x, y));
            y += 80;
            NewIcicleImage<IcicleLegs>(new Vector2(x, y));

            y = 50;
            x += 90;
            NewMark(new Vector2(x, y - 60), ItemShowMark.MarkType.NodeSmall, Coralite.IcicleCyan);
            NewIcicleImage<IcicleAxe>(new Vector2(x, y));
            y += 80;
            NewIcicleImage<IceAxe>(new Vector2(x, y));
            y += 80;
            NewIcicleImage<IcicleHammer>(new Vector2(x, y));

            y = 70;
            x += 90;
            NewIcicleImage<IcicleBow>(new Vector2(x, y));
            y += 80;
            NewIcicleImage<IcicleSilkKnief>(new Vector2(x, y));
            y += 80;
            NewIcicleImage<IcicleSword>(new Vector2(x, y));

            y = 60;
            x += 90;
            NewMark(new Vector2(x, y - 60), ItemShowMark.MarkType.NodeBig, Coralite.IcicleCyan);
            NewIcicleImage<BabyIceWing>(new Vector2(x, y));
            y += 80;
            NewIcicleImage<IcicleArrowItem>(new Vector2(x, y));
            y += 80;
            NewIcicleImage<IcicleThornStaff>(new Vector2(x, y));
            y += 80;

            y = 50;
            x += 90;
            NewIcicleImage<IcicleStaff>(new Vector2(x, y));
            //y += 80;
            //NewIcicleImage<IceAxe>(new Vector2(x, y));
            //y += 80;
            //NewIcicleImage<IcicleHammer>(new Vector2(x, y));
        }

        public void NewIcicleImage<T>(Vector2 pos) where T : ModItem
        {
            NewImage<T>(pos, Readfragment.KnowledgeButtonType.Wild, CoraliteConditions.DownedBabyIceDragon)
                .SetColor(Coralite.IcicleCyan);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (WorldValueSystem.Flag<DownedBabyIceDragon>())
                DrawParaNormal(spriteBatch, IcicleWeapons, Position.Y + 40, out _);
            else
                DrawParaNormal(spriteBatch, CoraliteConditions.DownedBabyIceDragon.Description, Position.Y + 40, out _);
        }

    }
}
