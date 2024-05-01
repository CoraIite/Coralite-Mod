using Terraria;

namespace Coralite.Content.Prefixes.FairyWeaponPrefixes
{
    public class Hollow() : BaseFairyPrefix
        (catchPowerMult: 0.88f, useSpeedMult: 1.12f, valueMult: 0.76f)
    {
        public override float RollChance(Item item)
        {
            return 0.66f;
        }
    }
}
