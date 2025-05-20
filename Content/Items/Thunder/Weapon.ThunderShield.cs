using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.Thunder
{
    public class ThunderShield : BaseFlyingShieldItem<ThunderShieldGuard>
    {
        public ThunderShield() : base(Item.sellPrice(0, 3), ItemRarityID.Yellow, AssetDirectory.ThunderItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 25;
            Item.shoot = ModContent.ProjectileType<ThunderShieldProj>();
            Item.knockBack = 5;
            Item.shootSpeed = 8;
            Item.damage = 64;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ZapCrystal>(4)
                .AddIngredient<InsulationCortex>(6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderShieldProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.ThunderItems + "ThunderShield";

        public bool hited;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 50;
            Projectile.extraUpdates = 2;
        }

        public override void SetOtherValues()
        {
            flyingTime = 25 * 2;
            backTime = 14 * 2;
            backSpeed = 10;
            trailCachesLength = 9;
            trailWidth = 25 / 2;
        }

        public override void OnShootDusts()
        {
            SpawnDusts();
        }

        public override void OnBackDusts()
        {
            SpawnDusts();
        }

        public void SpawnDusts()
        {
            if (Timer % 4 == 0)
                for (int i = 0; i < 2; i++)
                {
                    Projectile.SpawnTrailDust((float)(Projectile.width / 2), DustID.PortalBoltTrail, Main.rand.NextFloat(0.2f, 0.5f),
                        newColor: Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, 1.4f));
                }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (State != (int)FlyingShieldStates.Backing && !hited)
            {
                hited = true;
                //if (Main.rand.NextBool(8))
                //{
                //    SoundEngine.PlaySound(CoraliteSoundID.Thunder, Projectile.Center);
                //}
                Projectile.NewProjectileFromThis<ThunderShieldExProj>(Vector2.Lerp(Projectile.Center, target.Center, Main.rand.NextFloat(0, 1f)) + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), Vector2.Zero,
                    Projectile.damage, Projectile.knockBack, 20);
            }
        }

        public override Color GetColor(float factor)
        {
            return Coralite.ThunderveinYellow * factor;
        }
    }

    public class ThunderShieldGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.ThunderItems + "ThunderShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 54;
            Projectile.height = 52;
        }

        public override void SetOtherValues()
        {
            scalePercent = 1.4f;
            damageReduce = 0.25f;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            SoundEngine.PlaySound(CoraliteSoundID.BubbleShield_Electric_NPCHit43, Projectile.Center);
        }

        public override float GetWidth()
        {
            return Projectile.width / 2 / Projectile.scale;
        }
    }

    /// <summary>
    /// ai0传入闪电时间
    /// </summary>
    public class ThunderShieldExProj : BaseThunderProj
    {
        public override string Texture => AssetDirectory.Blank;

        const int DelayTime = 20;

        public ref float LightingTime => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.localAI[0];

        public ThunderTrail[] circles;
        public ThunderTrail[] outers;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 170;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.penetrate = -1;
        }

        public override bool ShouldUpdatePosition() => false;

        public float ThunderWidthFunc2(float factor)
        {
            return MathF.Sin(factor * MathHelper.Pi) * ThunderWidth * 1.5f;
        }

        public override bool? CanDamage()
        {
            if (Timer > LightingTime + (DelayTime / 2))
                return false;
            return base.CanDamage();
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Collision.CanHit(Projectile.Center, 1, 1, target.position, target.width, target.height))
            {
                return null;
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = Projectile.damage / 2;
        }

        public override Color ThunderColorFunc_Yellow(float factor)
        {
            return Color.Lerp(ThunderveinDragon.ThunderveinPurple, ThunderveinDragon.ThunderveinYellow, MathF.Sin(factor * MathHelper.Pi));
        }

        public override Color ThunderColorFunc2_Orange(float factor)
        {
            return Color.Lerp(ThunderveinDragon.ThunderveinPurple, ThunderveinDragon.ThunderveinOrange, MathF.Sin(factor * MathHelper.Pi));
        }

        public float GetAlpha2(float factor)
        {
            return ThunderAlpha * factor;
        }

        public override void AI()
        {
            if (circles == null)
            {
                ThunderAlpha = 1;
                circles = new ThunderTrail[3];
                outers = new ThunderTrail[4];
                Asset<Texture2D> thunderTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightingBodyF");

                for (int i = 0; i < circles.Length; i++)
                {
                    if (i == 0)
                        circles[i] = new ThunderTrail(thunderTex, ThunderWidthFunc2, ThunderColorFunc2_Orange, GetAlpha);
                    else
                        circles[i] = new ThunderTrail(thunderTex, ThunderWidthFunc2, ThunderColorFunc_Yellow, GetAlpha);

                    circles[i].UseNonOrAdd = true;
                    circles[i].PartitionPointCount = 3;
                    circles[i].SetRange((0, 5));
                    circles[i].SetExpandWidth(0);
                }

                for (int i = 0; i < outers.Length; i++)
                {
                    if (i < 2)
                        outers[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin, ThunderColorFunc2_Orange, GetAlpha2);
                    else
                        outers[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin, ThunderColorFunc_Yellow, GetAlpha2);

                    outers[i].UseNonOrAdd = true;
                    outers[i].SetRange((0, 10));
                    outers[i].SetExpandWidth(2);
                }

                foreach (var outer in outers)
                {
                    outer.CanDraw = true;
                    Vector2[] vec = new Vector2[5];
                    float length = Main.rand.NextFloat(2, 32);
                    Vector2 basePos = Projectile.Center + (Helper.NextVec2Dir() * length);
                    Vector2 dir = (basePos - Projectile.Center).SafeNormalize(Vector2.Zero);
                    vec[0] = basePos;

                    for (int i = 1; i < 5; i++)
                    {
                        vec[i] = basePos + (dir * i * (Projectile.width - length) / 10);
                    }

                    outer.BasePositions = vec;
                    outer.RandomThunder();
                }

                foreach (var circle in circles)
                {
                    circle.CanDraw = true;
                    int trailPointCount = Main.rand.Next(10, 20);
                    Vector2[] vec = new Vector2[trailPointCount];

                    float baseRot = Main.rand.NextFloat(6.282f);
                    for (int i = 0; i < trailPointCount; i++)
                    {
                        vec[i] = Projectile.Center + ((baseRot + (i * MathHelper.TwoPi / 20)).ToRotationVector2()
                            * ((Projectile.width / 2) + Main.rand.NextFloat(-8, 8)));
                    }

                    circle.BasePositions = vec;
                    circle.RandomThunder();
                }
            }

            if (Timer > 10 && Timer % 4 == 0)
            {
                foreach (var outer in outers)
                {
                    bool canDraw = Main.rand.NextBool();
                    outer.CanDraw = canDraw;
                    if (canDraw)//从内向外散发出去的闪电
                    {
                        Vector2[] vec = new Vector2[5];
                        float length = Main.rand.NextFloat(2, 32);
                        Vector2 basePos = Projectile.Center + (Helper.NextVec2Dir() * length);
                        Vector2 dir = (basePos - Projectile.Center).SafeNormalize(Vector2.Zero);
                        vec[0] = basePos;

                        for (int i = 1; i < 5; i++)
                        {
                            vec[i] = basePos + (dir * i * (Projectile.width - length) / 10);
                        }

                        outer.BasePositions = vec;
                        outer.RandomThunder();
                    }
                }
            }

            if (Timer > 10 && Timer % 6 == 0)
                foreach (var circle in circles)
                {
                    bool canDraw = Main.rand.NextBool();
                    circle.CanDraw = canDraw;
                    if (canDraw)//从内向外散发出去的闪电
                    {
                        int trailPointCount = Main.rand.Next(10, 20);
                        Vector2[] vec = new Vector2[trailPointCount];

                        float baseRot = Main.rand.NextFloat(6.282f);
                        for (int i = 0; i < trailPointCount; i++)
                        {
                            vec[i] = Projectile.Center + ((baseRot + (i * MathHelper.TwoPi / 20)).ToRotationVector2()
                                * ((Projectile.width / 2) + Main.rand.NextFloat(-8, 8)));
                        }

                        circle.BasePositions = vec;
                        circle.RandomThunder();
                    }
                }

            if (Timer < LightingTime)
            {
                float factor = Timer / LightingTime;
                float sinFactor = MathF.Sin(factor * MathHelper.Pi);

                ThunderWidth = 10 + (sinFactor * 15);
                SpawnDusts();
            }
            else
            {
                float factor = (Timer - LightingTime) / DelayTime;
                float x2Factor = Helper.X2Ease(factor);

                ThunderWidth = (1 - x2Factor) * 25;
                ThunderAlpha = 1 - x2Factor;
                if (Timer > LightingTime + DelayTime)
                    Projectile.Kill();
            }

            Timer++;
        }

        public void SpawnDusts()
        {
            if (Timer % 4 == 0)
            {
                PRTLoader.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 5, Projectile.width / 5),
                    Vector2.Zero, CoraliteContent.ParticleType<BigFog>(), Coralite.ThunderveinYellow * Main.rand.NextFloat(0.5f, 0.8f),
                    Main.rand.NextFloat(0.5f, 1f));
                ElectricParticle_Follow.Spawn(Projectile.Center, Helper.NextVec2Dir(Projectile.width / 3, Projectile.width * 0.56f), () => Projectile.Center, Main.rand.NextFloat(0.7f, 1.1f));
            }
            else
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 3, Projectile.width / 3)
                    , ModContent.DustType<LightningShineBall>(), Vector2.Zero, newColor: ThunderveinDragon.ThunderveinYellowAlpha, Scale: Main.rand.NextFloat(0.1f, 0.2f));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (circles != null)
                foreach (var circle in circles)
                    circle.DrawThunder(Main.instance.GraphicsDevice);
            if (outers != null)
                foreach (var outer in outers)
                    outer.DrawThunder(Main.instance.GraphicsDevice);
            return false;
        }
    }
}
