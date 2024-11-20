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
                Type t = typeof(ReadFragmant);

                var infos = t.GetProperties(BindingFlags.Public | BindingFlags.Static);

                foreach (var info in infos)
                    info.SetValue(null, Get(NoteReadfragment + info.Name));
            }

            internal static void Unload()
            {
                Type t = typeof(ReadFragmant);

                var infos = t.GetProperties(BindingFlags.Public | BindingFlags.Static);

                foreach (var info in infos)
                    info.SetValue(null, null);
            }
        }

        public static class MagikeChapter1
        {
            public static ATex KnowledgeCheckButton { get; private set; }

            public static ATex WhatIsMagike { get; private set; }

            public static ATex PlaceFirstLens { get; private set; }

            internal static void Load()
            {
                Type t = typeof(MagikeChapter1);

                var infos = t.GetProperties(BindingFlags.Public | BindingFlags.Static);

                foreach (var info in infos)
                    info.SetValue(null, Get(NoteMagikeS1 + info.Name));
            }

            internal static void Unload()
            {
                Type t = typeof(MagikeChapter1);

                var infos = t.GetProperties(BindingFlags.Public | BindingFlags.Static);

                foreach (var info in infos)
                    info.SetValue(null, null);
            }
        }
    }
}
