using Coralite.Core;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IceCube : ModNPC
    {
        public override string Texture => AssetDirectory.BabyIceDragon + Name;

        public ref float ExtendTrigger => ref NPC.ai[0];
        public ref float ExtendCount => ref NPC.ai[1];
        public ref float Timer => ref NPC.ai[3];

        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void SetDefaults()
        {
            NPC.width = 68;
            NPC.height = 138;
            NPC.lifeMax = 400;
        }

        public override void AI()
        {
            //随着ai1的增大而增大，随后爆炸
            if (ExtendTrigger == 1f)
            {
                ExtendTrigger = 0f;
                ExtendCount++;
                NPC.scale += 0.2f;
            }

            if (Timer % 10 == 0)
            {
                //生成特效粒子
            }

            if (ExtendCount > 20)
            {
                //大于多少后产生爆炸
            }

            Timer += 1f;
        }

        public override void OnKill()
        {
            //TODO:寻找到冰龙宝宝并让它眩晕
        }
    }
}