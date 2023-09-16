using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public partial class NightmarePlantera
    {
        /// <summary>
        /// 狂暴状态的AI
        /// </summary>
        public void Rampage()
        {
            //在这里单独判断是否脱战
            //如果全部玩家都死了或者距离太远那么就脱战

            if (!Main.dayTime)
            {
                ResetStates();
                return;
            }
            NPC.velocity.Y -= 0.25f;
            if (NPC.velocity.Y<-32)
            {
                NPC.velocity.Y = -32;
            }

        }
    }
}
