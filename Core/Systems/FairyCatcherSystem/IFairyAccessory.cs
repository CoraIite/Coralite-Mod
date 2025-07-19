using Coralite.Core.Systems.FairyCatcherSystem.Bases;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public interface IFairyAccessory : IFairyJarAccessory, IFairyTongAccessory, IFairySpawnModifyer
    {
    }

    public interface IFairySpawnModifyer
    {
        /// <summary>
        /// 在生成仙灵的时候自定义仙灵的概率
        /// </summary>
        /// <param name="catcherProj"></param>
        /// <param name="attempt"></param>
        void ModifyFairySpawn(ref FairyAttempt attempt)
        {

        }
    }

    public interface IFairyJarAccessory
    {
        /// <summary>
        /// 罐子刚生成初始化各种值的时候使用
        /// </summary>
        /// <param name="proj"></param>
        void ModifyJarInit(BaseJarProj proj) { }
        /// <summary>
        /// 在罐子投掷出时调用
        /// </summary>
        /// <param name="proj"></param>
        void OnJarShoot(BaseJarProj proj) { }
    }

    public interface IFairyTongAccessory
    {
        /// <summary>
        /// 在抓手类武器开始攻击时调用
        /// </summary>
        /// <param name="proj"></param>
        void OnTongStartAttack(BaseTongsProj proj) { }
    }
}
