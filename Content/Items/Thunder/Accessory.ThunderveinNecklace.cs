using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Thunder
{
    [AutoloadEquip(EquipType.Neck)]
    [PlayerEffect]
    public class ThunderveinNecklace : ModItem
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public override void SetDefaults()
        {
            Item.expert = true;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.sellPrice(0, 5);
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<ThunderElectrified>()] = true;
            player.GetArmorPenetration(DamageClass.Generic) += 7;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(ThunderveinNecklace));
        }
    }
}
