using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public class SpikeGelBall : GelBall
    {
        public override void SetDefaults()
        {
            
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode!=NetmodeID.MultiplayerClient)
            {
                //生成一些尖刺弹幕
            }
        }

    }
}
