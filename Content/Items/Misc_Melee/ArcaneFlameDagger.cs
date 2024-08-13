using Coralite.Content.Items.Icicle;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.UI.Chat;

namespace Coralite.Content.Items.Misc_Melee
{
    public class ArcaneFlameDagger : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Melee + Name;

        protected static ParticleGroup group;

        public static Color Green = new(162, 248, 2);
        public static Color Gray = new(172, 167, 198);
        public static Color Purple = new(212, 195, 255);

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ModContent.ProjectileType<ArcaneFlameDaggerProj>();
            Item.DamageType = DamageClass.Melee;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(0, 8));
            Item.rare = ModContent.RarityType<ArcaneFlameDaggerRarity>();
            Item.SetWeaponValues(50, 4, 6);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shootSpeed = 16;
            Item.UseSound = CoraliteSoundID.Swing_Item1;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            group?.UpdateParticles();
        }

        public override void PostDrawTooltipLine(DrawableTooltipLine line)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                group ??= new ParticleGroup();
                if (group != null)
                    SpawnParticle(line);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);

                group?.DrawParticlesInUI(Main.spriteBatch);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            }
        }

        public void SpawnParticle(DrawableTooltipLine line)
        {
            if (!Main.gamePaused && Main.timeForVisualEffects % 5 == 0 && Main.rand.NextBool(2))
            {
                Vector2 size = ChatManager.GetStringSize(line.Font, line.Text, line.BaseScale);
                Color c = Main.rand.NextFromList(Purple, Green, Gray);

                Vector2 pos = new Vector2(line.X, line.Y) + new Vector2(Main.rand.NextFloat(0, size.X), Main.rand.NextFloat(size.Y * 0.6f, size.Y * 1f));

                var p = group.NewParticle<FireParticle>(pos, -Vector2.UnitY * Main.rand.NextFloat(0.4f, 3f), c, Main.rand.NextFloat(0.1f, 0.3f));
                p.MaxFrameCount = 3;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ShadowFlameKnife)
                .AddIngredient(ItemID.CursedFlame, 20)
                .AddIngredient(ItemID.Ectoplasm, 20)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ArcaneFlameDaggerRarity : ModRarity
    {
        public override Color RarityColor
        {
            get
            {
                float factor = Math.Abs(MathF.Sin(Main.GlobalTimeWrappedHourly)) * 2;
                if (factor < 1)
                    return Color.Lerp(ArcaneFlameDagger.Green, ArcaneFlameDagger.Gray, factor);
                return Color.Lerp(ArcaneFlameDagger.Gray, ArcaneFlameDagger.Purple, (factor - 1));
            }
        }
    }

    public class ArcaneFlameDaggerProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Melee + "ArcaneFlameDagger";

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = 4;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            for (int i = 0; i < 2; i++)
                Projectile.SpawnTrailDust(12f, DustID.CursedTorch, Main.rand.NextFloat(-0.2f, 0.2f), Scale: Main.rand.NextFloat(1, 1.5f));
            if (Main.rand.NextBool(3))
                Projectile.SpawnTrailDust(12f, DustID.Shadowflame, Main.rand.NextFloat(-0.2f, 0.4f), Scale: Main.rand.NextFloat(1, 1.5f));

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 40f)
            {
                Projectile.velocity.X *= 0.99f;
                Projectile.velocity.Y += 0.5f;
                if (Projectile.velocity.Y > 14)
                    Projectile.velocity.Y = 14;

                Projectile.rotation += Projectile.velocity.Length() / 40;
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 12;
            height = 12;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, Main.rand.Next(60, 180));

            if (Projectile.penetrate != 1)
            {
                Projectile.ai[0] = 35f;
                float num28 = Projectile.velocity.Length();
                Vector2 vector5 = target.Center - Projectile.Center;
                vector5.Normalize();
                vector5 *= num28;
                Projectile.velocity = -vector5 * 0.9f;
                Projectile.netUpdate = true;

            }

            if (hit.Crit)//生成额外斩击弹幕
            {
                Vector2 dir = Helper.NextVec2Dir();
                Projectile.NewProjectileFromThis<ArcaneFlameDaggerSlash>(target.Center + dir * 8 * 24
                    , -dir * 24, Projectile.damage, 0, ai1: 24);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                int num390 = Dust.NewDust(Projectile.Center, 10, 10, DustID.CursedTorch);
                Main.dust[num390].noGravity = true;
                Dust dust2 = Main.dust[num390];
                dust2.velocity *= 2f;
                dust2 = Main.dust[num390];
                dust2.velocity -= Projectile.oldVelocity * 0.3f;
                dust2 = Main.dust[num390];
                dust2.scale += Main.rand.Next(150) * 0.001f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDraw(lightColor, -MathHelper.PiOver2 - 0.6f);
            return false;
        }
    }

    public class ArcaneFlameDaggerSlash : ModProjectile, IDrawPrimitive, IDrawWarp
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "SpurtTrail2";

        public ref float Alpha => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.ai[0];
        public ref float TrailWidth => ref Projectile.ai[1];

        public Player Owner => Main.player[Projectile.owner];

        public static Asset<Texture2D> GradientTexture;

        private Trail trail;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            GradientTexture = ModContent.Request<Texture2D>(AssetDirectory.Misc_Melee + "ArcaneFlameDaggerGradient");
        }

        public override void Unload()
        {
            GradientTexture = null;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.timeLeft = 200;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;

            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.netImportant = true;

            Projectile.extraUpdates = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.oldPos = new Vector2[24];
            for (int i = 0; i < 24; i++)
                Projectile.oldPos[i] = Projectile.Center;
        }

        public override void AI()
        {
            trail ??= new Trail(Main.graphics.GraphicsDevice, 24, new NoTip(), WidthFunction, ColorFunction);

            Lighting.AddLight(Projectile.Center, Color.LimeGreen.ToVector3());
            if (Timer < 10)
            {
                float factor = Timer / 14;
                Color c = Color.Lerp(ArcaneFlameDagger.Purple, ArcaneFlameDagger.Green, factor);
                for (int i = 0; i < 2; i++)
                {
                    var p = Particle.NewParticle<FireParticle>(Projectile.Center + Main.rand.NextVector2Circular(14, 14),
                          Projectile.velocity * Main.rand.NextFloat(0.1f, 0.4f), c, Main.rand.NextFloat(0.1f, 0.4f));
                    p.MaxFrameCount = 3;
                }
            }

            do
            {
                if (Timer < 10)
                {
                    if (Alpha < 1)
                        Alpha += 1 / 6f;
                    break;
                }

                Projectile.velocity *= 0.84f;
                TrailWidth *= 0.85f;
                Alpha -= 0.05f;
                if (TrailWidth < 2)
                    Projectile.Kill();

            } while (false);

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Timer < 12)
            {
                Projectile.oldPos[23] = Projectile.Center + Projectile.velocity;

                for (int i = 0; i < 23; i++)
                    Projectile.oldPos[i] = Vector2.Lerp(Projectile.oldPos[0], Projectile.oldPos[23], i / 23f);
            }
            else
            {
                for (int i = 0; i < 23; i++)
                    Projectile.oldPos[i] = Projectile.oldPos[i + 1];

                Projectile.oldPos[23] = Projectile.Center + Projectile.velocity;
            }
            trail.Positions = Projectile.oldPos;

            Timer++;
        }

        public float WidthFunction(float factor)
        {
            if (factor < 0.3f)
                return Helper.Lerp(0, TrailWidth, factor / 0.3f);
            return Helper.Lerp(TrailWidth, 0, (factor - 0.3f) / 0.7f);
        }

        public Color ColorFunction(Vector2 factor)
        {
            return Color.White;
        }

        public void DrawPrimitives()
        {
            if (trail == null || Timer < 0)
                return;

            Effect effect = Filters.Scene["AlphaGradientTrail"].GetShader().Shader;

            effect.Parameters["transformMatrix"].SetValue(Helper.GetTransfromMaxrix());
            effect.Parameters["sampleTexture"].SetValue(Projectile.GetTexture());
            effect.Parameters["gradientTexture"].SetValue(GradientTexture.Value);
            effect.Parameters["alpha"].SetValue(Alpha);

            trail.Render(effect);
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawWarp()
        {
            if (Timer < 0)
                return;

            List<CustomVertexInfo> bars = new();

            float w = 1f;
            Vector2 up = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2();
            Vector2 down = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float factor = 1f - i / 23f;
                Vector2 Center = Projectile.oldPos[i];
                float r = Projectile.rotation % 6.18f;
                float dir = (r >= 3.14f ? r - 3.14f : r + 3.14f) / MathHelper.TwoPi;
                float width = WidthFunction(factor) * 0.75f;
                Vector2 Top = Center + up * width;
                Vector2 Bottom = Center + down * width;

                bars.Add(new CustomVertexInfo(Top, new Color(dir, w, 0f, 1f), new Vector3(factor, 0f, w)));
                bars.Add(new CustomVertexInfo(Bottom, new Color(dir, w, 0f, 1f), new Vector3(factor, 1f, w)));
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, 0f, 1f);
            Matrix model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0f)) * Main.GameViewMatrix.TransformationMatrix;

            Effect effect = Filters.Scene["KEx"].GetShader().Shader;

            effect.Parameters["uTransform"].SetValue(model * projection);
            Main.graphics.GraphicsDevice.Textures[0] = FrostySwordSlash.WarpTexture.Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            effect.CurrentTechnique.Passes[0].Apply();
            if (bars.Count >= 3)
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
