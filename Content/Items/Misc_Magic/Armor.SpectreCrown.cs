using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.Misc_Magic;

[AutoloadEquip(EquipType.Head)]
[PlayerEffect]
public class SpectreCrown : ModItem
{
    public override string Texture => AssetDirectory.Misc_Magic + Name;
    public static LocalizedText bonus;

    public override void Load()
    {
        bonus = this.GetLocalization("ArmorBonus");
    }

    public override void Unload()
    {
        bonus = null;
    }

    public override void SetStaticDefaults()
    {
        ArmorIDs.Head.Sets.DrawHatHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = true;
        ArmorIDs.Head.Sets.DrawFullHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = true;
    }

    public override void SetDefaults()
    {
        Item.value = Item.sellPrice(0, 7, 50);
        Item.rare = ItemRarityID.Yellow;
        Item.defense = 12;
    }

    public override void UpdateEquip(Player player)
    {
        player.statManaMax2 += 20;
        player.GetDamage(DamageClass.Magic) += 0.05f;
        player.GetCritChance(DamageClass.Magic) += 5f;
        if (!player.isDisplayDollOrInanimate)
            player.socialGhost = true;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ItemID.SpectreRobe && legs.type == ItemID.SpectrePants;
    }

    public override void UpdateArmorSet(Player player)
    {
        player.setBonus = bonus.Value;
        if (player.TryGetModPlayer(out CoralitePlayer cp))
            cp.AddEffect(nameof(SpectreCrown));
    }

    public override void PreUpdateVanitySet(Player player)
    {
        if (!player.isDisplayDollOrInanimate)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp) && cp.HasEffect(nameof(SpectreCrown)))
                player.socialGhost = true;

            player.SetArmorEffectVisuals(player);
        }
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.SpectreBar, 12)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}
