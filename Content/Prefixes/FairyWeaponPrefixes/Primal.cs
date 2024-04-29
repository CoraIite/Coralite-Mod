using Terraria;

namespace Coralite.Content.Prefixes.FairyWeaponPrefixes
{
    public class Primal() : BaseFairyPrefix
        (damageMult: 1.12f, knockbackMult: 1.1f, critBonus: 5, valueMult: 1.26f)
    {
        public override float RollChance(Item item)
        {
            return 0.9f;
        }
    }
}
