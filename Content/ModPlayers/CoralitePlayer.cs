using Coralite.Content.Items.BotanicalItems.Seeds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Content.ModPlayers
{
    public class CoralitePlayer : ModPlayer
    {
        public byte RightClickReuseDelay = 0;

        #region 各种更新

        public override void PostUpdate()
        {
            if (RightClickReuseDelay > 0)
                RightClickReuseDelay--;
        }

        #endregion

        #region 钓鱼系统

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            bool inWater = !attempt.inLava && !attempt.inHoney;

            if (inWater&& Player.ZoneBeach && attempt.common&& !attempt.crate)
            {
                if (Main.rand.NextBool(15))
                    itemDrop = ModContent.ItemType<NacliteSeedling>();
            }
        }

        #endregion
    }
}
