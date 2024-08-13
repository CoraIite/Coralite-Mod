using Coralite.Systems.CoraliteBehaviorTree.Interfaces;
using System.Collections.Generic;

namespace Coralite.Systems.CoraliteBehaviorTree.Bases
{
    public abstract class BaseComposite : BaseBehavior, IComposite
    {
        protected List<IBehaviour> children = new();

        public override void AddChild(IBehaviour child)
        {
            children.Add(child);
        }

        public void ClearChild()
        {
            children.Clear();
        }

        public List<IBehaviour> GetChildren()
        {
            return children;
        }

        public void RemoveChild(IBehaviour child)
        {
            children.Remove(child);
        }

        public void SetChildren(List<IBehaviour> behaviours)
        {
            this.children = behaviours;
        }
    }
}
