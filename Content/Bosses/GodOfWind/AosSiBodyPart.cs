using Coralite.Core;
using Terraria;

namespace Coralite.Content.Bosses.GodOfWind
{
    public abstract class AosSiBodyPart : ModNPC
    {
        public override string Texture => AssetDirectory.GodOfWind + Name;

        /// <summary>
        /// 前一个NPC的索引
        /// </summary>
        public int PrePartIndex;
        /// <summary>
        /// 头部的索引
        /// </summary>
        public int HeadIndex;

        private void IndexOutOfRangeNPCStateErrorFunc()
        {
            Coralite.Instance.Logger.Info($"IndexOutOfRange ERROR: {NPC} 在检测前体节实体时发生数组越界，PrePartIndex值为:{PrePartIndex}");
            PrePartIndex = 1;
            NPC.active = false;
        }

        public NPC PrePart
        {
            get
            {
                if (PrePartIndex > 200 || PrePartIndex < 0)
                {
                    IndexOutOfRangeNPCStateErrorFunc();
                }

                return Main.npc[PrePartIndex];
            }
        }
        public NPC Head
        {
            get
            {
                if (HeadIndex > 200 || HeadIndex < 0)
                {
                    IndexOutOfRangeNPCStateErrorFunc();
                }

                return Main.npc[HeadIndex];
            }
        }

        public Player Target => Main.player[Head.target];

    }
}
