using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;

namespace Coralite.Content.Items.Nightmare
{
    public class ImpactCapsule() : BaseAccessory(ModContent.RarityType<NightmareRarity>(), Item.sellPrice(1))
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += 0.0777f;
        }
    }
}
