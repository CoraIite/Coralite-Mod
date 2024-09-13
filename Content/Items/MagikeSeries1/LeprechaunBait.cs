using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class LeprechaunBait:ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item+Name;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.HoldUp;

            Item.noUseGraphic = true;
            Item.channel = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }

    public class LeprechaunBaitHeldProj
    {

    }
}
