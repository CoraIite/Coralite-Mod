using Coralite.Content.ModPlayers;
using Coralite.Core;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Thunder
{
    public class ThunderveinSpear : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public int useCount;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 55;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.knockBack = 2f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ProjectileType<ThunderveinBladeSlash>();

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (useCount > 2)
                    useCount = 0;

                switch (useCount)
                {
                    default:
                    case 0:
                    case 1:
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<ThunderveinBladeSlash>(), damage, knockback, player.whoAmI, useCount);
                        break;
                    case 2:
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<ThunderveinBladeSlash>(), (int)(damage * 1.25f), knockback, player.whoAmI, useCount);

                        break;
                }
            }

            useCount++;
            return false;
        }

        public override bool AllowPrefix(int pre) => true;
        public override bool MeleePrefix() => true;

        public override void AddRecipes()
        {
            //CreateRecipe()
            //    .AddIngredient<ZapCrystal>(2)
            //    .AddTile(TileID.MythrilAnvil)
            //    .Register();
        }

        public bool Dash(Player Player, int DashDir)
        {
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        float dashDirection = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        DashDir = (int)dashDirection;
                        break;
                    }
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 80;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 20;
            Player.immuneTime = 20;
            Player.immune = true;
            Player.velocity = new Vector2(DashDir * 30, Player.velocity.Y);
            Player.direction = DashDir;
            if (Player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, Player.Center);

                for (int i = 0; i < 1000; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.ModProjectile is ThunderveinBladeSlash)
                    {
                        proj.Kill();
                        break;
                    }
                }

                //生成手持弹幕
                int damage = Player.GetWeaponDamage(Player.HeldItem);
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<ThunderveinBladeSlash>(),
                    damage, Player.HeldItem.knockBack, Player.whoAmI, 3);
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<ThunderveinBladeDash>(),
                    damage, Player.HeldItem.knockBack, Player.whoAmI, 10, DashDir);
            }

            return true;
        }
    }
}
