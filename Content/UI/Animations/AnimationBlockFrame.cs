namespace Coralite.Content.UI.Animations;

public enum AnimationBlockFrame
{
    /// <summary> 只有左边是外露，其他3面都连接 </summary>
    LeftSide,
    /// <summary> 只有右边是外露，其他3面都连接 </summary>
    RightSide,
    /// <summary> 只有上边是外露，其他3面都连接 </summary>
    TopSide,
    /// <summary> 只有下边是外露，其他3面都连接 </summary>
    DownSide,

    /// <summary> 向左突出，只有右边连接 </summary>
    LeftTip,
    /// <summary> 向右突出，只有右边连接 </summary>
    RightTip,
    /// <summary> 向上突出，只有右边连接 </summary>
    TopTip,
    /// <summary> 向下突出，只有右边连接 </summary>
    DownTip,

    /// <summary> 不连接的单个方块 </summary>
    Single,

    /// <summary> 左上角外露，右下连接 </summary>
    TopLeftCorner,
    /// <summary> 右上角外露，左下连接 </summary>
    TopRightCorner,
    /// <summary> 左下角外露，右上连接 </summary>
    DownLeftCorner,
    /// <summary> 右下角外露，左上连接 </summary>
    DownRightCorner,

    /// <summary> 左右外露，上下连接 </summary>
    VerticalLine,
    /// <summary> 上下外露，左右连接 </summary>
    HorizontalLine,

    /// <summary> 四个方向连接 </summary>
    Inside,
}
