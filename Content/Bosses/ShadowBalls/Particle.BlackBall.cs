using Coralite.Core;
using Coralite.Core.Prefabs.Particles;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class BigBlackBall():BaseFrameParticle(1,11,2,randRot:true)
    {
        public override string Texture => AssetDirectory.ShadowBalls+Name;

        public override void SetProperty()
        {
            base.SetProperty();
            PRTDrawMode = PRTDrawModeEnum.NonPremultiplied;
        }

        public override void AI()
        {
            base.AI();
            Rotation+=0.05f;
        }
    }

    public class MiddleBlackBall(): BaseFrameParticle(1, 11, 2, randRot: true)
    {
        public override string Texture => AssetDirectory.ShadowBalls + Name;

        public override void SetProperty()
        {
            base.SetProperty();
            PRTDrawMode = PRTDrawModeEnum.NonPremultiplied;
        }

        public override void AI()
        {
            base.AI();
            Rotation += 0.05f;
        }
    }

    public class SmallBlackBall(): BaseFrameParticle(1, 11, 2, randRot: true)
    {
        public override string Texture => AssetDirectory.ShadowBalls + Name;

        public override void SetProperty()
        {
            base.SetProperty();
            PRTDrawMode = PRTDrawModeEnum.NonPremultiplied;
        }

        public override void AI()
        {
            base.AI();
            Rotation += 0.05f;
        }
    }
}
