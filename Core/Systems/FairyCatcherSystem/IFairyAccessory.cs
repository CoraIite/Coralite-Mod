using Coralite.Core.Systems.FairyCatcherSystem.Bases;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public interface IFairyAccessory: IFairyJarAccessory
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
        void ModifyJarInit(BaseJarProj proj) { }
        /// <summary>
        /// 在罐子投掷出时调用
        /// </summary>
        /// <param name="proj"></param>
        void OnJarShoot(BaseJarProj proj) { }
    }
}
