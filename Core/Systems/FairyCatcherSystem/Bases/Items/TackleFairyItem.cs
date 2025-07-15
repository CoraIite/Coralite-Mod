using Coralite.Core.Systems.FairyCatcherSystem.NormalSkills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases.Items
{
    public abstract class TackleFairyItem:BaseFairyItem
    {
        public override int[] GetFairySkills()
        {
            return [CoraliteContent.FairySkillType<FSkill_Tackle>()];
        }
    }
}
