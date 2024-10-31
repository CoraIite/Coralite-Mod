using System;
using System.Reflection;
using static Coralite.Core.AssetDirectory;
using ATex = ReLogic.Content.Asset<Microsoft.Xna.Framework.Graphics.Texture2D>;

namespace Coralite.Core
{
    public partial class CoraliteAssets
    {
        public static class ReadFragmant
        {
            public static ATex BookName { get; private set; }

            internal static void Load()
            {
                Type t = typeof(Trail);

                var infos = t.GetProperties(BindingFlags.Public | BindingFlags.Static);

                foreach (var info in infos)
                    info.SetValue(null, Get(NoteReadfragment + info.Name));
            }

            internal static void Unload()
            {
                Type t = typeof(Trail);

                var infos = t.GetProperties(BindingFlags.Public | BindingFlags.Static);

                foreach (var info in infos)
                    info.SetValue(null, null);
            }

        }
    }
}
