using Coralite.Core.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.ParticleSystem
{
    public class ParticleSystem: ModSystem
    {
        public static Particle[] Particles = new Particle[Coralite.MaxParticleCount];

        public ParticleSystem()
        {
            Particles = new Particle[Coralite.MaxParticleCount];

            for (int j = 0; j < Coralite.MaxParticleCount; j++)
                Particles[j] = new Particle();
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            Particles = new Particle[Coralite.MaxParticleCount];

            for (int j = 0; j < Coralite.MaxParticleCount; j++)
                Particles[j] = new Particle();
        }

        public override void Unload()
        {
            ParticleLoader.Unload();
            for (int j = 0; j < Coralite.MaxParticleCount; j++)
                Particles[j] = null;

            Particles = null;
        }

        public override void PostUpdateDusts()
        {
            UpdateParticle();
        }

        /// <summary>
        /// 更新粒子
        /// </summary>
        public static void UpdateParticle()
        {
            //不在服务器上运行
            if (Main.netMode == NetmodeID.Server)
                return;

            for (int i = 0; i < Coralite.MaxParticleCount; i++)
            {
                Particle particle = Particles[i];
                if (!particle.active)
                    continue;

                //不在游戏暂停时运行
                if (!Main.gameInactive)
                {
                    ModParticle modParticle= ParticleLoader.GetParticle(particle.type);
                    modParticle.Update(particle);
                    if (modParticle.ShouldUpdateCenter(particle))
                        particle.center += particle.velocity;
                }

                //一些防止粒子持续时间过长的措施，额...还是建议在update里手动设置active比较好
                if (particle.center.Y > Main.screenPosition.Y + Main.screenHeight)
                    particle.active = false;

                if (particle.scale < 0.01f)
                    particle.active = false;

                if (particle.fadeIn > 200)
                    particle.active = false;
            }
        }
    }
}
