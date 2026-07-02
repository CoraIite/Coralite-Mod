using Coralite.Core;
using Coralite.Core.Systems.BossSystems;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeInterstitial3
{
    [VaultLoaden(AssetDirectory.CoraliteNote + "MagikeInterstitial3/")]
    public class MagikeInterstitial3Slab1 : KnowledgePage
    {
        public override bool CanShowInBook => ModContent.GetInstance<DownedCrystallineSentinel>().Value;

        public static ATex CrystallineSlab1 { get; private set; }

        public static LocalizedText SlabText { get; private set; }
        public static LocalizedText SlabUnlockName { get; private set; }
        public static LocalizedText SlabUnlockText { get; private set; }

        public override void OnInitialize()
        {
            SlabText = this.GetLocalization(nameof(SlabText));
            SlabUnlockName = this.GetLocalization(nameof(SlabUnlockName));
            SlabUnlockText = this.GetLocalization(nameof(SlabUnlockText));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Utils.DrawBorderString(spriteBatch, SlabText.Value, TitlePos, Coralite.CrystallinePurple, 1, 0.5f, 0f);

            CrystallineSlab1.Value.QuickCenteredDraw(spriteBatch, Center+new Vector2(0,30), scale: 1.15f);
        }
    }
}
