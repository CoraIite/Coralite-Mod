using Coralite.Content.Raritys;
using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class MagicCrystalFishingRod : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WoodFishingPole);

            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.fishingPole = 18; // Sets the poles fishing power
            Item.shootSpeed = 12f; // Sets the speed in which the bobbers are launched. Wooden Fishing Pole is 9f and Golden Fishing Rod is 17f.
            Item.shoot = ModContent.ProjectileType<MagicCrystalBobber>(); // The Bobber projectile.
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystal>(8)
                .AddIngredient<Basalt>(12)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void ModifyFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor)
        {
            lineOriginOffset = new Vector2(40, -41);
            // Sets the fishing line's color. Note that this will be overridden by the colored string accessories.
            lineColor = Coralite.Instance.MagicCrystalPink;
        }
    }

    public class MagicCrystalBobber : ModProjectile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = 61;
            Projectile.bobber = true;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            if (!Main.dedServ)
            {
                Lighting.AddLight(Projectile.Center, Coralite.Instance.MagicCrystalPink.ToVector3());
            }
        }
    }

}
