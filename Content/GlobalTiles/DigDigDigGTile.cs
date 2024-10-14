using Coralite.Content.WorldGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.GlobalTiles
{
    public class DigDigDigGTile:GlobalTile
    {
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!CoraliteWorld.DigDigDigWorld)
                return;
        
            switch (type)
            {
                case TileID.Stone:
                    {
                        if (!NPC.downedSlimeKing&&Main.rand.NextBool(1000))
                        {


                            //if (Main.netMode == NetmodeID.SinglePlayer)
                            //    Main.NewText(Language.GetTextValue("Announcement.HasAwoken", Main.npc[availableNPCSlot].TypeName), 175, 75);
                            //else if (Main.netMode == NetmodeID.Server)
                            //    ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", Main.npc[availableNPCSlot].GetTypeNetName()), new Color(175, 75, 255));

                        }
                    }
                    break;
                default:
                    return;
            }
        }
    }
}
