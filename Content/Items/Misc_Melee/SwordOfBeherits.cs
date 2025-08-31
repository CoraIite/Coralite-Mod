using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Humanizer;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Misc_Melee
{
    public class SwordOfBeherits : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Melee + Name;

        public List<int> randZrot;
        public List<int> randEXScale;

        public int combo = 1;
        public float rot;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 226;
            Item.useTime = Item.useAnimation = 8;
            Item.knockBack = 4f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.shoot = ProjectileType<SwordOfBeheritsSlash>();

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            randZrot ??= new List<int>();
            randEXScale ??= new List<int>();

            if (randZrot.Count < 1)
                randZrot.AddRange([1, 2, 3, 4, 5]);
            if (randEXScale.Count < 1)
                randEXScale.AddRange([-3, -2, -1, 1, 2, 3]);

            int zrot = Main.rand.NextFromList([.. randZrot]);
            randZrot.Remove(zrot);
            int exScale = Main.rand.NextFromList([.. randEXScale]);
            randEXScale.Remove(exScale);

            if (Main.rand.NextBool(2, 3))
                combo *= -1;

            Helper.PlayPitched("Misc/Slash", 0.5f, Main.rand.NextFloat(-0.1f, 0.1f), player.Center);
            Projectile.NewProjectile(source, player.Center, Vector2.Zero,
                type, (int)(damage * 1.6f), knockback, player.whoAmI, combo, zrot, exScale);

            //player.itemTime = 8;

            Vector2 posOffset = rot.ToRotationVector2() * Main.rand.NextFloat(-20, 80)
                + Helper.NextVec2Dir(0, 30);
            if (Main.rand.NextBool(4))
            {
                posOffset += (rot + MathHelper.PiOver2).ToRotationVector2() * Main.rand.NextFloat(40, 80);
            }

            Projectile.NewProjectile(source, Main.MouseWorld+ posOffset, Vector2.Zero,
                ProjectileType<BeheritsSpacial>(), damage, knockback, player.whoAmI, rot);

            rot += Main.rand.NextFloat(MathHelper.PiOver4/2, MathHelper.Pi/3*2);
            return false;
        }

        public override bool AllowPrefix(int pre)
        {
            return true;
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
        }
    }

    [VaultLoaden(AssetDirectory.Misc_Melee)]
    public class SwordOfBeheritsSlash : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.Misc_Melee + "SwordOfBeherits";

        public ref float Combo => ref Projectile.ai[0];
        public ref float ZRotBase => ref Projectile.ai[1];
        public ref float EXScaleBase => ref Projectile.ai[2];

        [VaultLoaden("{@classPath}" + "SwordOfBeheritsGradient")]
        public static ATex GradientTexture { get; set; }

        public SwordOfBeheritsSlash() : base(0.785f, trailCount: 48) { }

        public int delay;
        public int alpha;

        private float recordStartAngle;
        private float recordTotalAngle;
        private float extraScaleAngle;
        private float zRot;

        public const int ChannelTimeMax = 90 * 4;

        public override void SetSwingProperty()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 60;
            Projectile.width = 40;
            Projectile.height = 160;
            trailTopWidth = 20;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 0;
            useSlashTrail = true;
            Projectile.hide = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 95 * Projectile.scale;
        }

        protected override void InitializeSwing()
        {
            if (Projectile.IsOwnedByLocalPlayer())
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 6;
            alpha = 0;
            zRot = ZRotBase * 0.11f;
            onHitFreeze = 14;
            switch (Combo)
            {
                default:
                case 0: //下挥
                    startAngle = 1.6f + Main.rand.NextFloat(-0.2f, 0.3f);
                    totalAngle = 4f + Main.rand.NextFloat(-0.2f, 0.5f);
                    maxTime = 90;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    ExtraInit();

                    break;
                case 1://下挥，圆
                    startAngle = -1.6f + Main.rand.NextFloat(-0.3f, 0.2f);
                    totalAngle = -4f + Main.rand.NextFloat(-0.5f, 0.2f);
                    minTime = 0;
                    maxTime = 90;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    ExtraInit();

                    break;
            }

            base.InitializeSwing();
        }

        private void ExtraInit()
        {
            extraScaleAngle = EXScaleBase * 0.13f;
            recordStartAngle = Math.Abs(startAngle);
            recordTotalAngle = Math.Abs(totalAngle);
            Projectile.scale = Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(0, maxTime - minTime)), 1.05f - zRot, 1.05f + zRot * 0.75f);
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 1f, 0.15f, 0.4f);
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;
            float scale = 1f;

            if (Main.rand.NextBool(8))
            {
                Vector2 dir = RotateVec2.RotatedBy(1.57f * Math.Sign(totalAngle));
                Dust dust = Dust.NewDustPerfect(Top - (20 * RotateVec2) + Main.rand.NextVector2Circular(18, 18), DustID.LifeDrain,
                       dir * Main.rand.NextFloat(0.5f, 2f),255,Color.Transparent, Scale: Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;
            }

            alpha = (int)(Helper.X2Ease(timer, maxTime - minTime) * 100) + 100;
            if (Item.type == ItemType<SwordOfBeherits>())
            {
                scale = Owner.GetAdjustedItemScale(Item);
                scale = (1.5f * scale) - 0.5f;
                if (scale > 3f)
                    scale = 3f;
            }
            else
                Projectile.Kill();

            if (Combo < 3)
                Projectile.scale = scale * Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(timer, maxTime - minTime)), 1.05f - zRot, 1.05f + zRot * 0.75f);
            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            if (alpha > 20)
                alpha -= 10;
            if (Projectile.scale > 0.8f)
                Projectile.scale *= 0.999f;
            Slasher();
            if (Timer > maxTime + delay)
                Projectile.Kill();
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.8f);

            if (onHitTimer == 0)
            {
                onHitTimer = 1;
                if (VaultUtils.isServer)
                    return;

                float strength = 3;
                if (Combo > 2)
                    strength = 2;
                //float baseScale = 1;

                if (VisualEffectSystem.HitEffect_ScreenShaking)
                {
                    PunchCameraModifier modifier = new(Projectile.Center, RotateVec2, strength, 4, 6, 1000);
                    Main.instance.CameraModifiers.Add(modifier);
                }

                Dust dust;
                float offset = Projectile.localAI[1] + Main.rand.NextFloat(0, (Projectile.width * Projectile.scale) - Projectile.localAI[1]);
                Vector2 pos = Bottom + (RotateVec2 * offset);
                //if (VisualEffectSystem.HitEffect_Lightning)
                //{
                //    dust = Dust.NewDustPerfect(pos, DustType<EmperorSabreStrikeDust>(),
                //        Scale: Main.rand.NextFloat(baseScale, baseScale * 1.3f));
                //    dust.rotation = _Rotation + MathHelper.PiOver2 + Main.rand.NextFloat(-0.2f, 0.2f);

                //    dust = Dust.NewDustPerfect(pos, DustType<EmperorSabreStrikeDust>(),
                //             Scale: Main.rand.NextFloat(baseScale * 0.2f, baseScale * 0.3f));
                //    float leftOrRight = Main.rand.NextFromList(-0.3f, 0.3f);
                //    dust.rotation = _Rotation + MathHelper.PiOver2 + leftOrRight + Main.rand.NextFloat(-0.2f, 0.2f);
                //}

                if (VisualEffectSystem.HitEffect_Dusts)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Vector2 dir = -RotateVec2.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
                        dust = Dust.NewDustPerfect(pos, DustID.LifeDrain, dir * Main.rand.NextFloat(1f, 5f), newColor: Color.Cyan * 0.5f, Scale: Main.rand.NextFloat(1f, 2f));
                        dust.noGravity = true;
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 dir = RotateVec2.RotatedBy(Main.rand.NextFloat(-1.4f, 1.4f));
                        dust = Dust.NewDustPerfect(pos, DustID.LifeDrain, dir * Main.rand.NextFloat(1f, 10f), newColor: Color.Cyan * 0.5f, Scale: Main.rand.NextFloat(1.5f, 2f));
                        dust.noGravity = true;
                    }
                }
            }
        }

        public void DrawWarp()
        {
            if (oldRotate != null)
                WarpDrawer(0.75f);
        }

        protected override void DrawSlashTrail()
        {
            if (oldRotate == null)
                return;
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

                var topColor = Color.Lerp(new Color(238, 218, 130, 255), new Color(167, 127, 95, 0), 1 - factor);
                var bottomColor = Color.Lerp(new Color(109, 73, 86, 255), new Color(83, 16, 85, 0), 1 - factor);
                bars.Add(new(Top.Vec3(), topColor, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), bottomColor, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Helper.DrawTrail(Main.graphics.GraphicsDevice, () =>
                {
                    Effect effect = Filters.Scene["NoHLGradientTrail"].GetShader().Shader;

                    effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
                    effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.Split.Value);
                    effect.Parameters["gradientTexture"].SetValue(GradientTexture.Value);

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                    {
                        pass.Apply();
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                        Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                    }
                }, BlendState.NonPremultiplied, SamplerState.PointWrap, RasterizerState.CullNone);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
            }
        }
    }

    public class BeheritsSpacial : ModProjectile, IDrawWarp, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.Misc_Melee + Name;

        public ref float Rot => ref Projectile.ai[0];
        public ref float Width => ref Projectile.ai[1];
        public ref float Height => ref Projectile.ai[2];

        public ref float Timer => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 dir = Rot.ToRotationVector2() * 380;
            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size()
                , Projectile.Center + dir * Width, Projectile.Center - dir * Width,60,ref a);
        }

        public override void AI()
        {
            Timer++;

            Projectile.rotation = Rot;
            Width = Helper.SqrtEase(Timer / 20);
            Height = Helper.SinEase(Timer / 20);

            if (Timer<15)
            for (int i = 0; i < 2; i++)
            {
                Vector2 dir = Projectile.rotation.ToRotationVector2();
                Dust dust = Dust.NewDustPerfect(Projectile.Center + dir * Width*Main.rand.NextFloat(-300,300) + Main.rand.NextVector2Circular(6, 6)
                    , DustID.TeleportationPotion,dir * Main.rand.NextFloat(2f, 4f)
                    , 100, new Color(255, 0, 0, 255), Scale: Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;
            }

            if (Timer % 4 == 0)
                Projectile.frame++;

            if (Timer > 20)
                Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public void DrawWarp()
        {
            Vector2 pos = Projectile.Center - Main.screenPosition;

            float factor = 1 - (Timer / 20);

            float rot = Projectile.rotation;

            float r = rot % MathHelper.TwoPi;
            float dir = (r >= MathHelper.Pi ? (r - MathHelper.Pi) : (r + MathHelper.Pi)) / MathHelper.TwoPi;

            float r2 = (rot + MathHelper.Pi) % MathHelper.TwoPi;
            float dir2 = (r2 >= MathHelper.Pi ? (r2 - MathHelper.Pi) : (r2 + MathHelper.Pi)) / MathHelper.TwoPi;

            Texture2D tex = CoraliteAssets.Sparkle.ShotLineSPA.Value;

            Vector2 scale = new Vector2(Width * 2f, Height * 3f);
            Vector2 direction = (Projectile.rotation).ToRotationVector2() * 200 * factor;
            Vector2 direction2 = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 6;

            Rectangle frame = tex.Frame(1, 2, 0, 0);
            Main.spriteBatch.Draw(tex, pos - direction2 - direction, frame
                , new Color(dir, factor * 0.5f, 0, factor), Rot, new Vector2(frame.Width / 2, frame.Height), scale, 0, 0);
            Main.spriteBatch.Draw(tex, pos + direction2 + direction, tex.Frame(1, 2, 0, 1)
                , new Color(dir2, factor * 0.5f, 0, factor), Rot, new Vector2(frame.Width / 2, 0), scale, 0, 0);
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Texture2D tex = Projectile.GetTexture();

            Vector2 scale = new Vector2(Width * 1.75f, Height * 0.5f);
            Rectangle frame = tex.Frame(1, 6, 0, Projectile.frame);

            Main.spriteBatch.Draw(tex, pos, frame
                , new Color(255, 180, 210, 255), Rot, frame.Size() / 2, scale, 0, 0);
        }
    }
}
