using Coralite.Content.Items.FlyingShields;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class NPCDrop : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //蝙蝠
            AddRemodelRecipe(ItemID.BatBanner, ItemType<BatfangShield>(), 100, mainStack: 2);
        }
    }
}
