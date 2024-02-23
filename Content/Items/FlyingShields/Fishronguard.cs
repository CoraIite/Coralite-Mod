using Coralite.Content.Items.Misc;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.FlyingShields
{
    public class Fishronguard : BaseFlyingShieldItem<FishronguardGuard>
    {
        public Fishronguard() : base(Item.sellPrice(0, 5), ItemRarityID.Yellow, AssetDirectory.FlyingShieldItems)
        { }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 18;
            Item.shoot = ModContent.ProjectileType<FishronguardProj>();
            Item.knockBack = 4.5f;
            Item.shootSpeed = 15;
            Item.damage = 75;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DukeFishronSkin>(3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class FishronguardProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "Fishronguard";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 50;
        }

        public override void OnShootDusts()
        {
            if (Timer % 9 == 0)
            {
                Projectile.NewProjectileFromThis(Projectile.Center, Helper.NextVec2Dir(1, 4), ProjectileID.FlaironBubble,
                    Projectile.damage, Projectile.knockBack, -10);
            }

            for (int i = 0; i < 3; i++)
                Projectile.SpawnTrailDust(32f, Main.rand.NextBool(3) ? DustID.BlueTorch : DustID.Water, Main.rand.NextFloat(0.1f, 0.6f),
                    Scale: Main.rand.NextFloat(1f, 1.4f));
        }

        public override void OnBackDusts()
        {
            if (Timer < 60 && Timer % 12 == 0)
            {
                Projectile.NewProjectileFromThis(Projectile.Center, Helper.NextVec2Dir(1, 4), ProjectileID.FlaironBubble,
                    (int)(Projectile.damage * 0.8f), Projectile.knockBack, -10);
            }

            for (int i = 0; i < 3; i++)
                Projectile.SpawnTrailDust(32f, Main.rand.NextBool(3) ? DustID.BlueTorch : DustID.Water, Main.rand.NextFloat(0.1f, 0.6f),
                    Scale: Main.rand.NextFloat(1f, 1.4f));
        }

        public override void SetOtherValues()
        {
            flyingTime = 24;
            backTime = 12;
            backSpeed = 17;
            trailCachesLength = 9;
            trailWidth = 24 / 2;
        }

        public override Color GetColor(float factor)
        {
            return new Color(39, 100, 104) * factor;
        }
    }

    public class FishronguardGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "Fishronguard";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 50;
            Projectile.height = 50;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.2f;
            distanceAdder = 3;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            SoundEngine.PlaySound(CoraliteSoundID.DukeFishron_NPCHit14, Projectile.Center);
        }
    }
}
