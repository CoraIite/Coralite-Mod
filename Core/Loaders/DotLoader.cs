using Coralite.Core.Systems.DotSystem;
using System.Collections.Generic;

namespace Coralite.Core.Loaders;

public class DotLoader
{
    internal static IList<Dot> dots;
    internal static int DotCount { get; private set; } = 0;

    internal static IList<DotTag> dotTags;
    internal static int DotTagCount { get; private set; } = 0;

    /// <summary>
    /// 根据类型获取附着
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Dot GetDot(int type)
             => type >= 0 && type < DotCount ? dots[type] : null;

    /// <summary>
    /// 根据类型获取附着标签
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static DotTag GetDotTag(int type)
             => type >= 0 && type < DotTagCount ? dotTags[type] : null;

    /// <summary>
    /// 设置ID
    /// </summary>
    /// <returns></returns>
    public static int ReserveDotID() => DotCount++;

    /// <summary>
    /// 设置ID
    /// </summary>
    /// <returns></returns>
    public static int ReserveDotTagID() => DotTagCount++;

    internal static void Unload()
    {
        if (dots != null)
        {
            foreach (var item in dots)
                item.Unload();

            dots.Clear();
        }
        dots = null;
        DotCount = 0;

        if (dotTags != null)
        {
            foreach (var item in dotTags)
                item.Unload();

            dotTags.Clear();
        }
        dotTags = null;
        DotTagCount = 0;
    }
}
