using Coralite.Core;
using System;
using Terraria;
using Terraria.ModLoader;

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

        public NPC PrePart
        {
            get
            {
                if (PrePartIndex > 200 || PrePartIndex < 0)
                    throw new Exception("IndexOutOfRange, 请检测是否会越界");

                return Main.npc[PrePartIndex];
            }
        }
        public NPC Head
        {
            get
            {
                if (HeadIndex > 200 || HeadIndex < 0)
                    throw new Exception("IndexOutOfRange, 请检测是否会越界");

                return Main.npc[HeadIndex];
            }
        }

        public Player Target => Main.player[Head.target];

    }
}
