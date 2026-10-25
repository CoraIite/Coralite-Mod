namespace Coralite.Core.Systems.DotSystem;

/// <summary>
/// 武器显示的一种dot及其伤害倍率，倍率应用于本地玩家计算后的武器伤害
/// </summary>
/// <param name="DotType">注册的dot类型ID</param>
/// <param name="DamageMultiplier">显示伤害倍率，默认为1</param>
public readonly record struct DotTipInfo(int DotType, float DamageMultiplier = 1f);
