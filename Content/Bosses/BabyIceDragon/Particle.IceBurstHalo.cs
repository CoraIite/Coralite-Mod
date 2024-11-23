﻿using Coralite.Core;
using InnoVault.PRT;
using Terraria;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IceBurstHalo : BasePRT
    {
        public override string Texture => AssetDirectory.Particles + "IceHalo";

        public override bool ShouldUpdatePosition() => false;

        public override void SetProperty()
        {
            Scale = 0.02f;
            Color = Color.White;
            Rotation = Main.rand.NextFloat(6.282f);
            Frame = new Rectangle(0, 0, 128, 128);
            ShouldKillWhenOffScreen = false;
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override void AI()
        {
            if (Opacity > 14)
                Color *= 0.88f;

            Scale += 0.875f;

            Opacity++;
            if (Opacity > 20)
                active = false;
        }
    }
}
