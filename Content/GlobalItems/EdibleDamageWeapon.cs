using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.GlobalItems
{
    public partial class CoraliteGlobalItem
    {
        public bool EdibleDamage;

        public static LocalizedText Edible;

        public void RegisterEdibleDamageWeapon(int type)
        {
            if (type is ItemID.HamBat or ItemID.WaffleIron or ItemID.FruitcakeChakram or ItemID.Bananarang
                or ItemID.CandyCorn or ItemID.MolotovCocktail or ItemID.CandyCaneSword)
                EdibleDamage = true;
        }

        public static void SetEdibleDamage(Item i)
        {
            i.GetGlobalItem<CoraliteGlobalItem>().EdibleDamage = true;
        }

        public static bool IsEdibleDamage(Item i)
        {
            return i.GetGlobalItem<CoraliteGlobalItem>().EdibleDamage;
        }
    }
}
