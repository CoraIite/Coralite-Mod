using Terraria;

namespace Coralite.Content.Prefixes.FairyWeaponPrefixes
{
    public class Eternal() : BaseFairyPrefix
        (knockbackMult: 1.15f, critBonus: 5, catchPowerMult: 1.15f, useSpeedMult: 0.9f, shootSpeedMult: 1.05f, valueMult: 1.75f)
    {
        public override float RollChance(Item item)
        {
            return 0.9f;
        }
    }
}
