using Coralite.Core.Attributes;
using ReLogic.Utilities;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace Coralite.Core.Loaders
{
    public class PlayerEffectLoader : IReflactionLoader
    {
        public int Priority => 3;

        public IReflactionLoader.LoadSide Side => IReflactionLoader.LoadSide.All;

        private static Dictionary<string, int> effects = new Dictionary<string, int>();

        /// <summary>
        /// 效果总量
        /// </summary>
        public static int EffectCount { get; private set; }
        public static FrozenDictionary<string, int> Effects { get; private set; }

        public void SetUp(Mod Mod, Type type)
        {
            PlayerEffectAttribute playerEffectAttribute = type.GetAttribute<PlayerEffectAttribute>();
            if (playerEffectAttribute != null)
            {
                string name = type.Name;
                if (!string.IsNullOrEmpty(playerEffectAttribute.EffectName))
                    name = playerEffectAttribute.EffectName;

                effects.Add(name, EffectCount);
                EffectCount++;

                if(playerEffectAttribute.ExtraEffectNames != null)
                    foreach (var name2 in playerEffectAttribute.ExtraEffectNames)
                    {
                        effects.Add(name2, EffectCount);
                        EffectCount++;
                    }
                }
        }
         
        public void PostSetUp(Mod Mod)
        {
            Effects = effects.ToFrozenDictionary();
            effects = null;
        }

        public void PreUnload(Mod Mod)
        {
            Effects = null;
        }
    }
}
