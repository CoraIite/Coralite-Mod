using Coralite.Content.GlobalItems;
using Coralite.Content.GlobalProjectiles;
using Coralite.Content.Items.DessertSeries.CommonDots;
using Coralite.Core;
using Coralite.Core.Systems.DotSystem;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.DessertSeries
{
    public class MagicalAshes : ModItem, IDotWeapon
    {
        public override string Texture => AssetDirectory.DessertSeriesItem + Name;

        public override void SetDefaults()
        {
            Item.DefaultToMagicWeapon(ProjectileType<MagicalAshesProj>(), 12, 15f, true);
            Item.SetWeaponValues(12, 6);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.mana = 2;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(silver: 1));
            Item.UseSound = CoraliteSoundID.Swing_Item1;

            CoraliteGlobalItem.SetEdibleDamage(Item);

            Item.reuseDelay = 10;
            Item.noUseGraphic = true;
        }

        public DotTipInfo[] GetDotTips()
            => [new(GetInstance<SucroseDot>().Type, 3f)];

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BambooBlock, 12)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }

    public class MagicalAshesProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        internal ref float Stoped => ref Projectile.ai[0];
        internal ref float Timer => ref Projectile.ai[1];
        internal ref float Hitted => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            CoraliteGlobalProjectile.SetEdibleDamage(Projectile);

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
            if (Stoped >= 3)
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
                            , DustID.AncientLight, -rotDir * Main.rand.NextFromList(1, 2), 0, default, Main.rand.NextFloat(1, 1.5f));
                        dust.noGravity = true;
                    }
                }

                Dust dust3 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4 + 4 * Stoped, 4 + 4 * Stoped)
                    , DustID.AncientLight, -Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(1f) * 2f, 0, default, Main.rand.NextFloat(1, 1.5f));
                dust3.noGravity = true;
                return;
            }

            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4 + 8 * Stoped, 4 + 8 * Stoped)
                    , DustID.AncientLight, -Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(1f) * 2f, 0, default, Main.rand.NextFloat(1, 1.5f));
                dust.noGravity = true;
            }

            Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4 + 8 * Stoped, 4 + 8 * Stoped)
                , DustID.AncientLight, Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(1f) * 2f, 0, default, Main.rand.NextFloat(1, 1.5f));
            dust2.noGravity = true;

            if (Projectile.timeLeft % 3 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4 + 8 * Stoped, 4 + 8 * Stoped)
                   , DustID.AncientLight, -Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(1f) * 2f, 0, default, Main.rand.NextFloat(1, 1.5f));
            }

            Projectile.velocity *= 0.96f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            Timer++;
            if (Timer > 30)
            {
                Timer = 0;
                Stoped++;
            }

            if (Projectile.velocity.Length() < 2f && Stoped < 3)
                FinishFttack();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Hitted == 0)
            {
                Hitted = 1;
                Dot.AddDot<SucroseDot>(target, damageDone * 3, Projectile);
            }

            Projectile.velocity *= 0.5f;
            Stoped++;

            if (Projectile.damage > 7)
                Projectile.damage -= 2;

            if (Stoped == 3)
                FinishFttack();

            for (int i = 0; i < 3; i++)
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32), DustID.AncientLight, -Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(1f), 0, default, Main.rand.Next(1, 3));
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
