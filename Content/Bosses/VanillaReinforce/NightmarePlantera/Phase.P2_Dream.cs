using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Effects;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public sealed partial class NightmarePlantera
    {
        public void Dream_Phase2()
        {
            ((NightmareSky)SkyManager.Instance["NightmareSky"]).Timeleft = 100;

            switch ((int)State)
            {
                default:
                case (int)AIStates.nightmareBite:
                    NightmareBite();
                    break;

            }
        }

        public void NightmareBite()
        {
            do
            {

            } while (false);

            Timer++;
        }

        public void SetPhase2States()
        {

        }

        /// <summary>
        /// 梦境阶段的战斗的重设状态
        /// </summary>
        public void SetPhase2DreamingStates()
        {

        }
    }
}
