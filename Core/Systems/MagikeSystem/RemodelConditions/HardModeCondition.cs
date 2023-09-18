using System;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.RemodelConditions
{
    public class HardModeCondition : IMagikeCraftCondition
    {
        private static readonly Lazy<HardModeCondition> singleton = new Lazy<HardModeCondition>(() => new HardModeCondition());
        public static HardModeCondition Instance { get => singleton.Value; }

        public string Description => "困难模式中可重塑";

        public bool CanCraft(Item item) => Main.hardMode;
    }
}
