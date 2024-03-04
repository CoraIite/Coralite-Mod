using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public partial class ThunderveinDragon
    {
        public void FallingThunder()
        {
            switch (SonState)
            {
                default:
                    ResetStates();
                    break;
            }
        }
    }
}
