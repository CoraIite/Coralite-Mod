using Coralite.Content.Items.Glistent;
using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class LeafShield : BaseFlyingShieldItem<LeafShieldGuard>
    {
        public LeafShield() : base(Item.sellPrice(0, 0, 5), ItemRarityID.Blue, AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 20;
            Item.shoot = ModContent.ProjectileType<LeafShieldProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 12.5f;
            Item.damage = 16;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(8)
                .AddTile(TileID.LivingLoom)
                .Register();
        }

        public override void LeftShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 velocity, int type, int damage, float knockback)
        {
            int index = Projectile.NewProjectile(source, player.Center + new Vector2(0, -16), velocity, type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, player.Center + new Vector2(0, -16), Vector2.Zero, ModContent.ProjectileType<LeafShieldEXProj>(), damage / 2, knockback, player.whoAmI, index, 0,Main.rand.Next(40));
        }
    }

    public class LeafShieldProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "LeafShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 34;
        }

        public override void SetOtherValues()
        {
            flyingTime = 19;
            backTime = 15;
            backSpeed = 13f;
            trailCachesLength = 6;
            trailWidth = 8;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(CoraliteSoundID.Grass, Projectile.Center);
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnShootDusts()
        {
            extraRotation += 0.4f;
        }

        public override void OnBackDusts()
        {
            extraRotation += 0.4f;
        }

        public override Color GetColor(float factor)
        {
            return Color.Green * factor;
        }
    }

    public class LeafShieldGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "LeafShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 34;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.05f;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            SoundEngine.PlaySound(CoraliteSoundID.Grass, Projectile.Center);
        }
    }

    public class LeafShieldEXProj : ModProjectile
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public ref float Owner => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public ref float Rand => ref Projectile.ai[2];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 16;
            Projectile.DamageType = DamageClass.Melee;

            Projectile.penetrate = 3;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            Projectile.UpdateFrameNormally(3, 5);

            switch (State)
            {
                default:
                    break;
                case 0://围绕飞盾转圈圈
                    {
                        if (!Owner.GetProjectileOwner<LeafShieldProj>(out Projectile p))
                        {
                            State = 1;
                            Rand = Main.rand.Next(40);

                            return;
                        }

                        Rand++;
                        Projectile.ChaseGradually(p.Center + (Rand * 0.1f).ToRotationVector2() * 16 * 4, 8f, 9, 10);
                    }
                    break;
                case 1://围绕玩家转圈圈
                    {
                        Vector2 pos = Main.player[Projectile.owner].MountedCenter;
                        Projectile.ChaseGradually(pos + (Rand * 0.03f).ToRotationVector2() * Helper.Clamp(Rand, 0, 80 * Projectile.MaxUpdates) * 0.3f, 8f, 39, 40);

                        Rand++;
                        if (Rand > 80 * Projectile.MaxUpdates)
                            Projectile.Kill();
                    }
                    break;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Main.rand.NextBool(3))
            Projectile.SpawnTrailDust(DustID.Grass, Main.rand.NextFloat(0.2f, 0.4f), Scale: Main.rand.NextFloat(0.6f, 1.2f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle frame = new Rectangle(0, Projectile.frame, 1, 5);

            Projectile.DrawFramedShadowTrails(Color.White with { A=0}, 0.2f, 0.2f / 8, 1, 8, 1, 1, frame, 0);

            Projectile.QuickFrameDraw(frame, lightColor, 0);

            return false;
        }
    }
}
