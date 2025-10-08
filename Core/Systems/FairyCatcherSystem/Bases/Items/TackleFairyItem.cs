using Coralite.Core.Systems.FairyCatcherSystem.NormalSkills;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases.Items
{
    public abstract class TackleFairyItem : BaseFairyItem
    {
        public override int[] GetFairySkills()
        {
            return [CoraliteContent.FairySkillType<FSkill_Tackle>()];
        }
    }
}
