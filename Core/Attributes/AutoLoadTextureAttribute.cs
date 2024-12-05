using ReLogic.Content;
using System;

namespace Coralite.Core.Attributes
{
    /// <summary>
    /// 使用该标签后就可以自动加载类内部的 <see langword="static"/> <see cref="Asset{Texture2D}"/> 的贴图<br></br>
    /// 类需要输入 <see cref="TexturePath"/> 作为贴图路径<br></br>
    /// <br></br>
    /// 属性使用该标签能够修改所使用的贴图名称，不加默认使用属性名<br></br>
    /// 使用 <see cref="TextureName"/> 修改贴图名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class AutoLoadTextureAttribute : Attribute
    {
        public AutoLoadTextureAttribute(string texturePath)
        {
            TexturePath = texturePath;
        }

        public AutoLoadTextureAttribute(string textureName,int justNotRepeat)
        {
            TextureName = textureName;
        }

        public string TexturePath {  get; set; }
        public string TextureName { get; set; }
    }
}
