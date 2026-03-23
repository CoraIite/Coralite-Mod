using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class Leonids : BaseFlyingShieldItem<LeonidsGuard>
    {
        public Leonids() : base(Item.sellPrice(0, 1, 50), ItemRarityID.LightRed, AssetDirectory.FlyingShieldItems)
        { }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<LeonidsProj>();
            Item.knockBack = 6.5f;
            Item.shootSpeed = 16;
            Item.damage = 50;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MeteorFireball>()
                .AddIngredient(ItemID.TitaniumBar, 12)
                .AddIngredient(ItemID.SoulofFlight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            CreateRecipe()
                .AddIngredient<MeteorFireball>()
                .AddIngredient(ItemID.AdamantiteBar, 12)
                .AddIngredient(ItemID.SoulofFlight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class LeonidsProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "Leonids";

        public Particle chainParticle;
        /// <summary>
        /// 是否能够生成命中特效，放置一次性打到太多怪导致蹦出一万个粒子
        /// </summary>
        public bool canSpawnHitVFX=true;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 42;
        }

        public override void SetOtherValues()
        {
            flyingTime = 16;
            backTime = 22;
            backSpeed = 17;
            trailCachesLength = 6;
            trailWidth = 30 / 2;
        }

        public override void OnShootDusts()
        {
            if (Timer % (flyingTime / 2) == 0)
            {
                //射流星
                Projectile.NewProjectileFromThis<LeonidsMeteor>(Projectile.Center
                    , (Projectile.extraUpdates + 1) * Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.15f, 0.15f)) * Main.rand.NextFloat(0.8f, 1.2f),
                    (int)(Projectile.damage * 0.87f), Projectile.knockBack);
            }

            if (Timer % 4 == 0)
            {
                var p = PRTLoader.NewParticle<StarChain>(Projectile.Center + Main.rand.NextVector2CircularEdge(24, 24), Helper.NextVec2Dir() * Main.rand.NextFloat(0.2f, 0.6f), Color.Cyan, 0.01f);
                if (chainParticle != null)
                    p.ChainedParticle = chainParticle;

                p.Alpha = 0.9f;
                p.TargetScale = 0.8f;
                p.ShineTime = 4;
                p.FadeTime = 6;
                p.LineWidth = 20;
                chainParticle = p;
            }
        }

        public override void JumpInNpcs()
        {
            canSpawnHitVFX = true;

            base.JumpInNpcs();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (canSpawnHitVFX && State != (int)FlyingShieldStates.Backing&&VisualEffectSystem.HitEffect_SpecialParticles)
            {
                canSpawnHitVFX = false;

                Vector2 center = Main.rand.NextVector2FromRectangle(target.getRect());
                var p = PRTLoader.NewParticle<StarChain>(center, Vector2.Zero, Color.Cyan, 0.01f);

                p.Alpha = 0.9f;
                p.TargetScale =0.7f;
                p.ShineTime = 1;
                p.FadeTime = 12;
                p.LineWidth = 20;

                int count = Main.rand.Next(3, 6);
                for (int i = 0; i < count; i++)
                {
                    var p2 = PRTLoader.NewParticle<StarChain>(center, Helper.NextVec2Dir() * Main.rand.NextFloat(1, 4), Color.Cyan, 0.01f);

                    p2.Alpha = 0.8f;
                    p2.TargetScale = 0.5f;
                    p2.ShineTime = 4;
                    p2.FadeTime = 7;
                    p2.LineWidth = 32;
                    p2.ChainedParticle = p;
                }
            }
            base.OnHitNPC(target, hit, damageDone);
        }

        public override Color GetColor(float factor)
        {
            return new Color(32, 180, 186, 0) * factor;
        }
    }

    public class LeonidsGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 42;
            Projectile.height = 48;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.15f;
        }

        public override void DrawSelf(Texture2D mainTex, Vector2 pos, float rotation, Color lightColor, Vector2 scale, SpriteEffects effect)
        {
            Rectangle frameBox;
            Vector2 rotDir = Projectile.rotation.ToRotationVector2();
            Vector2 dir = rotDir * (DistanceToOwner / (Projectile.width * scalePercent));
            Color c = lightColor * 0.6f;
            c.A = lightColor.A;

            frameBox = mainTex.Frame(2, 1, 0, 0);
            Vector2 origin2 = frameBox.Size() / 2;

            //绘制基底
            Main.spriteBatch.Draw(mainTex, pos - (dir * 5), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上部
            frameBox = mainTex.Frame(2, 1, 1, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 3), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 7), frameBox, lightColor, rotation, origin2, scale, effect, 0);
        }
    }

    public class LeonidsMeteor : BaseHeldProj
    {
        public override string Texture => "Terraria/Images/Projectile_16";

        public const int trailCachesLength = 6;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.timeLeft = 40;
        }

        public override void Initialize()
        {
            Projectile.InitOldPosCache(trailCachesLength);
            Projectile.InitOldRotCache(trailCachesLength);
        }

        public override void AI()
        {
            if (VaultUtils.isServer)
                return;

            Lighting.AddLight(Projectile.Center, Coralite.IcicleCyan.ToVector3());

            Projectile.UpdateOldPosCache(addVelocity: true);
            Projectile.UpdateOldRotCache();

            Projectile.SpawnTrailDust(8f, DustID.Clentaminator_Cyan, -Main.rand.NextFloat(0.1f, 0.4f), Scale: Main.rand.NextFloat(0.6f, 0.8f));
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = (i * MathHelper.PiOver2).ToRotationVector2();
                for (int j = 0; j < 6; j++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Clentaminator_Cyan, dir * (1 + (j * 0.8f)), Scale: 1.6f - (j * 0.15f));
                    d.noGravity = true;
                }
            }

            SoundEngine.PlaySound(CoraliteSoundID.Hit_Item10, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrails();
            Texture2D mainTex = Projectile.GetTextureValue();
            Color c = Color.White;
            c.A = 0;

            var pos = Projectile.Center - Main.screenPosition;
            var origin = mainTex.Size() / 2;

            Main.spriteBatch.Draw(mainTex, pos, null, c, 0, origin, 0.7f, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, null, c * 0.7f, MathHelper.PiOver4, origin, 0.5f, 0, 0);
            return false;
        }

        public virtual void DrawTrails()
        {
            Texture2D Texture = CoraliteAssets.Trail.CircleA.Value;

            List<ColoredVertex> bars = new();

            for (int i = 0; i < trailCachesLength; i++)
            {
                float factor = (float)i / trailCachesLength;
                Vector2 Center = Projectile.oldPos[i] - Main.screenPosition;
                Vector2 normal = (Projectile.oldRot[i] + MathHelper.PiOver2).ToRotationVector2();
                Vector2 Top = Center + (normal * 5 * factor);
                Vector2 Bottom = Center - (normal * 5 * factor);

                var Color = new Color(20, 255, 199, 0) * factor;
                bars.Add(new(Top, Color, new Vector3(factor, 0, 1)));
                bars.Add(new(Bottom, Color, new Vector3(factor, 1, 1)));
            }

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            //Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
        }
    }
}
