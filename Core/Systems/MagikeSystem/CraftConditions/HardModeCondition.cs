using System;
using Terraria;
using Terraria.Localization;

namespace Coralite.Core.Systems.MagikeSystem.CraftConditions
{
    public class HardModeCondition : IMagikeCraftCondition
    {
        private static readonly Lazy<HardModeCondition> singleton = new(() => new HardModeCondition());
        public static HardModeCondition Instance { get => singleton.Value; }

        public string Description => Language.GetOrRegister($"Mods.Coralite.MagikeSystem.Conditions.HardModeCondition", () => "困难模式中可进行魔能合成").Value;

        public bool CanCraft(Item item) => Main.hardMode;
    }
}
