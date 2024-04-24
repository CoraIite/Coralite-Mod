using System;
using Terraria.Localization;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public sealed record FairySpawnCondition(LocalizedText Descripetion, Func<FairyAttempt, bool> Predicate)
    {
        public static FairySpawnCondition ZoneForest = new(FairySystem.ZoneForestDescription, attempt => attempt.Player.ZoneForest);

        //public readonly LocalizedText Descripetion = Descripetion;
        //public readonly Func<FairyAttempt, bool> Condition = Condition;
    }
}
