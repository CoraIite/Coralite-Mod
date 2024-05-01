using Terraria;

namespace Coralite.Content.Prefixes.FairyWeaponPrefixes
{
    public class Fragile() : BaseFairyPrefix
        (damageMult: 0.82f, valueMult: 0.9f)
    {
        public override float RollChance(Item item)
        {
            return 0.66f;
        }
    }
}
