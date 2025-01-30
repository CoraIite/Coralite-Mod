using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
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

        public float Priority => IDashable.HeldItemDash;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(18, 1f);
            Item.DefaultToRangedWeapon(10, AmmoID.Arrow, 27, 8f);

            Item.rare = ItemRarityID.Green;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 0, 80);

            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.UseSound = CoraliteSoundID.Bow_Item5;
        }

        public override void HoldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddDash(this);
            }
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
                .AddIngredient(ItemID.DemoniteBar, 18)
                .AddTile(TileID.SkyMill)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Cloud, 14)
                .AddIngredient(ItemID.CrimtaneBar, 18)
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
                        newVelocity.X = dashDirection * 8;
                        break;
                    }
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 100;
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
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, 1.57f - dashDirection * 0.3f, 1, 20);
            }

            return true;
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.ThyphionSeriesItems)]
    public class FarAwaySkyHeldProj : BaseDashBow
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "FarAwaySky";

        private Vector2 arrowPos;

        [AutoLoadTexture(Name = "FarAwaySkyArrow")]
        public static ATex ArrowTex { get; private set; }

        public ref float ArrowLength => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float RecordAngle => ref Projectile.localAI[2];

        public int State;
        public float handOffset = 0;

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
            if (Timer < DashTime + 2)
            {
                LockOwnerItemTime();

                Rotation = Helper.Lerp(RecordAngle, DirSign > 0 ? -1f : (3.141f + 1f), Coralite.Instance.HeavySmootherInstance.Smoother(Timer / DashTime));
                return;
            }

            if (Owner.controlUseItem)
            {
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
                    Owner.AddBuff(BuffType<CloudBonus>(), 60 * 8);
                    Vector2 dir = Rotation.ToRotationVector2();
                    WindCircle.Spawn(Projectile.Center + (dir * 30), -dir, Rotation, Color.White, 0.75f, 0.95f, new Vector2(1.5f, 0.8f));
                    WindCircle.Spawn(Projectile.Center + (dir * 20), -dir, Rotation, Color.SkyBlue, 0.55f, 1.55f, new Vector2(1.5f, 0.8f));
                }
            }
        }

        public void ShootState()
        {
            ArrowLength += 19;

            Vector2 dir = (arrowPos - (Owner.Center + Owner.velocity * 12)).SafeNormalize(Vector2.Zero);
            Vector2 dir2 = Rotation.ToRotationVector2();

            for (int i = 0; i < 4; i++)
            {
                Dust d = Dust.NewDustPerfect(arrowPos, DustID.Cloud, (-dir).RotateByRandom(-0.6f, 0.6f) * Main.rand.NextFloat(3, 6),
                    50, Scale: Main.rand.NextFloat(0.8f, 1f));

                //d.noGravity = true;
            }

            if (Timer % 2 == 0)
            {
                Color c = Main.rand.NextFromList(Color.White, Color.SkyBlue, Color.DeepSkyBlue);
                c.A = 100;

                PRTLoader.NewParticle<SpeedLine>(arrowPos, (-dir2).RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(3, 7),
                    c, Scale: Main.rand.NextFloat(0.2f, 0.4f));
            }

            for (int i = 0; i < 5; i++)
            {
                Color c = Main.rand.NextFromList(Color.White, Color.SkyBlue, Color.LightSkyBlue);
                c.A = 100;
                PRTLoader.NewParticle<Fog>(arrowPos, (-dir).RotateByRandom(-0.8f, 0.8f) * Main.rand.NextFloat(3, 6),
                   c, Scale: Main.rand.NextFloat(0.6f, 1f));
            }

            if (Timer < 4)
                handOffset -= 4;
            else
                handOffset += 4;

            if (Timer > 7)
                Projectile.Kill();
        }

        public override void SetCenter()
        {
            base.SetCenter();
            if (Special == 1)
            {
                if (State == 0)
                    arrowPos = Projectile.Center;
                else
                {
                    Vector2 dir = Rotation.ToRotationVector2();
                    arrowPos = Projectile.Center + dir * ArrowLength;
                }
            }
        }

        public override void Initialize()
        {
            RecordAngle = Rotation;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;
            Vector2 dir = Rotation.ToRotationVector2();

            Main.spriteBatch.Draw(mainTex, center+dir*handOffset, null, lightColor, Projectile.rotation, mainTex.Size() / 2, 1, DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            if (Special == 0)
                return false;

            Texture2D arrowTex = ArrowTex.Value;
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

            player.moveSpeed += 0.15f;
            player.GetDamage(DamageClass.Ranged) += 0.15f;

            if (Main.rand.NextBool())
                Dust.NewDustPerfect(player.Bottom + new Vector2(Main.rand.NextFloat(-16, 16), Main.rand.NextFloat(-6, 0))
                   , DustID.Cloud, -player.velocity.RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(0.1f, 0.2f),
                    50, Scale: Main.rand.NextFloat(0.8f, 1.4f));
        }
    }
}
