using Coralite.Content.DamageClasses;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class CrystalBomb : MagikeChargeableItem, IMagikeCraftable
    {
        public CrystalBomb() : base(500, Item.sellPrice(0, 0, 10)
            , ModContent.RarityType<MagicCrystalRarity>(), -1, AssetDirectory.MagikeSeries1Item)
        { }

        public override bool CanUseItem(Player player)
        {
            return MagikeHelper.TryCosumeMagike(5, Item, player);
        }

        public override void SetDefs()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.UseSound = CoraliteSoundID.Swing_Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<CrystalBombProj>();
            Item.shootSpeed = 14;
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<MagicCrystal, CrystalBomb>(MagikeHelper.CalculateMagikeCost(MALevel.MagicCrystal, 6, 60 * 3), 12)
                .AddIngredient<HardBasalt>(5)
                .AddIngredient(ItemID.RopeCoil)
                .Register();
        }
    }

    public class CrystalBombProj : ModProjectile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + nameof(CrystalBomb);

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.Explosive[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.DamageType = MagikeDamage.Instance;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
            }

            Projectile.rotation += Projectile.direction * 0.15f;

            if (Timer > 20)
                Projectile.velocity *= 0.9f;

            if (Timer > 70 && State == 0)
                Explosion();

            Timer++;

            for (int i = 0; i < 2; i++)
            {
                Vector2 dir = Projectile.rotation.ToRotationVector2();
                Dust d = Dust.NewDustPerfect(Projectile.Center + dir * 8, DustID.CrystalSerpent_Pink, dir.RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(2, 3));
                d.noGravity = true;
            }
        }

        public void Explosion()
        {
            State = 1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.Resize(16 * 7, 16 * 7);

            Point p = Projectile.Center.ToTileCoordinates();
            p -= new Point(3, 3);

            Player player = Main.player[Projectile.owner];

            for (int i = 0; i < 7; i++)
                for (int j = 0; j < 7; j++)
                    player.PickTile(p.X + i, p.Y + j, player.GetBestPickaxe().pick);

            Rectangle rect = new Rectangle(p.X * 16, p.Y * 16, 16 * 7, 16 * 7);

            for (int i = 0; i < 20; i++)
                Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(rect), DustID.CrystalSerpent_Pink);

            Helper.PlayPitched(CoraliteSoundID.LiteBoom_Item118);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (State == 0)
                Explosion();

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDraw(lightColor, 1.57f);
            return false;
        }
    }
}
