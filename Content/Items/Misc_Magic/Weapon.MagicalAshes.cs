using Coralite.Core;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Misc_Magic
{
    public class MagicalAshes : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Magic + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 7;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.reuseDelay = 10;
            Item.mana = 3;
            Item.knockBack = 6;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.sellPrice(0, 0, 1);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ProjectileType<MagicalAshesProj>();
            Item.shootSpeed = 15f;
            Item.UseSound = CoraliteSoundID.Swing_Item1;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BambooBlock, 20)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }

    public class MagicalAshesProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        internal ref float Stoped => ref Projectile.ai[0];
        internal ref float Timer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.penetrate = 3;
            Projectile.aiStyle = -1;

            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (Stoped == 3)
            {
                Timer++;

                for (int i = -1; i < 2; i += 2)
                {
                    Vector2 rotDir = (Projectile.rotation - i * MathHelper.PiOver2 + i * Timer * 0.15f).ToRotationVector2();
                    Vector2 pos = Projectile.Center - Projectile.rotation.ToRotationVector2() * 4
                        + (Projectile.rotation + i * MathHelper.PiOver2).ToRotationVector2() * 16
                        + rotDir * 16;
                    for (int j = 0; j < 2; j++)
                    {
                        Dust dust = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(14, 14)
                            , 261, -rotDir * Main.rand.NextFromList(1, 2), 0, default, Main.rand.NextFloat(1, 1.5f));
                        dust.noGravity = true;
                    }
                }

                Dust dust3 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4 + 4 * Stoped, 4 + 4 * Stoped)
                    , 261, -Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(1f) * 2f, 0, default, Main.rand.NextFloat(1, 1.5f));
                dust3.noGravity = true;
                return;
            }

            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4 + 8 * Stoped, 4 + 8 * Stoped)
                    , 261, -Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(1f) * 2f, 0, default, Main.rand.NextFloat(1, 1.5f));
                dust.noGravity = true;
            }

            Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4 + 8 * Stoped, 4 + 8 * Stoped)
                , 261, Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(1f) * 2f, 0, default, Main.rand.NextFloat(1, 1.5f));
            dust2.noGravity = true;

            if (Projectile.timeLeft % 3 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4 + 8 * Stoped, 4 + 8 * Stoped)
                   , 261, -Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(1f) * 2f, 0, default, Main.rand.NextFloat(1, 1.5f));
            }

            Projectile.velocity *= 0.94f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            Timer++;
            if (Timer > 20)
            {
                Timer = 0;
                Stoped++;
            }

            if (Projectile.velocity.Length() < 2f && Stoped < 3)
                FinishFttack();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity *= 0.8f;
            Stoped++;

            if (Stoped == 3)
                FinishFttack();

            for (int i = 0; i < 3; i++)
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32), 261, -Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(1f), 0, default, Main.rand.Next(1, 3));
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity;
            FinishFttack();
            return false;
        }

        public void FinishFttack()
        {
            Stoped = 3;
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= 0;
            Projectile.Resize(60, 60);
            Projectile.timeLeft = 40;
            Projectile.extraUpdates = 1;
            Timer = 0;
        }
    }
}
