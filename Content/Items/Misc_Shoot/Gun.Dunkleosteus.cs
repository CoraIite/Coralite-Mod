using Coralite.Content.Items.Materials;
using Coralite.Core;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Misc_Shoot
{
    public class Dunkleosteus : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Shoot + "Old_Dunkleosteus";

        public int shootStyle;

        public override void SetDefaults()
        {
            Item.damage = 42;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.knockBack = 3;
            Item.shootSpeed = 11f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 8, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ProjectileID.PurificationPowder;
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

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                switch (shootStyle)
                {
                    default:
                    case 0:
                        SoundEngine.PlaySound(CoraliteSoundID.Shotgun2_Item38, player.Center);
                        Projectile.NewProjectile(source, player.Center, velocity.RotatedBy(Main.rand.NextFloat(-0.06f, 0.06f)), type, damage, knockback, player.whoAmI);
                        break;
                    case 1:     //射出6发子弹
                        SoundEngine.PlaySound(CoraliteSoundID.Gun3_Item41, player.Center);
                        for (int i = 0; i < 2; i++)
                            Projectile.NewProjectile(source, player.Center, velocity.RotatedBy(Main.rand.NextFloat(-0.12f, 0.12f)) * 0.95f, type, (int)(damage * 0.5f), knockback, player.whoAmI);
                        for (int i = 0; i < 2; i++)
                            Projectile.NewProjectile(source, player.Center, velocity.RotatedBy(Main.rand.NextFloat(-0.12f, 0.12f)), type, (int)(damage * 0.5f), knockback, player.whoAmI);
                        for (int i = 0; i < 2; i++)
                            Projectile.NewProjectile(source, player.Center, velocity.RotatedBy(Main.rand.NextFloat(-0.04f, 0.04f)) * 1.1f, type, (int)(damage * 0.5f), knockback, player.whoAmI);

                        break;
                }
                Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<DunkleosteusHeldProj>(), damage, knockback, player.whoAmI, shootStyle);
                shootStyle = Main.rand.NextBool(5) ? 1 : 0;
            }
            return false;
        }

        public override float UseSpeedMultiplier(Player player)
        {
            return shootStyle == 1 ? 0.45f : 1f;//六连发时使用时间乘以20/9，大约为11点使用时间
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