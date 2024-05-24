using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Coralite.Content.CustomHooks
{
    public class Drawers : HookGroup
    {
        // 应该不会干涉任何东西
        public override SafetyLevel Safety => SafetyLevel.Safe;

        public static BlendState Reverse;

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
        private void Drawer(Terraria.On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);

            if (Main.gameMenu)
                return;

            //绘制拖尾
            SpriteBatch spriteBatch = Main.spriteBatch;
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            if (VisualEffectSystem.DrawTrail)
            {
                for (int k = 0; k < Main.maxProjectiles; k++) // Projectiles.
                    if (Main.projectile[k].active && Main.projectile[k].ModProjectile is IDrawPrimitive)
                        (Main.projectile[k].ModProjectile as IDrawPrimitive).DrawPrimitives();

                for (int k = 0; k < Main.maxNPCs; k++) // NPCs.
                    if (Main.npc[k].active && Main.npc[k].ModNPC is IDrawPrimitive)
                        (Main.npc[k].ModNPC as IDrawPrimitive).DrawPrimitives();

                for (int i = 0; i < ParticleSystem.Particles.Count; i++)
                {
                    Particle particle = ParticleSystem.Particles[i];
                    if (particle != null && particle.active && particle is IDrawParticlePrimitive p)
                        p.DrawPrimitives();
                }
            }

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int k = 0; k < Main.maxProjectiles; k++) //Projectiles
                if (Main.projectile[k].active && Main.projectile[k].ModProjectile is IPostDrawAdditive add)
                    add.DrawAdditive(spriteBatch);

            for (int k = 0; k < Main.maxNPCs; k++) //NPCs
                if (Main.npc[k].active && Main.npc[k].ModNPC is IPostDrawAdditive add)
                    add.DrawAdditive(spriteBatch);

            spriteBatch.End();

            //绘制Non
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int k = 0; k < Main.maxProjectiles; k++) //Projectiles
                if (Main.projectile[k].active && Main.projectile[k].ModProjectile is IDrawNonPremultiplied non)
                    non.DrawNonPremultiplied(Main.spriteBatch);

            for (int k = 0; k < Main.maxNPCs; k++) //NPCs
                if (Main.npc[k].active && Main.npc[k].ModNPC is IDrawNonPremultiplied non)
                    non.DrawNonPremultiplied(Main.spriteBatch);

            for (int i = 0; i < ParticleSystem.Particles.Count; i++)
            {
                Particle particle = ParticleSystem.Particles[i];
                if (particle != null && particle.active && particle is IDrawNonPremultiplied sp)
                    sp.DrawNonPremultiplied(spriteBatch);
            }

            spriteBatch.End();

            ////绘制反色
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, Reverse, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            //for (int k = 0; k < Main.maxProjectiles; k++) //Projectiles
            //    if (Main.projectile[k].active && Main.projectile[k].ModProjectile is IDrawColorReverse)
            //        (Main.projectile[k].ModProjectile as IDrawColorReverse).DrawColorReverse(Main.spriteBatch);

            //for (int k = 0; k < Main.maxNPCs; k++) //NPCs
            //    if (Main.npc[k].active && Main.npc[k].ModNPC is IDrawColorReverse)
            //        (Main.npc[k].ModNPC as IDrawColorReverse).DrawColorReverse(Main.spriteBatch);

            //spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            //绘制自己的粒子
            ArmorShaderData armorShaderData = null;
            for (int i = 0; i < ParticleSystem.Particles.Count; i++)
            {
                Particle particle = ParticleSystem.Particles[i];
                if (particle == null || !particle.active)
                    continue;

                if (!Helper.OnScreen(particle.Center - Main.screenPosition))
                    continue;

                if (particle.shader != armorShaderData)
                {
                    spriteBatch.End();
                    armorShaderData = particle.shader;
                    if (armorShaderData == null)
                    {
                        spriteBatch.Begin();
                        spriteBatch.End();

                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, default, default, default, Main.GameViewMatrix.TransformationMatrix);
                    }
                    else
                    {
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
                        particle.shader.Apply(null);
                    }
                }

                particle.Draw(spriteBatch);
            }

            spriteBatch.End();
        }
    }
}

