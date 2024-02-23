using Coralite.Content.Items.Materials;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.FlyingShields
{
    public class ShanHai : BaseFlyingShieldItem<ShanHaiGuard>
    {
        public ShanHai() : base(Item.sellPrice(0, 10), ItemRarityID.Cyan, AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<ShanHaiProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 12;
            Item.damage = 140;
            Item.crit = 4;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FragmentsOfLight>()
                .AddIngredient(ItemID.LightShard)
                .AddIngredient(ItemID.DarkShard)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ShanHaiProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "ShanHai";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 50;
        }

        public override void SetOtherValues()
        {
            flyingTime = 20;
            backTime = 30;
            backSpeed = 16;
            trailCachesLength = 8;
        }

        public override Color GetColor(float factor)
        {
            return new Color(28, 90, 144) * factor;
        }
    }

    public class ShanHaiGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "ShanHai";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 50;
            Projectile.height = 58;
            Projectile.scale = 1.25f;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.3f;
            distanceAdder = 2.5f;
            scalePercent = 2f;
        }

        public override void OnGuard()
        {
            base.OnGuard();
            Projectile.NewProjectileFromThis<TaiJi>(Projectile.Center, Projectile.rotation.ToRotationVector2() * 6.5f
                , Projectile.damage, Projectile.knockBack);
        }

        public override float GetWidth()
        {
            return Projectile.width / 2.2f;
        }
    }

    public class TaiJi : ModProjectile
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        float alpha = 0;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.width = Projectile.height = 64;
            Projectile.timeLeft = 32;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 25;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void AI()
        {
            alpha = 0.3f * MathF.Sin(MathHelper.Pi * Projectile.timeLeft / 32f);
            Projectile.rotation += 0.02f + 0.45f * Projectile.timeLeft / 32f;

            if (Projectile.timeLeft > 16)
            {
                Projectile.scale += 0.2f;
                int width = (int)(64 * Projectile.scale);
                Projectile.Resize(width, width);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.8f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null
                , lightColor * alpha, Projectile.rotation - 1.57f, mainTex.Size() / 2, 0.2f * Projectile.scale, 0, 0);
            return false;
        }
    }
}
