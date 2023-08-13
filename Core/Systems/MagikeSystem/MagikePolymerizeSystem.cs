using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem : ModSystem, ILocalizedModType
    {
        internal static Dictionary<int, List<PolymerizeRecipe>> PolymerizeRecipes = new Dictionary<int, List<PolymerizeRecipe>>();

        private void RegisterPolymerize()
        {
            Mod Mod = Coralite.Instance;


        }
    }

    public class PolymerizeRecipe
    {
        public int selfType;
        public int selfRequiredNumber;

        public Item ResultItem;

        private List<int> subItemTypes;
        public List<int> subItemStacks;

        public static PolymerizeRecipe CreateRecipe(int ResultItemType,int resultItemStack=1)
        {
            return new PolymerizeRecipe() { ResultItem = new Item(ResultItemType, resultItemStack) };
        }


    }
}
