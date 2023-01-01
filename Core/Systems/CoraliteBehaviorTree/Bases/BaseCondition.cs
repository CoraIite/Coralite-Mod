using Coralite.Systems.CoraliteBehaviorTree.Enums;
using Coralite.Systems.CoraliteBehaviorTree.Interfaces;

namespace Coralite.Systems.CoraliteBehaviorTree.Bases
{
    public abstract class BaseCondition : BaseBehavior, ICondition
    {
        protected bool negation = false;

        public bool IsNegation()
        {
            return negation;
        }

        public void SetNegation(bool negation)
        {
            this.negation = negation;
        }

    }
}
