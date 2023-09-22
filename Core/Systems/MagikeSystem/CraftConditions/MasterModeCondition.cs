using System;
using Terraria;
using Terraria.Localization;

namespace Coralite.Core.Systems.MagikeSystem.CraftConditions
{
    public class MasterModeCondition : IMagikeCraftCondition
    {
        private static readonly Lazy<MasterModeCondition> singleton = new Lazy<MasterModeCondition>(() => new MasterModeCondition());
        public static MasterModeCondition Instance { get => singleton.Value; }

        public string Description => Language.GetOrRegister($"Mods.Coralite.MagikeSystem.Conditions.MasterModeCondition", () => "大师模式以上可进行魔能合成").Value;

        public bool CanCraft(Item item) => Main.masterMode;
    }
}
