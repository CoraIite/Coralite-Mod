using Coralite.Content.ModPlayers;
using Coralite.Content.RecipeGroups;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.SmoothFunctions;
using Coralite.Helpers;
using System.IO;
using System.Linq;
using System.Threading;
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
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<CircumhorizonHeldProj>(), damage, knockback, player.whoAmI, rot, 0);
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
                                     where proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.type == ProjectileType<CircumhorizonHeldProj>()
                                     select proj)
                {
                    proj.Kill();
                    break;
                }

                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<CircumhorizonHeldProj>(),
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, 1.57f + dashDirection * 1, 1, 20);
            }

            return true;
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.ThyphionSeriesItems)]
    public class CircumhorizonHeldProj : BaseDashBow
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "Circumhorizon";

        public override int GetItemType() => ItemType<Circumhorizon>();

        public int dashState;
        public ref float RecordOwnerDirection => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float RecordAngle => ref Projectile.localAI[1];

        public SecondOrderDynamics_Float facotr;

        public static ATex CircumhorizonGradient { get; private set; }

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

        public override void Initialize()
        {
            if (Special == 0)
                return;

            RecordOwnerDirection = Owner.direction;
            RecordAngle = (RecordOwnerDirection > 0 ? 0.2f : MathHelper.Pi - 0.2f);
            Rotation = RecordAngle;

            if (VaultUtils.isClient)
            {
                facotr = new SecondOrderDynamics_Float(1f, 0.75f, 0, RecordAngle);
            }
        }

        #region 冲刺攻击部分

        public override void DashAttackAI()
        {
            if (Timer < DashTime + 2)//冲刺过程中
            {
                Owner.itemTime = Owner.itemAnimation = 2;

                Dashing_Angle();
            }
        }

        public void Dashing_Angle()
        {
            if (Timer < (int)(DashTime / 2))//前三分之一段，向上抬起弓
            {
                float angle = Helper.Lerp(RecordAngle,
                    RecordOwnerDirection > 0 ? (-MathHelper.PiOver2 + 0.2f) : (MathHelper.PiOver2 * 3 - 0.2f)
                    , Timer / (DashTime / 3));
                Rotation = facotr.Update(1 / 60f, angle);
                return;
            }

            if (Timer== (int)(DashTime / 2))//改变记录角度，之后转向向下
            {
                RecordAngle = RecordOwnerDirection > 0 ? (-MathHelper.PiOver2 + 0.2f) : (MathHelper.PiOver2 * 3 - 0.2f);
            }
        }

        #endregion

        public override void NormalShootAI()
        {

        }

        public override void NetCodeHeldSend(BinaryWriter writer)
        {
            writer.Write(dashState);
        }

        public override void NetCodeReceiveHeld(BinaryReader reader)
        {
            dashState = reader.ReadInt32();
        }

    }
}