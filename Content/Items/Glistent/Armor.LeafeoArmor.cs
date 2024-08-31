using Coralite.Content.ModPlayers;
using Coralite.Core;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Glistent
{
    [AutoloadEquip(EquipType.Head)]
    public class LeafeoHelmet : ModItem
    {
        public override string Texture => AssetDirectory.GlistentItems + Name;

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 1;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class LeafeoLightArmor : ModItem
    {
        public override string Texture => AssetDirectory.GlistentItems + Name;

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(silver: 10);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ItemType<LeafeoHelmet>() && legs.type == ItemType<LeafeoBoots>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(14)
                .AddIngredient(ItemID.IronBar, 8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class LeafeoBoots : ModItem
    {
        public override string Texture => AssetDirectory.GlistentItems + Name;

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 1;
        }

        public override void UpdateEquip(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.fallDamageModifyer -= 0.25f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LeafeoShield:ModProjectile
    {
        public override string Texture => AssetDirectory.OtherProjectiles+ "Circle";
    }
}
