using Coralite.Core.Loaders;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Core.Systems.DotSystem;

/// <summary>
/// 为dot武器在物品描述旁绘制额外的dot说明
/// </summary>
public interface IDotWeapon
{
    const float TipSpacing = 12f;

    /// <summary>
    /// 按显示顺序提供dot类型和各自的显示伤害倍率，没有额外说明时返回空数组
    /// </summary>
    DotTipInfo[] GetDotTips();

    /// <summary>
    /// 获取全部dot说明的总尺寸，多个dot的说明纵向排列
    /// </summary>
    Vector2 GetDotsSPTextSize(Item item)
    {
        Vector2 totalSize = Vector2.Zero;
        bool hasDot = false;
        foreach (DotTipInfo tip in GetDotTips())
        {
            Dot dot = DotLoader.GetDot(tip.DotType);
            if (dot == null)
                continue;

            Vector2 size = dot.GetDotTipTotalSize(item, tip.DamageMultiplier);
            if (hasDot)
                totalSize.Y += TipSpacing;

            totalSize.X = Math.Max(totalSize.X, size.X);
            totalSize.Y += size.Y;
            hasDot = true;
        }

        return totalSize;
    }

    /// <summary>
    /// 在物品描述旁依次绘制全部dot说明
    /// </summary>
    /// <param name="spriteBatch">当前用于物品描述绘制的SpriteBatch</param>
    /// <param name="topLeft">由SpecialMouseTextDraw定位的内容左上角，使用UI坐标</param>
    /// <param name="item">用于计算显示伤害的武器</param>
    void DrawDotsSPText(SpriteBatch spriteBatch, Vector2 topLeft, Item item)
    {
        bool hasDot = false;
        foreach (DotTipInfo tip in GetDotTips())
        {
            Dot dot = DotLoader.GetDot(tip.DotType);
            if (dot == null)
                continue;

            if (hasDot)
                topLeft.Y += TipSpacing;

            Vector2 size = dot.GetDotTipTotalSize(item, tip.DamageMultiplier);
            dot.DrawDotExtraDescription(topLeft, item, tip.DamageMultiplier);
            topLeft.Y += size.Y;
            hasDot = true;
        }
    }
}
