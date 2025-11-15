using Coralite.Core.Loaders;
using System.Collections.Generic;

namespace Coralite.Core.Systems.WorldValueSystem
{
    /// <summary>
    /// 专门用于自动化同步世界设置内容
    /// </summary>
    public abstract class WorldFlag : ModType
    {
        public int Type { get; internal set; }

        protected sealed override void Register()
        {
            ModTypeLookup<WorldFlag>.Register(this);

            WorldValueLoader.flags ??= new List<WorldFlag>();
            WorldValueLoader.flags.Add(this);

            Type = WorldValueLoader.ReserveFlagsID();
        }

        public sealed override void SetupContent()
        {
            SetStaticDefaults();
        }

        /// <summary>
        /// 获取值
        /// </summary>
        public bool Value
            => WorldValueSystem.WorldFlags[Type];

        /// <summary>
        /// 设置值，一般都是服务端调用这个东西，当然玩家端调用也行
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetAndSync(bool value)
        {
            WorldValueSystem.WorldFlags[Type] = value;

            if (VaultUtils.isServer)
                WorldValueSystem.SendWorldValue(-1, false);
            else if (VaultUtils.isClient)
                WorldValueSystem.SendWorldValue(-1, true);
        }

        /// <summary>
        /// 设置值，不带同步，一般用于保存读取
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Set(bool value)
        {
            WorldValueSystem.WorldFlags[Type] = value;
        }

        /// <summary>
        /// 进入世界时调用
        /// </summary>
        public virtual void OnEnterWorld()
        {

        }
    }
}
