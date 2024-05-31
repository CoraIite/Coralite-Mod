using Coralite.Content.Items.LandOfTheLustrousSeries;
using Terraria;

namespace Coralite.Content.Prefixes.GemWeaponPrefixes
{
    public class Vibrant : ModPrefix
    {
        public override PrefixCategory Category => PrefixCategory.Magic;

        public override void Apply(Item item)
        {
            item.rare = ModContent.RarityType<VibrantRarity>();
        }

        public override bool CanRoll(Item item)
        {
            return item.DamageType == DamageClass.Magic&&item.ModItem is BaseGemWeapon;
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult = 2.5f;
        }

        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult = 1.18f;
            knockbackMult = 1.15f;
            useTimeMult = 0.95f;
            manaMult = 0.8f;
            critBonus = 4;
        }
    }

    public class VibrantRarity : ModRarity
    {
        public override Color RarityColor => Main.DiscoColor;
    }
}
