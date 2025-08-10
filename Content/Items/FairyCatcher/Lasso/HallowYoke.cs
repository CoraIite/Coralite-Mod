using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
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

    [AutoLoadTexture(Path = AssetDirectory.FairyCatcherLasso)]
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
            shootTime = 20;

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
            if (Catch == 0)
            {
                //Projectile.NewProjectileFromThis<HallowYokeSlash>(Projectile.Center,
                //    (Projectile.Center - Owner.Center).SafeNormalize(Vector2.Zero) * 8, Projectile.damage / 2, 1);

                Helper.PlayPitched(CoraliteSoundID.StarFalling_Item105, Projectile.Center, pitchAdjust: -0.1f);
            }
        }

        public override Texture2D GetStringTex() => HallowChain.Value;
        public override Color GetStringColor(Vector2 pos) => Lighting.GetColor((int)pos.X / 16, (int)(pos.Y / 16f));
    }

    [AutoLoadTexture(Path = AssetDirectory.FairyCatcherLasso)]
    public class HallowYokeSlash:ModProjectile
    {
        public override string Texture => AssetDirectory.FairyCatcherLasso + "HallowYokeCatcher";

        public static ATex HallowSword { get; set; }

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void AI()
        {
            base.AI();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }

}
