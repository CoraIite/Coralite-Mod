using Coralite.Core.Loaders;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Coralite.Core.Systems.ParticleSystem
{
    public class ParticleGroup
    {
        private Particle[] _particles;

        public ParticleGroup(int maxParticle)
        {
            _particles = new Particle[maxParticle];
            for (int i = 0; i < maxParticle; i++)
                _particles[i] = new Particle();
        }

        public Particle NewParticle(Vector2 center, Vector2 velocity, int type, Color newColor = default, float Scale = 1f)
        {
            if (Main.netMode == NetmodeID.Server)
                return null;

            int result = -1;
            for (int i = 0; i < _particles.Length; i++)
            {
                Particle particle = _particles[i];
                if (particle.active)
                    continue;

                result = i;
                //设置各种初始值
                particle.fadeIn = 0f;
                particle.active = true;
                particle.type = type;
                particle.color = newColor;
                particle.center = center;
                particle.velocity = velocity;
                particle.shader = null;
                particle.frame.X = 0;
                particle.frame.Y = 0;
                particle.frame.Width = 8;
                particle.frame.Height = 8;
                particle.rotation = 0f;
                particle.scale = Scale;

                ParticleLoader.SetupParticle(particle);
                break;
            }

            if (result == -1)
                return null;

            return _particles[result];
        }

        public void UpdateParticles()
        {
            if (Main.netMode == NetmodeID.Server)//不在服务器上运行
                return;

            for (int i = 0; i < _particles.Length; i++)
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
        }

        public void DrawParticles(SpriteBatch spriteBatch)
        {
            ArmorShaderData armorShaderData = null;
            for (int i = 0; i < _particles.Length; i++)
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

                ParticleLoader.GetParticle(ParticleSystem.Particles[i].type).Draw(spriteBatch, particle);
            }
        }
    }
}
