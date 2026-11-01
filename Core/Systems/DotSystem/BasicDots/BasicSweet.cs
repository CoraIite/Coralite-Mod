using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.DotSystem.BasicDotTags;
using Coralite.Core.Systems.DotSystem.DotAccessories;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Core.Systems.DotSystem.BasicDots
{
    public abstract class BasicSweet : Dot
    {
        public override int BaseMaxDotLayer => 3;
        public override DamageClass DamageType => DamageClass.Magic;


        public override int[] LoadTags() => [ModContent.GetInstance<SweetTag>().Type];

        public override void OnAdd(DotInfo info)
        {
            HashSet<int> players = [];
            foreach (var dotInfo in DotInfos)
            {
                if (!players.Add(dotInfo.player))
                    continue;

                Player player = Main.player[dotInfo.player];
                ApplyPlayerBonuses(player);
            }
        }

        public override void ApplyPlayerBonuses(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                foreach (var effect in cp.EffectHandlers)
                    if (effect is ISweetBonus bonus)
                        bonus.ModifySweet(this);
        }

    }
}
