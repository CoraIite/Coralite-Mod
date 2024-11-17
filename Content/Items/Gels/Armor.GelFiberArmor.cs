using Coralite.Content.CustomHooks;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Gels
{
    [AutoloadEquip(EquipType.Head)]
    public class GelFiberHelmet : ModItem, ISpecialDrawHead
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 4;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GelFiber>(20)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class GelFiberBreastplate : ModItem
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public static LocalizedText bonus;

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 6;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ItemType<GelFiberHelmet>() && legs.type == ItemType<GelFiberBoots>();
        }

        public override void Load()
        {
            bonus = this.GetLocalization("ArmorBonus");
        }

        public override void Unload()
        {
            bonus = null;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = bonus.Value;

            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (Vector2.Distance(cp.oldOldCenter, player.Center) < 0.3f)
                {
                    player.endurance += 0.08f;
                    player.GetDamage(DamageClass.Melee).Flat += 5;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GelFiber>(34)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class GelFiberBoots : ModItem
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 4;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GelFiber>(18)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
