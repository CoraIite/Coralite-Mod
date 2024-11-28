using Coralite.Core;
using System.Linq;
using Terraria.ID;

namespace Coralite.Content.GlobalNPCs
{
    public class DigDigDigGNPC : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            foreach (var entry in shop.Entries)
            {
                if (entry.Item.type is ItemID.TeleportationPylonPurity or ItemID.TeleportationPylonSnow
                    or ItemID.TeleportationPylonDesert or ItemID.TeleportationPylonUnderground or ItemID.TeleportationPylonOcean
                    or ItemID.TeleportationPylonJungle or ItemID.TeleportationPylonHallow or ItemID.TeleportationPylonMushroom
                    &&!entry.Conditions.Contains(CoraliteConditions.NotInDigDigDig))
                {
                    entry.AddCondition(CoraliteConditions.NotInDigDigDig);//晶塔限定不在挖挖挖世界出售
                }
            }
        }
    }
}
