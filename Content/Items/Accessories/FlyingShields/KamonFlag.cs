using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.Accessories.FlyingShields
{
    [AutoloadEquip(EquipType.Back)]
    public class KamonFlag : BaseAccessory, IFlyingShieldAccessory, IDashable
    {
        public KamonFlag() : base(ItemRarityID.Lime, Item.sellPrice(0, 4, 50, 0))
        {
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DamageType = DamageClass.Generic;
            Item.damage = 75;
        }

        public bool isDashing;
        public bool hit;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldAccessories?.Add(this);
            }

            if (isDashing)
            {
            }
        }

        public void OnDashing(BaseFlyingShieldGuard projectile)
        {
            projectile.OnGuard_DamageReduce(projectile.damageReduce);

            if (projectile.Timer > projectile.dashTime / 4)
            {
                projectile.Owner.velocity.X = (projectile.dashDir.ToRotationVector2() * projectile.dashSpeed).X;
            }
        }

        public void OnDashHit(BaseFlyingShieldGuard projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            hit = true;

            if (projectile.parryTime > 0 && projectile.Timer > projectile.dashTime - projectile.parryTime * 1.5f)
            {
                projectile.OnParry();
                projectile.UpdateShieldAccessory(accessory => accessory.OnParry(projectile));
                projectile.OnGuard_DamageReduce(projectile.damageReduce);
            }
            else
            {
                projectile.OnGuard_DamageReduce(projectile.damageReduce);
                projectile.OnGuard();
                projectile.OnGuardNPC(target.whoAmI);
            }

            projectile.Timer = 0;
            projectile.Owner.velocity.X = (-Math.Sign(projectile.Owner.velocity.X)) * 6;
            projectile.Owner.velocity.Y = -3f;
        }

        public void OnDashOver(BaseFlyingShieldGuard projectile)
        {
            if (!hit)
                projectile.Owner.velocity *= 0.7f;
            isDashing = false;
        }

        public bool Dash(Player Player, int DashDir)
        {
            float dashDirection;
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        Player.direction = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        dashDirection = DashDir == CoralitePlayer.DashRight ? 0 : 3.141f;
                        break;
                    }
                default:
                    return false;
            }

            if (Player.TryGetModPlayer(out CoralitePlayer cp)
                && cp.TryGetFlyingShieldGuardProj(out BaseFlyingShieldGuard flyingShieldGuard)
                && flyingShieldGuard.CanDash())
            {
                SoundEngine.PlaySound(CoraliteSoundID.WhipSwing_Item152, Player.Center);

                flyingShieldGuard.TurnToDashing(this, 18, dashDirection, 14f);
                Player.GetModPlayer<CoralitePlayer>().DashDelay = 75;
                int damage = Player.GetWeaponDamage(Item);
                for (int i = -1; i < 2; i++)
                {
                    Projectile.NewProjectile(Player.GetSource_ItemUse(Item), Player.Center,
                        (dashDirection + i * 0.4f).ToRotationVector2().RotateByRandom(-0.2f, 0.2f) * 4,
                         ModContent.ProjectileType<BlackSpirit>(), damage, 0, Player.whoAmI);
                }

                isDashing = true;
                hit = false;
                return true;
            }

            return false;
        }
    }

    public class BlackSpirit : ModProjectile
    {
        public override string Texture => AssetDirectory.Accessories + Name;

        ref float Timer => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.penetrate = 3;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 2)
                {
                    Projectile.frame = 0;
                }
            }

            Projectile.SpawnTrailDust(DustID.Smoke, Main.rand.NextFloat(0.2f, 0.6f), 50, newColor: Color.Black, Scale: Main.rand.NextFloat(1f, 1.5f));
            if (Main.rand.NextBool())
            {
                Projectile.SpawnTrailDust(DustID.SilverFlame, Main.rand.NextFloat(0.2f, 0.6f)
                    , 50, Scale: Main.rand.NextFloat(1f, 1.5f));
            }
            if (Projectile.ai[0] == 0)
            {
                const int flyingTime = 80;
                if (Helper.TryFindClosestEnemy(Projectile.Center, 800, n => n.CanBeChasedBy(), out NPC target))
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
                if (Timer > 60)
                {
                    Projectile.ai[0]++;
                    if (Timer > 20)
                        Projectile.tileCollide = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Timer = 0;
            Projectile.tileCollide = true;
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 direction = -Vector2.UnitY;
            for (int i = 0; i < 10; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, direction.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)) * Main.rand.NextFloat(2f, 4f),
                   Alpha: 75, newColor: Color.Black, Scale: Main.rand.NextFloat(1f, 2f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var frameBox = mainTex.Frame(1, 3, 0, Projectile.frame);
            Projectile.DrawShadowTrails(new Color(200, 200, 200, 60), 1,
                1 / 6f, 0, 6, 1, Projectile.scale, frameBox, -1.57f);

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, lightColor, Projectile.rotation - 1.57f, frameBox.Size() / 2, Projectile.scale, 0, 0); ;

            return false;
        }
    }
}
