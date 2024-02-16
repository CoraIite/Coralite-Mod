using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Terraria.Audio;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;

namespace Coralite.Content.Items.FlyingShields
{
    public class TortoiseshellFortress : BaseFlyingShieldItem<TortoiseshellFortressGuard>
    {
        public TortoiseshellFortress() : base(Item.sellPrice(0, 2, 0, 0), ItemRarityID.Lime, AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 20;
            Item.shoot = ModContent.ProjectileType<TortoiseshellFortressProj>();
            Item.knockBack = 6;
            Item.shootSpeed = 10;
            Item.damage = 100;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TurtleShell, 2)
                .AddIngredient(ItemID.ChlorophyteBar, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class TortoiseshellFortressProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "TortoiseshellFortress";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 40;
        }

        public override void SetOtherValues()
        {
            flyingTime = 15;
            backTime = 12;
            backSpeed = 12;
            trailCachesLength = 9;
            trailWidth = 34 / 2;
        }

        public override void OnShootDusts()
        {
            extraRotation += 0.4f;
        }

        public override void OnBackDusts()
        {
            extraRotation += 0.4f;
        }

        public override void DrawTrails(Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var origin = mainTex.Size() / 2;
            for (int i = trailCachesLength - 1; i > 4; i--)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, null,
                lightColor * 0.6f * ((i - 4) * 1 / 3f), Projectile.oldRot[i] - 1.57f + extraRotation, origin, Projectile.scale, 0, 0);

            base.DrawTrails(lightColor);
        }

        public override Color GetColor(float factor)
        {
            return new Color(114, 84, 66) * factor;
        }
    }

    public class TortoiseshellFortressGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "TortoiseshellFortress";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 40;
            Projectile.height = 42;
            Projectile.scale = 1.5f;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.6f;
            StrongGuard += 0.15f;
            distanceAdder = 1;
            scalePercent = 1.6f;
        }

        public override void OnHoldShield()
        {
            Owner.velocity.X *= 0.9f;
            Owner.wingTime = 0;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 2;
            SoundEngine.PlaySound(CoraliteSoundID.GiantTortoise_Zombie33, Projectile.Center);
        }

        public override float GetWidth()
        {
            return Projectile.width/2;
        }
    }
}
