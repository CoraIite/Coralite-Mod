namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public interface IFairyAccessory
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
}
