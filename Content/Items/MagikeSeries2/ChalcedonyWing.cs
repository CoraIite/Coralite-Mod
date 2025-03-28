using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    [AutoloadEquip(EquipType.Wings)]
    public class ChalcedonyWing():BaseAccessory(ItemRarityID.LightRed,Item.sellPrice(0,2))
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item+Name;

        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(130, 6.75f, 1.2f);
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.6f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 0.6f;
            maxAscentMultiplier = 1.6f;
            constantAscend = 0.1f;
        }
    }
}
