using Coralite.Content.Items.GlobalItems;
using Coralite.Content.Projectiles.Globals;
using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class BigMacHamburger : BaseFlyingShieldItem<BigMacHamburgerGuard>
    {
        public BigMacHamburger() : base(Item.sellPrice(0, 2), ItemRarityID.LightRed, AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<BigMacHamburgerProj>();
            Item.knockBack = 7;
            Item.shootSpeed = 15;
            Item.damage = 45;
            CoraliteGlobalItem.SetEdibleDamage(Item);
        }
    }

    public class BigMacHamburgerProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "BigMacHamburger";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 34;
        }

        public override void SetOtherValues()
        {
            flyingTime = 24;
            backTime = 4;
            backSpeed = 15;
            trailCachesLength = 6;
            maxJump++;
            CoraliteGlobalProjectile.SetEdibleDamage(Projectile);
        }

        public override void OnShootDusts()
        {
            extraRotation += 0.6f;
        }

        public override void OnBackDusts()
        {
            extraRotation += 0.6f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            for (int i = 0; i < 3; i++)
            {
                Helper.SpawnDirDustJet(Projectile.Center,
                    () => Main.rand.NextFloat(-1.57f - 1f, -1.57f + 1f).ToRotationVector2()
                    , 2, 4, i => Main.rand.NextFloat(1, 1.5f) + i * 0.5f,
                    DustID.Poop, Scale: Main.rand.NextFloat(1f, 1.5f), noGravity: false);
            }
        }

        public override void DrawTrails(Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var origin = mainTex.Size() / 2;
            for (int i = trailCachesLength - 1; i > 0; i--)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, null,
                lightColor * 0.6f * (i * 1 / 6f), Projectile.oldRot[i] - 1.57f + extraRotation, origin, Projectile.scale, 0, 0);
        }
    }

    public class BigMacHamburgerGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "BigMacHamburger";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 34;
            Projectile.height = 38;
            CoraliteGlobalProjectile.SetEdibleDamage(Projectile);
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.15f;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            SoundEngine.PlaySound(Main.rand.NextBool() ? CoraliteSoundID.Bloody2_NPCHit13 :
                CoraliteSoundID.BloodyDeath4_NPCDeath21, Projectile.Center);
            for (int i = 0; i < 4; i++)
            {
                Helper.SpawnDirDustJet(Projectile.Center,
                    () => Projectile.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f))
                    , 2, 4, i => Main.rand.NextFloat(1f, 1.5f) + i * 0.6f,
                    DustID.Poop, noGravity: false);
            }
        }
    }

    public class BigMacExProj : ModProjectile
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

    }
}
