using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;

namespace Coralite.Content.Items.Nightmare
{
    [AutoloadEquip(EquipType.Head)]
    public class NightmarePlanteraMask() : BaseBossMask(ModContent.RarityType<NightmareRarity>(), Item.sellPrice(0, 0, 75))
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;
    }
}
