using Coralite.Core;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.NPCs.ShadowNPCs
{
    public class ShadowGolem:ModNPC
    {
        public override string Texture => AssetDirectory.ShadowNPCs+Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("影子巨像");
        }

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 48;
            NPC.lifeMax = 175;
            NPC.damage = 20;
            NPC.defense = 8;
            NPC.knockBackResist = 0.1f;
            NPC.aiStyle = -1;
            NPC.value = Item.buyPrice(0, 0, 3, 50);

            NPC.noGravity = false;
            NPC.noTileCollide = false;

        }

        public override void AI()
        {
            
        }
    }
}
