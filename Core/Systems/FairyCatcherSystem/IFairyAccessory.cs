using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Terraria.DataStructures;
using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public interface IFairyAccessory : IFairyJarAccessory, IFairyTongAccessory
        , IFairySpawnModifyer, IFairyGloveAccessory
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
        /// 在罐子投掷出时调用<br></br>
        /// <see cref="BaseJarProj.MaxFlyTime"/> 请使用 <see cref="ModifyFlyTime(ref int)"/> 更改
        /// </summary>
        /// <param name="proj"></param>
        void OnJarShoot(BaseJarProj proj) { }

        /// <summary>
        /// 因为需要在绘制中调用，所以使用该方法更改
        /// </summary>
        /// <param name="maxFlyTime"></param>
        void ModifyFlyTime(ref int maxFlyTime) { }
    }

    public interface IFairyTongAccessory
    {
        /// <summary>
        /// 在抓手类武器开始攻击时调用
        /// </summary>
        /// <param name="proj"></param>
        void OnTongStartAttack(BaseTongsProj proj) { }
    }

    public interface IFairyGloveAccessory
    {
        /// <summary>
        /// 在手套类武器开始攻击时调用
        /// </summary>
        /// <param name="proj"></param>
        void ModifyGloveInit(BaseGloveProj proj) { }

        /// <summary>
        /// 自定义手套武器在攻击时的操作
        /// </summary>
        /// <param name="player"></param>
        /// <param name="source"></param>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        /// <param name="type"></param>
        /// <param name="catch"></param>
        void ModifyShootGlove(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback, int @catch) { }
    }
}
