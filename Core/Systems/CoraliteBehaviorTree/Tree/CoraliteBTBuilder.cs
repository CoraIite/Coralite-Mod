using Coralite.Systems.CoraliteBehaviorTree.Interfaces;
using System.Collections.Generic;

namespace Coralite.Systems.CoraliteBehaviorTree.Tree
{
    public class CoraliteBTBuilder
    {
        private Stack<IBehaviour> behaviours = new Stack<IBehaviour>();
        private IBehaviour treeRoot = null;

        public CoraliteBTBuilder AddBehaviour(IBehaviour behaviour)
        {
            //如果没有根节点设置新节点为根节点
            //否则设置新节点为堆栈顶部节点的子节点
            if (treeRoot == null)
                treeRoot = behaviour;
            else
                behaviours.Peek().AddChild(behaviour);

            //将新节点压入堆栈
            behaviours.Push(behaviour);
            return this;
        }

        public CoraliteBTBuilder Back()
        {
            behaviours.Pop();
            return this;
        }

        public CoraliteBT End()
        {
            while (!(behaviours.Count == 0))
                behaviours.Pop();

            CoraliteBT tmp = new CoraliteBT(treeRoot);
            return tmp;
        }
    }
}
