using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Items.AlchorthentSeries
{
    public class BaseAlchSymbol : Particle
    {
        public LineDrawer line;
        public float maxScale = 24;
        public float fadeTime = 20;
        public float ShineTime = 20;
        public float disappearTime = 20;
        public int ownerProjIndex = -1;

        public virtual float LineWidth { get => 20; }
        public virtual bool FadeScale { get => true; }
        public virtual bool FadeLineOffset { get => true; }
        public virtual bool FadeColor { get => false; }

        public override void SetProperty()
        {
            ShouldKillWhenOffScreen = false;
            AlchorthentShaderData temp;
            shader = temp = new AlchorthentShaderData(CoraliteAssets.Laser.TwistLaser, Coralite.Instance.Assets.Request<Effect>("Effects/LineAdditive", ReLogic.Content.AssetRequestMode.ImmediateLoad), "MyNamePass");

            //temp.SetPowCount(0.6f);
            temp.SetFlowAdd(4);
            temp.SetLineColor(Color.Transparent);

            line = GetSymbolLine();

            //line.SetScale(16);
            line.SetLineWidth(LineWidth);
        }

        public virtual LineDrawer GetSymbolLine() => ExquisiteHammer.NewIronAlchSymbol();

        public override void AI()
        {
            if (shader is not AlchorthentShaderData data)
                return;

            if (ownerProjIndex.GetProjectileOwner(out Projectile p))
                Position = p.Center;

            data.SetTime((int)Main.timeForVisualEffects * 0.05f);

            if (Opacity == 0)
                data.SetLineColor(Color.Transparent);

            //先连接，然后闪一下，最后消失
            if (Opacity <= fadeTime)
            {
                float factor = Opacity / fadeTime;
                line.SetScale(maxScale * factor);

                data.SetLineColor(Color);
                data.SetLineOffset(Helper.BezierEase(factor));
            }
            else if (Opacity < fadeTime + ShineTime)
            {

            }
            else if (Opacity < fadeTime + ShineTime + disappearTime)
            {
                float baseF = (Opacity - fadeTime - ShineTime) / disappearTime;
                float f = Helper.BezierEase(baseF);
                if (FadeColor)
                    data.SetLineColor(Color.Lerp(Color, Color.Transparent, f));
                if (FadeScale)
                    line.SetScale(maxScale * (1 - baseF));
                if (FadeLineOffset)
                    data.SetLineOffset(1 - f);
            }
            else
                active = false;

            Opacity++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            line?.Draw(Position);
            return false;
        }
    }
}
