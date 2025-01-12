using Coralite.Core.Systems.DigSystem;
using System.Collections.Generic;

namespace Coralite.Content.ModPlayers.DigDigDig
{
    public partial class DigDigDigPlayer
    {
        /// <summary>
        /// 投镐视频
        /// </summary>
        public List<ICreatePickaxeAccessory> ThrownPickaxeAccessories = new();

        public void ResetPickaxeStats()
        {
            ThrownPickaxeAccessories??=new();
            if (ThrownPickaxeAccessories.Count > 0)
                ThrownPickaxeAccessories.Clear();
        }
    }
}
