using Coralite.Content.CustomHooks;
using Coralite.Core;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.FlyingShields
{
    [AutoloadEquip(EquipType.Head)]
    public class PirateKingHat : ModItem, ISpecialDrawHead
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public override void SetDefaults()
        {

            Item.defense = 2;
        }

        public override void UpdateEquip(Player player)
        {
        }

        public override void UpdateArmorSet(Player player)
        {
        }

    }
}
