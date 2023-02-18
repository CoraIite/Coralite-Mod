using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Coralite.Core
{
    public abstract class ProjState
    {
        // AI函数接受一个SMProjectile类型的mod弹幕对象
        public abstract void AI(ProjStateMachine proj);
    }

    /// <summary>
    /// 基于状态机的ModProjectile类，一定要先在Initialize里注册弹幕的状态才能使用哦
    /// </summary>
    public abstract class ProjStateMachine : ModProjectile
    {
        public ProjState currentState => projStates[State - 1];
        private List<ProjState> projStates = new List<ProjState>();
        private Dictionary<string, int> stateDict = new Dictionary<string, int>();

        private int State
        {
            get { return (int)Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }
        public int Timer
        {
            get { return (int)Projectile.ai[1]; }
            set { Projectile.ai[1] = value; }
        }

        /// <summary>
        /// 把当前状态变为指定的弹幕状态实例
        /// </summary>
        /// <typeparam name="T">注册过的<see cref="ProjState"/>类名</typeparam>
        public void SetState<T>() where T : ProjState
        {
            var name = typeof(T).FullName;
            if (!stateDict.ContainsKey(name)) throw new ArgumentException("这个状态并不存在");
            State = stateDict[name];
        }

        /// <summary>
        /// 注册状态
        /// </summary>
        /// <typeparam name="T">需要注册的<see cref="ProjState"/>类</typeparam>
        /// <param name="state">需要注册的<see cref="ProjState"/>类的实例</param>
        protected void RegisterState<T>(T state) where T : ProjState
        {
            var name = typeof(T).FullName;
            if (stateDict.ContainsKey(name)) throw new ArgumentException("这个状态已经注册过了");
            projStates.Add(state);
            stateDict.Add(name, projStates.Count);
        }

        /// <summary>
        /// 初始化函数，用于注册弹幕状态
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// 我把AI函数封住了，这样在子类无法重写AI函数，只能用before和after函数
        /// </summary>
        public sealed override void AI()
        {
            if (State == 0)
            {
                Initialize();
                State = 1;
            }

            AIBefore();
            currentState.AI(this);
            AIAfter();
        }

        /// <summary>
        /// 在状态机执行之后要执行的代码
        /// </summary>
        public virtual void AIAfter() { }

        /// <summary>
        /// 在状态机执行之前要执行的代码
        /// </summary>
        public virtual void AIBefore() { }
    }
}
