using System.Collections.Generic;
using Terraria;

namespace Coralite.Core.Systems.DotSystem;

public abstract class Dot : ModTexturedType
{
    private Queue<DotInfo> _dotInfos;

    /// <summary>
    /// 存储所有的伤害
    /// </summary>
    public Queue<DotInfo> DotInfos
    {
        get
        {
            _dotInfos ??= [];
            return _dotInfos;
        }
    }

    public virtual int CurrentLayer
    {
        get => DotInfos.Count;
    }

    /// <summary>
    /// 这个dot的伤害类型
    /// </summary>
    public virtual DamageClass DamageType => DamageClass.Generic;

    /// <summary>
    /// 这个附着的基础层数
    /// </summary>
    public virtual int MaxDotLayer => 1;

    protected override void Register()
    {
        
    }

    /// <summary>
    /// 更新dot
    /// </summary>
    /// <param name="npc"></param>
    public virtual void Update(NPC npc) { }

    /// <summary>
    /// 向一个NPC加入dot
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="npc"></param>
    /// <param name="damage"></param>
    /// <param name="player"></param>
    /// <param name="numAdd"></param>
    /// <returns></returns>
    public static bool AddDot<T>(NPC npc, int damage, int player,int crit, int numAdd = 1) where T:Dot
    {
        return false;
    }

    /// <summary>
    /// 将dot伤害施加给这个NPC
    /// </summary>
    /// <param name="npc"></param>
    public void ApplyDot(NPC npc)
    {
        if (DotInfos.TryDequeue(out DotInfo info))
        {
            if (Main.myPlayer == info.player)
                npc.SimpleStrikeNPC(GetDamage(info), 1, Main.rand.NextBool(info.critChance, 100), 0, DamageType, false, 0, true);
        }
    }

    /// <summary>
    /// 获取这个dot所造成的具体伤害，默认从传入的info获取伤害，也可以遍历<see cref="DotInfos"/>从所有dot层数中获取
    /// </summary>
    /// <param name="info"></param>
    public virtual int GetDamage(DotInfo info)
        => info.damage;

    #region 网络同步

    #endregion

    #region 绘制dot

    #endregion
}

public record DotInfo
{
    public readonly int player;
    public readonly int damage;
    /// <summary>
    /// 与给玩家增加暴击率相同，为整数的暴击几率
    /// </summary>
    public readonly int critChance;
}
