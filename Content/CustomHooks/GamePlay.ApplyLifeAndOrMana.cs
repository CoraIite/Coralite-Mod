using Coralite.Content.ModPlayers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Coralite.Content.CustomHooks
{
    public class ApplyLifeAndOrMana : HookGroup
    {
        public override void Load()
        {
            On_Player.ApplyLifeAndOrMana += On_Player_ApplyLifeAndOrMana;
        }

        public override void Unload()
        {
            On_Player.ApplyLifeAndOrMana -= On_Player_ApplyLifeAndOrMana;
        }

        private void On_Player_ApplyLifeAndOrMana(On_Player.orig_ApplyLifeAndOrMana orig, Player self, Item item)
        {
            orig.Invoke(self, item);

            if (self.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.HasEffect(Items.Misc_Equip.BloodmarkTopper.BloodSet))
                    cp.bloodPoolCount = 0;
            }
        }
    }
}
