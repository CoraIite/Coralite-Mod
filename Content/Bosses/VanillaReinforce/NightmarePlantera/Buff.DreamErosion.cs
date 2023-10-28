using Coralite.Content.ModPlayers;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class DreamErosion : ModBuff
    {
        public override string Texture => AssetDirectory.NightmarePlantera + Name;

        public override void SetStaticDefaults()
        {
            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (NightmarePlantera.NightmarePlanteraAlive(out _))
                player.buffTime[buffIndex] = 18000;
            else
            {
                player.DelBuff(buffIndex);
                if (player.TryGetModPlayer(out CoralitePlayer cp))
                    cp.nightmareCount = 0;
                buffIndex--;
            }
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams)
        {

        }
    }
}
