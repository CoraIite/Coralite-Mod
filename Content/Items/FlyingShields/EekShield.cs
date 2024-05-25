using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.UI.Chat;

namespace Coralite.Content.Items.FlyingShields
{
    public class EekShield : BaseFlyingShieldItem<EekShieldGuard>
    {
        public EekShield() : base(Item.sellPrice(0, 5), ModContent.RarityType<EEKRarity>(), AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<EekShieldProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 15;
            Item.damage = 42;
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.FlyingShieldItems + "EekShieldEffect").Value;
                var frameBox = mainTex.Frame(2, 1, 0, 0);
                var frameBox2 = mainTex.Frame(2, 1, 1, 0);
                var origin = frameBox.Size() / 2;

                Vector2 size = ChatManager.GetStringSize(line.Font, line.Text, line.BaseScale);

                Vector2 pos = new Vector2(line.X, line.Y);
                Vector2 pos2 = new Vector2(line.X + size.X, line.Y);

                for (int i = 0; i < 3; i++)
                {
                    Main.spriteBatch.Draw(mainTex, pos + (Main.GlobalTimeWrappedHourly + i * MathHelper.TwoPi / 3).ToRotationVector2() * 2, frameBox, new Color(255, 100, 100, 0)
                        , 0, origin, 1.2f, 0, 0);
                    Main.spriteBatch.Draw(mainTex, pos2 + (-Main.GlobalTimeWrappedHourly - i * MathHelper.TwoPi / 3).ToRotationVector2() * 2, frameBox2, new Color(255, 100, 100, 0)
                        , 0, origin, 1.2f, 0, 0);
                }

                Main.spriteBatch.Draw(mainTex, pos, frameBox, Color.White, 0, origin, 1.2f, 0, 0);
                Main.spriteBatch.Draw(mainTex, pos2, frameBox2, Color.White, 0, origin, 1.2f, 0, 0);

                Vector2 pos3 = new Vector2(line.X + size.X / 2, line.Y);
                for (int i = -1; i < 2; i += 2)
                {
                    Helper.DrawPrettyStarSparkle(1, 0, pos3 + new Vector2(i * size.X / 4, 15), Color.White,
                        Color.SkyBlue, 0.5f + MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.18f, 0, 0.5f,
                        0.5f, 1, 0, size / size.Y, Vector2.One);
                }
            }

            return true;
        }
    }

    public class EEKRarity : ModRarity
    {
        public override Color RarityColor => Color.Lerp(new Color(214, 57, 40), Color.Red, Math.Abs(MathF.Sin(Main.GlobalTimeWrappedHourly * 3)));
    }

    public class EekShieldProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "EekShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 40;
        }

        public override void SetOtherValues()
        {
            flyingTime = 18;
            backTime = 14;
            backSpeed = 15;
            trailCachesLength = 6;
            trailWidth = 12 / 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (State != (int)FlyingShieldStates.Backing)
            {
                float angle = Main.rand.NextFloat(6.282f);
                for (int i = 0; i < 3; i++)
                {
                    Vector2 dir = (angle + i * MathHelper.TwoPi / 3).ToRotationVector2();
                    Projectile.NewProjectileFromThis<EekShieldEXProj>(target.Center, dir * 5,
                        Projectile.damage / 2, Projectile.knockBack);
                }
            }
        }

        public override Color GetColor(float factor)
        {
            return Color.DarkRed * factor;
        }
    }

    public class EekShieldGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "EekShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 42;
            Projectile.height = 48;
        }

        public override void SetOtherValues()
        {
            scalePercent = 1.4f;
            damageReduce = 0.15f;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            SoundEngine.PlaySound(CoraliteSoundID.GiantTortoise_Zombie33, Projectile.Center);
        }

        public override float GetWidth()
        {
            return Projectile.width / 2 / Projectile.scale;
        }
    }

    public class EekShieldEXProj : ModProjectile
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "EekShield";

        float alpha = 1;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 6);
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 40;
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.SpawnTrailDust(DustID.RedTorch, Main.rand.NextFloat(0.1f, 0.5f), (int)(255 - alpha * 255));
            Projectile.velocity = Projectile.velocity.RotatedBy(MathF.Sin(Projectile.timeLeft * 0.12f) * 0.25f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.timeLeft < 20)
                alpha -= 1 / 20f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.8f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor *= alpha;
            Projectile.DrawShadowTrails(lightColor, 0.5f, 0.5f / 6, 1, 6, 1, extraRot: -1.57f, scale: 0.5f);
            Projectile.QuickDraw(lightColor, 0.5f, -1.57f);
            return false;
        }
    }
}
