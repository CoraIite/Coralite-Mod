
namespace Coralite.Content.Bosses.BabyIceDragon
{
    public partial class BabyIceDragon
    {
        /// <summary>
        /// 俯冲攻击，先飞上去（如果飞不上去就取消攻击），在俯冲向玩家，期间如果撞墙则原地眩晕
        /// </summary>
        public void Dive()
        {
            do
            {
                if (NPC.Center.Y > Target.Center.Y + 200)
                {

                }


            } while (false);

        }

        public void FlyUp()
        {
            //根据帧图来改变速度，大概效果是扇一下翅膀向上飞一小段
            NPC.velocity.X *= 0.98f;

            //只有扇翅膀的时候才会有向上加速度，否则减速
            switch (NPC.frame.Y)
            {
                default:
                case 0:
                case 3:
                case 4:
                    NPC.velocity.Y *= 0.96f;
                    break;
                case 1:
                case 2:
                    NPC.velocity.Y -= 3f;
                    break;
            }

            if (NPC.velocity.Y < -10)
                NPC.velocity.Y = -10;

        }
    }
}
