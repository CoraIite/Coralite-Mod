using Coralite.Content.ModPlayers;
using Coralite.Content.RecipeGroups;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class Circumhorizon : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(13, 2f);
            Item.DefaultToRangedWeapon(10, AmmoID.Arrow, 27, 7f);

            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 0, 10);

            Item.noUseGraphic = true;

            Item.UseSound = CoraliteSoundID.Bow_Item5;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One);
            float rot = dir.ToRotation();
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<AfterglowHeldProj>(), damage, knockback, player.whoAmI, rot, 0);
            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(WoodenBowGroup.GroupName)
                .AddIngredient(ItemID.Torch, 49)
                .AddIngredient(ItemID.Ruby)
                .AddTile(TileID.WorkBenches)
                .Register();
        }

        public bool Dash(Player Player, int DashDir)
        {
            Vector2 newVelocity = Player.velocity;
            float dashDirection;
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        dashDirection = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        newVelocity.X = dashDirection * 7;
                        break;
                    }
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 80;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 20;
            Player.velocity = newVelocity;
            Player.direction = (int)dashDirection;

            if (Player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Player.Center);

                foreach (var proj in from proj in Main.projectile
                                     where proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.type == ProjectileType<AfterglowHeldProj>()
                                     select proj)
                {
                    proj.Kill();
                    break;
                }

                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<AfterglowHeldProj>(),
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, 1.57f + dashDirection * 1, 1, 20);
            }

            return true;
        }
    }

    public class CircumhorizonHeldProj : BaseDashBow
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "Circumhorizon";

        public override int GetItemType() => ItemType<Circumhorizon>();

        public int dashState;


        private enum DashState
        {
            /// <summary>
            /// 冲刺中
            /// </summary>
            dashing,
            /// <summary>
            /// 冲刺动作结束，正在被玩家捏住
            /// </summary>
            holding,
            /// <summary>
            /// 在冲刺过程中与敌对目标产生了碰撞，释放后的三连箭，
            /// </summary>
            specialRelease,
        }

        public override void DashAttackAI()
        {

        }

        public override void NormalShootAI()
        {

        }
    }
}