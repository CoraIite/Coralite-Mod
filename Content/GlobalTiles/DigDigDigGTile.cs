using Coralite.Content.Bosses.DigDigDig.Stonelime;
using Coralite.Content.WorldGeneration;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.GlobalTiles
{
    public class DigDigDigGTile : GlobalTile
    {
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!CoraliteWorld.DigDigDigWorld)
                return;

            switch (type)
            {
                case TileID.Stone:
                    {
                        if (!NPC.downedSlimeKing && !noItem && !fail && Main.rand.NextBool(1500) && !NPC.AnyNPCs(ModContent.NPCType<Stonelime>()))
                        {
                            int index = NPC.NewNPC(new EntitySource_TileBreak(i, j), i * 16, j * 16, ModContent.NPCType<Stonelime>());

                            if (Main.netMode == NetmodeID.SinglePlayer)
                                Main.NewText(Language.GetTextValue("Announcement.HasAwoken", Main.npc[index].TypeName), 175, 75);
                            else if (VaultUtils.isServer)
                                ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", Main.npc[index].GetTypeNetName()), new Color(175, 75, 255));

                            int x = i - 5;
                            int y = j - 5;

                            for (int i2 = 0; i2 < 12; i2++)
                                for (int j2 = 0; j2 < 12; j2++)
                                    WorldGen.KillTile(x + i2, y + j2, noItem: true);

                            noItem = true;
                        }
                    }
                    break;
                default:
                    return;
            }
        }
    }
}
