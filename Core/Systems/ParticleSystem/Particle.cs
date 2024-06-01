using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Coralite.Core.Systems.ParticleSystem
{
    public abstract class Particle : ModTexturedType
    {
        public int Type { get; internal set; }
        //(GetType().Namespace + "." + Name).Replace('.', '/');    GetType().FullName.Replace('.', '/');
        public override string Texture => AssetDirectory.Particles + Name;

        public Vector2 Center;
        public Vector2[] oldCenter;
        public Vector2 Velocity;
        public float fadeIn;
        public float Scale;
        public float Rotation;
        public float[] oldRot;
        public bool active;
        public bool shouldKilledOutScreen = true;
        public Color color;
        public Rectangle Frame;
        public ArmorShaderData shader;

        /// <summary>
        /// 用于存储数据的地方，可以自由地在这里存储各种数据
        /// </summary>
        //public object[] datas;

        protected sealed override void Register()
        {
            ModTypeLookup<Particle>.Register(this);

            ParticleLoader.Particles ??= new List<Particle>();
            ParticleLoader.Particles.Add(this);

            Type = ParticleLoader.ReserveParticleID();
        }

        public virtual Particle NewInstance()
        {
            var inst = (Particle)Activator.CreateInstance(GetType(), true);
            inst.Type = Type;
            return inst;
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
        public static T NewParticle<T>(Vector2 center, Vector2 velocity, Color newColor = default, float Scale = 1f) where T : Particle
        {
            if (Main.netMode == NetmodeID.Server)
                return null;

            if (ParticleSystem.Particles.Count > VisualEffectSystem.ParticleCount - 2)
                return null;

            T p = ParticleLoader.GetParticle(CoraliteContent.ParticleType<T>()).NewInstance() as T;

            //设置各种初始值
            p.active = true;
            p.color = newColor;
            p.Center = center;
            p.Velocity = velocity;
            p.Scale = Scale;
            p.OnSpawn();

            ParticleSystem.Particles.Add(p);

            return p;
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
        public static Particle NewParticle(Vector2 center, Vector2 velocity, int type, Color newColor = default, float Scale = 1f)
        {
            if (Main.netMode == NetmodeID.Server)
                return null;

            if (ParticleSystem.Particles.Count > VisualEffectSystem.ParticleCount - 2)
                return null;

            Particle p = ParticleLoader.GetParticle(type).NewInstance();

            //设置各种初始值
            p.active = true;
            p.color = newColor;
            p.Center = center;
            p.Velocity = velocity;
            p.Scale = Scale;
            p.OnSpawn();

            ParticleSystem.Particles.Add(p);

            return p;
        }

        public static T NewPawticleInstance<T>(Vector2 center, Vector2 velocity, Color newColor = default, float Scale = 1f) where T : Particle
        {
            if (Main.netMode == NetmodeID.Server)
                return null;

            T p = ParticleLoader.GetParticle(CoraliteContent.ParticleType<T>()).NewInstance() as T;

            //设置各种初始值
            p.active = true;
            p.color = newColor;
            p.Center = center;
            p.Velocity = velocity;
            p.Scale = Scale;
            p.OnSpawn();

            return p;
        }

        public virtual void OnSpawn() { }

        public virtual void Update() { }

        public virtual bool ShouldUpdateCenter() => true;

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Rectangle frame = this.Frame;
            Vector2 origin = new Vector2(frame.Width / 2, frame.Height / 2);

            spriteBatch.Draw(GetTexture().Value, Center - Main.screenPosition, frame, color, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }

        public virtual void DrawInUI(SpriteBatch spriteBatch)
        {
            Rectangle frame = this.Frame;
            Vector2 origin = new Vector2(frame.Width / 2, frame.Height / 2);

            spriteBatch.Draw(GetTexture().Value, Center, frame, color, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }

        public Asset<Texture2D> GetTexture() => ParticleSystem.ParticleAssets[Type];
        
        /// <summary>
        /// 初始化中心数组
        /// </summary>
        /// <param name="length"></param>
        public void InitOldCenters(int length)
        {
            oldCenter = new Vector2[length];
            for (int i = 0; i < length; i++)
                oldCenter[i] = Center;
        }

        /// <summary>
        /// 初始化旋转数组
        /// </summary>
        /// <param name="length"></param>
        public void InitOldRotates(int length)
        {
            oldRot = new float[length];
            for (int i = 0; i < length; i++)
                oldRot[i] = Rotation;
        }

        /// <summary>
        /// 初始化旧的中心和旋转数组
        /// </summary>
        /// <param name="length"></param>
        public void InitOldCaches(int length)
        {
            oldCenter = new Vector2[length];
            oldRot = new float[length];
            for (int i = 0; i < length; i++)
            {
                oldCenter[i] = Center;
                oldRot[i] = Rotation;
            }

        }

        /// <summary>
        /// 普普通通地更新记录点
        /// </summary>
        /// <param name="length"></param>
        public void UpdatePosCachesNormally(int length)
        {
            if (oldCenter is null || length > oldCenter.Length)
                return;

            for (int i = 0; i < length - 1; i++)
                oldCenter[i] = oldCenter[i + 1];

            oldCenter[length - 1] = Center;
        }

        public void UpdateRotCachesNormally(int length)
        {
            if (oldRot is null || length > oldRot.Length)
                return;

            for (int i = 0; i < length - 1; i++)
                oldRot[i] = oldRot[i + 1];

            oldRot[length - 1] = Rotation;
        }

    }
}
