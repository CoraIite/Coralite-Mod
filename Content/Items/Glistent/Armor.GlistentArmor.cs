using Coralite.Content.DamageClasses;
using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Glistent
{
    [AutoloadEquip(EquipType.Head)]
    public class GlistentHelmet : ModItem
    {
        public override string Texture => AssetDirectory.GlistentItems + Name;

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
        }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 60, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<GlistentBreastplate>()
                && legs.type == ItemType<GlistentLegs>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(FairyDamage.Instance) += 0.08f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = bonus.Value;
        }

        public void UseArmorBonus(Player player)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GlistentBar>(8)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<LeafeoHelmet>()
                .AddIngredient(ItemID.Diamond, 2)
                .AddIngredient(ItemID.DemoniteBar, 7)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient<LeafeoHelmet>()
                .AddIngredient(ItemID.Diamond, 2)
                .AddIngredient(ItemID.CrimtaneBar, 7)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class GlistentBreastplate : ModItem
    {
        public override string Texture => AssetDirectory.GlistentItems + Name;

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 90, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 6;
        }

        public override void UpdateEquip(Player player)
        {
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
            {
                fcp.fairyCatchPowerBonus += 0.1f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GlistentBar>(10)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<LeafeoLightArmor>()
                .AddIngredient(ItemID.Diamond, 4)
                .AddIngredient(ItemID.DemoniteBar, 9)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient<LeafeoLightArmor>()
                .AddIngredient(ItemID.Diamond, 4)
                .AddIngredient(ItemID.CrimtaneBar, 9)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class GlistentLegs : ModItem
    {
        public override string Texture => AssetDirectory.GlistentItems + Name;

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 60, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 4;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.08f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GlistentBar>(8)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<LeafeoBoots>()
                .AddIngredient(ItemID.Diamond, 2)
                .AddIngredient(ItemID.DemoniteBar, 7)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient<LeafeoBoots>()
                .AddIngredient(ItemID.Diamond, 2)
                .AddIngredient(ItemID.CrimtaneBar, 7)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
