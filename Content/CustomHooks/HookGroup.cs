using Coralite.Core;

namespace Coralite.Content.CustomHooks
{
    public class HookGroup: IOrderedLoadable
    {
        public virtual SafetyLevel Safety => SafetyLevel.Severe;

        public virtual float Priority { get => 1f; }

        public virtual void Load() { }

        public virtual void Unload() { }
    }

    /// <summary>
    /// 规则:
    /// 1. IL 非常危险，至少得是Questionable级别以上的
    /// 2. 当有争议的时候选更危险的那个 (more dangerous)
    /// 3. 如果可以的话, 用简短的评价来解释一下这个钩子的危险等级
    /// </summary>
    public enum SafetyLevel
    {
        Safe = 0,
        Questionable = 1,
        Fragile = 2,
        Severe = 3
    }
}
