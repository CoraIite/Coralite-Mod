using Coralite.Core.Loaders;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Coralite.Core.Systems.ParticleSystem
{
    public class ParticleGroup : IEnumerable<Particle>
    {
        private List<Particle> _particles;

        public ParticleGroup()
        {
            _particles = new List<Particle>();
        }

        public IEnumerator<Particle> GetEnumerator()
        {
            return _particles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _particles.GetEnumerator();
        }

        public void Clear()
        {
            _particles.Clear();
        }

        public Particle this[int i] => _particles[i];

        public Particle NewParticle(Vector2 center, Vector2 velocity, int type, Color newColor = default, float Scale = 1f)
        {
            if (Main.netMode == NetmodeID.Server)
                return null;

            Particle particle = new Particle();
            particle.fadeIn = 0f;
            particle.active = true;
            particle.type = type;
            particle.color = newColor;
            particle.center = center;
            particle.velocity = velocity;
            particle.shader = null;
            particle.rotation = 0f;
            particle.scale = Scale;

            ParticleLoader.SetupParticle(particle);

            _particles.Add(particle);

            return particle;
        }

        public void Add(Particle particle)
        {
            _particles.Add(particle);
        }

        public void UpdateParticles()
        {
            if (Main.netMode == NetmodeID.Server)//不在服务器上运行
                return;

            for (int i = 0; i < _particles.Count; i++)
            {
                Particle particle = _particles[i];
                if (!particle.active)
                    continue;

                ModParticle modParticle = ParticleLoader.GetParticle(particle.type);
                modParticle.Update(particle);
                if (modParticle.ShouldUpdateCenter(particle))
                    particle.center += particle.velocity;

                //在粒子不活跃时把一些东西释放掉
                if (!particle.active)
                {
                    particle.shader = null;
                    particle.oldCenter = null;
                    particle.oldRot = null;
                    particle.trail = null;
                    particle.datas = null;
                }

                //一些防止粒子持续时间过长的措施，额...还是建议在update里手动设置active比较好
                if (particle.shouldKilledOutScreen && !Helper.OnScreen(particle.center - Main.screenPosition))
                    particle.active = false;

                if (particle.scale < 0.01f)
                    particle.active = false;

                if (particle.fadeIn > 1000)
                    particle.active = false;
            }

            _particles.RemoveAll(p => p is null || !p.active);
        }

        public void DrawParticles(SpriteBatch spriteBatch)
        {
            ArmorShaderData armorShaderData = null;
            for (int i = 0; i < _particles.Count; i++)
            {
                Particle particle = _particles[i];
                if (!particle.active)
                    continue;

                if (!Helper.OnScreen(particle.center - Main.screenPosition))
                    continue;

                if (particle.shader != armorShaderData)
                {
                    spriteBatch.End();
                    armorShaderData = particle.shader;
                    if (armorShaderData == null)
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, default, default, default, Main.GameViewMatrix.TransformationMatrix);
                    else
                    {
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
                        particle.shader.Apply(null);
                    }
                }

                ParticleLoader.GetParticle(particle.type).Draw(spriteBatch, particle);
            }

            if (armorShaderData != null)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, default, Main.GameViewMatrix.TransformationMatrix);
            }
        }

        public void DrawParticlesInUI(SpriteBatch spriteBatch)
        {
            ArmorShaderData armorShaderData = null;
            for (int i = 0; i < _particles.Count; i++)
            {
                Particle particle = _particles[i];
                if (!particle.active)
                    continue;

                if (!Helper.OnScreen(particle.center))
                    continue;

                if (particle.shader != armorShaderData)
                {
                    spriteBatch.End();
                    armorShaderData = particle.shader;
                    if (armorShaderData == null)
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, default, default, default, Main.UIScaleMatrix);
                    else
                    {
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
                        particle.shader.Apply(null);
                    }
                }

                ParticleLoader.GetParticle(particle.type).DrawInUI(spriteBatch, particle);
            }

            if (armorShaderData != null)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, default, default, default, Main.UIScaleMatrix);
            }

        }

        public void DrawParticlesPrimitive()
        {
            for (int k = 0; k < _particles.Count; k++) // Particles.
                if (_particles[k].active)
                {
                    ModParticle modParticle = ParticleLoader.GetParticle(_particles[k].type);
                    if (modParticle is IDrawParticlePrimitive p)
                        p.DrawPrimitives(_particles[k]);
                }
        }
    }
}
