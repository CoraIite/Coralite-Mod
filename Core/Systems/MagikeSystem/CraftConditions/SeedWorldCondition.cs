using System;
using Terraria;
using Terraria.Localization;

namespace Coralite.Core.Systems.MagikeSystem.CraftConditions
{
    public class DontStarveWorldCondition : IMagikeCraftCondition
    {
        private static readonly Lazy<DontStarveWorldCondition> singleton = new(() => new DontStarveWorldCondition());
        public static DontStarveWorldCondition Instance { get => singleton.Value; }

        public string Description => Language.GetOrRegister($"Mods.Coralite.MagikeSystem.Conditions.DontStarveWorldCondition", () => "在饥荒世界中可进行魔能合成").Value;

        public bool CanCraft(Item item) => Main.dontStarveWorld;
    }

    public class NotDontDigUpCondition : IMagikeCraftCondition
    {
        private static readonly Lazy<NotDontDigUpCondition> singleton = new(() => new NotDontDigUpCondition());
        public static NotDontDigUpCondition Instance { get => singleton.Value; }

        public string Description => Language.GetOrRegister($"Mods.Coralite.MagikeSystem.Conditions.NotDontDigUpCondition", () => "非颠倒世界中可进行魔能合成").Value;

        public bool CanCraft(Item item) => !Main.remixWorld && !Main.zenithWorld;
    }

    public class DontDigUpWorldCondition : IMagikeCraftCondition
    {
        private static readonly Lazy<DontDigUpWorldCondition> singleton = new(() => new DontDigUpWorldCondition());
        public static DontDigUpWorldCondition Instance { get => singleton.Value; }

        public string Description => Language.GetOrRegister($"Mods.Coralite.MagikeSystem.Conditions.DontDigUpWorldCondition", () => "在颠倒世界中可进行魔能合成").Value;

        public bool CanCraft(Item item) => Main.remixWorld || Main.zenithWorld;
    }
}
