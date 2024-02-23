using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.FlyingShields
{
    public class SilverAngel : BaseFlyingShieldItem<SilverAngelGuard>
    {
        public SilverAngel() : base(Item.sellPrice(0, 0, 0, 50), ItemRarityID.Blue, AssetDirectory.FlyingShieldItems) { }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 18;
            Item.shoot = ModContent.ProjectileType<SilverAngelProj>();
            Item.knockBack = 3;
            Item.shootSpeed = 16;
            Item.damage = 30;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SilverBar, 16)
                .AddIngredient(ItemID.TissueSample, 5)
                .AddIngredient(ItemID.Feather, 5)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.SilverBar, 16)
                .AddIngredient(ItemID.ShadowScale, 5)
                .AddIngredient(ItemID.Feather, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SilverAngelProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "SilverAngel";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 42;
        }

        public override void SetOtherValues()
        {
            flyingTime = 18;
            backTime = 5;
            backSpeed = 14;
            trailCachesLength = 10;
            trailWidth = 10 / 2;
        }

        public override void OnShootDusts()
        {
            if (Main.rand.NextBool())
                Projectile.SpawnTrailDust(10f,DustID.ShadowbeamStaff, Main.rand.NextFloat(0.5f, 0.8f),
                   Scale: Main.rand.NextFloat(1f, 1.3f));
            Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 0.2f, 0.4f));
        }

        public override void OnBackDusts()
        {
            if (Main.rand.NextBool())
                Projectile.SpawnTrailDust(10f, DustID.ShadowbeamStaff, Main.rand.NextFloat(0.5f, 0.8f),
                    Scale: Main.rand.NextFloat(1f, 1.3f));
            Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 0.2f, 0.4f));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (State != (int)FlyingShieldStates.Backing)
                if (!target.friendly && target.CanBeChasedBy())
                {
                    Projectile.NewProjectileFromThis<SilverAngelStrike>(target.Top + new Vector2(0, -20),
                        Vector2.Zero, Projectile.damage, 4, target.whoAmI);
                }
        }

        public override Color GetColor(float factor)
        {
            return new Color(77, 69, 181) * factor;
        }
    }

    public class SilverAngelGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "SilverAngel";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 42;
            Projectile.height = 44;
        }

        public override void SetOtherValues()
        {
            scalePercent = 2f;
            damageReduce = 0.15f;
        }

        public override void OnGuard()
        {
            base.OnGuard();
            for (int i = 0; i < 4; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16),
                    DustID.SilverFlame, Main.rand.NextFloat(Projectile.rotation - 0.6f, Projectile.rotation + 0.6f).ToRotationVector2() * Main.rand.NextFloat(1, 5),
                    Scale: Main.rand.NextFloat(1.5f, 2f));
                d.noGravity = true;
            }
        }

        public override float GetWidth()
        {
            return Projectile.width / 2/Projectile.scale + 4;
        }
    }

    public class SilverAngelStrike : ModProjectile
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public ref float Target => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        float distanceToTarget;
        float alpha;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override bool ShouldUpdatePosition() => State == 1;

        public override void OnSpawn(IEntitySource source)
        {
            if (!Target.GetNPCOwner(out NPC owner, Projectile.Kill))
                return;

            distanceToTarget = -20;
            alpha = 0.3f;
            Projectile.Center = owner.Top + new Vector2(0, distanceToTarget);
            Projectile.rotation = 1.8f;
            Projectile.scale = 0.01f;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.whoAmI == (int)Target)
                return base.CanHitNPC(target);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Projectile.SpawnTrailDust(DustID.ShadowbeamStaff, -Main.rand.NextFloat(0.5f, 0.8f),
                    Scale: Main.rand.NextFloat(1.5f, 2f));
            }
            for (int i = 0; i < 8; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24),
                    DustID.SilverFlame, Main.rand.NextFloat(-1.57f - 0.6f, -1.57f + 0.6f).ToRotationVector2() * Main.rand.NextFloat(1, 5),
                    Scale: Main.rand.NextFloat(1.5f, 2f));
                d.noGravity = true;
            }

            SoundEngine.PlaySound(CoraliteSoundID.Hit_Item10, Projectile.Center);
        }

        public override void AI()
        {
            if (!Target.GetNPCOwner(out NPC owner, Projectile.Kill))
                return;

            Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 0.2f, 0.4f));

            switch (State)
            {
                default:
                case 0://缓慢向上同时张开翅膀洒下羽毛
                    {
                        const int FlowTime = 20;

                        float factor = Timer / FlowTime;
                        float sqrtFactor = Coralite.Instance.SqrtSmoother.Smoother(factor);

                        distanceToTarget = Helper.Lerp(-20, -180, sqrtFactor);
                        alpha = Helper.Lerp(0.3f, 1f, sqrtFactor);
                        Projectile.rotation = Helper.Lerp(1.8f, 0, sqrtFactor);
                        Projectile.scale = Helper.Lerp(0.01f, 1f, sqrtFactor);
                        Projectile.Center = owner.Top + new Vector2(0, distanceToTarget);
                        if (Timer > FlowTime)
                        {
                            State++;
                            Projectile.timeLeft = 60;
                            Projectile.tileCollide = true;
                            Projectile.velocity = new Vector2(0, 15);
                            Projectile.extraUpdates++;
                        }
                        Timer++;
                    }
                    break;
                case 1:
                    {
                        if (Main.rand.NextBool())
                        {
                            Particle.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(18, 18),
                                new Vector2(0, Main.rand.NextFloat(3, 5)), CoraliteContent.ParticleType<SpeedLine>(),
                                newColor: new Color(77, 69, 181), Scale: Main.rand.NextFloat(0.1f, 0.4f));
                        }

                        Projectile.SpawnTrailDust(DustID.SilverCoin, Main.rand.NextFloat(0.2f, 0.3f),
                            Scale: Main.rand.NextFloat(1f, 1.2f));
                    }
                    break;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            var pos = Projectile.Center - Main.screenPosition;
            var frameBox = mainTex.Frame(3, 1, 0, 0);
            var origin = frameBox.Size() / 2;
            lightColor *= alpha;

            Helper.DrawPrettyLine(Projectile.Opacity, 0, pos, Color.White, new Color(77, 69, 181),
                Timer / 20, 0, 1, 1, 2, MathHelper.PiOver2, 1.8f, Vector2.One);
            Helper.DrawPrettyLine(Projectile.Opacity, 0, pos, Color.White, new Color(77, 69, 181),
                Timer / 20, 0, 1, 1, 2, MathHelper.PiOver2, 1.6f, Vector2.One*1.2f);

            //绘制左边翅膀
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, -Projectile.rotation, origin, 1, 0, 0);

            //绘制右边翅膀
            frameBox = mainTex.Frame(3, 1, 1, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, Projectile.rotation, origin, 1, 0, 0);

            //绘制本体
            frameBox = mainTex.Frame(3, 1, 2, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, 0f, origin, Projectile.scale, 0, 0);

            return false;
        }
    }
}
