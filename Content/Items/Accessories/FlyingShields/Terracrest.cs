using Coralite.Content.Items.Icicle;
using Coralite.Content.Items.Materials;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Accessories.FlyingShields
{
    public class Terracrest : BaseAccessory, IFlyingShieldAccessory
    {
        public Terracrest() : base(ItemRarityID.Yellow, Item.sellPrice(0, 5))
        { }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !((equippedItem.type == ModContent.ItemType<DemonsProtection>()//下位
                || equippedItem.type == ModContent.ItemType<HolyCharm>())//下位

                && incomingItem.type == ModContent.ItemType<Terracrest>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldLRMeantime = true;
            }
        }

        public void OnGuardInitialize(BaseFlyingShieldGuard projectile)
        {
            projectile.parryTime = 10;
            projectile.StrongGuard += 0.18f;
            projectile.damageReduce *= 1.2f;
            projectile.distanceAdder *= 1.2f;
        }

        public bool OnParry(BaseFlyingShieldGuard projectile)
        {
            Player Owner = projectile.Owner;

            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.parryTime < 100)
                {
                    Owner.immuneTime = 30;
                    Owner.immune = true;
                }

                int damage = (int)(projectile.Projectile.damage * (1.45f - 0.3f * cp.parryTime / 280f));

                SoundEngine.PlaySound(CoraliteSoundID.TerraBlade_Item60, projectile.Projectile.Center);
                Helper.PlayPitched("Misc/ShieldGuard", 0.4f, 0f, projectile.Projectile.Center);

                Projectile p = projectile.Projectile;
                Vector2 dir = p.rotation.ToRotationVector2();
                p.NewProjectileFromThis<TerracrestSpike>(p.Center, dir * Main.rand.NextFloat(17f, 19f)*p.scale,
                     damage, p.knockBack, Owner.whoAmI, ai1: 14);

                for (int i = -1; i < 2; i += 2)
                {
                    float exRot = Owner.direction * i * 0.3f;

                    Vector2 aimDir = dir.RotatedBy(Main.rand.NextFloat(exRot - 0.25f, exRot + 0.25f));

                    p.NewProjectileFromThis<TerracrestSpike>(p.Center + Owner.direction * i * (p.rotation + 1.57f).ToRotationVector2() * Main.rand.NextFloat(8, 16)
                        , aimDir * Main.rand.NextFloat(10f, 14f) * p.scale,
                         (int)(damage*0.95f), p.knockBack, Owner.whoAmI, ai1: Main.rand.NextFloat(12, 14));
                }

                p.NewProjectileFromThis<TerracrestSpike>(p.Center + Main.rand.NextVector2Circular(12,12)
                    , dir.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(8f, 16f) * p.scale,
                     (int)(damage * 0.95f), p.knockBack, Owner.whoAmI, ai1: Main.rand.NextFloat(8, 12));

                ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.TerraBlade, new ParticleOrchestraSettings()
                {
                    MovementVector = dir,
                    PositionInWorld = projectile.Projectile.Center,
                    UniqueInfoPiece = 90,
                }) ;

                if (cp.parryTime < 250)
                    cp.parryTime += 80;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HolyCharm>()
                .AddIngredient<DemonsProtection>()
                .AddIngredient<RustedShield>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }

    public class TerracrestSpike : ModProjectile, IDrawPrimitive, IDrawWarp
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "SpurtTrail3";

        public ref float Timer => ref Projectile.ai[0];
        public ref float TrailWidth => ref Projectile.ai[1];

        public ref float Alpha => ref Projectile.localAI[0];
        public ref float Length => ref Projectile.localAI[1];

        public Player Owner => Main.player[Projectile.owner];

        private Trail trail;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.timeLeft = 200;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.netImportant = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.oldPos = new Vector2[16];
            for (int i = 0; i < 16; i++)
                Projectile.oldPos[i] = Projectile.Center;

            Alpha = 1;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Timer > 20)
                return false;

            return null;
        }

        public override void AI()
        {
            trail ??= new Trail(Main.graphics.GraphicsDevice, 16, new NoTip(), WidthFunction, ColorFunction);

            Lighting.AddLight(Projectile.Center, Color.LimeGreen.ToVector3());

            const int ShootTime = 20;
            const int DelayTime = ShootTime + 15;

            if (Timer < ShootTime)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(TrailWidth, TrailWidth) / 2, DustID.TerraBlade,
                    Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(1f, 4f),Scale:Main.rand.NextFloat(0.5f,1f));
                if (Main.rand.NextBool())
                    dust.noGravity = true;
                if (Timer < 14)
                {
                    Projectile.oldPos[15] = Projectile.Center + Projectile.velocity;

                    for (int i = 0; i < 15; i++)
                        Projectile.oldPos[i] = Vector2.Lerp(Projectile.oldPos[0], Projectile.oldPos[15], i / 15f);
                }
                else
                {
                    for (int i = 0; i < 15; i++)
                        Projectile.oldPos[i] = Projectile.oldPos[i + 1];

                    Projectile.oldPos[15] = Projectile.Center + Projectile.velocity;
                }
            }
            else
            {
                Alpha -= 1 / 15f;
                Projectile.velocity = Vector2.Zero;
            }


            trail.Positions = Projectile.oldPos;

            Timer++;
            if (Timer>DelayTime)
            {
                Projectile.Kill();
            }
        }

        public float WidthFunction(float factor)
        {
            if (factor < 0.7f)
                return Helper.Lerp(0, TrailWidth, factor / 0.7f);
            return Helper.Lerp(TrailWidth, 0, (factor - 0.7f) / 0.3f);
        }

        public Color ColorFunction(Vector2 factor)
        {
            return Color.White;
        }

        public void DrawPrimitives()
        {
            if (trail == null || Timer < 0)
                return;

            Effect effect = Filters.Scene["AlphaNoHLGradientTrail"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(Projectile.GetTexture());
            effect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>(AssetDirectory.Accessories + "TerracrestGradient").Value);
            effect.Parameters["alpha"].SetValue(Alpha);

            trail.Render(effect);
        }

        public override bool PreDraw(ref Color lightColor) {

            Helper.DrawPrettyStarSparkle(Projectile.Opacity, 0, Projectile.oldPos[12] - Main.screenPosition,
                Color.White, Color.LimeGreen, Timer / 35, 0, 0.2f, 0.6f, 1, Projectile.rotation+1.57f,
                new Vector2(0.1f, 2.4f), Vector2.One);
            //ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, 0, Projectile.oldPos[12] - Main.screenPosition,
            //    Color.White, Color.LimeGreen, Timer / 35, 0, 0.2f, 0.6f, 1, Projectile.rotation+MathHelper.PiOver4,
            //    new Vector2(0.5f, 0.5f), Vector2.One*2);
            return false;
        }

        public void DrawWarp()
        {
            if (Timer < 0)
                return;

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

            float w = 1f;
            Vector2 up = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2();
            Vector2 down = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float factor = 1f - i / 15f;
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
