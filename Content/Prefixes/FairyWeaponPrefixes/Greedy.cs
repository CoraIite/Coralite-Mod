using Terraria;

namespace Coralite.Content.Prefixes.FairyWeaponPrefixes
{
    public class Greedy() : BaseFairyPrefix
        (damageMult: 0.92f, shootSpeedMult: 0.92f, valueMult: 0.85f)
    {
        public override float RollChance(Item item)
        {
            return 0.66f;
        }
    }
}
