using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.Misc_Melee;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Items.Misc_Summon
{
    public class Pisces : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Summon + Name;

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.SummonMeleeSpeed;
            Item.value = Item.sellPrice(0, 1, 50);
            Item.rare = ItemRarityID.LightRed;
            Item.damage = 50;
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<PiscesSwing>();
            Item.knockBack = 6.5f;
            Item.shootSpeed = 16;
        }

        public override bool AllowPrefix(int pre) => true;
        public override bool MeleePrefix() => true;

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse==2)
            {
                return false;
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ReinforcedFishingPole)
                .AddIngredient(ItemID.AdamantiteBar, 12)
                .AddIngredient(ItemID.SoulofFlight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.ReinforcedFishingPole)
                .AddIngredient(ItemID.TitaniumBar, 12)
                .AddIngredient(ItemID.SoulofFlight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class PiscesSwing : BaseSwingProj
    {
        public override string Texture => AssetDirectory.Misc_Summon + "Pisces";

        public ref float Combo => ref Projectile.ai[0];

        public PiscesSwing() : base(0.785f, trailCount: 30) { }

        public int delay;
        public int alpha;

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
            onHitFreeze = 14;
            switch (Combo)
            {
                default:
                case 0: //下挥
                    startAngle = 1.6f + Main.rand.NextFloat(-0.2f, 0.3f);
                    totalAngle = 4f + Main.rand.NextFloat(-0.2f, 0.5f);
                    maxTime = 90;
                    Smoother = Coralite.Instance.BezierEaseSmoother;

                    break;
                case 1://下挥，圆
                    startAngle = -1.6f + Main.rand.NextFloat(-0.3f, 0.2f);
                    totalAngle = -4f + Main.rand.NextFloat(-0.5f, 0.2f);
                    minTime = 0;
                    maxTime = 90;
                    Smoother = Coralite.Instance.BezierEaseSmoother;

                    break;
            }

            base.InitializeSwing();
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 1f, 0.15f, 0.4f);
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;

            if (Main.rand.NextBool(8))
            {
                Vector2 dir = RotateVec2.RotatedBy(1.57f * Math.Sign(totalAngle));
                Dust dust = Dust.NewDustPerfect(Top - (20 * RotateVec2) + Main.rand.NextVector2Circular(18, 18), DustID.LifeDrain,
                       dir * Main.rand.NextFloat(0.5f, 2f), 255, Color.Transparent, Scale: Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;
            }

            if (Item.type == ModContent.ItemType<SwordOfBeherits>())
            {
                float scale = Owner.GetAdjustedItemScale(Item);
                scale = (1.5f * scale) - 0.5f;
                if (scale > 3f)
                    scale = 3f;

                Projectile.scale = scale;
            }
            else
                Projectile.Kill();

            alpha = (int)(Helper.X2Ease(timer, maxTime - minTime) * 100) + 100;
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
                    Effect effect = ShaderLoader.GetShader("SimpleTrail");

                    effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
                    effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.Vanilla.Value);

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
}
