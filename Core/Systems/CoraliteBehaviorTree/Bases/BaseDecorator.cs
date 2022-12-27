using Coralite.Systems.CoraliteBehaviorTree.Enums;
using Coralite.Systems.CoraliteBehaviorTree.Interfaces;

namespace Coralite.Systems.CoraliteBehaviorTree.Bases
{
    public abstract class BaseDecorator : BaseBehavior, IDecorator
    {
        protected IBehaviour child;

        public override void AddChild(IBehaviour child)
        {
            this.child = child;
        }

        public abstract override EStatus Update();
    }
}
