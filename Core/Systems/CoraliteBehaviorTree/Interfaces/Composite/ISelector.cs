namespace Coralite.Systems.CoraliteBehaviorTree.Interfaces.Composite
{
    /// <summary>
    /// 选择节点
    /// <para>依次执行每个子节点直到其中一个执行成功或返回Running状态</para>
    /// </summary>
    public interface ISelector : IComposite
    {

    }
}
