using Coralite.Content.ModPlayers;
using Coralite.Core;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.Misc_Equip
{
    [AutoloadEquip(EquipType.Head)]
    public class DemonsCry : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Equip + Name;

        public static LocalizedText bonus;

        public override void Load()
        {
            bonus = this.GetLocalization("ArmorBonus");
        }

        public override void Unload()
        {
            bonus = null;
        }

        public override void SetDefaults()
        {
            Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(0, 0, 90));
            Item.defense = 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemID.MoltenBreastplate && legs.type == ItemID.MoltenGreaves;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = bonus.Value;

            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.MaxFlyingShield++;

            player.fireWalk = true;
            player.buffImmune[BuffID.OnFire] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
