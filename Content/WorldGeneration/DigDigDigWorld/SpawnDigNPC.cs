using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText DigTheNPC { get; set; }

        public void SpawnDigNPC(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = DigTheNPC.Value;

            NPC.NewNPC(new EntitySource_WorldGen(), Main.spawnTileX * 16, Main.spawnTileY * 16, NPCID.Angler);
            NPC.savedAngler = true;
            NPC.NewNPC(new EntitySource_WorldGen(), Main.spawnTileX * 16, Main.spawnTileY * 16, NPCID.Stylist);
            NPC.savedStylist = true;
            NPC.NewNPC(new EntitySource_WorldGen(), Main.spawnTileX * 16, Main.spawnTileY * 16, NPCID.TownSlimePurple);
            NPC.unlockedSlimePurpleSpawn = true;
        }
    }
}
