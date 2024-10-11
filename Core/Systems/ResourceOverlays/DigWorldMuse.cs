using Coralite.Content.WorldGeneration;
using Terraria.GameContent.UI.ResourceSets;

namespace Coralite.Core.Systems.ResourceOverlays
{
    public class DigWorldMuse : ModResourceOverlay
    {
        public override bool PreDrawResourceDisplay(PlayerStatsSnapshot snapshot, IPlayerResourcesDisplaySet displaySet, bool drawingLife, ref Color textColor, out bool drawText)
        {
            if (CoraliteWorld.DigDigDigWorld && !drawingLife)
            {
                drawText = false;
                return false;
            }

            return base.PreDrawResourceDisplay(snapshot, displaySet, drawingLife, ref textColor, out drawText);
        }

        //public override bool DisplayHoverText(PlayerStatsSnapshot snapshot, IPlayerResourcesDisplaySet displaySet, bool drawingLife)
        //{
        //    return base.DisplayHoverText(snapshot, displaySet, drawingLife);
        //}

        //public override bool PreDrawResource(ResourceOverlayDrawContext context)
        //{
        //    return base.PreDrawResource(context);
        //}
    }
}
