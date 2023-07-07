using Coralite.Compat.BossCheckList;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader;

namespace Coralite
{
    public class Coralite : Mod
    {
        public const int YujianHuluContainsMax = 10;
        public const int MaxParticleCount = 801;
        public readonly Color RedJadeRed;
        public readonly Color IcicleCyan;
        public readonly Color MagicCrystalPink;
        public readonly Color CrystallineMagikePurple;

        private List<IOrderedLoadable> loadCache;

        public NoSmoother NoSmootherInstance;
        public HeavySmoother  HeavySmootherInstance;
        public SqrtSmoother SqrtSmoother;
        public X2Smoother X2Smoother;

        //单例模式！
        public static Coralite Instance { get; private set; }

        public Coralite()
        {
            Instance = this;

            NoSmootherInstance=new NoSmoother();
            HeavySmootherInstance = new HeavySmoother();
            SqrtSmoother = new SqrtSmoother();
            X2Smoother = new X2Smoother();

            RedJadeRed = new Color(221, 50, 50);
            IcicleCyan = new Color(43, 255, 198);
            MagicCrystalPink = new Color(255, 190, 236);
            CrystallineMagikePurple = new Color(140, 130, 252);
        }

        public override void Load()
        {
            loadCache = new List<IOrderedLoadable>();

            foreach (Type type in Code.GetTypes())
            {
                if (!type.IsAbstract && type.GetInterfaces().Contains(typeof(IOrderedLoadable)))
                {
                    var instance = Activator.CreateInstance(type);//创建实例
                    loadCache.Add(instance as IOrderedLoadable);//添加到列表里
                }

                loadCache.Sort((n, t) => n.Priority.CompareTo(t.Priority));//按照优先度排序
            }

            for (int k = 0; k < loadCache.Count; k++)
            {
                loadCache[k].Load();
                //SetLoadingText("Loading " + loadCache[k].GetType().Name);     //使用反射来显示加载的内容，但我暂时不需要（
            }
        }

        public override void Unload()
        {
            if (loadCache is not null)
            {
                foreach (var loadable in loadCache)
                {
                    loadable.Unload();
                }

                loadCache = null;
            }
        }

        /// <summary>
        /// 设置加载的时候在屏幕上显示的内容
        /// </summary>
        public static void SetLoadingText(string text)
        {
            var Interface_loadMods = typeof(Mod).Assembly.GetType("Terraria.ModLoader.UI.Interface")!.GetField("loadMods", BindingFlags.NonPublic | BindingFlags.Static)!;
            var UIProgress_set_SubProgressText = typeof(Mod).Assembly.GetType("Terraria.ModLoader.UI.UIProgress")!.GetProperty("SubProgressText", BindingFlags.Public | BindingFlags.Instance)!.GetSetMethod()!;

            UIProgress_set_SubProgressText.Invoke(Interface_loadMods.GetValue(null), new object[] { text });
        }

        public override void PostSetupContent()
        {
            BossCheckListCalls.CallBossCheckList();
        }

    }
}