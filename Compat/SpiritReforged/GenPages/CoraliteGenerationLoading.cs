using SpiritReforged.Common.WorldGeneration.GenConfiguration;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Coralite.Compat.SpiritReforged.GenPages
{
    public class CoraliteGenerationLoading
    {
        internal static void LoadBindings()
        {
            GenConfigLoader.LoadingMods.Add(Coralite.Instance);

            GenConfigLoader.CrossmodConfigurables.Add($"{nameof(Coralite)}/CrystallineSkyIsland", LoadConfigsFrom<CyrstallineSkyIslandConfiguration>);
        }

        private static List<(MemberInfo, ConfigInfo)> LoadConfigsFrom<T>()
        {
            Type type = typeof(T);
            List<(MemberInfo, ConfigInfo)> configs = [];

            foreach (FieldInfo field in type.GetFields())
            {
                if (field.GetCustomAttribute<BindToGenConfigAttribute>() is BindToGenConfigAttribute gen)
                {
                    GenConfigParameters parameters = new(gen.Minimum, gen.Maximum, gen.Step);
                    configs.Add((field, new ConfigInfo(parameters, gen.ReverseMinMax, gen.Slider, gen.Denominator, gen.PriorityConfig)));
                }
            }

            foreach (PropertyInfo field in type.GetProperties())
            {
                if (field.GetCustomAttribute<BindToGenConfigAttribute>() is BindToGenConfigAttribute gen)
                {
                    GenConfigParameters parameters = new(gen.Minimum, gen.Maximum, gen.Step);
                    configs.Add((field, new ConfigInfo(parameters, gen.ReverseMinMax, gen.Slider, gen.Denominator, gen.PriorityConfig)));
                }
            }

            return configs;
        }
    }
}
