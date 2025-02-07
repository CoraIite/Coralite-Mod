using Coralite.Helpers;

namespace Coralite.Content.CoraliteNotes
{
    public struct ScaleController
    {
        public readonly float targetScale;
        public readonly float scaleFloatingRange;
        private readonly float lerpSpeed;

        public ScaleController(float targetScale, float scaleFloatingRange, float lerpSpeed = 0.1f)
        {
            this.targetScale = targetScale;
            this.scaleFloatingRange = scaleFloatingRange;
            this.lerpSpeed = lerpSpeed;
        }

        public float Scale { get; private set; }

        public readonly float ScalePercent => (Scale - targetScale) / scaleFloatingRange;

        public void ResetScale()
        {
            Scale = targetScale - scaleFloatingRange;
        }

        public void ToBigSize()
        {
            Scale = Helper.Lerp(Scale, targetScale + scaleFloatingRange, lerpSpeed);
        }

        public void ToNormalSize()
        {
            Scale = Helper.Lerp(Scale, targetScale, lerpSpeed);
        }
    }
}
