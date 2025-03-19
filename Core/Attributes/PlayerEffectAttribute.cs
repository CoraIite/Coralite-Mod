using System;

namespace Coralite.Core.Attributes
{
    /// <summary>
    /// 给类加上这个标签就表示这个类的名称是一个玩家效果<br></br>
    /// 可以使用<see cref="EffectName"/>修改字符串
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PlayerEffectAttribute:Attribute
    {
        public string EffectName;

        public string[] ExtraEffectNames;
    }
}
