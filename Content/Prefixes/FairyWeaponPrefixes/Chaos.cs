using Terraria;

namespace Coralite.Content.Prefixes.FairyWeaponPrefixes
{
    public class Chaos() : BaseFairyPrefix
        (damageMult: 0.9f, catchPowerMult: 0.85f, useSpeedMult: 1.15f, valueMult: 0.7f)
    {
        public override float RollChance(Item item)
        {
            return 0.66f;
        }
    }
}
