using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Coralite.Core.Configs
{
    //[BackgroundColor(51, 179, 189)]
    public class MagikeUIConfig:ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Range(0, 1)]
        [DefaultValue(0.8f)]
        [Increment(0.01f)]
        [Slider]
        public float UIScale;
    }
}
