using Coralite.Core.Systems.WorldValueSystem;

namespace Coralite.Core.Systems.MagikeSystem
{
    public class LearnedMagikeBase : WorldFlag
    {
        //魔能教程解锁：由玩家右键使用《魔能基础》书页这类合法交互触发。
        //该解锁本应玩家说了算（无 Boss/世界事件前置），且物品使用的 CanUseItem 只在持有者客户端运行，
        //服务端不会替远程玩家跑该回调，因此必须允许客户端发请求，否则多人下永远解锁不了。
        public override bool AcceptClientChangeRequest => true;
    }
}
