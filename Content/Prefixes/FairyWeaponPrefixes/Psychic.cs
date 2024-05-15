using Terraria;

namespace Coralite.Content.Prefixes.FairyWeaponPrefixes
{
    public class Psychic() : BaseFairyPrefix
        (damageMult: 1.05f, catchPowerMult: 1.12f, shootSpeedMult: 1.05f, valueMult: 1.22f)
    {
        public override float RollChance(Item item)
        {
            return 0.9f;
        }
    }
}
