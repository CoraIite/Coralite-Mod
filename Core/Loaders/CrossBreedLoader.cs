using Coralite.Core.Systems.BotanicalSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Core.Loaders
{
    class CrossBreedLoader : ModSystem
    {
        public static List<CrossBreedData> CrossBreedDatas;

        public override void OnModLoad()
        {
            if (Main.dedServ)
                return;

            Mod Mod = Coralite.Instance;

            CrossBreedDatas = new List<CrossBreedData>();

            foreach (Type t in Mod.Code.GetTypes())
            {
                if (t.GetInterfaces().Contains(typeof(ICrossBreedable)))
                {
                    ICrossBreedable items = Activator.CreateInstance(t) as ICrossBreedable;
                    items.AddCrossBreedRecipe();
                }
            }

        }

        public override void OnModUnload()
        {
            if (CrossBreedDatas is not null)
                CrossBreedDatas = null;
        }

        /// <summary>
        /// 注册Data
        /// </summary>
        /// <param name="data"></param>
        public static void RegisterCrossBreed(CrossBreedData data)
        {
            CrossBreedDatas?.Add(data);
        }

        public static void RegisterCrossBreed(int fatherType, int motherType, int mutantType, int percentage)
        {
            CrossBreedDatas?.Add(new CrossBreedData(fatherType, motherType, mutantType, percentage));
        }

        /// <summary>
        /// 移除指定的Data
        /// </summary>
        /// <param name="fatherType"></param>
        /// <param name="motherType"></param>
        /// <param name="mutantType"></param>
        /// <param name="percentage"></param>
        public static void RemoveCrossBreed(int fatherType, int motherType, int mutantType, int percentage)
        {
            CrossBreedData data = new CrossBreedData(fatherType, motherType, mutantType, percentage);
            foreach (var item in CrossBreedDatas)
            {
                if (item == data)
                {
                    CrossBreedDatas.Remove(item);
                    return;
                }
            }

        }

        /// <summary>
        /// 查找突变，如果找到了那么就能突变，同时将 data传出去
        /// </summary>
        /// <param name="fatherType"></param>
        /// <param name="motherType"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static bool FindCrossBreedRecipe(int fatherType, int motherType, out CrossBreedData? Data)
        {
            if (CrossBreedDatas is null)
                throw new Exception("出大问题！");

            foreach (var data in CrossBreedDatas)
            {
                if (data.CanMutant(fatherType, motherType))
                {
                    Data = data;
                    return true;
                }

            }

            Data = null;
            return false;
        }
    }
}
