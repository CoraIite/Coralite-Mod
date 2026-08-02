using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Steel
{
    public class B9Bullet : ModItem
    {
        public override string Texture => AssetDirectory.SteelItems + Name;

        public override void SetDefaults()
        {
            Item.ammo = AmmoID.Bullet;
            Item.damage = 9;
            Item.knockBack = 3f;
            Item.maxStack = Item.CommonMaxStack;
            Item.shootSpeed = 4.5f;
            Item.consumable = true;

            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 0, 0, 5);
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<B9BulletProj>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(300)
                .AddIngredient<B9Alloy>()
                .AddIngredient(ItemID.SoulofLight)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class B9BulletProj : ModProjectile
    {
        public override string Texture => AssetDirectory.SteelItems + nameof(B9Bullet);

        public ref float Target => ref Projectile.ai[0];
        public ref float RandTime => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Bullet);
            Projectile.aiStyle = -1;
            Projectile.alpha = 0;
        }

        public override bool? CanDamage()
        {
            if (Target != -1)
            {
                return false;
            }

            return null;
        }

        public override void AI()
        {
            if (Projectile.alpha < 255)
            {
                Projectile.alpha += 20;
                if (Projectile.alpha > 255)
                    Projectile.alpha = 255;
            }

            if (Projectile.IsOwnedByLocalPlayer() && Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Target = -1;
                RandTime = 15 + Main.rand.Next(10) * 2;
                Projectile.netUpdate = true;
                return;
            }

            Timer++;

            if (Target.GetNPCOwner(out NPC target, () =>
            {
                if (Target != -1)
                    Projectile.Kill();
            }))
            {
                Projectile.velocity *= 0.93f;
                Projectile.rotation = Projectile.rotation.AngleLerp((target.Center - Projectile.Center).ToRotation(), 0.4f * Helper.Clamp(Timer / 10, 0, 1));

                if (Timer > 30)
                {
                    Projectile.Kill();

                    Vector2 dir = Projectile.rotation.ToRotationVector2();
                    for (int i = 0; i < 6; i++)
                    {
                      Dust d=  Dust.NewDustPerfect(Projectile.Center, DustID.TheDestroyer, dir.RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(1f, 6f), 255, Color.White, Scale: Main.rand.NextFloat(1, 1.5f));
                        d.noGravity = true;
                    }

                    Projectile.NewProjectileFromThis<B9Laser>(Projectile.Center, Projectile.rotation.ToRotationVector2() * 16, Projectile.damage, Projectile.knockBack);
                }

                return;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();


            if (Timer > RandTime)
            {
                Timer = 0;
                if (Helper.TryFindClosestEnemy(Projectile.Center, 1000, n => n.CanBeChasedBy() && Collision.CanHit(Projectile, n), out NPC tar))
                {
                    Target = tar.whoAmI;
                    Timer = 0;
                    Projectile.tileCollide = false;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            float rot = Projectile.rotation + Main.rand.NextFloat(-0.2f, 0.2f);
           Dust d= Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<B9BulletShell>(), -rot.ToRotationVector2() * Main.rand.NextFloat(4, 14), 0, Color.White);

            d.rotation = Projectile.rotation;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDraw(Color.White * (Projectile.alpha / 255f), MathHelper.PiOver2);

            return false;
        }
    }

    public class B9Laser : ModProjectile
    {
        public override string Texture => AssetDirectory.SteelItems + Name;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Bullet);
            Projectile.aiStyle = -1;
            Projectile.alpha = 0;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            if (Projectile.alpha < 255)
            {
                Projectile.alpha += 20;
                if (Projectile.alpha > 255)
                    Projectile.alpha = 255;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Main.rand.NextBool(6))
            {
                Projectile.SpawnTrailDust(DustID.TheDestroyer, Main.rand.NextFloat(0.1f, 0.2f),255,Color.White, Scale: Main.rand.NextFloat(0.7f, 1.2f));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTextureValue();

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * (Projectile.alpha / 255f), Projectile.rotation, new Vector2(tex.Width, tex.Height / 2), Projectile.scale, 0, 0);

            return false;
        }
    }

    public class B9BulletShell : ModDust
    {
        public override string Texture => AssetDirectory.SteelItems+Name;

        public override void OnSpawn(Dust dust)
        {
            dust.scale = 1;
            dust.alpha = 255;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;

            if (dust.velocity.Y < 8)
            {
                dust.velocity.Y += 0.5f;
            }

            dust.velocity.X *= 0.98f;

            dust.rotation += MathF.Sign(dust.velocity.X) * dust.velocity.Length() / 20;

            dust.fadeIn++;
            dust.alpha = (int)(255 *Helper.SqrtEase( 1 - dust.fadeIn / 35f));

            if (Collision.SolidCollision(dust.position - (Vector2.One * 3f), 6, 6))
            {
                dust.velocity *= 0.25f;
            }

            if (dust.fadeIn > 35)
            {
                dust.active = false;
            }

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Texture2D.Value.QuickCenteredDraw(Main.spriteBatch, dust.position - Main.screenPosition, Lighting.GetColor(dust.position.ToTileCoordinates(), dust.color) * (dust.alpha / 255f)*0.8f, dust.rotation, dust.scale);

            return false;
        }
    }
}
