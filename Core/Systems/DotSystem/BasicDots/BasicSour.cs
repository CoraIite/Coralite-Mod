using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.DotSystem.BasicDotTags;
using Coralite.Core.Systems.DotSystem.DotAccessories;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Core.Systems.DotSystem.BasicDots
{
    public abstract class BasicSour : Dot
    {
        public override int BaseMaxDotLayer => 6;

        /// <summary>
        /// 基础的造成伤害的间隔
        /// </summary>
        public virtual int BasicDamageInterval => 120;
        public int DamageInterval => Math.Clamp((int)(BasicDamageInterval * DamageIntervalMultiplier), 1, 60 * 10);

        /// <summary>
        /// 伤害间隔倍率，实际间隔为基础间隔乘以该值
        /// </summary>
        public float DamageIntervalMultiplier = 1f;

        protected int Timer { get; set; }

        public override int[] LoadTags() => [ModContent.GetInstance<SourTag>().Type];

        public override void OnAdd(DotInfo info)
        {
            DamageIntervalMultiplier = 1f;

            HashSet<int> players = [];
            foreach (var dotInfo in DotInfos)
            {
                if (dotInfo.player < 0 || dotInfo.player >= Main.maxPlayers
                    || !players.Add(dotInfo.player))
                    continue;

                Player player = Main.player[dotInfo.player];
                if (player.TryGetModPlayer(out CoralitePlayer cp))
                    foreach (var effect in cp.EffectHandlers)
                        if (effect is ISourBonus bonus)
                            bonus.ModifySour(this);
            }
        }

        public override void Update(NPC npc)
        {
            if (CurrentLayer <= 0)
            {
                RemoveDot();
                return;
            }

            Timer++;
            if (Timer >= DamageInterval)
            {
                Timer = 0;
                ApplyDot(npc);
            }
        }
    }
}
