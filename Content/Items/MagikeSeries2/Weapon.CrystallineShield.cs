using Coralite.Content.DamageClasses;
using Coralite.Content.NPCs.Crystalline;
using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class CrystallineShield : BaseFlyingShieldItem<CrystallineShieldGuard>
    {
        public CrystallineShield() : base(Item.sellPrice(0, 1, 50), ModContent.RarityType<CrystallineMagikeRarity>(), AssetDirectory.MagikeSeries2Item)
        { }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<CrystallineShieldProj>();
            Item.knockBack = 6.5f;
            Item.shootSpeed = 16;
            Item.damage = 50;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrystallineEngram>()
                .AddTile<SkarnCutterTile>()
                .Register();
        }
    }

    public class CrystallineShieldProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 42;
        }

        public override void SetOtherValues()
        {
            flyingTime = 16;
            backTime = 22;
            backSpeed = 17;
            trailCachesLength = 6;
            trailWidth = 30 / 2;
        }

        public override void OnShootDusts()
        {
        }

        public override Color GetColor(float factor)
        {
            return new Color(32, 180, 186, 0) * factor;
        }

        public override void DrawSelf(Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTextureValue();
            var pos = Projectile.Center - Main.screenPosition;

            Rectangle frameBox = mainTex.Frame(2, 1, 0, 0);

            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, Projectile.rotation - 1.57f + extraRotation, frameBox.Size() / 2, Projectile.scale, 0, 0);
        }
    }

    public class CrystallineShieldGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 44;
            Projectile.height = 54;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.15f;
        }

        public override void DrawSelf(Texture2D mainTex, Vector2 pos, float rotation, Color lightColor, Vector2 scale, SpriteEffects effect)
        {
            Rectangle frameBox;
            Vector2 rotDir = Projectile.rotation.ToRotationVector2();
            Vector2 dir = rotDir * (DistanceToOwner / (Projectile.width * scalePercent));
            Color c = lightColor * 0.6f;
            c.A = lightColor.A;

            frameBox = mainTex.Frame(2, 1, 0, 0);
            Vector2 origin2 = frameBox.Size() / 2;

            //绘制基底
            Main.spriteBatch.Draw(mainTex, pos - (dir * 5), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上部
            frameBox = mainTex.Frame(2, 1, 1, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 5), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 10), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 15), frameBox, lightColor, rotation, origin2, scale, effect, 0);
        }
    }

    [VaultLoaden(AssetDirectory.CrystallineNPCs)]
    public class CrystallineShieldExpand : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        [VaultLoaden("{@classPath}" + "CrystallineSentinelGradientBlack")]
        public static ATex GradientTextureBlack { get; set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelShieldParticle")]
        public static ATex ShieldParticle { get; set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelShieldParticle_Glow")]
        public static ATex ShieldParticleGlow { get; set; }
        public ref float Timer => ref Projectile.ai[0];
        public ref float DeathTimer => ref Projectile.ai[1];
        public ref float ProjOwner => ref Projectile.ai[2];
        private PRTGroup shardGroup;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = MagikeDamage.Instance;
        }

        public override void AI()
        {
            if (!VaultUtils.isServer)
                shardGroup ??= [];

            float velDecr = Utils.Remap(Timer, 0, 640, 0.017f, 0.12f);
             if (Timer > 120f)
            {
                DeathTimer++;
            }

            if (DeathTimer > 120)
                Projectile.Kill();


            if (!VaultUtils.isServer)
            {
                shardGroup?.Update();
            }

            float scale = Timer / 30f*2;
            {
                int width = (int)(40 * scale);
                Projectile.Resize(width, width);
            }

            if (Timer % 2 == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 position = Projectile.Center + Helper.NextVec2Dir(1, 3);
                    Vector2 vel = Projectile.DirectionTo(position).RotatedBy(-MathHelper.PiOver2) * Main.rand.NextFloat(1, 4);
                    var prt = PRTLoader.NewParticle<CrystallineFlashParticle>(position - vel * 5, vel * 0.75f + Projectile.velocity * 0.25f);
                    prt.Scale /= 2f;
                }
            }

            Timer++;
        }

        public override void OnKill(int timeLeft)
        {
            int prtCount = 5;
            for (int i = 0; i < prtCount; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 12);
                Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 3.5f);
                var prt = PRTLoader.NewParticle<CrystallineFragmentParticle>(pos, vel);
                prt.Scale = Main.rand.NextFloat(0.4f, 1f);
            }

            for (int i = 0; i < 6 * 6; i++)
            {
                Vector2 position = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(16);
                Vector2 vel = Projectile.rotation.ToRotationVector2().RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1, 6);
                PRTLoader.NewParticle<CrystallineFlashParticle>(position, vel * 0.75f);
            }

            Helper.PlayPitchedVariants(AssetDirectory.Sounds.Crystalline + "Sentinel_Explosion", 0.4f, 0, 0, 2, Projectile.Center);
        }

        public override bool PreDraw(ref Color drawColor)
        {
            //CoraliteSystem.InitBars();

            //int length = NPC.oldPos.Length - 1;
            //float timer = /*(float)(Main.timeForVisualEffects * 0.05f)*/0;
            //for (int i = 0; i < length; i++)
            //{
            //    if (NPC.oldPos[i + 1] == Vector2.Zero || NPC.oldPos[i] == Vector2.Zero)
            //        continue;
            //    float factor = 1 - i / (float)length;

            //    var normal = (NPC.oldPos[i + 1] - NPC.oldPos[i]).SafeNormalize(Vector2.One).RotatedBy(MathHelper.PiOver2);
            //    float width = WidthFunction(factor);
            //    Vector2 top = NPC.oldPos[i] + NPC.Size / 2 + normal * width;
            //    Vector2 bottom = NPC.oldPos[i] + NPC.Size / 2 - normal * width;
            //    Color color = ColorFunction(factor);

            //    CoraliteSystem.Vertexes.Add(new(top, color, new Vector3(factor + timer, 0, 0)));
            //    CoraliteSystem.Vertexes.Add(new(bottom, color, new Vector3(factor + timer, 1, 0)));
            //}

            //if (CoraliteSystem.Vertexes.Count > 2)
            //{
            //    Effect effect = ShaderLoader.GetShader("TurbulenceArrow");

            //    effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());

            //    effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.05f);
            //    effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
            //    effect.Parameters["udissolveS"].SetValue(0.5f);
            //    effect.Parameters["uBaseImage"].SetValue(CoraliteAssets.Laser.Body.Value);
            //    effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.AirFlow2.Value);
            //    effect.Parameters["uGradient"].SetValue(GradientTextureBlack.Value);
            //    effect.Parameters["uDissolve"].SetValue(CoraliteAssets.Laser.VanillaFlowA.Value);
            //    foreach (var pass in effect.CurrentTechnique.Passes)
            //    {
            //        pass.Apply();
            //        var arr = CoraliteSystem.Vertexes.ToArray();
            //        Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;

            //        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, arr, 0, CoraliteSystem.Vertexes.Count - 2);
            //    }
            //}
            shardGroup?.Draw(Main.spriteBatch);

            DrawShard();

            //Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, $"{CounterFactor}", NPC.position.X - Main.screenPosition.X, NPC.position.Y - Main.screenPosition.Y, Color.White, Color.Black, Vector2.Zero);
            return false;
        }

        public void DrawShard()
        {
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;

            var baseTime = 30;
            var step = 15;
            float maxShardCount = 14f;
            int drawcount = (int)Utils.Remap(Timer, baseTime, baseTime + step * 6, 1, maxShardCount + 1);
            for (int i = 0; i < drawcount; i++)
            {
                float fadeinFactor = Utils.Remap(Timer, baseTime + i * step, baseTime + i * step * 2, 0f, 1f);
                float factor = (i + 1f) / (maxShardCount + 1);
                float rotSpeed = Utils.Remap(factor, 0f, 1f, 0.05f, 0.02f);
                float maxRadius = Utils.Remap(Projectile.velocity.Length(), 0f, 10f, 80f, 20f);
                float radius = Utils.MultiLerp(factor, [0, 0.25f, 0.5f, 1f]) * maxRadius;
                float visualSpeed = /*Utils.Remap(factor, 0f, 1f, 0.05f, 0.01f);*/0.01f;
                float dir = (float)(-Timer * rotSpeed + i * 1214f - Main.timeForVisualEffects * visualSpeed);
                Vector2 targetPos = Projectile.Center + dir.ToRotationVector2() * radius - Main.screenPosition;

                int trailLength = (int)(MathHelper.TwoPi * radius / 16f);
                Vector2 scale = new(0.2f, 0.3f);
                float iTimeFactor = MathF.Sin((float)(Main.timeForVisualEffects * 0.04f + i * 91208)) * 0.5f + 0.5f;
                for (int j = 0; j < trailLength; j++)
                {
                    float trailFactor = j / (float)trailLength;
                    float trailDir = dir + MathHelper.Pi * 2f / 3f * trailFactor;
                    Vector2 trailPos = Projectile.Center + trailDir.ToRotationVector2() * radius - Main.screenPosition;
                    float alpha = Utils.Remap(trailFactor, 0, 1f, 1, 0f) * 0.5f * fadeinFactor * iTimeFactor;
                    Main.spriteBatch.Draw(star, trailPos, null, Color.Violet with { A = 0 } * alpha, trailDir, star.Size() / 2, scale, 0, 0);
                }

                var frameBox = ShieldParticle.Frame(1, 13, 0, (Projectile.whoAmI * 12901 + i * 109) % 13);

                float shieldScale = 0.75f;
                Main.spriteBatch.Draw(ShieldParticle.Value, targetPos, frameBox, Color.White * fadeinFactor * 0.5f * iTimeFactor, dir + Projectile.whoAmI * 634f, frameBox.Size() / 2, shieldScale, 0, 0);
                Main.spriteBatch.Draw(ShieldParticleGlow.Value, targetPos, frameBox, Color.White * fadeinFactor * iTimeFactor, dir + Projectile.whoAmI * 634f, frameBox.Size() / 2, shieldScale, 0, 0);
            }
        }

        public static float WidthFunction(float factor) => Utils.Remap(factor, 0, 1, 2, 18);

        public static Color ColorFunction(float factor) => Color.Lerp(Color.Black, Color.White, factor) * Utils.Remap(factor, 0, 1, 0, 1);
    }
}
