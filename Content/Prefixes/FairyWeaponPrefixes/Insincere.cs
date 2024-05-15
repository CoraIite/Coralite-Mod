using Terraria;

namespace Coralite.Content.Prefixes.FairyWeaponPrefixes
{
    public class Insincere() : BaseFairyPrefix
        (damageMult: 0.8f, knockbackMult: 0.9f, catchPowerMult: 0.7f, shootSpeedMult: 0.9f, valueMult: 0.65f)
    {
        public override float RollChance(Item item)
        {
            return 0.66f;
        }
    }
}
