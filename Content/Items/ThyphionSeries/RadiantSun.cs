using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class RadiantSun : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(35, 1f);
            Item.DefaultToRangedWeapon(10, AmmoID.Arrow, 25, 10f);

            Item.rare = ItemRarityID.LightRed;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 2);

            Item.noUseGraphic = true;

            Item.UseSound = CoraliteSoundID.Bow_Item5;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One);
            float rot = dir.ToRotation();
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<RadiantSunHeldProj>(), damage, knockback, player.whoAmI, rot, 0);
            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddIngredient(ItemID.HellwingBow)
                .AddIngredient(ItemID.MoltenFury)
                .AddIngredient<Afterglow>()
                .AddTile(TileID.Anvils)
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

    public class RadiantSunHeldProj : BaseDashBow,IDrawAdditive
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "RadiantSun";

        private Vector2 arrowPos;

        private static Asset<Texture2D> GlowTex;
        private static Asset<Texture2D> ArrowTex;

        public ref float ArrowLength => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float RecordAngle => ref Projectile.localAI[2];

        public int State;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            GlowTex = Request<Texture2D>(AssetDirectory.ThyphionSeriesItems + "RadiantSun_Glow");
            ArrowTex = Request<Texture2D>(AssetDirectory.ThyphionSeriesItems + "RadiantSunArrow");
        }

        public override void Unload()
        {
            ArrowTex = null;
        }

        public override int GetItemType()
            => ItemType<RadiantSun>();

        public override Vector2 GetOffset()
            => new(14, 0);

        public override void AIAfter()
        {
            Lighting.AddLight(Projectile.Center, Color.LightGoldenrodYellow.ToVector3()/2);
        }

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
                Owner.itemTime = Owner.itemAnimation = 2;

                Rotation = Helper.Lerp(RecordAngle, OwnerDirection > 0 ? -1.53f : (3.141f + 1.53f), Coralite.Instance.HeavySmootherInstance.Smoother(Timer / DashTime));
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
                    Owner.AddBuff(BuffType<CloudBonus>(), 60 * 8);
                    Vector2 dir = Rotation.ToRotationVector2();
                    WindCircle.Spawn(Projectile.Center + (dir * 30), -dir, Rotation, Color.Yellow, 0.75f, 0.95f, new Vector2(1.5f, 0.8f));
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

                Particle.NewParticle<SpeedLine>(arrowPos, (-dir2).RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(3, 7),
                    c, Scale: Main.rand.NextFloat(0.2f, 0.4f));
            }

            for (int i = 0; i < 5; i++)
            {
                Color c = Main.rand.NextFromList(Color.White, Color.SkyBlue, Color.LightSkyBlue);
                c.A = 100;
                Particle.NewParticle<Fog>(arrowPos, (-dir).RotateByRandom(-0.8f, 0.8f) * Main.rand.NextFloat(3, 6),
                   c, Scale: Main.rand.NextFloat(0.6f, 1f));
            }

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
            var effect = OwnerDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            var origin = mainTex.Size() / 2;

            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, origin, 1,effect , 0f);
            Main.spriteBatch.Draw(GlowTex.Value, center, null, Color.White, Projectile.rotation, origin, 1, effect, 0f);

            if (Special == 0)
                return false;

            Texture2D arrowTex = ArrowTex.Value;
            Vector2 dir = Rotation.ToRotationVector2();
            Main.spriteBatch.Draw(arrowTex, arrowPos - Main.screenPosition, null, lightColor, Projectile.rotation + 1.57f
                , new Vector2(arrowTex.Width / 2, arrowTex.Height * 5 / 6), 1, 0, 0f);

            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
        }
    }

    public class RadiantSunBall
    {

    }

    public class RadiantSunFlow:Particle
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "RadiantSunFlow";
    }
}
