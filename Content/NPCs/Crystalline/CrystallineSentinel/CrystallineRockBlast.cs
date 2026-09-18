using Coralite.Core;
using Coralite.Core.Prefabs.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline
{
    /// <summary>
    /// 碎岩爆点：3×7 帧图的一次性冲击，28 帧播完。碎岩出手、转阶段碎裂与死亡演出共用。
    /// </summary>
    public class CrystallineRockBlast() : BaseFrameParticle(3, 7, 4)
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        public ref float Timer => ref ai[0];

        public override void SetProperty()
        {
            Scale = Main.rand.NextFloat(0.7f, 1.3f) * 0.7f;
            Color = Color.White;
            base.SetProperty();
        }

        public override void AI()
        {
            Velocity *= 0.96f;
            Timer++;
            if (Timer > 4 * 7)
                Kill();
            base.AI();
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = TexValue;
            Rectangle frameBox = TexValue.Frame(3, 7, Frame.X, Frame.Y);
            spriteBatch.Draw(tex, Position - Main.screenPosition, frameBox, Color, Rotation + MathHelper.PiOver2,
                frameBox.Size() / 2, Scale, SpriteEffects.None, 0);
            return false;
        }
    }

    /// <summary>
    /// 飞弹爆炸的碎片：7 帧随机一帧，随生命周期淡出。
    /// </summary>
    public class CrystallineFragmentParticle : Particle
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        public readonly int MaxFrame = 7;
        public ref float Alpha => ref ai[0];
        public ref float Offset => ref ai[1];
        public ref float FrameY => ref ai[2];

        public override void SetProperty()
        {
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Lifetime = 42 + Main.rand.Next(-18, 12);
            Color = Color.White;
            Scale = Main.rand.NextFloat(0.5f, 1.5f);
            Offset = Main.rand.Next(100);
            FrameY = Main.rand.Next(MaxFrame);
        }

        public override void AI()
        {
            Velocity *= 0.98f;
            Alpha = Utils.Remap(LifetimeCompletion, 0f, 1f, 1f, 0f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = TexValue;
            var frameBox = tex.Frame(1, MaxFrame, 0, Frame.Y);

            spriteBatch.Draw(tex, Position - Main.screenPosition, frameBox, Color * Alpha, Rotation, frameBox.Size() / 2, Scale, 0, 0);
            return false;
        }
    }
}
