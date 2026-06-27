using Coralite.Core.Loaders;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Core.Systems.WorldValueSystem
{
    /// <summary>
    /// 专门用于自动化同步世界设置内容
    /// </summary>
    public abstract class WorldFlag : ModType
    {
        public int Type { get; internal set; }

        /// <summary>
        /// 为 <see langword="true"/> 的时候表示需要在世界生成是重置变量为默认值<br></br>
        /// 默认 <see langword="true"/>
        /// </summary>
        public virtual bool NeedResetPostWoldGen { get => true; }

        /// <summary>
        /// 是否允许客户端通过 <see cref="CoraliteNetWorkEnum.WorldFlagChangeRequest"/> 请求修改此 flag。
        /// 进度类 flag 应维持默认 <see langword="false"/>，由服务端游戏事件权威写入。
        /// </summary>
        public virtual bool AcceptClientChangeRequest => false;

        /// <summary>
        /// 服务端处理客户端解锁请求时的额外校验（消耗物品、前置条件等）。
        /// 仅当 <see cref="AcceptClientChangeRequest"/> 为 <see langword="true"/> 且请求 value 为 <see langword="true"/> 时调用。
        /// </summary>
        public virtual bool TryAuthorizeClientUnlock(Player player) => true;

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
        /// 设置值并同步。服务端/单人直接写入并广播；客户端仅发送单 flag 请求包，不本地覆写整表。
        /// </summary>
        public void SetAndSync(bool value)
        {
            if (VaultUtils.isSinglePlayer)
            {
                WorldValueSystem.WorldFlags[Type] = value;
                return;
            }

            if (VaultUtils.isServer)
            {
                WorldValueSystem.WorldFlags[Type] = value;
                WorldValueSystem.SendWorldValue(-1);
                return;
            }

            if (VaultUtils.isClient)
                WorldValueSystem.RequestFlagChange(Type, value);
        }

        /// <summary>
        /// 设置值，不带同步，一般用于保存读取
        /// </summary>
        public void Set(bool value)
        {
            WorldValueSystem.WorldFlags[Type] = value;
        }

        /// <summary>
        /// 进入世界时调用
        /// </summary>
        public virtual void OnEnterWorld(Player player)
        {

        }
    }
}
