using Coralite.Core.Loaders;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.ParticleSystem
{
    public class ParticleSystem : ModSystem
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
            if (Main.netMode == NetmodeID.Server)//不在服务器上运行
                return;
            if (Main.gameInactive)//不在游戏暂停时运行
                return;

            for (int i = 0; i < Coralite.MaxParticleCount; i++)
            {
                Particle particle = Particles[i];
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
                if (particle.shouldKilledOutScreen && !Helpers.Helper.OnScreen(particle.center - Main.screenPosition))
                    particle.active = false;

                if (particle.scale < 0.01f)
                    particle.active = false;

                if (particle.fadeIn > 1000)
                    particle.active = false;
            }
        }
    }
}
