using Coralite.Core;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike
{
    public class MagConnectStaff:ModItem
    {
        public override string Texture => AssetDirectory.MachineItems+Name;

        public override void SetDefaults()
        {
            
        }

        public override bool CanUseItem(Player player)
        {
            return base.CanUseItem(player);
        }
    }
}
