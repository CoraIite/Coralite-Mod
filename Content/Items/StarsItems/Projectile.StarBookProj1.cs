using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using static Coralite.Core.VertexInfos;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.StarsItems
{
    public class StarBookProj1 : BaseChannelProj, IDrawWarp, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.StarsProjectiles + "StarBookMagic";

        public const string STARSTRAIL = AssetDirectory.OtherProjectiles + "StarsTrail";

        public ref float Count => ref Projectile.ai[0];
        public ref float LightScale => ref Projectile.localAI[0];

        public bool canExplore = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星之书-星光球");
        }

        public override void SetDefaults()
        {
            Projectile.width = 75;
            Projectile.height = 75;

            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 500;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        #region AI

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                _Rotation = Main.rand.Next(8) * 0.785f - 1.57f;
                Projectile.netUpdate = true;
            }
        }
        protected override void AIBefore()
        {
            if (!completeAndRelease)
                Owner.itemTime = Owner.itemAnimation = 2;
        }

        protected override void AIAfter()
        {
            timer++;
        }

        protected override void CompleteAndRelease()
        {
            //控制大球飞出
            if (timer < 2)
            {
                Projectile.velocity = Vector2.Normalize(Main.MouseWorld - Projectile.Center) * 16f;
                LightScale = 0f;
            }

            //添加光亮
            Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.97f, 0.5f) * 1.8f);

            //控制scale，主要是绘制会变
            if (canExplore && LightScale < 0.4f)
                LightScale += 0.03f;
            else
            {
                canExplore = false;
                Point TileCoordinates = Projectile.Center.ToTileCoordinates();
                Tile tile = Framing.GetTileSafely(TileCoordinates);
                if (tile.HasTile && WorldGen.SolidOrSlopedTile(tile))//有实心方块时减小scale
                {
                    LightScale -= 0.03f;
                    Projectile.alpha -= 30;
                }

                if (LightScale < 0.2f)
                    Projectile.Kill();
            }

            //记录拖尾数组
            for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
                Projectile.oldPos[i] = Projectile.oldPos[i - 1];

            Projectile.oldPos[0] = Projectile.Center;
        }

        protected override void OnChannel()
        {
            if (Count >= 8)
            {
                OnChannelComplete(1500, 15);
                Projectile.Center = Owner.Center;
                Projectile.oldPos = new Vector2[20];
                Projectile.damage = (int)(Projectile.damage * 1.5f);
                Helper.PlayPitched("Stars/StarsSpawn", 0.3f, 0f, Projectile.Center);
                for (int i = 0; i < 20; i++)
                    Projectile.oldPos[i] = Projectile.Center;
                return;
            }

            if (timer < 22 && timer % 3 == 0)
            {
                //发射小符文
                float rotate = _Rotation + 0.785f * (timer / 3);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center + rotate.ToRotationVector2() * 120f, Vector2.Zero, ProjectileType<StarBookProj2>(), (int)(Projectile.damage * 0.7f * (timer / 3 + 1) / 8), Projectile.knockBack, Projectile.owner);
            }

            Projectile.Center = Owner.Center;

            if (LightScale < 0.02f)
                LightScale = 0.7f;
            else
                LightScale -= 0.016f;
        }

        protected override void OnRelease()
        {
            canChannel = false;
            LightScale = 0;

            //控制所有的小符文转状态
            int projType = ProjectileType<StarBookProj2>();
            for (int i = 0; i < 1000; i++)
                if (Main.projectile[i].type == projType && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].ai[0] == 0)
                {
                    Main.projectile[i].ai[0] = 1;
                    Main.projectile[i].velocity = Vector2.Normalize(Main.projectile[i].Center - Owner.Center) * 18f;
                }

            if (timer > 12)
                SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
            Projectile.Kill();
        }

        #endregion

        #region 绘制

        public override bool PreDraw(ref Color lightColor)
        {
            //绘制拖尾
            if (completeAndRelease && timer > 1)
                DrawTrail();

            return false;
        }

        public void DrawTrail()
        {
            //RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

            Vector2 dir = Vector2.Normalize(Projectile.velocity.RotatedBy(1.57f));
            Vector2 Top = Projectile.oldPos[0] + dir * 30;
            Vector2 Bottom = Projectile.oldPos[0] - dir * 30;

            Color starYellow = new Color(255, 254, 191, (int)(150 + MathF.Cos(timer * 0.1f) * 100));
            var w = 1f;
            bars.Add(new(Top - Main.screenPosition, starYellow, new Vector3(1, 1, w)));
            bars.Add(new(Bottom - Main.screenPosition, starYellow, new Vector3(1, 0, w)));

            Top = Projectile.oldPos[19] + dir * 40;
            Bottom = Projectile.oldPos[19] - dir * 40;
            w = 1f;
            bars.Add(new(Top - Main.screenPosition, starYellow, new Vector3(0, 1, w)));
            bars.Add(new(Bottom - Main.screenPosition, starYellow, new Vector3(0, 0, w)));


            List<CustomVertexInfo> triangleList = new()
            {
                bars[0],
                bars[1],
                bars[2],

                bars[1],
                bars[2],
                bars[3]
            };

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

            Main.graphics.GraphicsDevice.Textures[0] = Request<Texture2D>(STARSTRAIL).Value;
            //Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
            //Main.graphics.GraphicsDevice.RasterizerState = originalState;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }

        public void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;
            Vector2 origin = new Vector2(128, 128);

            float cosProgress = MathF.Cos(timer * 0.1f);
            float currentScale = LightScale * (1f + cosProgress * 0.1f);
            int a = (int)(160 - cosProgress * 40);
            //绘制圈圈
            Rectangle source = new Rectangle(0, 0, 256, 256);

            spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, source,
                                    new Color(255, 255, 255, a), timer * 0.1f, origin, currentScale, SpriteEffects.None, 0f);

            //绘制光球
            source = new Rectangle(0, 256, 256, 256);
            currentScale = LightScale * (1.5f + cosProgress * 0.3f);
            spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, source,
                                    Color.White, timer * 0.15f, origin, currentScale, SpriteEffects.None, 0f);
            a = (int)(140 + cosProgress * 40);
            spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, source,
                        new Color(255, 255, 255, a), -timer * 0.17f, origin, currentScale, SpriteEffects.FlipVertically, 0f);

        }

        public void DrawWarp()
        {
            if (completeAndRelease)
                return;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Texture2D mainTex = TextureAssets.Projectile[Type].Value;
            Vector2 origin = new Vector2(mainTex.Width / 2, mainTex.Height / 6);
            Rectangle source = new Rectangle(0, 255, 256, 256);

            Main.spriteBatch.Draw(mainTex, Projectile.Center + new Vector2(Owner.direction * 20, -20) - Main.screenPosition, source, Color.White, timer * 0.1f, origin, LightScale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            if (completeAndRelease && timer > 1)
                DrawSelf(spriteBatch);
        }

        #endregion

        #region 碰撞

        public override bool? CanDamage()
        {
            if (completeAndRelease)
                return true;

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (completeAndRelease && !canExplore)
            {
                LightScale -= 0.04f;
                Projectile.alpha -= 30;
                Projectile.damage = (int)(Projectile.damage * 0.7f);

                Color starYellow = new Color(255, 254, 191);
                if (Main.netMode != NetmodeID.Server)
                    for (int i = 0; i < 2; i++)
                        Particle.NewParticle(target.position + Main.rand.NextVector2CircularEdge(target.width, target.height),
                            Main.rand.NextVector2CircularEdge(1, 1), CoraliteContent.ParticleType<HorizontalStar>(), starYellow, 0.3f);

            }
        }

        public override void Kill(int timeLeft)
        {
            Color starYellow = new Color(255, 254, 191);
            if (Main.netMode != NetmodeID.Server)
                for (int i = 0; i < 6; i++)
                    Particle.NewParticle(Projectile.Center, Vector2.Normalize(Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f))) * Main.rand.Next(6, 9),
                       CoraliteContent.ParticleType<HorizontalStar>(), starYellow, 0.3f);
        }

        #endregion
    }
}

//下面这个是使用Old贴图时的代码，因为换了贴图所以就全废弃了
//public void DrawSelf()
//{
//    Texture2D mainTex = TextureAssets.Projectile[Type].Value;
//    Vector2 origin = new Vector2(mainTex.Width / 2, mainTex.Height / 6);

//    float cosProgress = Helper.Cos(timer * 0.1f);
//    int r = (int)(238.5f + cosProgress * 16.5f);
//    int g = (int)(230.5f + cosProgress * 23.5f);
//    int b = (int)(156 + cosProgress * 35);
//    int a = Projectile.alpha - 35;
//    float currentScale = LightScale * (1f + (cosProgress * 0.2f));

//    //绘制光球
//    Rectangle source = new Rectangle(0, 0, 256, 256);
//    for (int i = 0; i < 2; i++)
//        Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, source,
//                                new Color(r, g, b, a), 0f, origin, currentScale, SpriteEffects.None, 0f);

//    //绘制圈圈
//    source = new Rectangle(0, 256, 256, 256);
//    a = 70 - 40 * (1 - (Projectile.alpha / 255));
//    currentScale = LightScale * (1f - (cosProgress * (cosProgress > 0 ? 1f : 0.4f)));
//    Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, source,
//                            new Color(r, g, b, a), 0f, origin, currentScale, SpriteEffects.None, 0f);

//    //绘制光芒*2
//    float rotation = timer * 0.04f;
//    a = (int)(100 - cosProgress * 99);
//    source = new Rectangle(0, 512, 256, 256);
//    currentScale = LightScale * (0.8f - (cosProgress * 0.2f));
//    Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, source,
//                            new Color(r, g, b, a), rotation, origin, currentScale, SpriteEffects.None, 0f);

//    rotation = -rotation;
//    a = (int)(50 - cosProgress * 30);
//    currentScale = LightScale * (0.8f + (cosProgress * 0.2f));
//    Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, source,
//                            new Color(r, g, b, a), rotation, origin, currentScale, SpriteEffects.FlipHorizontally, 0f);
//}