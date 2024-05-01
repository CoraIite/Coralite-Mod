using Terraria;

namespace Coralite.Content.Prefixes.FairyWeaponPrefixes
{
    public class Dim() : BaseFairyPrefix
        (catchPowerMult: 0.9f, valueMult: 0.85f)
    {
        public override float RollChance(Item item)
        {
            return 0.66f;
        }
    }
}
