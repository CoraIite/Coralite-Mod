using Coralite.Core;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Misc_Equip
{
    [AutoloadEquip(EquipType.Body)]
    public class FlorafrayChestplate : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Equip + Name;

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(gold: 5);
            //Item.rare = RarityType<BloodRarity>();
            Item.defense = 15;
        }
    }
}
