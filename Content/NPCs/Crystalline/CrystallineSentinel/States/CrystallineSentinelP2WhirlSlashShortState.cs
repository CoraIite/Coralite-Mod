using Coralite.Content.NPCs.Crystalline.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.NPCs.Crystalline.States
{
    /// <summary>
    /// 二阶段旋风斩·短版：刀刃半径对折（110 px）、蓄力段砍掉 70 帧、预警环寿命对折，
    /// 起手到落刃几乎没有前摇——只由螺旋冲刺收招时“玩家刚好停在 220~280 px 环带里”接出来，是这条连段的收尾。<br/>
    /// 旧 <c>P2WhirlSlash(true)</c>（CrystallineSentinel.cs:1560-1691 的 lightVer 分支）。
    /// </summary>
    [VaultState((int)CrystallineSentinelStateId.P2WhirlSlashShort, typeof(CrystallineSentinelContext))]
    internal sealed class CrystallineSentinelP2WhirlSlashShortState : CrystallineSentinelP2WhirlSlashState
    {
        public override CrystallineSentinelStateId StateIndex => CrystallineSentinelStateId.P2WhirlSlashShort;

        protected override bool LightVer => true;
    }
}
