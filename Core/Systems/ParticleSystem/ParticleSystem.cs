using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.Pools;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ID;

namespace Coralite.Core.Systems.ParticleSystem
{
    public class ParticleSystem : ModSystem
    {
        //public static Particle[] Particles = new Particle[Coralite.MaxParticleCount];
        public static List<Particle> Particles = new(VisualEffectSystem.ParticleCount);

        //public static ObjectPool pool=new ObjectPool(typeof(Particle),);

        public static Asset<Texture2D>[] ParticleAssets;

        public override void PostAddRecipes()
        {
            if (Main.dedServ)
                return;

            ParticleAssets = new Asset<Texture2D>[ParticleLoader.ParticleCount];

            for (int i = 0; i < ParticleLoader.ParticleCount; i++)
            {
                Particle particle = ParticleLoader.GetParticle(i);
                if (particle != null)
                    ParticleAssets[i] = ModContent.Request<Texture2D>(particle.Texture);
            }
        }

        public override void Unload()
        {
            ParticleLoader.Unload();

            ParticleAssets = null;
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

            for (int i = 0; i < Particles.Count; i++)
            {
                Particle particle = Particles[i];

                if (particle == null)
                    continue;

                try
                {
                    particle.Update();
                    if (particle.ShouldUpdateCenter())
                        particle.Center += particle.Velocity;

                    //在粒子不活跃时把一些东西释放掉
                    if (!particle.active)
                    {
                        particle.oldCenter = null;
                        particle.oldRot = null;
                    }

                    //一些防止粒子持续时间过长的措施，额...还是建议在update里手动设置active比较好
                    if (particle.shouldKilledOutScreen && !Helpers.Helper.OnScreen(particle.Center - Main.screenPosition))
                        particle.active = false;

                    if (particle.Scale < 0.001f)
                        particle.active = false;

                    if (particle.fadeIn > 1000)
                        particle.active = false;
                }
                catch (System.Exception)
                {
                    particle.active = false;
                }
            }

            Particles.RemoveAll(p => p == null || !p.active);
        }
    }
}
