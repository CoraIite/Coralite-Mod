using Terraria;

namespace Coralite.Content.Prefixes.FairyWeaponPrefixes
{
    public class Trivial() : BaseFairyPrefix
        (knockbackMult: 0.8f, catchPowerMult: 0.95f, valueMult: 0.85f)
    {
        public override float RollChance(Item item)
        {
            return 0.66f;
        }
    }
}
