using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.ComponentModel.Design;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class FarAwaySky : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(20, 1f);
            Item.DefaultToRangedWeapon(10, AmmoID.Arrow, 27, 7.5f);

            Item.rare = ItemRarityID.Green;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 0, 80);

            Item.noUseGraphic = true;

            Item.UseSound = CoraliteSoundID.Bow_Item5;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One);
            float rot = dir.ToRotation();
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<FarAwaySkyHeldProj>(), damage, knockback, player.whoAmI, rot, 0);
            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Cloud, 14)
                .AddIngredient(ItemID.ShadowScale, 12)
                .AddTile(TileID.SkyMill)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Cloud, 14)
                .AddIngredient(ItemID.TissueSample, 12)
                .AddTile(TileID.SkyMill)
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
                                     where proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.type == ProjectileType<FarAwaySkyHeldProj>()
                                     select proj)
                {
                    proj.Kill();
                    break;
                }

                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<FarAwaySkyHeldProj>(),
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, 1.57f - dashDirection * 1, 1, 20);
            }

            return true;
        }
    }

    public class FarAwaySkyHeldProj : BaseDashBow
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "FarAwaySky";

        private Vector2 arrowPos;

        private static Asset<Texture2D> ArrowTex;

        public ref float ArrowRotation => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float RecordAngle => ref Projectile.localAI[2];

        public int State;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            ArrowTex = Request<Texture2D>(AssetDirectory.ThyphionSeriesItems + "FarAwaySkyArrow");
        }

        public override void Unload()
        {
            ArrowTex = null;
        }

        public override int GetItemType()
            => ItemType<FarAwaySky>();

        public override Vector2 GetOffset()
            => new(10, 0);

        public override void DashAttackAI()
        {
            switch (State)
            {
                default:
                case 0:
                    DashState();
                    break;
                case 1:
                    ShootState();
                    break;
            }

            Projectile.rotation = Rotation;
            Timer++;
        }

        public void DashState()
        {
            arrowPos = Projectile.Center;

            if (Timer < DashTime + 2)
            {
                Owner.itemTime = Owner.itemAnimation = 2;

                Rotation = Helper.Lerp(RecordAngle, OwnerDirection > 0 ? -1f : 4.141f, Timer / DashTime);
                return;
            }

            if (Owner.controlUseItem)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    if (Main.rand.NextBool(10))
                    {
                        Vector2 dir = Rotation.ToRotationVector2();
                        Vector2 center = Projectile.Center + dir * 20;
                    }
                }

                Projectile.timeLeft = 2;
                Owner.itemTime = Owner.itemAnimation = 2;
            }
            else
            {
                SoundEngine.PlaySound(CoraliteSoundID.Bow2_Item102, Owner.Center);

                if (Main.myPlayer == Projectile.owner)
                {
                    State = 1;
                    Timer = 0;
                    Projectile.timeLeft = 100;
   }
            }
        }

        public void ShootState()
        {

        }

        public override void Initialize()
        {
            RecordAngle = Rotation;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, mainTex.Size() / 2, 1, OwnerDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            if (Special == 0)
                return false;

            Texture2D arrowTex = ArrowTex.Value;
            Vector2 dir = Rotation.ToRotationVector2();
            Main.spriteBatch.Draw(arrowTex, arrowPos - Main.screenPosition, null, lightColor, Projectile.rotation + 1.57f
                , new Vector2(arrowTex.Width / 2, arrowTex.Height * 5 / 6), 1, 0, 0f);

            return false;
        }
    }

    public class CloudBonus : ModBuff
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(CloudBonus));
        }
    }
}
