using System;

namespace Coralite.Core.Attributes
{
    /// <summary>
    /// 给类加上这个标签就表示这个类的名称是一个玩家效果<br></br>
    /// 可以使用<see cref="OverrideEffectName"/>修改字符串
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PlayerEffectAttribute : Attribute
    {
        /// <summary>
        /// 覆盖的效果名，写了这个就用这个，不写就用类名
        /// </summary>
        public string OverrideEffectName;

        /// <summary>
        /// 额外的效果名，写多少添加多少
        /// </summary>
        public string[] ExtraEffectNames;
    }
}
