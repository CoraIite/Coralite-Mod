namespace Coralite.Core.Systems.KeySystem
{
    /// <summary>
    /// 关键知识的物品，使用时解锁知识
    /// </summary>
    public abstract class KeyItem : ModItem
    {
        /// <summary>
        /// 解锁的知识的ID，详细请见<see cref="KeyKnowledgeID"/>
        /// </summary>
        public abstract int KnowledgeType { get; }

        /// <summary>
        /// 解锁知识
        /// </summary>
        public void UnlockKnowledge()
        {
            KeyKnowledge knowledge = CoraliteContent.GetKKnowledge(KnowledgeType);

            knowledge.UnlockKnowledge();
        }
    }
}
