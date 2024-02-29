using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.FlyingShields
{
    public class DeliciousSteak : BaseFlyingShieldItem<DeliciousSteakGuard>
    {
        public DeliciousSteak() : base(Item.sellPrice(0, 0, 15, 0), ItemRarityID.Green, AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<DeliciousSteakProj>();
            Item.knockBack = 7;
            Item.shootSpeed = 14;
            Item.damage = 27;
        }
    }

    public class DeliciousSteakProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "DeliciousSteak";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 34;
        }

        public override void SetOtherValues()
        {
            flyingTime = 24;
            backTime = 4;
            backSpeed = 14;
            trailCachesLength = 6;
            maxJump++;
        }

        public override void OnShootDusts()
        {
            extraRotation += 0.3f;
        }

        public override void OnBackDusts()
        {
            extraRotation += 0.3f;
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

    public class DeliciousSteakGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "DeliciousSteak";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 34;
            Projectile.height = 38;
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
}
