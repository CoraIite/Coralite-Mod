﻿//global usings
global using InnoVault;
global using Microsoft.Xna.Framework;
global using Terraria.ModLoader;
using Coralite.Compat.BossCheckList;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.BossSystems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;

namespace Coralite
{
    public class Coralite : Mod
    {
        public const int YujianHuluContainsMax = 3;

        private List<IOrderedLoadable> loadCache;
        /// <summary>
        /// 所有继承了<see cref="ICLLoader"/>接口的类的实例
        /// </summary>
        internal static List<ICLLoader> Loaders { get; private set; } = new List<ICLLoader>();

        public NoSmoother NoSmootherInstance;
        public HeavySmoother HeavySmootherInstance;
        public SqrtSmoother SqrtSmoother;
        public X2Smoother X2Smoother;
        public X3Smoother X3Smoother;
        public SinSmoother SinSmoother;
        public BezierEaseSmoother BezierEaseSmoother;
        /// <summary>
        /// 从0快速接近1，之后快速返回0<br></br>
        /// 在0.5的时候到达1
        /// </summary>
        public ReverseX2Smoother ReverseX2Smoother;

        public static Color RedJadeRed => new(221, 50, 50);
        /// <summary> ffbeec </summary>
        public static Color MagicCrystalPink => new(255, 190, 236);
        public static Color GlistentGreen => new(127, 218, 153);
        public static Color CrimsonRed => new(231, 48, 54);
        public static Color CorruptionPurple => new(107, 66, 208);
        public static Color IcicleCyan => new(43, 255, 198);
        public static Color ShadowPurple => new(240, 168, 255);
        public static Color CrystallineMagikePurple => new(140, 130, 252);
        public static Color HallowYellow => new(253, 236, 144);
        public static Color ThunderveinYellow => new(255, 202, 101);
        public static Color SoulCyan => new(122, 174, 188);
        public static Color FeatherLime => new(122, 161, 82);
        public static Color SplendorMagicoreLightBlue => new(190, 225, 235);

        private static Coralite _instance;

        //单例模式！
        public static Coralite Instance
        {
            get
            {
                _instance ??= (Coralite)ModLoader.GetMod("Coralite");

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public override void Load()
        {
            Instance = this;
            NoSmootherInstance = new NoSmoother();
            HeavySmootherInstance = new HeavySmoother();
            SqrtSmoother = new SqrtSmoother();
            X2Smoother = new X2Smoother();
            X3Smoother = new X3Smoother();
            SinSmoother = new SinSmoother();
            BezierEaseSmoother = new BezierEaseSmoother();
            ReverseX2Smoother = new ReverseX2Smoother();

            loadCache = [];

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
                //SetLoadingText("Loading " + loadCache[k].GetType().Name);
            }

            Loaders = VaultUtils.GetSubInterface<ICLLoader>();
            foreach (var load in Loaders)
            {
                load.LoadData();
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

            foreach (var load in Loaders)
            {
                load.UnLoadData();
            }

            Loaders.Clear();

            Instance = null;
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

            foreach (var load in Loaders)
            {
                load.SetupData();
            }

            if (!Main.dedServ)
            {
                foreach (var load in Loaders)
                {
                    load.LoadAsset();
                }
            }
        }

        public override object Call(params object[] args)
        {
            int argsLength = args.Length;
            //Array.Resize(ref args, 15);

            try
            {
                string methodName = args[0] as string;

                switch (methodName)
                {
                    default: break;
                    case "GetBossDowned"://获取击败BOSS的bool值
                    case "BossDowned":
                        if (argsLength < 2)
                            return new ArgumentNullException("ERROR: Must specify a boss or event name as a string.");
                        if (args[1] is not string)
                            return new ArgumentException("ERROR: The argument to \"Downed\" must be a string.");
                        return GetBossDowned(args[1].ToString());
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Call Error: {e.StackTrace} {e.Message}");
            }

            return new ArgumentException("ERROR: Invalid method name.");
        }

        /// <summary>
        /// 获取击败BOSS的bool
        /// </summary>
        /// <param name="bossName"></param>
        /// <returns></returns>
        public static bool GetBossDowned(string bossName)
        {
            return bossName.ToLower() switch
            {
                "赤玉灵" or
                "Rediancie"
                    => DownedBossSystem.downedRediancie,
                "冰龙宝宝" or
                "BabyIceDragon" or
                "Baby Ice Dragon"
                    => DownedBossSystem.downedBabyIceDragon,
                // "影子球" or "ShadowBalls" => DownedBossSystem.xxxx,
                "荒雷龙" or
                "ThunderveinDragon" or
                "Thundervein Dragon"
                    => DownedBossSystem.downedThunderveinDragon,
                "史莱姆皇帝" or
                "至高帝史莱姆王" or
                "至高帝·史莱姆王" or
                "至高帝" or
                "SlimeEmperor" or
                "Slime Emperor"
                    => DownedBossSystem.downedSlimeEmperor,
                "赤血玉灵" or
                "血咒精赤玉灵" or
                "血咒精·赤玉灵" or
                "血咒精" or
                "Bloodiancie"
                    => DownedBossSystem.downedBloodiancie,
                "梦魇之花" or
                "梦界主世纪之花" or
                "梦界主·世纪之花" or
                "梦界主" or
                "NightmarePlantera" or
                "Nightmare Plantera"
                    => DownedBossSystem.downedNightmarePlantera,
                _ => false,
            };
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI) => CLNetWork.NetWorkHander(reader, whoAmI);
    }
}