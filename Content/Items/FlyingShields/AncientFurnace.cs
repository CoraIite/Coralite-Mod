using Coralite.Core;
using Terraria.ModLoader;

namespace Coralite.Content.Items.FlyingShields
{
    public class AncientFurnace:ModItem
    {
        public override string Texture => AssetDirectory.FlyingShieldItems+Name;


    }

    public class AncientFurnaceTile:ModTile
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;


    }

    public class AncientFurnaceProj
    {

    }
}
