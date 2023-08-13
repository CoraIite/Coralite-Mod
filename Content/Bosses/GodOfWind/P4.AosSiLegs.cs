using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace Coralite.Content.Bosses.GodOfWind
{
    public class AosSiLegs: AosSiBodyPart
    {
        public override string Texture => AssetDirectory.GodOfWind + "AosSiLeftLeg";

        public static Asset<Texture2D> RightLeg;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            RightLeg = ModContent.Request<Texture2D>(AssetDirectory.GodOfWind + "AosSiRightLeg");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            RightLeg = null;
        }

    }
}
