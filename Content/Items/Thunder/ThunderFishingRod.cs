﻿using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.Thunder
{
    public class ThunderFishingRod : ModItem
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WoodFishingPole);

            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.fishingPole = 49; // Sets the poles fishing power
            Item.shootSpeed = 15f; // Sets the speed in which the bobbers are launched. Wooden Fishing Pole is 9f and Golden Fishing Rod is 17f.
            Item.shoot = ModContent.ProjectileType<ThunderBobber>(); // The Bobber projectile.
        }

        public override void HoldItem(Player player)
        {
            player.accFishingLine = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float spreadAmount = 75f; // how much the different bobbers are spread out.

            Vector2 bobberSpeed = velocity + new Vector2(Main.rand.NextFloat(-spreadAmount, spreadAmount) * 0.05f, Main.rand.NextFloat(-spreadAmount, spreadAmount) * 0.05f);

            Projectile.NewProjectile(source, position, bobberSpeed, type, 0, 0f, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ZapCrystal>(2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public override void ModifyFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor)
        {
            // Change these two values in order to change the origin of where the line is being drawn.
            // This will make it draw 47 pixels right and 31 pixels up from the player's center, while they are looking right and in normal gravity.
            lineOriginOffset = new Vector2(30, -31);
            // Sets the fishing line's color. Note that this will be overridden by the colored string accessories.
            lineColor = Coralite.Instance.ThunderveinYellow;
        }
    }

    public class ThunderBobber : ModProjectile
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = 61;
            Projectile.bobber = true;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;

            //DrawOriginOffsetY = 4; // Adjusts the draw position
        }

        public override void AI()
        {
            // Always ensure that graphics-related code doesn't run on dedicated servers via this check.
            if (!Main.dedServ)
            {
                Lighting.AddLight(Projectile.Center, Coralite.Instance.ThunderveinYellow.ToVector3() * 1.5f);
            }
        }
    }
}
