using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class GemrainAegis : BaseFlyingShieldItem<GemrainAegisGuard>
    {
        public GemrainAegis() : base(Item.sellPrice(0, 0, 80), ItemRarityID.LightRed, AssetDirectory.FlyingShieldItems)
        { }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 27;
            Item.shoot = ModContent.ProjectileType<GemrainAegisProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 15;
            Item.damage = 54;
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

        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 38;
        }

        public override void SetOtherValues()
        {
            flyingTime = 20;
            backTime = 12;
            backSpeed = 15.5f;
            trailCachesLength = 8;
            trailWidth = 32 / 2;
        }

        public override void AI()
        {
            base.AI();
        }

        public override Color GetColor(float factor)
        {
            float x = (Main.GlobalTimeWrappedHourly * 0.5f) + factor;
            float f = MathF.Truncate(x);
            x -= f;
            Color c = Main.hslToRgb(x, 1, 0.8f) * factor;
            return c;
        }

        public void BrokenSound()
            => Helper.PlayPitched(CoraliteSoundID.GlassBroken_Shatter, Projectile.Center, volume: 0.2f);

        public void Broken(Vector2 dir)
        {
            BrokenSound();

            int damage = (int)(Projectile.damage * 0.45f);
            for (int i = 0; i < 4; i++)
            {
                Projectile.NewProjectileFromThis<GemrainAegisEXProj>(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                    dir.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(8, 14), damage, Projectile.knockBack);
            }

            Projectile.Kill();

            int count = Main.rand.Next(3, 6);
            for (int i = 0; i < count; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Glass, Helper.NextVec2Dir(1, 4));
            }
        }

        public override void TurnToBack()
        {
            Broken(Projectile.velocity.SafeNormalize(Vector2.Zero));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Broken(Projectile.velocity.SafeNormalize(Vector2.Zero));
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Broken(-oldVelocity.SafeNormalize(Vector2.Zero));

            return true;
        }

        public override ATex GetTrailTex()
            => CoraliteAssets.Laser.EnergyFlowSPA;

        public override void DrawTrails(Color lightColor)
        {
            Texture2D Texture = GetTrailTex().Value;

            List<ColoredVertex> bars = [];
            float r = 0.2989f * lightColor.R / 255 + 0.5870f * lightColor.G / 255 + 0.1140f * lightColor.B / 255;

            for (int i = 0; i < trailCachesLength; i++)
            {
                float factor = (float)i / trailCachesLength;
                Vector2 Center = Projectile.oldPos[i] - Main.screenPosition;
                Vector2 normal = (Projectile.oldRot[i] + MathHelper.PiOver2).ToRotationVector2();
                Vector2 Top = Center + (normal * trailWidth* factor);
                Vector2 Bottom = Center - (normal * trailWidth* factor);

                var color = GetColor(factor) * r;
                float u = factor + (float)Main.timeForVisualEffects * 0.05f;
                bars.Add(new(Top, color, new Vector3(u, 0, 1)));
                bars.Add(new(Bottom, color, new Vector3(u, 1, 1)));
            }

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            var s = Main.graphics.GraphicsDevice.SamplerStates[0];
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            Main.graphics.GraphicsDevice.SamplerStates[0] = s;

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
            Projectile.scale = 1.2f;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.25f;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 4;
            SoundEngine.PlaySound(CoraliteSoundID.GlassBroken_Shatter, Projectile.Center);
            int damage = (int)(Projectile.damage * 0.75f);
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            for (int i = 0; i < 5; i++)
            {
                Projectile.NewProjectileFromThis<GemrainAegisEXProj>(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                    dir.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(8, 12)
                    , damage, Projectile.knockBack);
            }
        }
    }

    public class GemrainAegisEXProj : ModProjectile
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "GemrainAegisProj";

        float alpha = 1;
        private bool span;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 4);
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 20;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 120;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.localAI[0] < 20)
                Projectile.localAI[0] = 20;
            return false;
        }

        public void Initialize()
        {
            Projectile.frame = Main.rand.Next(6);
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override void AI()
        {
            if (!span)
            {
                Initialize();
                span = true;
            }
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] > 60)
            {
                alpha -= 1 / 60f;
            }

            if (Projectile.localAI[0] > 20)
            {
                if (Projectile.shimmerWet)
                {
                    if (Projectile.velocity.Y > -12)
                        Projectile.velocity.Y -= 0.9f;
                }
                else if (Projectile.velocity.Y < 8)
                    Projectile.velocity.Y += 0.25f;

                Projectile.velocity.X *= 0.96f;
            }
            else
            {
                if (Main.rand.NextBool(2))
                    Projectile.SpawnTrailDust(DustID.RainbowMk2, -0.2f, 0
                        , Main.hslToRgb(Main.rand.NextFloat(0, 1f), 1, 0.8f));
            }

            Projectile.rotation += Projectile.velocity.X / 20;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.9f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTextureValue();
            var frameBox = mainTex.Frame(6, 1, Projectile.frame, 0);
            lightColor *= alpha;

            Projectile.DrawShadowTrails(lightColor, 0.5f, 0.5f / 4, 1, 4, 1, 0.05f, frameBox, 0, -1);

            var box = new Rectangle(Projectile.frame, 0, 6, 1);
            Projectile.QuickFrameDraw(box, lightColor, 0);
            Projectile.QuickFrameDraw(box, Color.White * 0.4f * alpha, 0);

            return false;
        }
    }
}
