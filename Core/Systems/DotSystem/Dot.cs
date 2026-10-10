using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Core.Systems.DotSystem;

public abstract class Dot : ModTexturedType
{
    private List<int> _players;

    /// <summary>
    /// 存储所有的叠加了这个附着的玩家
    /// </summary>
    public List<int> Players
    {
        get
        {
            _players ??= [];
            return _players;
        }
    }

    private LinkedList<int> _damages;

    /// <summary>
    /// 存储所有的伤害
    /// </summary>
    public LinkedList<int> Damages
    {
        get
        {
            _damages ??= [];
            return _damages;
        }
    }

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
    public static bool AddDot<T>(NPC npc, int damage, int player, int numAdd = 1) where T:Dot
    {
        return false;
    }

    #region 网络同步

    #endregion

    #region 绘制dot

    #endregion
}
