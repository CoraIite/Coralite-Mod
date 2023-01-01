namespace Coralite.Systems.CoraliteBehaviorTree.Interfaces
{
    /// <summary>
    /// 条件基接口
    /// </summary>
    public interface ICondition:IBehaviour
    {
        bool IsNegation();

        void SetNegation(bool negation);
    }
}
