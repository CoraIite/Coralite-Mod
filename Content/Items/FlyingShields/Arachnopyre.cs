using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class Arachnopyre : BaseFlyingShieldItem<ArachnopyreGuard>
    {
        public Arachnopyre() : base(Item.sellPrice(0, 0, 0, 10), ItemRarityID.LightRed, AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<ArachnopyreProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 13;
            Item.damage = 30;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpiderFang, 12)
                .AddIngredient(ItemID.LivingFireBlock, 30)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ArachnopyreProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "Arachnopyre";

        private bool canShootSpider=true;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 40;
        }

        public override void SetOtherValues()
        {
            flyingTime = 20;
            backTime = 4;
            backSpeed = 15;
            trailCachesLength = 7;
            trailWidth = 30 / 2;
        }

        public override Color GetColor(float factor)
        {
            return new Color(82, 65, 65) * factor;
        }

        public override void Shooting()
        {
            Chasing();
            if (firstShoot && Timer >= flyingTime - 6)
            {
                Projectile.tileCollide = false;
                if (Timer == flyingTime - 6)
                {
                    firstShoot = false;
                    Projectile.tileCollide = recordTileCollide;
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

            Tile tile = Framing.GetTileSafely(Projectile.Center);
            if (tile.WallType != 0)
            {
                Timer -= 0.5f;
            }
            else
                Timer--;

            if (Timer < 0)
                TurnToBack();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (canShootSpider)
            {
                canShootSpider = false;

            }
            base.OnHitNPC(target, hit, damageDone);
        }
    }

    public class ArachnopyreGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "Arachnopyre";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 42;
            Projectile.height = 42;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.2f;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            SoundEngine.PlaySound(CoraliteSoundID.Spider_NPCHit29, Projectile.Center);
        }
    }

    public class ArachnopyreSpider:ModProjectile
    {

    }
}
