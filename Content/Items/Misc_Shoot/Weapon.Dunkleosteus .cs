using Coralite.Content.Items.Misc;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Misc_Shoot
{
    public class Dunkleosteus :ModItem
    {
        public override string Texture => AssetDirectory.Misc_Shoot + "Old_Dunkleosteus";

        public int shootStyle;

        public override void SetDefaults()
        {
            Item.damage = 44;
            Item.useTime = 6;
            Item.useAnimation = 6;
            Item.knockBack = 3;
            Item.shootSpeed = 11f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = 10;
            Item.useAmmo = AmmoID.Bullet;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        /// <summary>
        /// 66%概率不消耗弹药
        /// </summary>
        /// <param name="ammo"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextBool(1, 3);

        public override bool CanUseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (Main.rand.NextBool(1,5))
                {
                    Item.useTime = 11;
                    Item.useAnimation = 11;
                    shootStyle = 1;
                    Item.UseSound = CoraliteSoundID.Shotgun2_Item38;
                }
                else
                {
                    Item.useTime = 5;
                    Item.useAnimation = 5;
                    shootStyle = 0;
                    Item.UseSound = CoraliteSoundID.Gun3_Item41;
                }
            }
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                switch (shootStyle)
                {
                    default:
                    case 0:
                        Projectile.NewProjectile(source, player.Center, velocity.RotatedBy(Main.rand.NextFloat(-0.06f, 0.06f)), type, damage, knockback, player.whoAmI);
                        break;
                    case 1:     //射出6发子弹
                        for (int i = 0; i < 2; i++)
                            Projectile.NewProjectile(source, player.Center, velocity.RotatedBy(Main.rand.NextFloat(-0.12f, 0.12f))*0.95f, type, (int)(damage * 0.5f), knockback, player.whoAmI);
                        for (int i = 0; i < 2; i++)
                            Projectile.NewProjectile(source, player.Center, velocity.RotatedBy(Main.rand.NextFloat(-0.12f, 0.12f)), type, (int)(damage * 0.5f), knockback, player.whoAmI);
                        for (int i = 0; i < 2; i++)
                            Projectile.NewProjectile(source, player.Center, velocity.RotatedBy(Main.rand.NextFloat(-0.04f, 0.04f))*1.1f, type, (int)(damage * 0.5f), knockback, player.whoAmI);

                        break;
                }
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<DunkleosteusHeldProj>(), damage, knockback, player.whoAmI, shootStyle);

            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Megashark)
            .AddIngredient<AncientCore>()
            .AddIngredient<DukeFishronSkin>(5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}