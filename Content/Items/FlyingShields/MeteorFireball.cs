using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class MeteorFireball : BaseFlyingShieldItem<MeteorFireballGuard>
    {
        public MeteorFireball() : base(Item.sellPrice(0, 1, 50), ItemRarityID.Orange, AssetDirectory.FlyingShieldItems)
        { }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 17;
            Item.shoot = ModContent.ProjectileType<MeteorFireballProj>();
            Item.knockBack = 4.5f;
            Item.shootSpeed = 15;
            Item.damage = 22;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FallenStar, 10)
                .AddIngredient(ItemID.SunplateBlock, 5)
                .AddTile(TileID.SkyMill)
                .Register();
        }
    }

    public class MeteorFireballProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "MeteorFireball";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 38;
        }

        public override void SetOtherValues()
        {
            flyingTime = 13;
            backTime = 13;
            backSpeed = 17;
            trailCachesLength = 6;
            trailWidth = 16 / 2;
        }

        public override void OnShootDusts()
        {
            if (Timer == (flyingTime * 2 / 3))
            {
                //射流星
                Projectile.NewProjectileFromThis<MeteorFireballMeteor>(Projectile.Center
                    , Projectile.velocity * 1.5f, Projectile.damage / 2, Projectile.knockBack);
            }
        }

        public override Color GetColor(float factor)
        {
            return Color.Silver with { A = 0 } * factor;
        }
    }

    public class MeteorFireballGuard : BaseFlyingShieldGuard
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

        public override float GetWidth()
        {
            return Projectile.width / 2.2f;
        }

        public override void DrawSelf(Texture2D mainTex, Vector2 pos, float rotation, Color lightColor, Vector2 scale, SpriteEffects effect)
        {
            Rectangle frameBox;
            Vector2 rotDir = Projectile.rotation.ToRotationVector2();
            Vector2 dir = rotDir * (DistanceToOwner / (Projectile.width * scalePercent));
            Color c = lightColor * 0.6f;
            c.A = lightColor.A;

            frameBox = mainTex.Frame(3, 1, 0, 0);
            Vector2 origin2 = frameBox.Size() / 2;

            //绘制基底
            Main.spriteBatch.Draw(mainTex, pos - (dir * 5), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上部
            frameBox = mainTex.Frame(3, 1, 1, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 3), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 7), frameBox, lightColor, rotation, origin2, scale, effect, 0);
            //绘制上部
            frameBox = mainTex.Frame(3, 1, 2, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 7), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 12), frameBox, lightColor, rotation, origin2, scale, effect, 0);
        }
    }

    public class MeteorFireballMeteor : BaseHeldProj
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public const int trailCachesLength = 8;

        public ref float Timer => ref Projectile.ai[0];
        public ref float Scale => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.timeLeft = 40;
            Projectile.tileCollide = false;
        }

        public override void Initialize()
        {
            Projectile.InitOldPosCache(trailCachesLength);
            Scale = 1;
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Red.ToVector3());

            if (Timer < 20)
            {
                Projectile.SpawnTrailDust(8f, DustID.RedStarfish, -Main.rand.NextFloat(0.1f, 0.4f), Scale: Main.rand.NextFloat(0.6f, 0.8f));
                if (Main.rand.NextBool(3))
                {
                    PRTLoader.NewParticle<MeteorFireballParticle>(Projectile.Center + Main.rand.NextVector2Circular(4, 4)
                        , Projectile.velocity.RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(0.1f, -0.2f), Color.White
                        , Main.rand.NextFloat(0.1f, 0.2f));
                }
            }

            if (Timer > 10 && Timer < 30)
            {
                Projectile.velocity *= 0.9f;
                float factor = (Timer - 10) / 20;
                Scale = 0.2f + 0.8f * Helper.HeavyEase(1 - factor);
            }
            else if (Timer > 30)
            {
                Projectile.Kill();
            }

            Timer++;
            Projectile.rotation += Projectile.velocity.Length() / 20 * MathF.Sign(Projectile.velocity.X);

            Projectile.UpdateOldPosCache(addVelocity: true);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                float t = i / 15f * MathHelper.TwoPi * 3;
                float x = 2 * MathF.Cos(t) + 5 * MathF.Cos(2 * t / 3);
                float y = 2 * MathF.Sin(t) - 5 * MathF.Sin(2 * t / 3);
                Vector2 dir = new Vector2(x, y).RotatedBy(Projectile.rotation) * 0.2f;
                if (i % 3 == 0)
                {
                    PRTLoader.NewParticle<MeteorFireballParticle>(Projectile.Center, dir, Color.White
                        , Main.rand.NextFloat(0.1f, 0.2f));
                }
                else
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.RedStarfish, dir, Scale: Main.rand.NextFloat(0.8f, 1.2f));
                    d.noGravity = true;
                }
            }

            SoundEngine.PlaySound(CoraliteSoundID.Hit_Item10, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrails();
            Texture2D mainTex = Projectile.GetTexture();
            Color c = Color.White;

            var pos = Projectile.Center - Main.screenPosition;

            mainTex.QuickCenteredDraw(Main.spriteBatch, pos, c, Projectile.rotation, Scale);
            return false;
        }

        public virtual void DrawTrails()
        {
            Texture2D Texture = CoraliteAssets.Trail.CircleA.Value;

            List<ColoredVertex> bars = new();

            float w = 5 * Scale;
            Vector2 normal = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(1.57f);
            for (int i = 0; i < trailCachesLength; i++)
            {
                float factor = (float)i / trailCachesLength;
                Vector2 Center = Projectile.oldPos[i];
                Vector2 Top = Center - Main.screenPosition + (normal * w * factor);
                Vector2 Bottom = Center - Main.screenPosition - (normal * w * factor);

                var color = Color.Red * factor;
                bars.Add(new(Top, color, new Vector3(factor, 0, 1)));
                bars.Add(new(Bottom, color, new Vector3(factor, 1, 1)));
            }

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
        }
    }

    public class MeteorFireballParticle : Particle
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "MeteorFireballMeteor";

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
            Rotation = Main.rand.NextFloat(6.282f);
        }

        public override void AI()
        {
            Opacity++;
            if (Opacity < 4)
            {
                Scale *= 1.4f;
            }

            if (Opacity > 4 + 8)
            {
                Scale *= 0.9f;
                Velocity *= 0.8f;
                Color *= 0.8f;
            }

            Rotation += Math.Sign(Velocity.X) * Velocity.Length() / 14;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = TexValue;

            Vector2 position = Position - Main.screenPosition;
            spriteBatch.Draw(mainTex, position, null, Color, Rotation, mainTex.Size() / 2
                , Scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
