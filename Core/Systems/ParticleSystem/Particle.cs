using Coralite.Core.Loaders;
using Coralite.Core.Systems.Trails;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Coralite.Core.Systems.ParticleSystem
{
    public class Particle
    {
        public Vector2 center;
        public Vector2[] oldCenter;
        public Vector2 velocity;
        public float fadeIn;
        public float scale;
        public float rotation;
        public float[] oldRot;
        //public bool noLight;
        public bool active;
        public Color color;
        //public int alpha;
        public Rectangle frame;
        public ArmorShaderData shader;
        public int type;
        public Trail trail;

        /// <summary>
        /// 用于存储数据的地方，可以自由地在这里存储各种数据
        /// </summary>
        public object[] datas;

        /// <summary>
        /// 生成粒子，返回粒子实例
        /// </summary>
        /// <param name="center"></param>
        /// <param name="velocity"></param>
        /// <param name="type"></param>
        /// <param name="newColor"></param>
        /// <param name="Scale"></param>
        /// <returns></returns>
        public static Particle NewParticleDirect(Vector2 center, Vector2 velocity, int type, Color newColor = default, float Scale = 1f)
        {
            return ParticleSystem.Particles[NewParticle(center, velocity, type, newColor, Scale)];
        }

        /// <summary>
        /// 生成例子，返回粒子在数组中的索引
        /// </summary>
        /// <param name="center"></param>
        /// <param name="velocity"></param>
        /// <param name="type"></param>
        /// <param name="newColor"></param>
        /// <param name="Scale"></param>
        /// <returns></returns>
        public static int NewParticle(Vector2 center, Vector2 velocity, int type, Color newColor = default, float Scale = 1f)
        {
            if (Main.netMode == NetmodeID.Server)
                return Coralite.MaxParticleCount - 1;

            int result = Coralite.MaxParticleCount - 1;
            for (int i = 0; i < Coralite.MaxParticleCount; i++)
            {
                Particle particle = ParticleSystem.Particles[i];
                if (particle.active)
                    continue;

                result = i;
                //设置各种初始值
                particle.fadeIn = 0f;
                particle.active = true;
                particle.type = type;
                particle.color = newColor;
                //particle.alpha = Alpha;
                particle.center = center;
                particle.velocity = velocity;
                particle.shader = null;
                particle.frame.X = 0;
                particle.frame.Y = 0;
                particle.frame.Width = 8;
                particle.frame.Height = 8;
                particle.rotation = 0f;
                particle.scale = Scale;
                //particle.noLight = false;

                ParticleLoader.SetupParticle(particle);

                break;
            }

            return result;
        }

        /// <summary>
        /// 初始化中心数组
        /// </summary>
        /// <param name="lenth"></param>
        public void InitOldCenters(int lenth)
        {
            oldCenter = new Vector2[lenth];
            for (int i = 0; i < lenth; i++)
            {
                oldCenter[i] = center;
            }
        }

        /// <summary>
        /// 初始化旋转数组
        /// </summary>
        /// <param name="lenth"></param>
        public void InitOldRotates(int lenth)
        {
            oldRot = new float[lenth];
            for (int i = 0; i < lenth; i++)
            {
                oldRot[i] = rotation;
            }
        }

        /// <summary>
        /// 初始化旧的中心和旋转数组
        /// </summary>
        /// <param name="lenth"></param>
        public void InitOldCaches(int lenth)
        {
            oldCenter = new Vector2[lenth];
            oldRot = new float[lenth];
            for (int i = 0; i < lenth; i++)
            {
                oldCenter[i] = center;
                oldRot[i] = rotation;
            }

        }

    }
}
