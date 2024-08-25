using Coralite.Content.Items.FlyingShields;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class NPCDrop : IMagikeRemodelable
    {
        public void AddMagikeRemodelRecipe()
        {
            //蝙蝠
            AddRemodelRecipe( ItemID.BatBanner, ItemType<BatfangShield>(),100, mainStack: 2);
        }
    }
}
