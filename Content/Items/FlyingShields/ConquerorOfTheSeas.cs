using Coralite.Content.Dusts;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class ConquerorOfTheSeas : BaseFlyingShieldItem<ConquerorOfTheSeasGuard>
    {
        public ConquerorOfTheSeas() : base(Item.sellPrice(0, 10), ItemRarityID.Red, AssetDirectory.FlyingShieldItems)
        { }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 20;
            Item.shoot = ModContent.ProjectileType<ConquerorOfTheSeasProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 14f;
            Item.damage = 225;
            Item.crit = 6;
        }
    }

    public class ConquerorOfTheSeasProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "ConquerorOfTheSeas";

        private bool hited;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 50;
        }

        public override void SetOtherValues()
        {
            flyingTime = 12;
            backTime = 14;
            backSpeed = 14.5f;
            trailCachesLength = 8;
            trailWidth = 30 / 2;
        }

        public override void OnShootDusts()
        {
            Projectile.SpawnTrailDust(DustID.Water, Main.rand.NextFloat(0.1f, 0.7f));
        }

        public override Color GetColor(float factor)
        {
            return new Color(122, 122, 156) * factor;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!hited && Projectile.IsOwnedByLocalPlayer())
            {
                hited = true;
                int damage = (int)(Projectile.damage * 0.8f);
                if (State == (int)FlyingShieldStates.Shooting)
                {
                    Projectile.NewProjectileFromThis<ConquerorShipAnchor>(Projectile.Center
                        , Projectile.velocity.SafeNormalize(Vector2.Zero) * 20, damage, Projectile.knockBack);

                    Helper.PlayPitched(CoraliteSoundID.MinecartTrack_Item52, Projectile.Center, pitch: -0.5f);
                }

                Projectile.NewProjectileFromThis<ConquerorWaterWave>(Projectile.Center
                    , Vector2.Zero, damage, Projectile.knockBack);

                for (int i = 0; i < 3; i++)
                    ConquerorOfTheSeasGuard.TentacleDust(Projectile, (i % 2 == 0 ? 1 : -1) * i * 0.3f);

                Helper.PlayPitched(CoraliteSoundID.Water_Splash, Projectile.Center,volumeAdjust:-0.2f, pitch: 0.3f);
            }

            base.OnHitNPC(target, hit, damageDone);
        }
    }

    public class ConquerorOfTheSeasGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 58;
            Projectile.height = 62;
            Projectile.scale = 1.25f;
            Projectile.localNPCHitCooldown = 24;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.35f;
            distanceAdder = 2.6f;
            strongGuard = 0.15f;
            scalePercent = 1.3f;
        }

        public override void OnHoldShield()
        {
            if (Projectile.frameCounter > 0)
            {
                Projectile.frameCounter--;
                if (Projectile.frameCounter == 0)
                {
                    Projectile.frame = 0;
                    return;
                }

                if (Projectile.frameCounter % 3 == 0)
                    if (++Projectile.frame > 4)
                        Projectile.frame = 1;
            }
        }

        public override void OnGuard()
        {
            base.OnGuard();
            Projectile.frameCounter = 3 * 4 * Main.rand.Next(8, 12);
        }

        public override void OnGuardNPC(int npcIndex)
        {
            Projectile.NewProjectileFromThis<ConquerorSlash>(Owner.Center, Vector2.Zero, Projectile.damage, Projectile.knockBack,
                Main.rand.Next(3), Projectile.rotation);
            int howMany = Main.rand.Next(2, 4);
            for (int i = 0; i < howMany; i++)
                TentacleDust(Projectile);
        }

        public override void OnGuardProjectile(int projIndex)
        {
            Projectile.NewProjectileFromThis<ConquerorFlyProj>(Owner.Center, Projectile.rotation.ToRotationVector2() * 12,
                (int)(Projectile.damage * 0.85f), Projectile.knockBack);
            int howMany = Main.rand.Next(2, 4);
            for (int i = 0; i < howMany; i++)
                TentacleDust(Projectile);
        }

        public static void TentacleDust(Projectile Projectile, float exrot = 0)
        {
            float baseAngle = Projectile.rotation + Main.rand.NextFloat(-0.5f, 0.5f) + exrot;
            float exRot = Main.rand.NextFloat(MathHelper.TwoPi);
            float exRot2 = Main.rand.NextFloat(0.05f, 0.15f);
            float vel = Main.rand.NextFloat(0.4f, 0.9f);
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(8, 8);

            for (int i = 0; i < 16; i++)
            {
                Vector2 dir = (baseAngle + (MathF.Sin(i + exRot) * exRot2)).ToRotationVector2();
                Dust d = Dust.NewDustPerfect(pos, DustID.GemEmerald, dir * (i * vel), Scale: 2f - (i * 0.05f));
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
            Color c = lightColor * 0.7f;
            c.A = lightColor.A;
            Color c2 = lightColor * 0.5f;
            c2.A = lightColor.A;

            frameBox = mainTex.Frame(5, 2, 0, 1);
            Vector2 origin2 = frameBox.Size() / 2;

            //绘制基底
            Main.spriteBatch.Draw(mainTex, pos - (dir * 5), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上部
            frameBox = mainTex.Frame(5, 2, Projectile.frame, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 4), frameBox, c2, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 9), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 14), frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //
            frameBox = mainTex.Frame(5, 2, 1, 1);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 14), frameBox, c2, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 19), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 24), frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //
            frameBox = mainTex.Frame(5, 2, 2, 1);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 24), frameBox, c2, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 28), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 32), frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //
            frameBox = mainTex.Frame(5, 2, 3, 1);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 32), frameBox, c2, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 36), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 40), frameBox, lightColor, rotation, origin2, scale, effect, 0);
        }
    }

    public class ConquerorSlash : BaseSwingProj
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "ConquerorOfTheSeasProj2";

        public ref float Combo => ref Projectile.ai[0];
        public ref float StartAngle => ref Projectile.ai[1];

        public ConquerorSlash() : base(-MathHelper.PiOver2, trailCount: 35) { }

        public int delay;
        public int alpha;

        public static Asset<Texture2D> GradientTexture;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            GradientTexture = ModContent.Request<Texture2D>(AssetDirectory.FlyingShieldItems + "ConquerorGradient");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            GradientTexture = null;
        }

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAllAndFollowPlayer, 10);
        }

        public override void SetSwingProperty()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 48;
            Projectile.width = 40;
            Projectile.height = 80;
            trailTopWidth = -15;
            distanceToOwner = 20;
            minTime = 0;
            onHitFreeze = 0;
            useShadowTrail = true;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 100;
        }

        protected override float GetStartAngle() => StartAngle;

        protected override void InitializeSwing()
        {
            Projectile.extraUpdates = 3;
            alpha = 255;
            switch (Combo)
            {
                default:
                case 0: //下挥，较为椭圆
                    startAngle = 1.6f;
                    totalAngle = 3.8f;
                    maxTime = 55;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    Projectile.scale = 0.9f;
                    delay = 24;
                    break;
                case 1: //重下挥
                    startAngle = 2.2f;
                    totalAngle = 4.6f;
                    maxTime = 55;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    delay = 28;
                    Projectile.scale = 0.8f;
                    break;
                case 2://上挥 较圆
                    startAngle = -1.6f;
                    totalAngle = -4.6f;
                    maxTime = 55;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    delay = 12;
                    break;
            }

            base.InitializeSwing();
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Owner.Center, Coralite.IcicleCyan.ToVector3());
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;
            Dust d = Dust.NewDustPerfect(Projectile.Center + (RotateVec2 * Main.rand.NextFloat(-20, 20)), DustID.FireflyHit,
                RotateVec2.RotatedBy(1.57f) * Main.rand.NextFloat(1, 4), 150);
            d.noGravity = true;
            switch ((int)Combo)
            {
                default:
                case 0:
                case 1:
                case 2:

                    if (timer < 24)
                        distanceToOwner = Helper.EllipticalEase(1.6f - (3.8f * Smoother.Smoother(timer, maxTime - minTime)), 25, 50);
                    else
                        distanceToOwner -= 0.8f;

                    break;
            }

            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            if (alpha > 20)
                alpha -= 10;
            Slasher();
            distanceToOwner *= 0.9f;
            if (Timer > maxTime + delay)
                Projectile.Kill();
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, mainTex.Frame(),
                                                lightColor, Projectile.rotation + extraRot, origin, Projectile.scale + 0.2f, CheckEffect(), 0f);
        }

        protected override void DrawShadowTrail(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            Vector2 toCenter = new(Projectile.width / 2, Projectile.height / 2);

            SpriteEffects effect = CheckEffect();
            for (int i = 1; i < 10; i += 1)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                lightColor * (0.5f - (i * 0.5f / 10)), Projectile.oldRot[i] + extraRot, origin, Projectile.scale, effect, 0);
        }

        protected override void DrawSlashTrail()
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            List<VertexPositionColorTexture> bars = new();
            GetCurrentTrailCount(out float count);

            for (int i = 0; i < count; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - (i / count);
                Vector2 Center = GetCenter(i);
                Vector2 Top = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]));
                Vector2 Bottom = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] - ControlTrailBottomWidth(factor) + oldDistanceToOwner[i]));

                var topColor = Color.Lerp(new Color(238, 218, 130, alpha), new Color(167, 127, 95, 0), 1 - factor);
                bars.Add(new(Top.Vec3(), topColor, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), topColor, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

                Effect effect = ShaderLoader.GetShader("NoHLGradientTrail");

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.SlashFlatFade.Value);
                effect.Parameters["gradientTexture"].SetValue(GradientTexture.Value);

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }

    public class ConquerorFlyProj : ModProjectile
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "ConquerorOfTheSeasProj1";

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 6);
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 32;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.timeLeft = 200;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 3;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.2f, 0.15f));
            Projectile.SpawnTrailDust(Main.rand.NextBool() ? DustID.GemEmerald : DustID.WoodFurniture, Main.rand.NextFloat(0.2f, 0.4f));
            Projectile.rotation += Projectile.velocity.X * 0.05f;

            int npcIndex = -1;
            Vector2 vector52 = Projectile.Center;
            float num607 = 500f;
            if (Projectile.localAI[0] > 0f)
                Projectile.localAI[0]--;

            if (Projectile.ai[0] == 0f && Projectile.localAI[0] == 0f)
            {
                for (int i = 0; i < 200; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(this) && (Projectile.ai[0] == 0f || Projectile.ai[0] == i + 1))
                    {
                        Vector2 center7 = npc.Center;
                        float num609 = Vector2.Distance(center7, vector52);
                        if (num609 < num607 && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                        {
                            num607 = num609;
                            vector52 = center7;
                            npcIndex = i;
                        }
                    }
                }

                if (npcIndex >= 0)
                {
                    Projectile.ai[0] = npcIndex + 1;
                    Projectile.netUpdate = true;
                }
            }

            if (Projectile.localAI[0] == 0f && Projectile.ai[0] == 0f)
                Projectile.localAI[0] = 30f;

            bool flag33 = false;
            if (Projectile.ai[0] != 0f)
            {
                int num610 = (int)(Projectile.ai[0] - 1f);
                if (Main.npc[num610].active && !Main.npc[num610].dontTakeDamage && Projectile.localNPCImmunity[num610] == 0)
                {
                    float num611 = Main.npc[num610].position.X + (Main.npc[num610].width / 2);
                    float num612 = Main.npc[num610].position.Y + (Main.npc[num610].height / 2);
                    float num613 = Math.Abs(Projectile.position.X + (Projectile.width / 2) - num611) + Math.Abs(Projectile.position.Y + (Projectile.height / 2) - num612);
                    if (num613 < 1000f)
                    {
                        flag33 = true;
                        vector52 = Main.npc[num610].Center;
                    }
                }
                else
                {
                    Projectile.ai[0] = 0f;
                    flag33 = false;
                    Projectile.netUpdate = true;
                }
            }

            if (flag33)
            {
                Vector2 v6 = vector52 - Projectile.Center;
                float num614 = Projectile.velocity.ToRotation();
                float num615 = v6.ToRotation();
                double num616 = num615 - num614;
                if (num616 > Math.PI)
                    num616 -= MathHelper.TwoPi;

                if (num616 < -Math.PI)
                    num616 += MathHelper.TwoPi;

                Projectile.velocity = Projectile.velocity.RotatedBy(num616 * 0.10000000149011612);
            }

            float num617 = Projectile.velocity.Length();
            Projectile.velocity.Normalize();
            Projectile.velocity *= num617 + 0.0025f;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.WoodFurniture, Helper.NextVec2Dir(2, 8), Scale: Main.rand.NextFloat(1, 1.6f));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;

            Projectile.DrawShadowTrails(lightColor, 0.5f, 0.5f / 6, 1, 6, 1);

            Main.spriteBatch.Draw(mainTex, pos, null, lightColor, Projectile.rotation, mainTex.Size() / 2, Projectile.scale, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, null, new Color(92, 202, 158, 0), Projectile.rotation, mainTex.Size() / 2, Projectile.scale * 1.2f, 0, 0);

            return false;
        }
    }

    public class ConquerorWaterWave : ModProjectile,IDrawNonPremultiplied,IPostDrawAdditive
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 12);
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 80;
            Projectile.scale = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = 1;
                Projectile.rotation = Main.rand.NextFloat(6.282f);
            }

            if (Projectile.frame < 10)
                for (int i = 0; i < 3; i++)
                {
                    Vector2 pos = Helper.NextVec2Dir(Projectile.frame * 3.5f, Projectile.frame * 10);
                    Dust d = Dust.NewDustPerfect(Projectile.Center + pos, DustID.Water_Desert, pos.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1, 3f)
                        , 0, Color.White with { A = 0 }, Scale: Main.rand.NextFloat(0.8f, 1.2f));
                    d.noGravity = true;
                }

            if (++Projectile.frameCounter > 1)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 18)
                    Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            if (Projectile.frame < 15)
                Projectile.QuickFrameDraw(new Rectangle(0, Projectile.frame, 1, 16), Color.White with { A = 50 }, 0);
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Color c = Color.White;
            c.A = 40;

            if (Projectile.frame < 15)
                Projectile.QuickFrameDraw(new Rectangle(0, Projectile.frame, 1, 16)
                , c, MathHelper.TwoPi / 3);
            Projectile.QuickFrameDraw(new Rectangle(0, Projectile.frame - 3, 1, 16)
                , c, MathHelper.TwoPi * 2 / 3, 1.5f);
        }
    }

    public class ConquerorShipAnchor : ModProjectile
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "ConquerorOfTheSeasProj2";

        public ref float Timer => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 12);
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 48;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            Timer++;

            if (Timer % 2 == 0 && Main.rand.NextBool(2))
            {
                PRTLoader.NewParticle<PixelLine>(Projectile.Center + Main.rand.NextVector2Circular(12, 12)
                    , Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2, 3), Color.LightSteelBlue);
            }

            if (Timer > 50)
            {
                Projectile.Kill();
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color c = Color.LightSlateGray;
            if (Timer > 20)
            {
                lightColor *= 1 - (Timer - 20) / 30;
                c *= 1 - (Timer - 20) / 30;
            }

            DrawShadowTrails(TextureAssets.Extra[98].Value, Projectile
                , c * 0.5f, 1, 1 / 12f, 1, 12, 1, 1 / 12f, -1.57f, 1.5f);

            float rot = (Timer / 40 + Main.GlobalTimeWrappedHourly) % 1;
            float rotation = Projectile.rotation - 1.57f;

            var mainTex = Projectile.GetTexture();
            Vector2 pos = Projectile.Center - Main.screenPosition;

            Rectangle frame = mainTex.Frame(2, 1, 1, 0);
            Vector2 origin = new Vector2(mainTex.Width / 2, mainTex.Height / 2);
            Vector2 origin2 = new Vector2(0, mainTex.Height / 2);
            Main.spriteBatch.Draw(mainTex, pos, frame, lightColor, rotation
                , origin2, new Vector2(1 - rot, 1), 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, frame, lightColor, rotation
                , origin, new Vector2(rot, 1), SpriteEffects.FlipHorizontally, 0);

            frame = mainTex.Frame(2, 1, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, frame, lightColor, rotation
                , origin2, new Vector2(rot, 1), SpriteEffects.FlipHorizontally, 0);
            Main.spriteBatch.Draw(mainTex, pos, frame, lightColor, rotation
                , origin, new Vector2(1 - rot, 1), 0, 0);

            return false;
        }

        public static void DrawShadowTrails(Texture2D tex,Projectile projectile,Color drawColor, float maxAlpha, float alphaStep, int start, int howMany, int step, float scaleStep, float extraRot = 0, float scale = -1)
        {
            Texture2D mainTex = tex;
            Vector2 toCenter = new(projectile.width / 2, projectile.height / 2);

            for (int i = start; i < howMany; i += step)
                Main.spriteBatch.Draw(mainTex, projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                    drawColor * (maxAlpha - (i * alphaStep)), projectile.oldRot[i] + extraRot, mainTex.Size() / 2, (scale == -1 ? projectile.scale : scale) * (1 - (i * scaleStep)), 0, 0);
        }

    }
}
