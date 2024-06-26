using Coralite.Content.ModPlayers;
using Terraria;

namespace Coralite.Helpers
{
    public partial class Helper
    {
        /// <summary>
        /// 玩家是否使用了特殊攻击键
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool UseSpecialAttack(this Player player) => player.TryGetModPlayer(out CoralitePlayer cp) && cp.useSpecialAttack;
    }
}
