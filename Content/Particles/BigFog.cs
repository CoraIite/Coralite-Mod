﻿using Coralite.Core.Systems.ParticleSystem;
using Terraria;

namespace Coralite.Content.Particles
{
    public class BigFog : Particle
    {
        public override void OnSpawn()
        {
            Frame = new Rectangle(0, 256 * Main.rand.Next(4), 256, 256);
        }

        public override void Update()
        {
            Velocity *= 0.98f;
            Rotation += 0.01f;
            Scale *= 0.997f;
            color *= 0.94f;

            fadeIn++;
            if (fadeIn > 60 || color.A < 10)
                active = false;
        }

    }
}
