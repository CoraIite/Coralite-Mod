using Coralite.Core.Loaders;
using Coralite.Helpers;
using InnoVault.PRT;
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

        public IEnumerator<Particle> GetEnumerator() => _particles.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _particles.GetEnumerator();

        public void Clear() => _particles.Clear();

        public Particle this[int i] => _particles[i];

        public T NewParticle<T>(Vector2 center, Vector2 velocity, Color newColor = default, float Scale = 1f) where T : Particle
        {
            if (Main.netMode == NetmodeID.Server)
                return null;

            T p = ParticleLoader.GetParticle(CoraliteContent.ParticleType<T>()).Clone() as T;

            //设置各种初始值
            p.active = true;
            p.Color = newColor;
            p.Position = center;
            p.Velocity = velocity;
            p.Scale = Scale;
            p.SetProperty();

            _particles.Add(p);

            return p;
        }

        public Particle NewParticle(Vector2 center, Vector2 velocity, int type, Color newColor = default, float Scale = 1f)
        {
            if (Main.netMode == NetmodeID.Server)
                return null;

            Particle p = ParticleLoader.GetParticle(type).Clone();

            //设置各种初始值
            p.active = true;
            p.Color = newColor;
            p.Position = center;
            p.Velocity = velocity;
            p.Scale = Scale;
            p.SetProperty();

            _particles.Add(p);

            return p;
        }

        public void Add(Particle particle)
        {
            _particles.Add(particle);
        }

        public bool Any() => _particles.Count > 0;

        public void UpdateParticles()
        {
            if (Main.netMode == NetmodeID.Server)//不在服务器上运行
                return;

            for (int i = 0; i < _particles.Count; i++)
            {
                var particle = _particles[i];

                if (particle == null)
                    continue;

                particle.AI();
                if (particle.ShouldUpdateCenter())
                    particle.Position += particle.Velocity;

                //在粒子不活跃时把一些东西释放掉
                if (!particle.active)
                {
                    particle.oldPositions = null;
                    particle.oldRotations = null;
                }

                //一些防止粒子持续时间过长的措施，额...还是建议在update里手动设置active比较好
                //if (particle.shouldKilledOutScreen && !Helper.OnScreen(particle.Center - Main.screenPosition))
                //    particle.active = false;

                if (particle.Scale < 0.001f)
                    particle.active = false;

                if (particle.fadeIn > 1000)
                    particle.active = false;

                if (!particle.active)
                    _particles.Remove(particle);
            }

            _particles.RemoveAll(p => p is null || !p.active);
        }

        public void DrawParticles(SpriteBatch spriteBatch)
        {
            //ArmorShaderData armorShaderData = null;
            //for (int i = 0; i < _particles.Count; i++)
            //{
            //    var particle = _particles[i];
            //    if (particle == null || !particle.active)
            //        continue;

            //    if (!Helper.OnScreen(particle.Position - Main.screenPosition))
            //        continue;

            //    if (particle.shader != armorShaderData)
            //    {
            //        spriteBatch.End();
            //        armorShaderData = particle.shader;
            //        if (armorShaderData == null)
            //            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, default, default, default, Main.GameViewMatrix.TransformationMatrix);
            //        else
            //        {
            //            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
            //            particle.shader.Apply(null);
            //        }
            //    }

            //    particle.Draw(spriteBatch);
            //}

            //if (armorShaderData != null)
            //{
            //    spriteBatch.End();
            //    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, default, Main.GameViewMatrix.TransformationMatrix);
            //}
        }

        public void DrawParticlesInUI(SpriteBatch spriteBatch)
        {
            //ArmorShaderData armorShaderData = null;
            //foreach (var particle in _particles)
            //{
            //    if (!particle.active)
            //        continue;

            //    if (!Helper.OnScreen(particle.Position))
            //        continue;

            //    if (particle.shader != armorShaderData)
            //    {
            //        spriteBatch.End();
            //        armorShaderData = particle.shader;
            //        if (armorShaderData == null)
            //            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, default, default, default, Main.UIScaleMatrix);
            //        else
            //        {
            //            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            //            particle.shader.Apply(null);
            //        }
            //    }

            //    particle.DrawInUI(spriteBatch);
            //}

            //if (armorShaderData != null)
            //{
            //    spriteBatch.End();
            //    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, default, default, default, Main.UIScaleMatrix);
            //}
        }

        public void DrawParticlesPrimitive()
        {
            foreach (var particle in _particles)
                if (particle.active && particle is IDrawParticlePrimitive p)
                    p.DrawPrimitives();
        }
    }
}
