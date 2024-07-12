﻿using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.Magike
{
    public class CrystalFrame : ModTile
    {
        public override string Texture => AssetDirectory.MagikeTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileBlockLight[Type] = false;
            Main.tileSolid[Type] = true;
            Main.tileNoFail[Type] = true;

            DustType = DustID.CrystalSerpent_Pink;
            HitSound = CoraliteSoundID.DigStone_Tink;
            AddMapEntry(Coralite.Instance.MagicCrystalPink);

            AnimationFrameHeight = 90;
        }

        public override bool CanDrop(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 5;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            base.AnimateIndividualTile(type, i, j, ref frameXOffset, ref frameYOffset);
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            const int frameTime = 6 * 4;
            const int restTime = 180;
            if (frameCounter > restTime && frameCounter <= restTime + frameTime)
            {
                frame = (frameCounter - restTime) / 6;
            }
            else if (frameCounter > restTime * 2 + frameTime && frameCounter <= restTime * 2 + frameTime * 2)
            {
                frame = 4 - (frameCounter - restTime * 2 - frameTime) / 6;
            }
            else if (frameCounter > restTime * 2 + frameTime * 2)
            {
                frameCounter = 0;
                frame = 0;
            }
        }
    }
}
