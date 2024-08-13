using Coralite.Content.WorldGeneration;
using Terraria;

namespace Coralite.Content.CoraliteConditions
{
    public class CoraliteConditions
    {
        public static readonly Condition CoralCatCondition = new("Conditions.CoralCat", () => CoraliteWorld.coralCatWorld);
    }
}
