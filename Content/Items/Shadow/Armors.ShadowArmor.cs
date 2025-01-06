using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Shadow
{
    [AutoloadEquip(EquipType.Head)]
    public class ShadowHead : ModItem, IControllableArmorBonus
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public static LocalizedText ArmorMonus;

        public override void Load()
        {
            if (!Main.dedServ)
                ArmorMonus = this.GetLocalization("ArmorBonus");
        }

        public override void Unload()
        {
            if (!Main.dedServ)
                ArmorMonus = null;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 18;
            Item.value = Item.sellPrice(0, 0, 80, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<ShadowBreastplate>() && legs.type == ItemType<ShadowLegs>();
        }

        public override void UpdateEquip(Player player)
        {
            player.manaCost -= 0.10f;
            player.statManaMax2 += 60;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = ArmorMonus.Value;
            if (Main.myPlayer == player.whoAmI && player.ownedProjectileCounts[ProjectileType<ShadowCircle>()] < 1)
            {
                //生成弹幕
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Microsoft.Xna.Framework.Vector2.Zero,
                    ProjectileType<ShadowCircle>(), 50, 0, player.whoAmI);
            }
        }

        public void UseArmorBonus(Player player)
        {
            foreach (var proj in Main.projectile)
            {
                if (proj.active && proj.owner == player.whoAmI && proj.type == ProjectileType<ShadowCircle>())
                {
                    //设置特殊行动
                    (proj.ModProjectile as ShadowCircle).StartAttack();
                    break;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowCrystal>(8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class ShadowBreastplate : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 18;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 8;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.10f;
            player.GetCritChance(DamageClass.Generic) += 6f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowCrystal>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class ShadowLegs : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 18;
            Item.value = Item.sellPrice(0, 0, 80, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 6;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed(DamageClass.Generic) += 0.08f;
            player.GetKnockback(DamageClass.Generic) += 0.06f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowCrystal>(8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
