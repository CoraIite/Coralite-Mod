using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Weapons
{
    public class WateringPot:ModItem
    {
        public override string Texture => AssetDirectory.Weapons_Melee+Name;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 34;
            Item.useTime = 10;
            Item.damage = 13;
            Item.shootSpeed = 10f;
            Item.knockBack = 4f;
            Item.useAnimation = 10;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item19;
            Item.shoot = ModContent.ProjectileType<WateringPotProj>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public override bool CanUseItem(Player Player)
        {
            for (int k = 0; k <= Main.maxProjectiles; k++)
                if (Main.projectile[k].active && Main.projectile[k].owner == Player.whoAmI && Main.projectile[k].type == ModContent.ProjectileType<WateringPotProj>())
                    return false;

            return base.CanUseItem(Player);
        }
    }

    public class WateringPotProj:ModProjectile
    {
        public override string Texture => AssetDirectory.Weapons_Melee + "WateringPot";

        public override void SetDefaults()
        {
            
        }

        public override void AI()
        {
            Projectile.rotation += 0.3f;


        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
    }
}
