using Coralite.Systems.CoraliteBehaviorTree.Interfaces;

namespace Coralite.Systems.CoraliteBehaviorTree.Tree
{
    public class CoraliteBT
    {
        private IBehaviour root;

        public CoraliteBT(IBehaviour root)
        {
            this.root = root;
        }

        public void Tick()
        {
            root.Tick();
        }

        public bool HaveRoot()
        {
            return root != null;
        }

        public void SetRoot(IBehaviour inNode)
        {
            root = inNode;
        }

        public void Release()
        {
            root.Release();
        }
    }
}
