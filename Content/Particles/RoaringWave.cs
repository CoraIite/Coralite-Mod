using Coralite.Core;
using Terraria;

namespace Coralite.Content.Particles
{
    public class RoaringWave : Particle
    {
        public override bool ShouldUpdatePosition() => false;

        public override string Texture => AssetDirectory.Particles + "Roaring";

        /// <summary>
        /// 控制大小每帧变大多少
        /// </summary>
        public float ScaleMul = 1.1f;

        /// <summary>
        /// 经过多少帧后开始消失
        /// </summary>
        public int SpawnTime = 15;

        /// <summary>
        /// 超过<see cref="SpawnTime"/>时每帧缩小多少
        /// </summary>
        public float FadePercent = 0.9f;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Frame = new Rectangle(0, 128, 128, 128);
            Rotation = Main.rand.NextFloat(6.282f);
        }

        public override void AI()
        {
            Scale *= ScaleMul;

            if (Opacity > SpawnTime)
                Color *= FadePercent;

            Opacity++;
            if (Color.A < 20)
                active = false;
        }
    }
}
