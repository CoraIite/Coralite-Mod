using Coralite.Core.Attributes;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;

namespace Coralite.Core.Loaders
{
    public class TextureLoader : IReflactionLoader
    {
        public int Priority => 1;

        public IReflactionLoader.LoadSide Side => IReflactionLoader.LoadSide.Cilent;

        public void Load(Mod Mod, Type type)
        {
            //if (type.IsAbstract)
            //    return;

            AutoLoadTextureAttribute autoLoadTextureAttribute = type.GetCustomAttribute<AutoLoadTextureAttribute>();

            if (autoLoadTextureAttribute == null)
                return;

            //获取类内部所有的静态属性
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static);

            if (properties.Length < 1)
                return;

            string texPath = autoLoadTextureAttribute.Path;

            foreach (var property in properties)
            {
                if (property.PropertyType != typeof(ATex))
                    continue;

                //获取贴图路径与贴图名，然后赋值
                string p = texPath;
                string texName = property.Name;
                AutoLoadTextureAttribute propAtt = property.GetCustomAttribute<AutoLoadTextureAttribute>();
                if (propAtt != null)
                {
                    if (!string.IsNullOrEmpty(propAtt.Path))
                        p = propAtt.Path;

                    if (!string.IsNullOrEmpty(propAtt.Name))
                        texName = propAtt.Name;
                }

                property.SetValue(null, ModContent.Request<Texture2D>(p + texName));
            }
        }

        public void Unload(Mod Mod, Type type)
        {
            //if (type.IsAbstract)
            //    return;

            AutoLoadTextureAttribute autoLoadTextureAttribute = type.GetCustomAttribute<AutoLoadTextureAttribute>();

            if (autoLoadTextureAttribute == null)
                return;

            //获取类内部所有的静态属性
            PropertyInfo[] properties = type.GetProperties(BindingFlags.GetProperty | BindingFlags.Static);

            if (properties.Length < 1)
                return;

            foreach (var property in properties)
            {
                if (property.PropertyType != typeof(ATex))
                    return;

                property.SetValue(null, null);
            }
        }
    }
}
