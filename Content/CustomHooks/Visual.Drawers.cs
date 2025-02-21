using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Systems.ParticleSystem;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.CustomHooks
{
    public class Drawers : HookGroup
    {
        // 应该不会干涉任何东西
        public override SafetyLevel Safety => SafetyLevel.Safe;

        public static BlendState Reverse;

        public static List<Point> SpecialTiles = [];
        public static Dictionary<Point,byte> SpecialTilesCounter = [];

        public override void Load()
        {
            if (Main.dedServ)
                return;

            On_Main.DrawDust += Drawer;

            Reverse = new BlendState()
            {
                ColorBlendFunction = BlendFunction.ReverseSubtract,

                ColorSourceBlend = Blend.One,
                AlphaSourceBlend = Blend.One,
                ColorDestinationBlend = Blend.InverseSourceAlpha,
                AlphaDestinationBlend = Blend.InverseSourceAlpha
            };
        }

        /// <summary>
        /// 在绘制粒子之后插一段，使用自己的渲染方式
        /// </summary>
        private void Drawer(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);

            if (Main.gameMenu)
                return;

            //绘制拖尾
            SpriteBatch spriteBatch = Main.spriteBatch;
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            if (VisualEffectSystem.DrawTrail)
            {
                foreach (var p in Main.projectile)
                {
                    if (p.ModProjectile == null || !p.active)
                    {
                        continue;
                    }
                    if (p.ModProjectile is IDrawPrimitive primitive)
                    {
                        primitive.DrawPrimitives();
                    }
                }

                foreach (var n in Main.npc)
                {
                    if (n.ModNPC == null || !n.active)
                    {
                        continue;
                    }
                    if (n.ModNPC is IDrawPrimitive primitive)
                    {
                        primitive.DrawPrimitives();
                    }
                }

                foreach (var prt in PRTLoader.PRT_InGame_World_Inds)
                {
                    if (prt == null || !prt.active)
                    {
                        continue;
                    }
                    if (prt is IDrawParticlePrimitive p)
                    {
                        p.DrawPrimitive();
                    }
                }
            }

            //绘制Non
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int k = 0; k < Main.maxProjectiles; k++) //Projectiles
                if (Main.projectile[k].active && Main.projectile[k].ModProjectile is IDrawNonPremultiplied non)
                    non.DrawNonPremultiplied(Main.spriteBatch);

            for (int k = 0; k < Main.maxNPCs; k++) //NPCs
                if (Main.npc[k].active && Main.npc[k].ModNPC is IDrawNonPremultiplied non)
                    non.DrawNonPremultiplied(Main.spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int k = 0; k < Main.maxProjectiles; k++) //Projectiles
                if (Main.projectile[k].active && Main.projectile[k].ModProjectile is IPostDrawAdditive add)
                    add.DrawAdditive(spriteBatch);

            for (int k = 0; k < Main.maxNPCs; k++) //NPCs
                if (Main.npc[k].active && Main.npc[k].ModNPC is IPostDrawAdditive add)
                    add.DrawAdditive(spriteBatch);

            spriteBatch.End();

            if (SpecialTiles.Count != 0)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                for (int i = SpecialTiles.Count - 1; i >= 0; i--)
                {
                    var p = SpecialTiles[i];
                    Tile t = Main.tile[p];

                    ModTile mt = TileLoader.GetTile(t.TileType);
                    mt?.SpecialDraw(p.X, p.Y, Main.spriteBatch);

                    SpecialTilesCounter[p]--;
                    if (SpecialTilesCounter[p] < 1)
                    {
                        SpecialTiles.Remove(p);
                        SpecialTilesCounter.Remove(p);
                    }
                }
                Main.NewText(SpecialTiles.Count);
                spriteBatch.End();
            }

            ////绘制反色
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, Reverse, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            //for (int k = 0; k < Main.maxProjectiles; k++) //Projectiles
            //    if (Main.projectile[k].active && Main.projectile[k].ModProjectile is IDrawColorReverse)
            //        (Main.projectile[k].ModProjectile as IDrawColorReverse).DrawColorReverse(Main.spriteBatch);

            //for (int k = 0; k < Main.maxNPCs; k++) //NPCs
            //    if (Main.npc[k].active && Main.npc[k].ModNPC is IDrawColorReverse)
            //        (Main.npc[k].ModNPC as IDrawColorReverse).DrawColorReverse(Main.spriteBatch);

            //spriteBatch.End();

            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            ////绘制自己的粒子
            //ArmorShaderData armorShaderData = null;
            //for (int i = 0; i < ParticleSystem.Particles.Count; i++)
            //{
            //    Particle particle = ParticleSystem.Particles[i];
            //    if (particle == null || !particle.active)
            //        continue;

            //    if (!VaultUtils.IsPointOnScreen(particle.Position - Main.screenPosition))
            //        continue;

            //    if (particle.shader != armorShaderData)
            //    {
            //        spriteBatch.End();
            //        armorShaderData = particle.shader;
            //        if (armorShaderData == null)
            //        {
            //            spriteBatch.Begin();
            //            spriteBatch.End();

            //            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, default, default, default, Main.GameViewMatrix.TransformationMatrix);
            //        }
            //        else
            //        {
            //            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
            //            particle.shader.Apply(null);
            //        }
            //    }

            //    particle.Draw(spriteBatch);
            //}

            //spriteBatch.End();
        }

        public static void AddSpecialTile(int i, int j)
        {
            if (SpecialTilesCounter.TryGetValue(new Point(i, j), out _))
                SpecialTilesCounter[new Point(i, j)] = 5;
            else
            {
                SpecialTilesCounter.Add(new Point(i, j), 5);
                SpecialTiles.Add(new Point(i, j));
            }
        }
    }
}

