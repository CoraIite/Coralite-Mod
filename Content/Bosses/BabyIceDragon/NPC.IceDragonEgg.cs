using Coralite.Core;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IceDragonEgg : ModNPC
    {
        public override string Texture => AssetDirectory.BabyIceDragon + Name;

        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 85;
            NPC.damage = 0;
            NPC.defense = 8;
            NPC.lifeMax = 800;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 1f;
            NPC.value = Item.buyPrice(0, 0, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;

        }

        public override void Load()
        {

        }

        public override void AI()
        {
            //受击时摇晃一下
        }

        public override void HitEffect(int hitDirection, double damage)
        {

        }

        public override void OnKill()
        {
            //生成冰龙宝宝

            //生成蛋壳gore
        }
    }
}
