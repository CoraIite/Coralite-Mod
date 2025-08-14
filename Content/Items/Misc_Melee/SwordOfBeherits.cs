using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
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
            Helper.PlayPitched("CoreKeeper/swordLegendaryAttack", 0.7f, Main.rand.NextFloat(0.2f, 0.3f), player.Center);
            Projectile.NewProjectile(source, player.Center, Vector2.Zero,
                type, (int)(damage * 1.6f), knockback, player.whoAmI, Main.rand.Next(2), -1);

            //player.itemTime = 8;
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

    [AutoLoadTexture(Path = AssetDirectory.Misc_Melee)]
    public class SwordOfBeheritsSlash : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.Misc_Melee + "SwordOfBeherits";

        public ref float Combo => ref Projectile.ai[0];

        [AutoLoadTexture(Name = "SwordOfBeheritsGradient")]
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
            zRot = Main.rand.Next(0, 6) * 0.1f;
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
            extraScaleAngle = Main.rand.Next(-3, 4) * 0.2f;
            recordStartAngle = Math.Abs(startAngle);
            recordTotalAngle = Math.Abs(totalAngle);
            Projectile.scale = Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(0, maxTime - minTime)), 1.05f - zRot, 1.05f + zRot*0.75f);
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 0.3f, 0.3f, 1f);
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;
            float scale = 1f;

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

                Helper.PlayPitched("CoreKeeper/swordLegendaryImpact", 0.5f, 0, Projectile.Center);

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
                        dust = Dust.NewDustPerfect(pos, DustID.AncientLight, dir * Main.rand.NextFloat(1f, 5f), newColor: Color.Cyan * 0.5f, Scale: Main.rand.NextFloat(1f, 2f));
                        dust.noGravity = true;
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 dir = RotateVec2.RotatedBy(Main.rand.NextFloat(-1.4f, 1.4f));
                        dust = Dust.NewDustPerfect(pos, DustID.AncientLight, dir * Main.rand.NextFloat(1f, 10f), newColor: Color.Cyan * 0.5f, Scale: Main.rand.NextFloat(1.5f, 2f));
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

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            base.DrawSelf(mainTex, origin, lightColor, extraRot);
        }
    }
}
