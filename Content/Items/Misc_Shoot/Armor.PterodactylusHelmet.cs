using Coralite.Core;
using Coralite.Core.Attributes;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.Misc_Shoot
{
    [PlayerEffect]
    [AutoloadEquip(EquipType.Head)]
    public class PterodactylusHelmet : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;

        public static LocalizedText bonus;

        public override void Load()
        {
            if (!Main.dedServ)
                bonus = this.GetLocalization("ArmorBonus");
        }

        public override void Unload()
        {
            bonus = null;
        }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(silver: 30);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemID.FossilShirt && legs.type == ItemID.FossilPants;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = bonus.Value;

            //if (player.TryGetModPlayer(out CoralitePlayer cp))
            //    cp.AddEffect(nameof(AmberSeed));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FossilOre, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }

    }
}
