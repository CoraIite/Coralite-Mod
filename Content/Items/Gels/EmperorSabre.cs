using Coralite.Content.Items.Icicle;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Gels
{
    /// <summary>
    /// 下挥=>下挥=》横挥=》上挥=》反横挥=》重下挥=》重上挥=》指挥
    /// </summary>
    public class EmperorSabre : ModItem
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public int useCount;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 26;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.knockBack = 2f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Expert;
            Item.shoot = ProjectileType<IcicleSwordSplash>();

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.expert = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (useCount > 6)
                {
                    useCount = 0;
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<EmperorSabreSummon>(), damage, knockback, player.whoAmI);

                    return false;
                }

                switch (useCount)
                {
                    default:
                    case 0:
                    case 1:
                    case 3:
                    case 5:
                    case 6:
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<EmperorSabreSlash>(), damage, knockback, player.whoAmI, useCount);
                        break;
                    case 2:
                    case 4:
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<EmperorSabreSlash2>(), damage, knockback, player.whoAmI, useCount);
                        break;
                }

                Helper.PlayPitched("Misc/Slash", 0.4f, 0f, player.Center);
            }

            useCount++;
            return false;
        }

    }

    public class EmperorSabreSlash : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.GelItems + "EmperorSabre";

        public ref float Combo => ref Projectile.ai[0];

        public static Asset<Texture2D> trailTexture;
        public static Asset<Texture2D> WarpTexture;
        public static Asset<Texture2D> GradientTexture;

        public EmperorSabreSlash() : base(new Vector2(56, 64).ToRotation() - 0.1f, trailLength: 48) { }

        public int delay;
        public int alpha;
        public float[] oldLength;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            trailTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "NormalSlashTrail3");
            WarpTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "WarpTex");
            GradientTexture = Request<Texture2D>(AssetDirectory.GelItems + "EmperorSabreGradient");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            WarpTexture = null;
            GradientTexture = null;
        }

        public override void SetDefs()
        {
            Projectile.localNPCHitCooldown = 22;
            Projectile.width = 40;
            Projectile.height = 80;
            trailTopWidth = 2;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 12;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 65 * Projectile.scale;
        }

        protected override void Initializer()
        {
            if (Main.myPlayer == Projectile.owner)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 3;
            alpha = 0;
            switch (Combo)
            {
                default:
                case 0: //下挥，较为椭圆
                    startAngle = 1.8f;
                    totalAngle = 4.6f;
                    minTime = 12;
                    maxTime = (int)(Owner.itemTimeMax * 1.7f);
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    Projectile.scale = 0.9f;
                    delay = 24;
                    break;
                case 1://下挥，圆
                    startAngle = 1.4f;
                    totalAngle = 3.6f;
                    minTime = 0;
                    maxTime = (int)(Owner.itemTimeMax * 1.7f);
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    delay = 24;
                    break;
                case 3://上挥 较圆
                    startAngle = -1.6f;
                    totalAngle = -4.6f;
                    minTime = 0;
                    maxTime = (int)(Owner.itemTimeMax * 1.2f);
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    delay = 24;
                    break;
                case 5: //重下挥
                    startAngle = 1.2f;
                    totalAngle = 4f;
                    minTime = 26;
                    onHitFreeze = 16;
                    maxTime = (int)(Owner.itemTimeMax * 1.5f) + 6;
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    delay = 38;
                    break;
                case 6://重上挥
                    startAngle = -1.8f;
                    totalAngle = -4.8f;
                    minTime = 26;
                    onHitFreeze = 16;
                    maxTime = (int)(Owner.itemTimeMax * 1.5f) + 6;
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    delay = 38;
                    break;

            }

            oldLength = new float[trailLength];
            base.Initializer();
        }

        protected override void InitializeCaches()
        {
            for (int j = trailLength - 1; j >= 0; j--)
            {
                oldRotate[j] = 100f;
                oldDistanceToOwner[j] = distanceToOwner;
                oldLength[j] = Projectile.height * Projectile.scale;
            }
        }

        protected override void UpdateCaches()
        {
            for (int i = trailLength - 1; i > 0; i--)
            {
                oldRotate[i] = oldRotate[i - 1];
                oldDistanceToOwner[i] = oldDistanceToOwner[i - 1];
                oldLength[i] = oldLength[i - 1];
            }

            oldRotate[0] = _Rotation;
            oldDistanceToOwner[0] = distanceToOwner;
            oldLength[0] = Projectile.height * Projectile.scale;
        }

        protected override void BeforeSlash()
        {
            startAngle -= Math.Sign(totalAngle) * 0.05f;
            _Rotation = startAngle;
            Slasher();
            if ((int)Timer == minTime)
                InitializeCaches();
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;
            switch ((int)Combo)
            {
                default:
                case 0:
                    alpha = (int)(Coralite.Instance.X2Smoother.Smoother(timer, maxTime - minTime) * 140) + 100;
                    Projectile.scale = Helper.EllipticalEase(2.3f - 4.6f * Smoother.Smoother(timer, maxTime - minTime), 0.8f, 1.2f);
                    break;
                case 1:
                    alpha = (int)(Coralite.Instance.SqrtSmoother.Smoother(timer, maxTime - minTime) * 140) + 100;
                    break;
                case 3:
                    alpha = (int)(Coralite.Instance.SqrtSmoother.Smoother(timer, maxTime - minTime) * 140) + 100;
                    Projectile.scale = Helper.EllipticalEase(1.6f - 4.6f * Smoother.Smoother(timer, maxTime - minTime), 1.2f, 1f);
                    break;
                case 5:
                    alpha = (int)(Coralite.Instance.SqrtSmoother.Smoother(timer, maxTime - minTime) * 140) + 100;
                    Projectile.scale = Helper.EllipticalEase(1.6f - 4.6f * Smoother.Smoother(timer, maxTime - minTime), 1.2f, 1.4f);

                    break;
                case 6:
                    alpha = (int)(Coralite.Instance.SqrtSmoother.Smoother(timer, maxTime - minTime) * 140) + 100;
                    Projectile.scale = Helper.EllipticalEase(1.6f - 4.6f * Smoother.Smoother(timer, maxTime - minTime), 1.4f, 1.2f);

                    break;
            }
            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            if (alpha > 20)
                alpha -= 5;

            Slasher();
            if (Timer > maxTime + 38)
                Projectile.Kill();
        }

        public void DrawWarp()
        {
            if (Timer < minTime)
                return;

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();
            float counts = 0f;
            for (int i = 0; i < oldRotate.Length; i++)
                if (oldRotate[i] != 100f)
                    counts += 1f;

            float w = 1f;
            for (int i = 0; i < oldRotate.Length; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - i / counts;
                Vector2 Center = GetCenter(i);
                float r = oldRotate[i] % 6.18f;
                float dir = (r >= 3.14f ? r - 3.14f : r + 3.14f) / MathHelper.TwoPi;
                Vector2 Top = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]);
                Vector2 Bottom = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] - ControlTrailBottomWidth(factor) * 0.75f + oldDistanceToOwner[i]);

                bars.Add(new CustomVertexInfo(Top, new Color(dir, w, 0f, 1f), new Vector3(factor, 0f, w)));
                bars.Add(new CustomVertexInfo(Bottom, new Color(dir, w, 0f, 1f), new Vector3(factor, 1f, w)));
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, 0f, 1f);
            Matrix model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0f)) * Main.GameViewMatrix.ZoomMatrix;

            Effect effect = Filters.Scene["KEx"].GetShader().Shader;

            effect.Parameters["uTransform"].SetValue(model * projection);
            Main.graphics.GraphicsDevice.Textures[0] = WarpTexture.Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            effect.CurrentTechnique.Passes[0].Apply();
            if (bars.Count >= 3)
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        protected override void DrawSlashTrail()
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            List<VertexPositionColorTexture> bars = new List<VertexPositionColorTexture>();

            float length = 0f;
            for (int i = 0; i < oldRotate.Length; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;
                length += 1f;
            }

            for (int i = 0; i < oldRotate.Length; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - i / length;
                Vector2 Center = GetCenter(i);
                //float trailWidth = ControlTrailBottomWidth(factor) + trailTopWidth;

                //Vector2 current = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth - trailWidth / 2 + oldDistanceToOwner[i]);  //当前的拖尾中心点
                //Vector2 next;   //下一个点
                //if (i == oldRotate.Length - 1)
                //    next=current+current- Center + oldRotate[i -1].ToRotationVector2() * (oldLength[i - 1] + trailTopWidth - trailWidth / 2 + oldDistanceToOwner[i - 1]);
                //else
                //    next = Center + oldRotate[i + 1].ToRotationVector2() * (oldLength[i + 1] + trailTopWidth - trailWidth / 2 + oldDistanceToOwner[i + 1]);

                //Vector2 normalToNext = (next - current).SafeNormalize(Vector2.Zero);    
                //Vector2 normalPerp = normalToNext.RotatedBy(MathHelper.PiOver2);  //法线
                //trailWidth /= 2;
                //Vector2 Top = current + (normalPerp * trailWidth);
                //Vector2 Bottom = current- (normalPerp * trailWidth);
                Vector2 Top = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]);
                Vector2 Bottom = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] - ControlTrailBottomWidth(factor) + oldDistanceToOwner[i]);

                var topColor = Color.Lerp(new Color(238, 218, 130, alpha), new Color(167, 127, 95, 0), 1 - factor);
                var bottomColor = Color.Lerp(new Color(109, 73, 86, alpha), new Color(83, 16, 85, 0), 1 - factor);
                //var w = Helper.Lerp(0.5f, 0.05f, factor);
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
                effect.Parameters["sampleTexture"].SetValue(trailTexture.Value);
                effect.Parameters["gradientTexture"].SetValue(GradientTexture.Value);

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                    //Main.graphics.GraphicsDevice.DrawUserPrimitives(1, bars.ToArray(), 0, bars.Count - 2);
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            }
        }
    }

    public class EmperorSabreSlash2 : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.GelItems + "EmperorSabre2";

        public EmperorSabreSlash2() : base(0, 48) { }

        public ref float Combo => ref Projectile.ai[0];

        public int alpha;
        public float[] oldLength;

        public override void SetDefs()
        {
            Projectile.localNPCHitCooldown = 22;
            Projectile.width = 60;
            Projectile.height = 60;
            distanceToOwner = 2;
            minTime = 0;
            onHitFreeze = 4;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 75 * Projectile.scale;
        }

        protected override void Initializer()
        {
            if (Main.myPlayer == Projectile.owner)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 3;
            alpha = 0;
            switch (Combo)
            {
                default:
                case 2:
                    startAngle = 1.4f;
                    totalAngle = 4f;
                    minTime = 12;
                    Projectile.scale = 0.65f;
                    maxTime = (int)(Owner.itemTimeMax * 1.5f);
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    break;
                case 4:
                    startAngle = -2.8f;
                    totalAngle = -5.6f;
                    minTime = 0;
                    maxTime = Owner.itemTimeMax * 2;
                    Smoother = Coralite.Instance.SqrtSmoother;
                    Projectile.scale = 0.9f;
                    break;
            }

            oldLength = new float[trailLength];
            base.Initializer();
        }

        protected override void InitializeCaches()
        {
            for (int j = trailLength - 1; j >= 0; j--)
            {
                oldRotate[j] = 100f;
                oldDistanceToOwner[j] = distanceToOwner;
                oldLength[j] = Projectile.height * Projectile.scale;
            }
        }

        protected override void UpdateCaches()
        {
            for (int i = trailLength - 1; i > 0; i--)
            {
                oldRotate[i] = oldRotate[i - 1];
                oldDistanceToOwner[i] = oldDistanceToOwner[i - 1];
                oldLength[i] = oldLength[i - 1];
            }

            oldRotate[0] = _Rotation;
            oldDistanceToOwner[0] = distanceToOwner;
            oldLength[0] = Projectile.height * Projectile.scale;
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;

            switch (Combo)
            {
                default:
                case 2:
                    alpha = (int)(Coralite.Instance.X2Smoother.Smoother(timer, maxTime - minTime) * 140) + 100;
                    Projectile.scale = Helper.EllipticalEase(1.4f - 4f * Smoother.Smoother(timer, maxTime - minTime), 0.8f, 2.2f);
                    break;
                case 4:
                    alpha = (int)(Coralite.Instance.X2Smoother.Smoother(timer, maxTime - minTime) * 140) + 100;
                    Projectile.scale = Helper.EllipticalEase(2.8f - 5.6f * Smoother.Smoother(timer, maxTime - minTime), 0.9f, 2.6f);
                    break;
            }

            base.OnSlash();
        }

        public void DrawWarp()
        {
            if (Timer < minTime)
                return;

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();
            float counts = 0f;
            for (int i = 0; i < oldRotate.Length; i++)
                if (oldRotate[i] != 100f)
                    counts += 1f;

            float w = 1f;
            for (int i = 0; i < oldRotate.Length; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - i / counts;
                Vector2 Center = GetCenter(i);
                float r = oldRotate[i] % 6.18f;
                float dir = (r >= 3.14f ? r - 3.14f : r + 3.14f) / MathHelper.TwoPi;
                Vector2 Top = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]);
                Vector2 Bottom = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] - ControlTrailBottomWidth(factor) * 0.75f + oldDistanceToOwner[i]);

                bars.Add(new CustomVertexInfo(Top, new Color(dir, w, 0f, 1f), new Vector3(factor, 0f, w)));
                bars.Add(new CustomVertexInfo(Bottom, new Color(dir, w, 0f, 1f), new Vector3(factor, 1f, w)));
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, 0f, 1f);
            Matrix model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0f)) * Main.GameViewMatrix.ZoomMatrix;

            Effect effect = Filters.Scene["KEx"].GetShader().Shader;

            effect.Parameters["uTransform"].SetValue(model * projection);
            Main.graphics.GraphicsDevice.Textures[0] = EmperorSabreSlash.WarpTexture.Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            effect.CurrentTechnique.Passes[0].Apply();
            if (bars.Count >= 3)
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        protected override void DrawSlashTrail()
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            List<VertexPositionColorTexture> bars = new List<VertexPositionColorTexture>();

            float length = 0f;
            for (int i = 0; i < oldRotate.Length; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;
                length += 1f;
            }

            for (int i = 0; i < oldRotate.Length; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - i / length;
                Vector2 Center = GetCenter(i);
                //float trailWidth = ControlTrailBottomWidth(factor) + trailTopWidth;

                //Vector2 current = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth - trailWidth / 2 + oldDistanceToOwner[i]);  //当前的拖尾中心点
                //Vector2 next;   //下一个点
                //if (i == oldRotate.Length - 1)
                //    next=current+current- Center + oldRotate[i -1].ToRotationVector2() * (oldLength[i - 1] + trailTopWidth - trailWidth / 2 + oldDistanceToOwner[i - 1]);
                //else
                //    next = Center + oldRotate[i + 1].ToRotationVector2() * (oldLength[i + 1] + trailTopWidth - trailWidth / 2 + oldDistanceToOwner[i + 1]);

                //Vector2 normalToNext = (next - current).SafeNormalize(Vector2.Zero);    
                //Vector2 normalPerp = normalToNext.RotatedBy(MathHelper.PiOver2);  //法线
                //trailWidth /= 2;
                //Vector2 Top = current + (normalPerp * trailWidth);
                //Vector2 Bottom = current- (normalPerp * trailWidth);
                Vector2 Top = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]);
                Vector2 Bottom = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] * 0.3f + oldDistanceToOwner[i]);

                var topColor = Color.Lerp(new Color(238, 218, 130, alpha), new Color(167, 127, 95, 0), 1 - factor);
                var bottomColor = Color.Lerp(new Color(109, 73, 86, alpha), new Color(83, 16, 85, 0), 1 - factor);
                //var w = Helper.Lerp(0.5f, 0.05f, factor);
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
                effect.Parameters["sampleTexture"].SetValue(EmperorSabreSlash.trailTexture.Value);
                effect.Parameters["gradientTexture"].SetValue(EmperorSabreSlash.GradientTexture.Value);

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                    //Main.graphics.GraphicsDevice.DrawUserPrimitives(1, bars.ToArray(), 0, bars.Count - 2);
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            }
        }
    }

    public class EmperorSabreSummon : BaseSwingProj
    {
        public override string Texture => AssetDirectory.GelItems + "EmperorSabre";

        public EmperorSabreSummon() : base(new Vector2(56, 64).ToRotation() - 0.1f) { }

        public override void SetDefs()
        {
            Projectile.localNPCHitCooldown = 22;
            Projectile.width = 40;
            Projectile.height = 80;
            trailTopWidth = 2;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 12;
        }

        protected override float GetStartAngle()
        {
            return (Owner.direction > 0 ? 0 : MathHelper.Pi);
        }

        protected override void Initializer()
        {
            if (Main.myPlayer == Projectile.owner)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 2;
            startAngle = 1.4f;
            totalAngle = 1.5f;
            minTime = 12;
            Projectile.scale = 0.9f;
            maxTime = (int)(Owner.itemTimeMax);
            Smoother = Coralite.Instance.NoSmootherInstance;

            base.Initializer();
        }

        protected override void BeforeSlash()
        {
            startAngle -= Math.Sign(totalAngle) * 0.05f;
            _Rotation = startAngle;
            Slasher();
        }

        protected override void OnSlash()
        {
            Projectile.scale += 0.02f;
            if ((int)Timer == maxTime && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.One) * 8,
                    ProjectileType<GelSpawner>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            Slasher();
            if (Timer > maxTime + 28)
                Projectile.Kill();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target) => false;
    }

    public class GelSpawner : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 150;
            Projectile.extraUpdates = 50;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.usesIDStaticNPCImmunity = true;
        }

        public override void AI()
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.t_Slime, Alpha: 150, newColor: new Color(78, 136, 255, 80), Scale: Main.rand.NextFloat(1f, 1.4f));
            dust.noGravity = true;
            dust.velocity = -Projectile.velocity * Main.rand.NextFloat(0.1f, 0.3f);
        }

        public override void Kill(int timeLeft)
        {
            SpawnGelChaser();
        }

        private void SpawnGelChaser()
        {
            SoundEngine.PlaySound(CoraliteSoundID.QueenSlime2_Bubble_Item155, Projectile.Center);
            float rot = Main.rand.NextFloat(MathHelper.TwoPi);
            int damage = (int)(Projectile.damage * 0.8f);

            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = rot.ToRotationVector2();
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + dir * Main.rand.NextFloat(60, 80),
                    dir * Main.rand.NextFloat(2, 4), ProjectileType<GelChaser>(), damage, Projectile.knockBack, Projectile.owner, ai1: Projectile.Center.X, ai2: Projectile.Center.Y);
                rot += Main.rand.NextFloat(MathHelper.PiOver2 - 0.3f, MathHelper.PiOver2 + 0.3f);
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }

    /// <summary>
    /// 使用ai1,ai2传入目标位置
    /// </summary>
    public class GelChaser : ModProjectile
    {
        public override string Texture => AssetDirectory.SlimeEmperor + "SmallGelBall";

        protected float Scale;

        protected ref float State => ref Projectile.ai[0];
        protected Vector2 Center
        {
            get => new Vector2(Projectile.ai[1], Projectile.ai[2]);
            set
            {
                Projectile.ai[1] = value.X;
                Projectile.ai[2] = value.Y;
            }
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1200;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.t_Slime,
                      Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)) * Main.rand.NextFloat(0.2f, 0.5f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1f, 1.4f));
                dust.noGravity = true;
            }
        }

        public override void AI()
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.t_Slime, Alpha: 150, newColor: new Color(78, 136, 255, 80), Scale: Main.rand.NextFloat(1f, 1.4f));
            dust.noGravity = true;
            dust.velocity = -Projectile.velocity * Main.rand.NextFloat(0.1f, 0.3f);

            switch ((int)State)
            {
                default:
                case 0:
                    Scale += 0.05f;//膨胀小动画
                    if (Scale > 1)
                    {
                        Scale = 1f;
                        State = 1;
                    }

                    Projectile.rotation += 0.1f;
                    Projectile.velocity *= 0.99f;
                    Projectile.velocity = Projectile.velocity.RotatedBy(0.05f);
                    break;
                case 1:
                    Projectile.velocity = (Center - Projectile.Center).SafeNormalize(Vector2.One) * 10;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    State++;
                    break;
                case 2:
                    Projectile.localAI[2]++;
                    if (Projectile.localAI[2] > 120)
                        Projectile.Kill();

                    if (Vector2.Distance(Projectile.Center, Center) < 16)
                    {
                        State++;
                        Projectile.velocity *= 0;
                        Projectile.localAI[2] = 0;
                    }
                    break;
                case 3:
                    Projectile.localAI[2]++;
                    if (Projectile.localAI[2] > 4)
                        Projectile.Kill();
                    break;
            }
        }

        public override bool? CanHitNPC(NPC target) => State > 1 && !target.friendly;

        public override void Kill(int timeLeft)
        {
            //生成粒子

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;
            var pos = Projectile.Center - Main.screenPosition;
            Color color = lightColor * Scale;
            var frameBox = mainTex.Frame(1, 2, 0, 0);

            //绘制自己
            Main.spriteBatch.Draw(mainTex, pos, frameBox, color, Projectile.rotation, frameBox.Size() / 2, Scale, 0, 0);

            float factor = MathF.Sin(Main.GlobalTimeWrappedHourly);
            color = new Color(50, 152 + (int)(100 * factor), 225);
            color *=  0.75f;

            //绘制影子拖尾
            Projectile.DrawShadowTrails(color, 0.3f, 0.03f, 1, 8, 2, Scale, frameBox);

            //绘制发光
            frameBox = mainTex.Frame(1, 2, 0, 1);

            Main.spriteBatch.Draw(mainTex, pos, frameBox, color, Projectile.rotation, frameBox.Size() / 2, Scale, 0, 0);

            return false;
        }

    }
}
