using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    [PlayerEffect]
    public class CthulhuFlyingShield : BaseFlyingShieldItem<CthulhuFlyingShieldGuard>, IBuffHeldItem
    {
        public CthulhuFlyingShield() : base(Item.sellPrice(0, 0, 0, 10), ItemRarityID.Expert, AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<CthulhuFlyingShieldProj>();
            Item.knockBack = 8;
            Item.shootSpeed = 14.5f;
            Item.damage = 23;
            Item.expert = true;
        }

        public void UpdateBuffHeldItem(Player player)
        {
            player.dashType = 2;
        }

        public override void HoldItem(Player player)
        {
            if (player.ItemTimeIsZero && player.ownedProjectileCounts[Item.shoot] == 0)
                player.GetModPlayer<CoralitePlayer>().AddEffect(nameof(CthulhuFlyingShield));

            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddDash(this);
            }
        }
    }

    public class CthulhuFlyingShieldProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "CthulhuFlyingShieldProj";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 40;
        }

        public override void OnShootDusts()
        {
            extraRotation += 0.4f;
        }

        public override void OnBackDusts()
        {
            extraRotation += 0.4f;
        }

        public override void SetOtherValues()
        {
            flyingTime = 25;
            backTime = 6;
            backSpeed = 14.5f;
            trailCachesLength = 9;
            trailWidth = 16 / 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 direction = (Projectile.Center - target.Center).SafeNormalize(Vector2.One);

            Helper.SpawnDirDustJet(target.Center, () => direction.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)), 2, 12,
                (i) => i * 1f, DustID.Blood, Scale: Main.rand.NextFloat(1f, 2f), noGravity: false, extraRandRot: 0.1f);

            for (int i = 0; i < 10; i++)
                Dust.NewDustPerfect(target.Center, DustID.Blood, direction.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)) * Main.rand.NextFloat(2f, 4f),
                    Scale: Main.rand.NextFloat(1f, 2f));

            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectileFromThis<SmallCthulhuEye>(Projectile.Center
                , Projectile.velocity.SafeNormalize(Vector2.Zero).RotateByRandom(-0.2f,0.2f) * 2, (int)(Projectile.damage * 2f), Projectile.knockBack / 2);
        }

        public override void DrawTrails(Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTextureValue();
            var origin = mainTex.Size() / 2;
            for (int i = trailCachesLength - 1; i > 4; i--)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, null,
                lightColor * 0.6f * ((i - 4) * 1 / 3f), Projectile.oldRot[i] - 1.57f + extraRotation, origin, Projectile.scale, 0, 0);

            base.DrawTrails(lightColor);
        }

        public override Color GetColor(float factor)
        {
            return Color.DarkRed * factor;
        }
    }

    public class SmallCthulhuEye : ModProjectile
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        ref float Timer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                if (Projectile.frame == 0)
                    Projectile.frame = 1;
                else
                    Projectile.frame = 0;
            }

            if (Projectile.ai[0] == 0)
            {
                const int flyingTime = 60;
                if (Helper.TryFindClosestEnemy(Projectile.Center, 800, n => n.CanBeChasedBy() && Projectile.localNPCImmunity.IndexInRange(n.whoAmI) && Projectile.localNPCImmunity[n.whoAmI] == 0, out NPC target))
                {
                    float selfAngle = Projectile.velocity.ToRotation();
                    float targetAngle = (target.Center - Projectile.Center).ToRotation();
                    float speed = Projectile.velocity.Length();
                    if (speed < 12)
                    {
                        speed += 0.5f;
                    }
                    Projectile.velocity = selfAngle.AngleLerp(targetAngle, Math.Clamp(Coralite.Instance.X2Smoother.Smoother(Timer / flyingTime), 0, 1f)).ToRotationVector2() * speed;
                }

                Projectile.rotation = Projectile.velocity.ToRotation();
                Timer++;
                if (Timer == 20)
                    Projectile.tileCollide = true;
                else if (Timer == 60)
                    Projectile.ai[0]++;
            }
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 direction = -Vector2.UnitY;
            for (int i = 0; i < 10; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.Blood, direction.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)) * Main.rand.NextFloat(2f, 4f),
                    Scale: Main.rand.NextFloat(1f, 2f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTextureValue();
            var frameBox = mainTex.Frame(1, 2, 0, Projectile.frame);
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, lightColor, Projectile.rotation - 1.57f, frameBox.Size() / 2, Projectile.scale, 0, 0); ;

            return false;
        }
    }

    public class CthulhuFlyingShieldGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "CthulhuFlyingShieldGuard";

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
            SoundEngine.PlaySound(CoraliteSoundID.Fleshy2_NPCHit7, Projectile.Center);
        }

        public override float GetWidth()
        {
            return Projectile.width / 2 / Projectile.scale + 2;
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
            Main.spriteBatch.Draw(mainTex, pos - dir * 5, frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上部
            frameBox = mainTex.Frame(3, 1, 1, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 2, frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 7, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上上部
            frameBox = mainTex.Frame(3, 1, 2, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 7, frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 12, frameBox, lightColor, rotation, origin2, scale, effect, 0);
        }
    }
}
