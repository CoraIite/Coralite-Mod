using System.Collections.Generic;

namespace Coralite.Core.Systems.FairyCatcherSystem.FairyFreePart
{
    public struct FairyFreeInfo
    {
        public List<IFairyFreeRule> Rules;

        public FairyFreeInfo AddRule(IFairyFreeRule rule)
        {
            Rules ??= new List<IFairyFreeRule>();
            Rules.Add(rule);
            return this;
        }
    }
}
