using Coralite.Content.Items.LandOfTheLustrousSeries;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Prefixes.GemWeaponPrefixes
{
    public class VibrantAccessory : ModPrefix
    {
        public static LocalizedText tip;

        public override void Load()
        {
            if (!Main.dedServ)
                tip = this.GetLocalization("VibrantTips");
        }
        public override void Unload()
        {
            if (!Main.dedServ)
                tip = this.GetLocalization("VibrantTips");
        }

        public override PrefixCategory Category => PrefixCategory.Accessory;

        public override void Apply(Item item)
        {
            item.rare = ModContent.RarityType<VibrantRarity>();
        }

        public override bool CanRoll(Item item)
        {
            return item.accessory && item.ModItem is BaseGemWeapon;
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult = 2.5f;
        }

        public override void ApplyAccessoryEffects(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.03f;
            player.GetCritChance(DamageClass.Magic) += 0.03f;
            player.statManaMax2 += 20;
        }

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            return [new TooltipLine(Mod, "VibrantAccessory", tip.Value) {
                OverrideColor=Color.Orange
            }];
        }
    }
}
