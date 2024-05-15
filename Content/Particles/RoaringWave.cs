﻿using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Terraria;

namespace Coralite.Content.Particles
{
    public class RoaringWave : Particle
    {
        public override bool ShouldUpdateCenter() => false;

        public override string Texture => AssetDirectory.Particles + "Roaring";

        public override void OnSpawn()
        {
            Frame = new Rectangle(0, 128, 128, 128);
            Rotation = Main.rand.NextFloat(6.282f);
        }

        public override void Update()
        {
            Scale *= 1.1f;

            if (fadeIn > 15)
                color *= 0.9f;

            fadeIn++;
            if (color.A < 20)
                active = false;
        }
    }
}
