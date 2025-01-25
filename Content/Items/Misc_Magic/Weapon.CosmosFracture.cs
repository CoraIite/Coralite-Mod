using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Misc_Magic
{
    public class CosmosFracture : ModItem, IMagikeCraftable
    {
        public override string Texture => AssetDirectory.Misc_Magic + Name;
        public override bool AltFunctionUse(Player Player) => true;

        private bool rightClick;

        public override void SetDefaults()
        {
            Item.damage = 275;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.mana = 18;
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
            if (Main.myPlayer != player.whoAmI)
                return false;
            if (rightClick && player.statMana > 25)
            {
                CoralitePlayer coralitePlayer = player.GetModPlayer<CoralitePlayer>();
                if (coralitePlayer.rightClickReuseDelay == 0)
                {
                    coralitePlayer.rightClickReuseDelay = 200;

                    for (int i = 0; i < 6; i++)
                        Projectile.NewProjectile(source, player.Center + ((-1.57f + (i * 1.047f)).ToRotationVector2() * 120), Vector2.Zero, ProjectileType<CosmosFractureProj2>(), (int)(damage * 0.2f), knockback, player.whoAmI, 2, 5.712f + (i * 1.047f));

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

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateRecipe(ItemID.SkyFracture, ModContent.ItemType<CosmosFracture>(), 1_0000)
                .SetAntiMagikeCost(1_0000)
                .Register();
        }

        //public override void AddRecipes()
        //{
        //    CreateRecipe(1)
        //    .AddIngredient(ItemID.SkyFracture, 1)
        //    .AddIngredient(ItemID.LunarBar, 15)
        //    .AddTile(TileID.LunarCraftingStation)
        //    .Register();
        //}
    }
}
