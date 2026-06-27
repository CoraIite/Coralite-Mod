using InnoVault.DataModules;

namespace Coralite.Core.Network.DataModules
{
    /// <summary>
    /// 噩梦能量 ModPlayer 同步/存档模块（武器域 worker 负责数值逻辑，此处仅提供 DataModule 接入点）。
    /// </summary>
    public sealed class NightmareEnergyModule : DataModule
    {
        public short Energy;
        public short EnergyMax;
        public int HitCount;
    }
}
