using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Shadow
{
    public class ShadowSword : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        private float rotation;

        public override void SetDefaults()
        {
            Item.height = Item.width = 40;
            Item.damage = 24;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.knockBack = 6f;
            Item.reuseDelay = 20;
            Item.mana = 9;

            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item9;

            Item.autoReuse = true;
            Item.useTurn = false;
            Item.noMelee = true;
            //Item.noUseGraphic = true;

            Item.shoot = ProjectileType<ShadowSwordHeldProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectile(source, player.Center + (new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * 50f), new Vector2(0, -0.03f), ProjectileType<ShadowSwordProj>(), damage, 6, player.whoAmI);
                rotation += 3;
            }

            //Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, 6, player.whoAmI, 0);

            rotation -= 5.5f;

            if (rotation > 8)
                rotation = 0;

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<ShadowCrystal>(12)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }

    public class ShadowSwordHeldProj : BaseSwingProj
    {
        public override string Texture => AssetDirectory.ShadowItems + "ShadowSword";

        public ref float Combo => ref Projectile.ai[0];

        public static Asset<Texture2D> GradientTexture;
        public int alpha;

        public ShadowSwordHeldProj() : base(MathHelper.PiOver4, trailCount: 34) { }

        public override void SetSwingProperty()
        {
            Projectile.localNPCHitCooldown = 48;
            Projectile.width = 40;
            Projectile.height = 84;
            trailTopWidth = 2;
            distanceToOwner = 8;
            minTime = 20;
            useSlashTrail = true;
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            GradientTexture = Request<Texture2D>(AssetDirectory.ShadowItems + "ShadowSwordGradient");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            GradientTexture = null;
        }

        protected override void InitializeSwing()
        {
            if (Projectile.IsOwnedByLocalPlayer())
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 3;
            switch (Combo)
            {
                default:
                case 0: //先举过头顶 之后挥向鼠标位置，并射出弹幕
                    startAngle = 1.8f;
                    totalAngle = 4.8f;
                    minTime = 80;
                    maxTime = (int)(Owner.itemTimeMax * 0.7f) + 24 + 80;
                    Smoother = Coralite.Instance.SqrtSmoother;
                    Projectile.scale = 0.9f;
                    break;
            }
            base.InitializeSwing();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool? CanHitNPC(NPC target) => false;

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 65 * Projectile.scale;
        }

        protected override void BeforeSlash()
        {
            if (Timer < 40)
            {
                startAngle -= Math.Sign(totalAngle) * 0.05f;
            }

            _Rotation = startAngle = GetStartAngle() - (Owner.direction * 2.8f);
            Slasher();
            if (Timer == minTime)
            {
                InitializeCaches();
            }
            base.BeforeSlash();
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;
            switch ((int)Combo)
            {
                default:
                case 0:
                    alpha = (int)(Coralite.Instance.X2Smoother.Smoother(timer, maxTime - minTime) * 140) + 100;
                    Projectile.scale = Helper.EllipticalEase(2.8f - (4.8f * Smoother.Smoother(timer, maxTime - minTime)), 0.8f, 1.2f);
                    break;
            }

            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            base.AfterSlash();
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
                var bottomColor = Color.Lerp(new Color(109, 73, 86, alpha), new Color(83, 16, 85, 0), 1 - factor);
                bars.Add(new(Top.Vec3(), topColor, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), bottomColor, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

                Effect effect = Filters.Scene["SimpleGradientTrail"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.ZoomMatrix;
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
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            }
        }
    }
}
