using Terraria;

namespace Coralite.Content.Prefixes.FairyWeaponPrefixes
{
    public class Powerless() : BaseFairyPrefix
        (knockbackMult: 0.95f, catchPowerMult: 0.95f, shootSpeedMult: 0.92f, valueMult: 0.85f)
    {
        public override float RollChance(Item item)
        {
            return 0.66f;
        }
    }
}
