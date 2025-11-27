using System;

namespace Coralite.Core.Systems.MagikeSystem.Attributes
{
    /// <summary>
    /// 可升级的属性，会自动加载到json文件里
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UpgradeablePropAttribute() : Attribute
    {
    }
}
