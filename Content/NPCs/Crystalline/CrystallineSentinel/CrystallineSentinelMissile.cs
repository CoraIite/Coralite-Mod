using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.NPCs.Crystalline
{
    /// <summary>
    /// 一阶段的追踪飞弹：可以被打，也会追着玩家绕；命中开盾中的本体会破盾（<see cref="CrystallineSentinel.OnHitByMissile"/>），
    /// 爆炸时对 80 px 内的玩家造成接触伤害。
    /// </summary>
    [VaultLoaden(AssetDirectory.CrystallineNPCs)]
    public class CrystallineSentinelMissile : ModNPC
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        [VaultLoaden("{@classPath}" + "CrystallineSentinelGradientBlack")]
        public static ATex GradientTextureBlack { get; set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelShieldParticle")]
        public static ATex ShieldParticle { get; set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelShieldParticle_Glow")]
        public static ATex ShieldParticleGlow { get; set; }
        public ref float Timer => ref NPC.ai[0];
        public ref float DeathTimer => ref NPC.ai[1];
        public ref float CounterFactor => ref NPC.ai[2];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 3;
            NPC.QuickTrailSets(Helper.NPCTrailingMode.RecordAll, 24);
            NPCID.Sets.ImmuneToRegularBuffs[Type]/* tModPorter NPCID.Sets.ImmuneToAllBuffs was removed. If immunity to whip tag effects are desired, also set NPCID.Sets.ImmuneToWhipTags to true. */ = true;
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 40;
            NPC.damage = 70;
            NPC.lifeMax = 7000;
            NPC.friendly = false;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.SuperArmor = true;
            NPC.aiStyle = -1;
            NPC.HitSound = CoraliteSoundID.CrystalHit_DD2_WitherBeastHurt;
        }

        public override void OnSpawn(IEntitySource source)
        {
            NPC.frame.Y = 2;
            CounterFactor = 4;
            Helper.PlayPitched(AssetDirectory.Sounds.Crystalline + "Sentinel_FloatingStart", 0.4f, 0, NPC.Center);
            Helper.PlayPitched(AssetDirectory.Sounds.Crystalline + "Sentinel_FloatingLoop", 0.4f, 0, NPC.Center, (s) => s.IsLooped = true);
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override bool PreHoverInteract(bool mouseIntersects)
        {
            return false;
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            modifiers.Knockback *= 2.5f;
            DeathTimer = 0;
            modifiers.HideCombatText();
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (NPC.velocity.Length() < 0.6f)
                NPC.velocity = NPC.velocity.ToRotation().ToRotationVector2() * 0.6f;
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (NPC.velocity.Length() < 0.6f)
                NPC.velocity = NPC.velocity.ToRotation().ToRotationVector2() * 0.6f;
        }

        public override bool CanHitNPC(NPC target) => target.type == ModContent.NPCType<CrystallineSentinel>();

        public override void OnHitNPC(NPC target, NPC.HitInfo hit)
        {
            DeathTimer = 1000;

            //命中开盾期间的战斗体时破盾，话说居然没有target的onhitbynpc接口
            if (target.ModNPC is not CrystallineSentinel sentinel)
                return;
            sentinel.OnHitByMissile();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            DeathTimer = 1000;
        }

        public override bool? CanFallThroughPlatforms() => true;

        public override void AI()
        {
            float velDecr = Utils.Remap(Timer, 0, 640, 0.017f, 0.12f);
            NPC.chaseable = false;
            //if (Timer < 30f)
            //    NPC.velocity *= 0.98f;
            if (NPC.velocity.Length() > 0.5f)
                NPC.velocity -= NPC.velocity.SafeNormalize(Vector2.One) * velDecr;
            else if (Timer > 120f)
            {
                DeathTimer++;
            }

            if (DeathTimer > 120)
                NPC.Kill();

            NPC.TargetClosest(false);

            var target = Main.player[NPC.target];
            if (CounterFactor > 0)
            {
                if (target.Alives() && target.Distance(NPC.Center) < 1200f)
                {
                    float chaseFactor = Utils.Remap(Timer, 15, 120, 0.005f, 0.095f) * Utils.Remap(CounterFactor, 0, 10, 0f, 1f);

                    float timeFactor = Utils.Remap(Timer, 0f, 480f, 1f, 0.05f);
                    float speedModifier = Utils.Remap(Timer, 15, 30, 0.01f, 0.15f) * Utils.Remap(CounterFactor, 0, 10, 0.5f, 1f) * timeFactor;
                    Movement(target.Center, speedModifier);
                    //NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(target.Center) * 9f, chaseFactor);
                    //NPC.velocity = NPC.velocity.Length() * Utils.AngleLerp(NPC.velocity.ToRotation(), NPC.AngleTo(target.Center), chaseFactor).ToRotationVector2();

                    var normal = NPC.velocity.SafeNormalize(Vector2.One).RotatedBy(MathHelper.PiOver2);

                    float velFactor = Utils.Remap(NPC.velocity.Length(), 0, 10, 0.03f, 0.5f);
                    Vector2 disturb = normal * MathF.Sin(Timer * 0.02f + NPC.whoAmI) * 0.3f * velFactor;
                    if (NPC.velocity.Length() < 2f)
                        disturb /= 2f;
                    NPC.velocity += disturb;
                }
            }
            //Main.NewText($"{CounterFactor}");
            if (CounterFactor < 0)
                CounterFactor -= 0.01f;
            else
                CounterFactor += 0.01f;
            CounterFactor = MathHelper.Clamp(CounterFactor, 0, 10);
            if (NPC.velocity.Length() > 0.1f)
                NPC.rotation = NPC.velocity.ToRotation();

            float scale = Utils.Remap(NPC.velocity.Length(), 0, 10, 2, 1f);
            {
                NPC.position = NPC.Center;
                //NPC.scale = scale;
                NPC.height = NPC.width = (int)(40 * scale);
                NPC.Center = NPC.position;
            }
            if (Timer > 0 && Timer % 120 == 0 && NPC.frame.Y > 0)
                NPC.frame.Y--;

            if (Timer % 2 == 0)
            {
                float range = Utils.Remap(NPC.velocity.Length(), 0, 10, 33, 11);
                int prtCount = 1;
                if (range > 18)
                    prtCount++;
                for (int i = 0; i < prtCount; i++)
                {
                    Vector2 position = NPC.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, range);
                    Vector2 vel = NPC.DirectionTo(position).RotatedBy(-MathHelper.PiOver2) * Main.rand.NextFloat(1, 4);
                    var prt = PRTLoader.NewParticle<CrystallineFlashParticle>(position - vel * 5, vel * 0.75f + NPC.velocity * 0.25f);
                    prt.Scale /= 2f;
                }
            }

            Timer++;
        }

        public void Movement(Vector2 targetPos, float speedModifier, float cap = 10f)
        {
            if (Math.Abs(NPC.Center.X - targetPos.X) > 5f)
            {
                if (NPC.Center.X < targetPos.X)
                {
                    NPC.velocity.X += speedModifier;
                    if (NPC.velocity.X < 0)
                        NPC.velocity.X += speedModifier * 2;
                }
                else
                {
                    NPC.velocity.X -= speedModifier;
                    if (NPC.velocity.X > 0)
                        NPC.velocity.X -= speedModifier * 2;
                }
            }
            if (NPC.Center.Y < targetPos.Y)
            {
                NPC.velocity.Y += speedModifier;
                if (NPC.velocity.Y < 0)
                    NPC.velocity.Y += speedModifier * 2;
            }
            else
            {
                NPC.velocity.Y -= speedModifier;
                if (NPC.velocity.Y > 0)
                    NPC.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(NPC.velocity.X) > cap)
                NPC.velocity.X = cap * Math.Sign(NPC.velocity.X);
            if (Math.Abs(NPC.velocity.Y) > cap)
                NPC.velocity.Y = cap * Math.Sign(NPC.velocity.Y);
        }

        public override void OnKill()
        {
            int prtCount = 18;
            for (int i = 0; i < prtCount; i++)
            {
                Vector2 pos = NPC.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 12);
                Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 3.5f);
                var prt = PRTLoader.NewParticle<CrystallineFragmentParticle>(pos, vel);
                prt.Scale = Main.rand.NextFloat(0.4f, 1f);
            }

            for (int i = 0; i < 6 * 6; i++)
            {
                Vector2 position = NPC.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(16);
                Vector2 vel = NPC.rotation.ToRotationVector2().RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1, 6);
                PRTLoader.NewParticle<CrystallineFlashParticle>(position, vel * 0.75f + NPC.velocity * 0.25f);
            }

            float range = 80;
            foreach (var p in Main.ActivePlayers)
            {
                if (p.Distance(NPC.Center) > range)
                    continue;

                p.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), NPC.damage, (p.Center.X > NPC.Center.X).ToDirectionInt());
            }

            Helper.PlayPitchedVariants(AssetDirectory.Sounds.Crystalline + "Sentinel_Explosion", 0.4f, 0, 0, 2, NPC.Center);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            CoraliteSystem.InitBars();

            int length = NPC.oldPos.Length - 1;
            for (int i = 0; i < length; i++)
            {
                if (NPC.oldPos[i + 1] == Vector2.Zero || NPC.oldPos[i] == Vector2.Zero)
                    continue;
                float factor = 1 - i / (float)length;

                var normal = (NPC.oldPos[i + 1] - NPC.oldPos[i]).SafeNormalize(Vector2.One).RotatedBy(MathHelper.PiOver2);
                float width = WidthFunction(factor);
                Vector2 top = NPC.oldPos[i] + NPC.Size / 2 + normal * width;
                Vector2 bottom = NPC.oldPos[i] + NPC.Size / 2 - normal * width;
                Color color = ColorFunction(factor);

                CoraliteSystem.Vertexes.Add(new(top, color, new Vector3(factor, 0, 0)));
                CoraliteSystem.Vertexes.Add(new(bottom, color, new Vector3(factor, 1, 0)));
            }

            if (CoraliteSystem.Vertexes.Count > 2)
            {
                Effect effect = ShaderLoader.GetShader("TurbulenceArrow");

                effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());

                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.05f);
                effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
                effect.Parameters["udissolveS"].SetValue(0.5f);
                effect.Parameters["uBaseImage"].SetValue(CoraliteAssets.Laser.Body.Value);
                effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.AirFlow2.Value);
                effect.Parameters["uGradient"].SetValue(GradientTextureBlack.Value);
                effect.Parameters["uDissolve"].SetValue(CoraliteAssets.Laser.VanillaFlowA.Value);
                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    var arr = CoraliteSystem.Vertexes.ToArray();
                    Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;

                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, arr, 0, CoraliteSystem.Vertexes.Count - 2);
                }
            }

            Texture2D mainTex = NPC.GetTexture();
            Vector2 pos = NPC.Center - screenPos;
            var frameBox = mainTex.Frame(1, 3, NPC.frame.X, NPC.frame.Y);
            Vector2 origin = frameBox.Size() / 2;

            //绘制本体
            spriteBatch.Draw(mainTex, pos, frameBox, Color.Lerp(drawColor, Color.White, 0.5f), NPC.rotation + MathHelper.PiOver2, origin, NPC.scale * 1.5f, 0, 0);

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
                float maxRadius = Utils.Remap(NPC.velocity.Length(), 0f, 10f, 80f, 20f);
                float radius = Utils.MultiLerp(factor, [0, 0.25f, 0.5f, 1f]) * maxRadius;
                float visualSpeed = /*Utils.Remap(factor, 0f, 1f, 0.05f, 0.01f);*/0.01f;
                float dir = (float)(-Timer * rotSpeed + i * 1214f - Main.timeForVisualEffects * visualSpeed);
                Vector2 targetPos = NPC.Center + dir.ToRotationVector2() * radius - Main.screenPosition;

                int trailLength = (int)(MathHelper.TwoPi * radius / 16f);
                Vector2 scale = new(0.2f, 0.3f);
                float iTimeFactor = MathF.Sin((float)(Main.timeForVisualEffects * 0.04f + i * 91208)) * 0.5f + 0.5f;
                for (int j = 0; j < trailLength; j++)
                {
                    float trailFactor = j / (float)trailLength;
                    float trailDir = dir + MathHelper.Pi * 2f / 3f * trailFactor;
                    Vector2 trailPos = NPC.Center + trailDir.ToRotationVector2() * radius - Main.screenPosition;
                    float alpha = Utils.Remap(trailFactor, 0, 1f, 1, 0f) * 0.5f * fadeinFactor * iTimeFactor;
                    Main.spriteBatch.Draw(star, trailPos, null, Color.Violet with { A = 0 } * alpha, trailDir, star.Size() / 2, scale, 0, 0);
                }

                var frameBox = ShieldParticle.Frame(1, 13, 0, (NPC.whoAmI * 12901 + i * 109) % 13);

                float shieldScale = 0.75f;
                Main.spriteBatch.Draw(ShieldParticle.Value, targetPos, frameBox, Color.White * fadeinFactor * 0.5f * iTimeFactor, dir + NPC.whoAmI * 634f, frameBox.Size() / 2, shieldScale, 0, 0);
                Main.spriteBatch.Draw(ShieldParticleGlow.Value, targetPos, frameBox, Color.White * fadeinFactor * iTimeFactor, dir + NPC.whoAmI * 634f, frameBox.Size() / 2, shieldScale, 0, 0);
            }
        }

        public static float WidthFunction(float factor) => Utils.Remap(factor, 0, 1, 2, 18);

        public static Color ColorFunction(float factor) => Color.Lerp(Color.Black, Color.White, factor) * Utils.Remap(factor, 0, 1, 0, 1);
    }
}
