using Coralite.Content.DamageClasses;
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
    public class DevilsYoke : BaseFairyCatcher
    {
        public override string Texture => AssetDirectory.FairyCatcherLasso + Name;

        public override int CatchPower => 30;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<DevilsYokeSwing>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 22;
            Item.shootSpeed = 9;
            Item.SetWeaponValues(25, 3);
            Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(0, 1));
            Item.autoReuse = true;
        }
    }

    [VaultLoaden(AssetDirectory.FairyCatcherLasso)]
    public class DevilsYokeSwing() : BaseLassoSwing(4)
    {
        public override string Texture => AssetDirectory.FairyCatcherLasso + "DevilsYokeCatcher";

        public static ATex PurpleChain { get; set; }

        private bool shooted = false;

        public override float GetShootRandAngle => Main.rand.NextFloat(-0.1f,0.1f);

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
            MaxDistance = 168;
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
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24), DustID.Shadowflame
                    , Helper.NextVec2Dir(1, 2));
                d.noGravity = true;
            }
        }

        protected override void OnHitEvent(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!shooted)
            {
                shooted = true;
                float rot = Main.rand.NextFloat(6.282f);
                Vector2 pos = target.Center - rot.ToRotationVector2() * 54;
                Projectile.NewProjectileFromThis<DevilsYokeChainProj>(pos, Vector2.Zero, Projectile.damage, 1, rot);

                Helper.PlayPitched(CoraliteSoundID.MinecartTrack_Item52, Projectile.Center);
            }
        }

        public override Texture2D GetStringTex() => PurpleChain.Value;
        public override Color GetStringColor(Vector2 pos) => Lighting.GetColor((int)pos.X / 16, (int)(pos.Y / 16f));
    }

    public class DevilsYokeChainProj : ModProjectile
    {
        public override string Texture => AssetDirectory.FairyCatcherLasso + "PurpleChain";

        public ref float Angle => ref Projectile.ai[0];
        public ref float SpawnCount => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 16;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 5 && SpawnCount < 5)
            {
                Projectile.NewProjectileFromThis<DevilsYokeChainProj>(Projectile.Center + Angle.ToRotationVector2() * 24
                    , Vector2.Zero, Projectile.damage, Projectile.knockBack, Angle+Main.rand.NextFloat(-0.2f,0.2f), SpawnCount + 1);
            }
            else if (Timer > 30)
                Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8)
                    , DustID.Shadowflame, Angle.ToRotationVector2() * Main.rand.NextFloat(0.5f, 1.5f));
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 dir = Angle.ToRotationVector2();
            Vector2 pos = Projectile.Center - Main.screenPosition;

            Texture2D mainTex = Projectile.GetTexture();
            Color c = Color.White * 0.75f;
            if (Timer < 20)
                c *= Timer / 20;

            mainTex.QuickCenteredDraw(Main.spriteBatch, pos + dir * mainTex.Width / 2, c, Angle);
            mainTex.QuickCenteredDraw(Main.spriteBatch, pos - dir * mainTex.Width / 2, c, Angle);

            return false;
        }
    }
}
