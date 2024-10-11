using Coralite.Content.WorldGeneration;

namespace Coralite.Content.ModPlayers.DigDigDig
{
    public class DigDigDigPlayer:ModPlayer
    {
        /// <summary>
        /// 灵感值
        /// </summary>
        public int Muse;

        public override void PreUpdate()
        {
        }

        public override void PostUpdateMiscEffects()
        {
            if (CoraliteWorld.DigDigDigWorld)
            {
                Player.statMana = 0;//禁用魔力
                Player.statManaMax2 = 0;
            }
        }
    }
}
