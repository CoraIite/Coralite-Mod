using Coralite.Content.Items.GlobalItems;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleBow : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 23;
            Item.useTime = Item.useAnimation = 26;
            Item.knockBack = 3f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Ranged;
            Item.useAmmo = AmmoID.Arrow;
            Item.value = Item.sellPrice(0, 1);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ProjectileType<IcicleBowHeldProj>();
            Item.UseSound = CoraliteSoundID.Bow_Item5;

            Item.useTurn = false;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.channel = true;
            CoraliteGlobalItem.SetColdDamage(Item);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One);
                float rot = dir.ToRotation();
                Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<IcicleBowHeldProj>(), damage, knockback, player.whoAmI, rot, 0);
                Projectile.NewProjectile(source, player.Center, dir * 13, ProjectileType<IcicleArrow>(), damage, knockback, player.whoAmI);
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<IcicleCrystal>(2)
            .AddIngredient<IcicleScale>(2)
            .AddTile(TileID.IceMachine)
            .Register();
        }

        public bool Dash(Player Player, int DashDir)
        {
            Vector2 newVelocity = Player.velocity;
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        float dashDirection = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        newVelocity.X = dashDirection * 10;
                        break;
                    }
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 80;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 20;
            Player.immuneTime = 20;
            Player.immune = true;
            Player.velocity = newVelocity;

            if (Player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, Player.Center);
                for (int i = 0; i < 4; i++)//生成冰晶粒子
                {
                    Vector2 center = Player.Center + ((-1.57f + (i * 1.57f)).ToRotationVector2() * 64);
                    Vector2 velocity = (i * 1.57f).ToRotationVector2() * 4;
                    IceStarLight.Spawn(center, velocity, 1f, () => Player.Center, 16);
                }

                for (int i = 0; i < 1000; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.ModProjectile is IcicleBowHeldProj)
                    {
                        proj.Kill();
                        break;
                    }
                }

                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<IcicleBowHeldProj>(),
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, (Main.MouseWorld - Player.Center).ToRotation(), 1);
            }

            return true;

        }
    }
}