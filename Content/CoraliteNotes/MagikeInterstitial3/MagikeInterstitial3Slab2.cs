using Coralite.Core;
using Coralite.Core.Systems.BossSystems;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.CoraliteNotes.MagikeInterstitial3
{
    [VaultLoaden(AssetDirectory.CoraliteNote + "MagikeInterstitial3/")]
    public class MagikeInterstitial3Slab2 : KnowledgePage
    {
        public override bool CanShowInBook => ModContent.GetInstance<DownedCrystallineSentinel>().Value;

        public static ATex CrystallineSlab2 { get; private set; }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Utils.DrawBorderString(spriteBatch, MagikeInterstitial3Slab1.SlabText.Value, TitlePos, Coralite.CrystallinePurple, 1, 0.5f, 0f);

            CrystallineSlab2.Value.QuickCenteredDraw(spriteBatch, Center + new Vector2(0, 30), scale: 1.15f);
        }
    }
}
