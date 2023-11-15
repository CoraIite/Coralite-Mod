using Coralite.Compat.BossCheckList;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader;

namespace Coralite
{
    public partial class Coralite : Mod
    {
        public const int YujianHuluContainsMax = 10;
        public const int MaxParticleCount = 1201;

        private List<IOrderedLoadable> loadCache;

        public NoSmoother NoSmootherInstance;
        public HeavySmoother HeavySmootherInstance;
        public SqrtSmoother SqrtSmoother;
        public X2Smoother X2Smoother;
        public SinSmoother SinSmoother;
        public BezierEaseSmoother BezierEaseSmoother;
        /// <summary>
        /// 从0快速接近1，之后快速返回0<br></br>
        /// 在0.5的时候到达1
        /// </summary>
        public ReverseX2Smoother ReverseX2Smoother;

        //单例模式！
        public static Coralite Instance { get; private set; }

        public Coralite()
        {
            Instance = this;

            NoSmootherInstance = new NoSmoother();
            HeavySmootherInstance = new HeavySmoother();
            SqrtSmoother = new SqrtSmoother();
            X2Smoother = new X2Smoother();
            SinSmoother = new SinSmoother();
            BezierEaseSmoother = new BezierEaseSmoother();
            ReverseX2Smoother = new ReverseX2Smoother();
            InitColor();
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

            /////////////////////////////////其他乱七八糟的加载内容////////////////////////////////////////////

            LoadCurrency();
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