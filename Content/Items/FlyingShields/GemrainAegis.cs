using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.FlyingShields
{
    public class GemrainAegis : BaseFlyingShieldItem<GemrainAegisGuard>
    {
        public GemrainAegis() : base(Item.sellPrice(0, 0, 80), ItemRarityID.LightRed, AssetDirectory.FlyingShieldItems)
        { }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<GemrainAegisProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 12;
            Item.damage = 50;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GlazeBulwark>()
                .AddIngredient(ItemID.Ruby)
                .AddIngredient(ItemID.Topaz)
                .AddIngredient(ItemID.Amethyst)
                .AddIngredient(ItemID.Sapphire)
                .AddIngredient(ItemID.Emerald)
                .AddIngredient(ItemID.GelBalloon)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class GemrainAegisProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "GemrainAegis";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 38;
        }

        public override void SetOtherValues()
        {
            flyingTime = 15;
            backTime = 12;
            backSpeed = 12;
            trailCachesLength = 6;
            trailWidth = 18 / 2;
        }

        public override Color GetColor(float factor)
        {
            float x = Main.GlobalTimeWrappedHourly * 0.5f+factor;
            float f = MathF.Truncate(x);
            x -= f;
            Color c = Main.hslToRgb(x, 1, 0.8f) * factor;
            return c;
        }
    }

    public class GemrainAegisGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "GemrainAegis";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 38;
            Projectile.height = 42;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.1f;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 4;
            SoundEngine.PlaySound(CoraliteSoundID.GlassBroken_Shatter, Projectile.Center);

            Vector2 dir = Projectile.rotation.ToRotationVector2();
            for (int i = 0; i < 5; i++)
            {
                Projectile.NewProjectileFromThis<GemrainAegisEXProj>(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                    dir.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(8, 12)
                    , (int)(Projectile.damage * 0.85f), Projectile.knockBack, Main.rand.Next(6));
            }
        }
    }

    public class GemrainAegisEXProj : ModProjectile
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "GemrainAegisProj";

        float alpha = 1;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 20;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 120;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.localAI[0] < 20)
                Projectile.localAI[0] = 20;
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = (int)Projectile.ai[0];
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override void AI()
        {
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] > 60)
            {
                alpha -= 1 / 60f;
            }

            if (Projectile.localAI[0] > 20)
            {
                if (Projectile.velocity.Y < 8)
                    Projectile.velocity.Y += 0.25f;

                Projectile.velocity.X *= 0.96f;
            }
            else
            {
                if (Main.rand.NextBool(2))
                    Projectile.SpawnTrailDust(DustID.RainbowMk2, -0.2f, 0
                        , Main.hslToRgb(Main.rand.NextFloat(0, 1f), 1, 0.6f));
            }

            Projectile.rotation += Projectile.velocity.X / 20;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Glass, Helper.NextVec2Dir(1, 4));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var frameBox = mainTex.Frame(6, 1, Projectile.frame, 0);
            lightColor *= alpha;

            Projectile.DrawShadowTrails(lightColor, 0.5f, 0.5f / 4, 1, 4, 1, 0.05f, frameBox, 0, -1);
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, Color.White * alpha, Projectile.rotation, frameBox.Size() / 2,
                Projectile.scale, 0, 0);

            return false;
        }
    }
}
