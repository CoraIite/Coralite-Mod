using System.Collections.Generic;

namespace Coralite.Systems.CoraliteBehaviorTree.Interfaces
{
    /// <summary>
    /// 组合节点
    /// </summary>
    public interface IComposite : IBehaviour
    {
        new void AddChild(IBehaviour child);

        void RemoveChild(IBehaviour child);

        void ClearChild();

        List<IBehaviour> GetChildren();

        void SetChildren(List<IBehaviour> behaviours);
    }
}
