using Coralite.Core.Systems.WorldValueSystem;

namespace Coralite.Core.Systems.MagikeSystem
{
    public class LearnedMagikeAdvanced : WorldFlag
    {
        //魔能进阶解锁：玩家右键使用《魔能进阶》卷轴触发（CanUseItem 仅持有者客户端运行）。
        //同 LearnedMagikeBase，允许客户端发请求，多人下方可解锁。
        public override bool AcceptClientChangeRequest => true;
    }
}
