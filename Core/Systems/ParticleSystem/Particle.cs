using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Coralite.Core.Systems.ParticleSystem
{
    public abstract class Particle : ModTexturedType
    {
        public int ID { get; internal set; }
        //(GetType().Namespace + "." + Name).Replace('.', '/');    GetType().FullName.Replace('.', '/');
        public override string Texture => AssetDirectory.Particles + Name;

        public Vector2 Position;
        public Vector2[] oldPositions;
        public Vector2 Velocity;
        public float fadeIn;
        public float Scale;
        public float Rotation;
        public float[] oldRotations;
        public bool active;
        public bool shouldKilledOutScreen = true;
        public Color color;
        public Rectangle Frame;
        public ArmorShaderData shader;

        public bool drawNonPremultiplied;

        /// <summary>
        /// 用于存储数据的地方，可以自由地在这里存储各种数据
        /// </summary>
        //public object[] datas;

        protected sealed override void Register()
        {
            ModTypeLookup<Particle>.Register(this);

            ParticleLoader.Particles ??= new List<Particle>();
            ParticleLoader.Particles.Add(this);

            ID = ParticleLoader.ReserveParticleID();
        }

        public virtual Particle Clone()
        {
            var inst = (Particle)Activator.CreateInstance(GetType(), true);
            inst.ID = ID;
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

            T p = ParticleLoader.GetParticle(CoraliteContent.ParticleType<T>()).Clone() as T;

            //设置各种初始值
            p.active = true;
            p.color = newColor;
            p.Position = center;
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

            Particle p = ParticleLoader.GetParticle(type).Clone();

            //设置各种初始值
            p.active = true;
            p.color = newColor;
            p.Position = center;
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

            T p = ParticleLoader.GetParticle(CoraliteContent.ParticleType<T>()).Clone() as T;

            //设置各种初始值
            p.active = true;
            p.color = newColor;
            p.Position = center;
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
            Rectangle frame = Frame;
            Vector2 origin = frame.Size() / 2;

            spriteBatch.Draw(TexValue, Position - Main.screenPosition, frame, color, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }

        public virtual void DrawInUI(SpriteBatch spriteBatch)
        {
            Rectangle frame = Frame;
            Vector2 origin = frame.Size() / 2;

            spriteBatch.Draw(TexValue, Position, frame, color, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }

        public virtual void DrawNonPremultiplied(SpriteBatch spriteBatch) { }

        /// <summary>
        /// 获取加载的粒子纹理资源
        /// </summary>
        public Texture2D TexValue;

        /// <summary>
        /// 初始化中心数组
        /// </summary>
        /// <param name="length"></param>
        public void InitializePositionCache(int length)
        {
            oldPositions = new Vector2[length];
            for (int i = 0; i < length; i++)
                oldPositions[i] = Position;
        }

        /// <summary>
        /// 初始化旋转数组
        /// </summary>
        /// <param name="length"></param>
        public void InitializeRotationCache(int length)
        {
            oldRotations = new float[length];
            for (int i = 0; i < length; i++)
                oldRotations[i] = Rotation;
        }

        /// <summary>
        /// 初始化旧的中心和旋转数组
        /// </summary>
        /// <param name="length"></param>
        public void InitializeCaches(int length)
        {
            oldPositions = new Vector2[length];
            oldRotations = new float[length];
            for (int i = 0; i < length; i++)
            {
                oldPositions[i] = Position;
                oldRotations[i] = Rotation;
            }
        }

        /// <summary>
        /// 普普通通地更新记录点
        /// </summary>
        /// <param name="length"></param>
        public void UpdatePositionCache(int length)
        {
            if (oldPositions is null || length > oldPositions.Length)
                return;

            for (int i = 0; i < length - 1; i++)
                oldPositions[i] = oldPositions[i + 1];

            oldPositions[length - 1] = Position;
        }

        public void UpdateRotationCache(int length)
        {
            if (oldRotations is null || length > oldRotations.Length)
                return;

            for (int i = 0; i < length - 1; i++)
                oldRotations[i] = oldRotations[i + 1];

            oldRotations[length - 1] = Rotation;
        }

    }
}
