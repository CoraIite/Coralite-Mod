using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.FlyingShields
{
    public class RoyalAngel : BaseFlyingShieldItem<RoyalAngelGuard>
    {
        public RoyalAngel() : base(Item.sellPrice(0, 5), ItemRarityID.Yellow, AssetDirectory.FlyingShieldItems) { }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 18;
            Item.shoot = ModContent.ProjectileType<RoyalAngelProj>();
            Item.knockBack = 3;
            Item.shootSpeed = 16;
            Item.damage = 100;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SilverAngel>()
                .AddIngredient(ItemID.Ectoplasm, 5)
                .AddIngredient(ItemID.SoulofFlight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class RoyalAngelProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "RoyalAngel";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 50;
        }

        public override void SetOtherValues()
        {
            flyingTime = 18;
            backTime = 5;
            backSpeed = 16;
            trailCachesLength = 8;
            trailWidth = 10 / 2;
        }

        public override void OnShootDusts()
        {
            if (Main.rand.NextBool())
                Projectile.SpawnTrailDust(10f, DustID.GoldCoin, Main.rand.NextFloat(0.2f, 0.4f),
                   Scale: Main.rand.NextFloat(1f, 1.3f));
            Lighting.AddLight(Projectile.Center, Color.Gold.ToVector3());
        }

        public override void OnBackDusts()
        {
            if (Main.rand.NextBool())
                Projectile.SpawnTrailDust(10f, DustID.GoldCoin, Main.rand.NextFloat(0.2f, 0.4f),
                    Scale: Main.rand.NextFloat(1f, 1.3f));
            Lighting.AddLight(Projectile.Center, Color.Gold.ToVector3());
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (State != (int)FlyingShieldStates.Backing)
                if (!target.friendly && target.CanBeChasedBy())
                {
                    Projectile.NewProjectileFromThis<RoyalAngelStrike>(target.Center + Main.rand.NextFloat(-1.57f - 0.5f, -1.57f + 0.5f).ToRotationVector2() * 20,
                        Vector2.Zero, Projectile.damage, 4, target.whoAmI);
                }
        }

        public override Color GetColor(float factor)
        {
            Color c = Color.Gold;
            c.A = 0;
            return c * factor;
        }
    }

    public class RoyalAngelGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "RoyalAngel";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 50;
            Projectile.height = 52;
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
                    DustID.GoldCoin, Main.rand.NextFloat(Projectile.rotation - 0.6f, Projectile.rotation + 0.6f).ToRotationVector2() * Main.rand.NextFloat(1, 5),
                    Scale: Main.rand.NextFloat(1.5f, 2f));
                d.noGravity = true;
            }
        }

        public override float GetWidth()
        {
            return Projectile.width / 2 / Projectile.scale + 4;
        }
    }

    public class RoyalAngelStrike : ModProjectile
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public ref float Target => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        float distanceToTarget;
        float alpha;
        float selfRotation;
        float angle;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 24;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override bool ShouldUpdatePosition() => State == 1;

        public override void OnSpawn(IEntitySource source)
        {
            if (!Target.GetNPCOwner(out NPC owner, Projectile.Kill))
                return;

            angle = (owner.Center- Projectile.Center ).ToRotation();
            selfRotation = angle;
            distanceToTarget = -20;
            alpha = 0.4f;
            Projectile.Center = owner.Top + new Vector2(0, distanceToTarget);
            Projectile.rotation = 2.5f;
            Projectile.scale = 0.01f;
            Projectile.velocity = new Vector2(Main.rand.NextFromList(-1, 1), 0);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.whoAmI == (int)Target&&State>0)
                return base.CanHitNPC(target);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Projectile.SpawnTrailDust(DustID.PurpleTorch, -Main.rand.NextFloat(0.5f, 0.8f),
                    Scale: Main.rand.NextFloat(1.5f, 2f));
            }
            Vector2 dir = angle.ToRotationVector2();
            for (int i = 0; i < 8; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24),
                    DustID.GoldCoin, dir.RotatedBy(Main.rand.NextFloat(-0.4f,0.4f)) * Main.rand.NextFloat(1, 5),
                    Scale: Main.rand.NextFloat(1.5f, 2f));
                d.noGravity = true;
            }
        }

        public override void AI()
        {
            if (!Target.GetNPCOwner(out NPC owner, Projectile.Kill))
                return;

            Lighting.AddLight(Projectile.Center, Color.Gold.ToVector3());

            switch (State)
            {
                default:
                case 0://缓慢向上同时张开翅膀洒下羽毛
                    {
                        const int FlowTime = 20;

                        float factor = Timer / FlowTime;
                        float sqrtFactor = Coralite.Instance.SqrtSmoother.Smoother(factor);

                        distanceToTarget = Helper.Lerp(-20, -270, sqrtFactor);
                        alpha = Helper.Lerp(0.4f, 1f, sqrtFactor);
                        Projectile.rotation = Helper.Lerp(2.5f, 0, sqrtFactor);
                        selfRotation =Helper.Lerp(angle,angle+Projectile.velocity.X*MathHelper.TwoPi,sqrtFactor) ;
                        Projectile.scale = Helper.Lerp(0.01f, 1f, sqrtFactor);
                        Projectile.Center = owner.Center + angle.ToRotationVector2() * distanceToTarget;
                        Timer++;
                        if (Timer > FlowTime)
                        {
                            State++;
                            Projectile.timeLeft = 60;
                            Projectile.tileCollide = true;
                            Projectile.velocity = angle.ToRotationVector2() * 12;
                            Projectile.extraUpdates += 2;
                            selfRotation = angle;
                            Projectile.InitOldPosCache(14);
                        }
                    }
                    break;
                case 1:
                    {
                        //if (Main.rand.NextBool())
                        //{
                        //    Particle.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(18, 18),
                        //        angle.ToRotationVector2()* Main.rand.NextFloat(3, 5), CoraliteContent.ParticleType<SpeedLine>(),
                        //        newColor: Color.Gold*0.8f, Scale: Main.rand.NextFloat(0.1f, 0.4f));
                        //}

                        Projectile.SpawnTrailDust(DustID.GoldCoin, Main.rand.NextFloat(0.2f, 0.3f),
                            Scale: Main.rand.NextFloat(1f, 1.2f));

                        Projectile.UpdateOldPosCache();
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

            float rot = selfRotation - 1.57f;

            ProjectilesHelper.DrawPrettyLine(Projectile.Opacity, 0, pos, Color.White, Color.Gold,
                Timer / 20, 0, 1, 1, 2, selfRotation, 2.3f, Vector2.One);
            ProjectilesHelper.DrawPrettyLine(Projectile.Opacity, 0, pos, Color.White, Color.Gold,
                Timer / 20, 0, 1, 1, 2, selfRotation, 2f, Vector2.One * 1.2f);


            if (State > 0)
            {
                var frameBox2 = mainTex.Frame(3, 1, 2, 0);

                for (int i = 13; i >1; i--)
                {
                    Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, frameBox2, lightColor * (i * 0.5f / 14), rot
                        , origin, Projectile.scale * (0.5f + i * 0.5f/14), 0, 0);
                }
            }

            //绘制左边翅膀
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, -Projectile.rotation + rot, origin, 1, 0, 0);

            //绘制右边翅膀
            frameBox = mainTex.Frame(3, 1, 1, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, Projectile.rotation + rot, origin, 1, 0, 0);

            //绘制本体
            frameBox = mainTex.Frame(3, 1, 2, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, rot, origin, Projectile.scale, 0, 0);

            return false;
        }
    }
}
