using Coralite.Content.DamageClasses;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Lasso
{
    public class HallowYoke : BaseFairyCatcher
    {
        public override string Texture => AssetDirectory.FairyCatcherLasso + Name;

        public override int CatchPower => 30;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<HallowYokeSwing>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 25;
            Item.shootSpeed = 9;
            Item.SetWeaponValues(25, 3);
            Item.SetShopValues(ItemRarityColor.Pink5, Item.sellPrice(0, 3));
            Item.autoReuse = true;
        }
    }

    [VaultLoaden(AssetDirectory.FairyCatcherLasso)]
    public class HallowYokeSwing() : BaseLassoSwing(4)
    {
        public override string Texture => AssetDirectory.FairyCatcherLasso + "HallowYokeCatcher";

        public override int HowManyPerShoot => 3;

        public static ATex HallowChain { get; set; }

        public override float HandleDrawOffset => 8;

        public override float GetShootRandAngle => Main.rand.NextFloat(-0.1f, 0.1f);

        public override void SetSwingProperty()
        {
            minDistance = 52;
            delayTime = 18;
            shootTime = 10;

            base.SetSwingProperty();

            Projectile.width = Projectile.height = 40;
            Projectile.extraUpdates = 4;
            DrawOriginOffsetX = 8;
        }

        public override void SetMaxDistance()
        {
            MaxDistance = 210;
        }

        public override void OnAttackSwing()
        {
            SpawnDust();
        }

        public override void OnAttackBack()
        {
            SpawnDust();
        }

        private void SpawnDust()
        {
            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24), DustID.HallowedWeapons
                    , Helper.NextVec2Dir(1, 2));
                d.noGravity = true;
            }
        }

        public override void OnShootFairy()
        {
            base.OnShootFairy();
            if (Catch == 0 && Collision.CanHit(Owner.Center, 1, 1, Projectile.Center, 1, 1))
            {
                Projectile.NewProjectileFromThis<HallowYokeSlash>(Projectile.Center,
                    (Projectile.Center - Owner.Center).SafeNormalize(Vector2.Zero) * 8, Projectile.damage / 2, 1);

                Helper.PlayPitched(CoraliteSoundID.StarFalling_Item105, Projectile.Center, pitchAdjust: -0.2f,volumeAdjust:-0.4f);
            }
        }

        public override Texture2D GetStringTex() => HallowChain.Value;
        public override Color GetStringColor(Vector2 pos) => Lighting.GetColor((int)pos.X / 16, (int)(pos.Y / 16f));
    }

    [VaultLoaden(AssetDirectory.FairyCatcherLasso)]
    public class HallowYokeSlash:ModProjectile
    {
        public override string Texture => AssetDirectory.FairyCatcherLasso + "HallowYokeCatcher";

        public static ATex HallowSword { get; set; }

        public ref float Timer => ref Projectile.ai[0];
        public ref float Rot => ref Projectile.ai[1];

        public ref float SwordScale => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.tileCollide = true;

            Projectile.DamageType = FairyDamage.Instance;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Timer < 10)
            {
                return false;
            }

            float rot = Rot;

            for (int i = 0; i < 4; i++)
            {
                Vector2 pos = Projectile.Center + rot.ToRotationVector2() * 60;

                if (targetHitbox.Intersects(Utils.CenteredRectangle(pos, Projectile.Size)))
                    return true;

                rot += MathHelper.PiOver2;
            }

            return false;
        }

        public override void AI()
        {
            if (Timer == 0)
                Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);

            //旋转向前
            const int preTime = 26;
            const int postTime = 30;

            Rot += Projectile.spriteDirection * MathF.Sin(Timer / (postTime + preTime) * MathHelper.Pi) * 0.3f;


            if (Timer < 10)
                SwordScale = Helper.HeavyEase(Timer / 10);
            else if (Timer > preTime)
            {
                Projectile.velocity *= 0.9f;
                SwordScale = Helper.HeavyEase(1 - (Timer - preTime) / postTime);
            }

            if (Timer > postTime + preTime)
                Projectile.Kill();

            SpawnDust();

            Lighting.AddLight(Projectile.Center, (Color.Gold * 0.5f).ToVector3());

            Timer++;
        }

        public void SpawnDust()
        {
            float rot = Rot;

            for (int i = 0; i < 4; i++)
            {
                Vector2 pos = Projectile.Center + rot.ToRotationVector2() * 60 * SwordScale;
                if (Main.rand.NextBool())
                {
                    Dust d = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(16, 16), DustID.HallowedWeapons
                        , (rot - MathHelper.PiOver2).ToRotationVector2() * Main.rand.NextFloat(1, 3));
                    d.noGravity = true;

                }
                rot += MathHelper.PiOver2;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Timer += 2;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.99f);
            Projectile.velocity *= 0.6f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Projectile.GetTexture().QuickCenteredDraw(Main.spriteBatch, pos, lightColor, Rot);

            Texture2D swordTex = HallowSword.Value;
            Texture2D lightShotSPA = CoraliteAssets.Trail.LightShotSPA.Value;

            float rot = Rot;
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = rot.ToRotationVector2();
                Vector2 SwordPos = Projectile.Center + dir * (6 + SwordScale * 10)-Main.screenPosition;
                Vector2 lightPos = Projectile.Center + dir * (-6+SwordScale * 10)-Main.screenPosition;

                Color c = Color.PaleGoldenrod * 0.4f;
                c.A = 0;

                Vector2 origin = new(lightShotSPA.Width, lightShotSPA.Height / 2);
                Vector2 lightScale = new Vector2(1,1.5f) * SwordScale * 0.4f;
                Main.spriteBatch.Draw(lightShotSPA, lightPos, null, c, rot + MathHelper.Pi
                    , origin, lightScale, 0, 0);
                Main.spriteBatch.Draw(lightShotSPA, lightPos, null, (Color.White * 0.2f) with { A = 0 }, rot + MathHelper.Pi
                    , origin,lightScale, 0, 0);

                Main.spriteBatch.Draw(swordTex, SwordPos, null, Color.PaleGoldenrod * 0.3f, rot + MathHelper.PiOver4
                    , new Vector2(0, swordTex.Height), SwordScale, 0, 0);
                Main.spriteBatch.Draw(swordTex, SwordPos, null, c, rot + MathHelper.PiOver4
                    , new Vector2(0, swordTex.Height), SwordScale, 0, 0);

                rot += MathHelper.PiOver2;
            }

            return false;
        }
    }
}
