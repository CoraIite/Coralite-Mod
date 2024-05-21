using Coralite.Content.Items.FairyCatcher.Accessories;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core.Systems.FairyCatcherSystem.ElfTrade
{
    public class AccessoryTrade : IElfTradable
    {
        public void AddElfTradable()
        {
            ElfPortalTrade.CreateElfTrade(ItemID.WarriorEmblem, ItemType<ElfEmblem>()).Register();
            ElfPortalTrade.CreateElfTrade(ItemID.RangerEmblem, ItemType<ElfEmblem>()).Register();
            ElfPortalTrade.CreateElfTrade(ItemID.SummonerEmblem, ItemType<ElfEmblem>()).Register();
            ElfPortalTrade.CreateElfTrade(ItemID.SorcererEmblem, ItemType<ElfEmblem>()).Register();
        }
    }
}
