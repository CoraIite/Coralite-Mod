using Coralite.Core;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.ShadowItems
{
    public class ShadowSword : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        private float rotation = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("潜伏");

            Tooltip.SetDefault("尽管是把剑，但却充满魔力！");
        }

        public override void SetDefaults()
        {
            Item.height = Item.width = 40;

            Item.damage = 24;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.knockBack = 6f;
            Item.reuseDelay = 20;
            Item.mana = 9;
            Item.crit = 10;

            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item9;

            Item.autoReuse = true;
            Item.useTurn = true;
            Item.noMelee = false;

            Item.shoot = ModContent.ProjectileType<ShadowSwordProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectile(source, player.Center + new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * 50f, new Vector2(0, -0.03f), type, damage, 6, player.whoAmI);
                rotation += 3;
            }

            rotation -= 5.5f;

            if (rotation > 8)
                rotation = 0;

            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<ShadowCrystal>(), 12);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
