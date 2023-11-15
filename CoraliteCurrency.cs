using Coralite.Content.Items.Magike;
using Coralite.Content.NPCs.Town;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace Coralite
{
    public partial class Coralite
    {
        public static int MagicCrystalCurrencyID;

        public void LoadCurrency()
        {
            MagicCrystalCurrencyID = CustomCurrencyManager.RegisterCurrency(new MagicCrystalCurrency(ModContent.ItemType<MagicCrystal>(), 9999));
        }
    }
}
