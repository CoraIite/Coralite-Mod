using Terraria;

namespace Coralite.Content.Prefixes.FairyWeaponPrefixes
{
    public class Resonance() : BaseFairyPrefix
        (knockbackMult: 1.1f, catchPowerMult: 1.1f, useSpeedMult: 0.95f, shootSpeedMult: 1.05f, valueMult: 1.35f)
    {
        public override float RollChance(Item item)
        {
            return 0.9f;
        }
    }
}
