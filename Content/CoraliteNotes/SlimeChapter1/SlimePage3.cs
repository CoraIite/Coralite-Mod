using Coralite.Core;

namespace Coralite.Content.CoraliteNotes.SlimeChapter1
{
    public class SlimePage3 : ItemShowPage
    {
        public override void AddImages()
        {
            int yBase = 40;

            NewImage<Items.Gels.EmperorGel>(new Vector2(0, 0 + yBase), Readfragment.KnowledgeButtonType.Ball, CoraliteConditions.DownedSlimeEmperor)
                .SetColor(Color.SkyBlue);

            NewImage<Items.Gels.EmperorSabre>(new Vector2(0, -120 + yBase), Readfragment.KnowledgeButtonType.Ball, CoraliteConditions.DownedSlimeEmperor)
                .SetColor(Color.SkyBlue);
            NewImage<Items.Gels.GelWhip>(new Vector2(-120, -60 + yBase), Readfragment.KnowledgeButtonType.Ball, CoraliteConditions.DownedSlimeEmperor)
                .SetColor(Color.SkyBlue);
             NewImage<Items.Gels.RoyalClassics>(new Vector2(120, -60 + yBase), Readfragment.KnowledgeButtonType.Ball, CoraliteConditions.DownedSlimeEmperor)
                .SetColor(Color.SkyBlue);
             NewImage<Items.Gels.SlimeEruption>(new Vector2(-180, 20 + yBase), Readfragment.KnowledgeButtonType.Ball, CoraliteConditions.DownedSlimeEmperor)
                .SetColor(Color.SkyBlue);
            NewImage<Items.Gels.SlimeSceptre>(new Vector2(180, 20 + yBase), Readfragment.KnowledgeButtonType.Ball, CoraliteConditions.DownedSlimeEmperor)
                .SetColor(Color.SkyBlue);
        }
    }
}
