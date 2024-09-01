using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class Noctiflair : BaseFlyingShieldItem<NoctiflairGuard>
    {
        public Noctiflair() : base(Item.sellPrice(0, 10), ItemRarityID.Red, AssetDirectory.FlyingShieldItems) { }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<NoctiflairProj>();
            Item.knockBack = 3;
            Item.shootSpeed = 17;
            Item.damage = 183;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RoyalAngel>()
                .AddIngredient(ItemID.LunarBar, 5)
                .AddIngredient(ItemID.SoulofFlight, 5)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }

    public class NoctiflairProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "Noctiflair";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 50;
        }

        public override void SetOtherValues()
        {
            flyingTime = 20;
            backTime = 5;
            backSpeed = 20;
            trailCachesLength = 8;
            trailWidth = 10 / 2;
        }

        public override void OnShootDusts()
        {
            if (Main.rand.NextBool())
                Projectile.SpawnTrailDust(10f, DustID.MushroomTorch, Main.rand.NextFloat(0.2f, 0.4f),
                   Scale: Main.rand.NextFloat(1f, 1.3f));
            Lighting.AddLight(Projectile.Center, Color.Gold.ToVector3());
        }

        public override void OnBackDusts()
        {
            if (Main.rand.NextBool())
                Projectile.SpawnTrailDust(10f, DustID.MushroomTorch, Main.rand.NextFloat(0.2f, 0.4f),
                    Scale: Main.rand.NextFloat(1f, 1.3f));
            Lighting.AddLight(Projectile.Center, Color.Gold.ToVector3());
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (State != (int)FlyingShieldStates.Backing)
                if (!target.friendly && target.CanBeChasedBy())
                {
                    Projectile.NewProjectileFromThis<NoctiflairStrike>(target.Center + (Main.rand.NextFloat(-1.57f - 0.5f, -1.57f + 0.5f).ToRotationVector2() * 20),
                        Vector2.Zero, (int)(Projectile.damage * 0.7f), 4, target.whoAmI);
                }
        }

        public override Color GetColor(float factor)
        {
            return new Color(83, 129, 155, 0) * factor;
        }
    }

    public class NoctiflairGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        private float wing;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 50;
            Projectile.height = 52;
        }

        public override void SetOtherValues()
        {
            scalePercent = 1.2f;
            damageReduce = 0.3f;
        }

        public override void OnHoldShield()
        {
            wing += Math.Abs(Owner.velocity.Y / 60);
            wing += 0.007f;
        }

        public override void OnGuard()
        {
            base.OnGuard();
            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16),
                    DustID.MushroomTorch, Main.rand.NextFloat(Projectile.rotation - 0.6f, Projectile.rotation + 0.6f).ToRotationVector2() * Main.rand.NextFloat(1, 5),
                    Scale: Main.rand.NextFloat(1.5f, 2f));
                d.noGravity = true;
            }
        }

        public override float GetWidth()
        {
            return Projectile.width * 0.4f / Projectile.scale;
        }

        public override void DrawSelf(Texture2D mainTex, Vector2 pos, float rotation, Color lightColor, Vector2 scale, SpriteEffects effect)
        {
            Rectangle frameBox;
            Vector2 rotDir = Projectile.rotation.ToRotationVector2();
            Vector2 dir = rotDir * (DistanceToOwner / (Projectile.width * scalePercent));
            Color c = lightColor * 0.5f;
            c.A = lightColor.A;

            float exRot = rotation - (Owner.direction * (0.3f + (0.4f * MathF.Sin(wing))));
            float exRot2 = rotation + (Owner.direction * (0.3f + (0.4f * MathF.Sin(wing))));

            frameBox = mainTex.Frame(5, 1, 0, 0);
            Vector2 origin2 = frameBox.Size() / 2;
            //绘制水晶
            Main.spriteBatch.Draw(mainTex, pos - (dir * 5), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制翅膀
            frameBox = mainTex.Frame(5, 1, 1, 0);

            Main.spriteBatch.Draw(mainTex, pos - (dir * 5), frameBox, c, exRot, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, exRot, origin2, scale, effect, 0);

            frameBox = mainTex.Frame(5, 1, 2, 0);
            Main.spriteBatch.Draw(mainTex, pos - (dir * 5), frameBox, c, exRot2, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, exRot2, origin2, scale, effect, 0);

            //绘制上部
            frameBox = mainTex.Frame(5, 1, 3, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 3), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 8), frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上上部
            frameBox = mainTex.Frame(5, 1, 4, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 11), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 16), frameBox, lightColor, rotation, origin2, scale, effect, 0);
        }
    }

    public class NoctiflairStrike : ModProjectile
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public ref float Target => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        float distanceToTarget;
        float alpha;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 24;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override bool ShouldUpdatePosition() => State > 0;

        public override void OnSpawn(IEntitySource source)
        {
            if (!Target.GetNPCOwner(out NPC owner, Projectile.Kill))
                return;

            if (State == 2)
            {
                Projectile.extraUpdates++;
                Projectile.timeLeft = 80;
                Projectile.velocity = (owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero)
                    .RotatedBy(Main.rand.NextFloat(-1.2f, 1.2f)) * Main.rand.NextFloat(15.5f, 18);
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.InitOldPosCache(14);
                Projectile.InitOldRotCache(14);
                Projectile.tileCollide = true;
                return;
            }
            Projectile.localAI[0] = Projectile.Center.Y;
            distanceToTarget = -20;
            alpha = 0f;
            Projectile.Center = owner.Top + new Vector2(0, distanceToTarget);
            Projectile.rotation = 2.5f;
            Projectile.scale = 0.01f;
            Projectile.velocity = new Vector2(Main.rand.NextFromList(-1, 1), 0);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (State > 1)
                return base.CanHitNPC(target);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24),
                    DustID.UnusedWhiteBluePurple, Helper.NextVec2Dir() * Main.rand.NextFloat(2, 8),
                    Scale: Main.rand.NextFloat(1f, 2.5f));

            }
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            for (int i = 0; i < 8; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24),
                    DustID.MushroomTorch, dir.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(1, 5),
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
                        const int FlowTime = 15;

                        float factor = Timer / FlowTime;
                        float sqrtFactor = Coralite.Instance.SqrtSmoother.Smoother(factor);

                        distanceToTarget = Helper.Lerp(-20, -230, sqrtFactor);
                        alpha = Helper.Lerp(0f, 0.5f, sqrtFactor);
                        Projectile.rotation = Helper.Lerp(2.5f, 0, sqrtFactor);
                        Projectile.scale = Helper.Lerp(0.01f, 1f, sqrtFactor);
                        Projectile.Center = owner.Center + new Vector2(0, distanceToTarget);
                        Timer++;
                        if (Timer > FlowTime)
                        {
                            State++;
                            Timer = 0;
                            Projectile.timeLeft = 60;
                            Projectile.velocity = new Vector2(0, -14);
                            Projectile.extraUpdates++;
                            Projectile.InitOldPosCache(14);
                        }
                    }
                    break;
                case 1://飞升
                    {
                        const int FlyTine = 25;

                        float factor = Timer / FlyTine;
                        float x2Factor = Coralite.Instance.SqrtSmoother.Smoother(factor);

                        Projectile.rotation = Helper.Lerp(0, 1.6f, x2Factor);

                        Projectile.UpdateOldPosCache();

                        Timer++;
                        if (Timer > FlyTine)
                        {
                            State++;
                            Projectile.timeLeft = 80;
                            Projectile.velocity = (owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-1.2f, 1.2f)) * 18;
                            Projectile.rotation = Projectile.velocity.ToRotation();
                            Projectile.InitOldPosCache(14);
                            Projectile.InitOldRotCache(14);

                            SoundEngine.PlaySound(CoraliteSoundID.MagicShoot_Item9, Projectile.Center);

                            int damage = Projectile.damage / 2;
                            if (Main.rand.NextBool())
                                Projectile.NewProjectileFromThis<NoctiflairStrike>(Projectile.Center,
                                    Vector2.Zero, damage, 4, Target, 2);

                            if (Main.rand.NextBool(4))
                                Projectile.NewProjectileFromThis<NoctiflairStrike>(Projectile.Center,
                                    Vector2.Zero, damage, 4, Target, 2);

                            if (Main.rand.NextBool(8))
                                Projectile.NewProjectileFromThis<NoctiflairStrike>(Projectile.Center,
                                    Vector2.Zero, damage, 4, Target, 2);

                            if (Main.rand.NextBool(16))
                                Projectile.NewProjectileFromThis<NoctiflairStrike>(Projectile.Center,
                                    Vector2.Zero, damage, 4, Target, 2);

                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 dir = (i * MathHelper.Pi).ToRotationVector2();
                                for (int j = 0; j < 7; j++)
                                {
                                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.MushroomTorch, dir * (2 + (j * 1.3f)), Scale: 2f - (j * 0.1f));
                                    d.noGravity = true;
                                }
                            }
                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 dir = (MathHelper.PiOver2 + (i * MathHelper.Pi)).ToRotationVector2();
                                for (int j = 0; j < 6; j++)
                                {
                                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.MushroomTorch, dir * (2f + (j * 0.8f)), Scale: 2f - (j * 0.15f));
                                    d.noGravity = true;
                                }
                            }
                            for (int i = 0; i < 16; i++)
                            {
                                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.MushroomTorch,
                                    (i * MathHelper.TwoPi / 16).ToRotationVector2() * 3f, Scale: Main.rand.NextFloat(1.4f, 1.6f));
                                d.noGravity = true;
                            }
                        }
                    }
                    break;
                case 2://下砸
                    {
                        if (!owner.CanBeChasedBy())
                        {
                            Projectile.Kill();
                            return;
                        }
                        Projectile.SpawnTrailDust(DustID.MushroomTorch, Main.rand.NextFloat(0.1f, 0.8f),
                            Scale: Main.rand.NextFloat(0.9f, 1.3f));

                        float length = Projectile.velocity.Length();
                        float factor = Coralite.Instance.X2Smoother.Smoother(Projectile.timeLeft / 80f);
                        Projectile.velocity = Projectile.rotation.AngleLerp((owner.Center - Projectile.Center).ToRotation(), 1 - factor)
                            .ToRotationVector2() * length;
                        Projectile.rotation = Projectile.velocity.ToRotation();
                        if (!Projectile.tileCollide && Projectile.Center.Y > Projectile.localAI[0])
                        {
                            Projectile.tileCollide = true;
                        }

                        Projectile.UpdateOldPosCache();
                        Projectile.UpdateOldRotCache();
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

            if (State == 2)
            {
                Texture2D exTex2 = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "VanillaStarTrail").Value;
                var origin2 = exTex2.Size() / 2;
                float rot2 = Projectile.rotation + 1.57f;
                Vector2 scale = new Vector2(0.8f, 1) * Projectile.scale;
                for (int i = 1; i < 14; i++)
                {
                    Color c = Color.Lerp(new Color(64, 58, 79), new Color(83, 129, 255), i / 22f) * (0.9f * i / 14f);
                    Main.spriteBatch.Draw(exTex2, Projectile.oldPos[i] - Main.screenPosition, null, c, Projectile.oldRot[i] + 1.57f
                        , origin2, scale * (0.7f + (i * 0.3f / 14)), 0, 0);
                }

                Main.spriteBatch.Draw(exTex2, pos, null, new Color(83, 129, 255, 0) * 0.8f, rot2
                    , origin2, new Vector2(Projectile.scale * 0.6f, Projectile.scale), 0, 0);

                Vector2 exPos = pos + (Projectile.rotation.ToRotationVector2() * 28);
                Helper.DrawPrettyStarSparkle(Projectile.Opacity, 0, exPos, Color.White * 0.5f, Color.DeepSkyBlue * 0.8f,
                    1, 0, 1, 1, 2, 0, new Vector2(2.2f, 1), Vector2.One);
                Helper.DrawPrettyStarSparkle(Projectile.Opacity, 0, exPos, Color.White, Color.SkyBlue,
                    1, 0, 1, 1, 2, 0, new Vector2(1.2f, 0.75f), Vector2.One * 1.2f);

                return false;
            }

            Helper.DrawPrettyLine(Projectile.Opacity, 0, pos, Color.White, Color.Gold,
                Timer / 20, 0, 1, 1, 2, -1.57f, 2.3f, Vector2.One);
            Helper.DrawPrettyLine(Projectile.Opacity, 0, pos, Color.White, Color.Gold,
                Timer / 20, 0, 1, 1, 2, -1.57f, 2f, Vector2.One * 1.2f);

            if (State > 0)
            {
                var frameBox2 = mainTex.Frame(3, 1, 2, 0);

                for (int i = 13; i > 1; i--)
                {
                    Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, frameBox2, lightColor * (i * 0.3f / 14), 0
                        , origin, Projectile.scale * (0.5f + (i * 0.5f / 14)), 0, 0);
                }
            }

            //绘制左边翅膀
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, -Projectile.rotation, origin, 1, 0, 0);

            //绘制右边翅膀
            frameBox = mainTex.Frame(3, 1, 1, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, Projectile.rotation, origin, 1, 0, 0);

            //绘制本体
            frameBox = mainTex.Frame(3, 1, 2, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, 0, origin, Projectile.scale, 0, 0);

            return false;
        }
    }
}
