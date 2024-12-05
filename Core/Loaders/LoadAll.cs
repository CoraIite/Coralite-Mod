using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader.Core;

namespace Coralite.Core.Loaders
{
    public class LoadAll : ModSystem
    {
        public override void Load()
        {
            List<IReflactionLoader> loaders = FillLoaderList();

            bool isCilent = !Main.dedServ;

            for (int i = 0; i < loaders.Count; i++)
            {
                if (loaders[i].Side == IReflactionLoader.LoadSide.All)
                    loaders[i].PreLoad(Mod);
                else if (loaders[i].Side == IReflactionLoader.LoadSide.Cilent && isCilent)
                    loaders[i].PreLoad(Mod);
            }

            foreach (var mod in ModLoader.Mods)
                if (mod is ICoralite or Coralite)
                    foreach (Type type in AssemblyManager.GetLoadableTypes(Mod.Code))
                        for (int i = 0; i < loaders.Count; i++)
                        {
                            if (loaders[i].Side == IReflactionLoader.LoadSide.All)
                                loaders[i].Load(Mod, type);
                            else if (loaders[i].Side == IReflactionLoader.LoadSide.Cilent && isCilent)
                                loaders[i].Load(Mod, type);
                        }

            for (int i = 0; i < loaders.Count; i++)
            {
                if (loaders[i].Side == IReflactionLoader.LoadSide.All)
                    loaders[i].PostLoad(Mod);
                else if (loaders[i].Side == IReflactionLoader.LoadSide.Cilent && isCilent)
                    loaders[i].PostLoad(Mod);
            }
        }

        public override void PostSetupContent()
        {
            List<IReflactionLoader> loaders = FillLoaderList();

            bool isCilent = !Main.dedServ;

            for (int i = 0; i < loaders.Count; i++)
            {
                if (loaders[i].Side == IReflactionLoader.LoadSide.All)
                    loaders[i].PreSetUp(Mod);
                else if (loaders[i].Side == IReflactionLoader.LoadSide.Cilent && isCilent)
                    loaders[i].PreSetUp(Mod);
            }

            foreach (var mod in ModLoader.Mods)
                if (mod is ICoralite or Coralite)
                    foreach (Type type in AssemblyManager.GetLoadableTypes(Mod.Code))
                        for (int i = 0; i < loaders.Count; i++)
                        {
                            if (loaders[i].Side == IReflactionLoader.LoadSide.All)
                                loaders[i].SetUp(Mod, type);
                            else if (loaders[i].Side == IReflactionLoader.LoadSide.Cilent && isCilent)
                                loaders[i].SetUp(Mod, type);
                        }

            for (int i = 0; i < loaders.Count; i++)
            {
                if (loaders[i].Side == IReflactionLoader.LoadSide.All)
                    loaders[i].PostSetUp(Mod);
                else if (loaders[i].Side == IReflactionLoader.LoadSide.Cilent && isCilent)
                    loaders[i].PostSetUp(Mod);
            }
        }

        public override void Unload()
        {
            List<IReflactionLoader> loaders = FillLoaderList();

            bool isCilent = !Main.dedServ;

            for (int i = 0; i < loaders.Count; i++)
            {
                if (loaders[i].Side == IReflactionLoader.LoadSide.All)
                    loaders[i].PreUnload(Mod);
                else if (loaders[i].Side == IReflactionLoader.LoadSide.Cilent && isCilent)
                    loaders[i].PreUnload(Mod);
            }

            foreach (var mod in ModLoader.Mods)
                if (mod is ICoralite or Coralite)
                    foreach (Type type in AssemblyManager.GetLoadableTypes(Mod.Code))
                        for (int i = 0; i < loaders.Count; i++)
                        {
                            if (loaders[i].Side == IReflactionLoader.LoadSide.All)
                                loaders[i].Unload(Mod, type);
                            else if (loaders[i].Side == IReflactionLoader.LoadSide.Cilent && isCilent)
                                loaders[i].Unload(Mod, type);
                        }
        }

        public List<IReflactionLoader> FillLoaderList()
        {
            List<IReflactionLoader> loaders = new List<IReflactionLoader>();

            foreach (var mod in ModLoader.Mods)
                if (mod is ICoralite or Coralite)
                    foreach (Type type in AssemblyManager.GetLoadableTypes(Mod.Code))
                    {
                        if (!type.IsAbstract && type.GetInterfaces().Contains(typeof(IReflactionLoader)))
                        {
                            var instance = Activator.CreateInstance(type);//创建实例
                            loaders.Add(instance as IReflactionLoader);//添加到列表里
                        }

                        loaders.Sort((n, t) => n.Priority.CompareTo(t.Priority));//按照优先度排序
                    }

            return loaders;
        }
    }
}
