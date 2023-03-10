using Coralite.Content.ModPlayers;
using Coralite.Content.Projectiles.Projectiles_Magic;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Weapons_Magic
{
    public class CosmosFracture : ModItem
    {
        public override string Texture => AssetDirectory.Weapons_Magic + Name;
        public override bool AltFunctionUse(Player Player) => true;

        private bool rightClick;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("异界裂隙");

            Tooltip.SetDefault("划破空间，来自异界的武器将撕裂你的敌人");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 265;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.mana = 22;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 4;
            Item.crit = 26;

            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.shoot = ProjectileType<CosmosFractureProj1>();

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.channel = true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.mana = 0;
                Item.shoot = ProjectileType<CosmosFractureProj2>();
                Item.channel = false;
                rightClick = true;
            }
            else
            {
                Item.mana = 20;
                Item.shoot = ProjectileType<CosmosFractureProj1>();
                Item.channel = true;
                rightClick = false;
            }

            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (rightClick && player.statMana > 25)
            {
                CoralitePlayer coralitePlayer = player.GetModPlayer<CoralitePlayer>();
                if (coralitePlayer.rightClickReuseDelay == 0)
                {
                    coralitePlayer.rightClickReuseDelay = 200;

                    for (int i = 0; i < 6; i++)
                        Projectile.NewProjectile(source, player.Center + (-1.57f + i * 1.047f).ToRotationVector2() * 120, Vector2.Zero, ProjectileType<CosmosFractureProj2>(), (int)(damage * 0.2f), knockback, player.whoAmI, 2, (5.712f + i * 1.047f));

                    SoundEngine.PlaySound(SoundID.Item71, player.Center);
                    player.statMana -= 25;
                }
                else
                    SoundEngine.PlaySound(SoundID.Item63, player.Center);
            }
            else
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage * 3, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
            .AddIngredient(ItemID.SkyFracture, 1)
            .AddIngredient(ItemID.LunarBar, 15)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}
