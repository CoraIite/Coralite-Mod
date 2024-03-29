using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.ShadowCastle
{
    [AutoloadEquip(EquipType.Face)]
    public class ShadowMask : ModItem
    {
        public override string Texture => AssetDirectory.ShadowCastleItems + Name;

        public override void SetStaticDefaults()
        {
            int slot = EquipLoader.GetEquipSlot(Mod, "ShadowMask", EquipType.Face);
            ArmorIDs.Face.Sets.PreventHairDraw[slot] = true;
            ArmorIDs.Face.Sets.OverrideHelmet[slot] = true;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Orange;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.noKnockback = true;
            player.moveSpeed += 0.03f;
        }
    }
}
