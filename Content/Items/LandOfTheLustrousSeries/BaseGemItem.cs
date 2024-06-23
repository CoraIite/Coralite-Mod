using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class BaseGemItem(int value, int rare, string texturePath, bool pathHasName = false) : BaseMaterial(Item.CommonMaxStack, value, rare, texturePath, pathHasName)
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = CoraliteSoundID.Ding_Item4;
        }
    }
}
