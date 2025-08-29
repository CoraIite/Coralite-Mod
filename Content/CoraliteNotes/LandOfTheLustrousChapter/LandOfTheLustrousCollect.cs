using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.CoraliteNotes.LandOfTheLustrousChapter
{
    [VaultLoaden(AssetDirectory.NoteWeapons)]
    public class LandOfTheLustrousCollect : CollectionPage
    {
        public static bool[] Unlocks = new bool[(int)GemWeapons.Count];

        public static ATex LandOfTheLustrousCollectT { get; private set; }
        public static ATex LandOfTheLustrousCollectButton { get; private set; }
        public static ATex LandOfTheLustrousCollectButtonLight { get; private set; }

        public enum GemWeapons
        {
            PyropeCrown,
            AmethystNecklace,
            AquamarineBracelet,
            PinkDiamondRose,
            ZumurudRing,
            PearlBrooch,
            RubyScepter,
            PeridotTalisman,
            SapphireHairpin,
            TourmalineMonoclastic,
            TopazMirror,
            ZirconGrail,

            Phosphophyllite,

            LandOfTheLustrous,
            Count,
        }

        public override void Recalculate()
        {
            RemoveAllChildren();
            AddImages();

            base.Recalculate();
        }

        public void AddImages()
        {
            int y = -400;
            Vector2 center = new Vector2(PageWidth / 2, PageHeight / 2 - 80);

            NewImage<PyropeCrown>(center + new Vector2(-16, 310), null
                , GemWeapons.PyropeCrown, CollectImage.LockIconType.Small, 1.25f);
            NewImage<AmethystNecklace>(center + new Vector2(152, 285), Condition.DownedEyeOfCthulhu
                , GemWeapons.AmethystNecklace, CollectImage.LockIconType.Small, 1.3f);
            NewImage<AquamarineBracelet>(center + new Vector2(-127, 312), Condition.DownedEowOrBoc
                , GemWeapons.AquamarineBracelet, CollectImage.LockIconType.Small, 1.3f);
            NewImage<PinkDiamondRose>(center + new Vector2(194, 237), Condition.DownedSkeletron
                , GemWeapons.PinkDiamondRose, CollectImage.LockIconType.Small, 1.3f);
            NewImage<ZumurudRing>(center + new Vector2(-230, -2), Condition.Hardmode
                , GemWeapons.ZumurudRing, CollectImage.LockIconType.Small, 1.3f);
            NewImage<PearlBrooch>(center + new Vector2(250, -54), Condition.DownedQueenSlime
                , GemWeapons.PearlBrooch, CollectImage.LockIconType.Small, 1.3f);
            NewImage<RubyScepter>(center + new Vector2(-208, 254), Condition.DownedMechBossAll
                , GemWeapons.RubyScepter, CollectImage.LockIconType.Small, 1.2f);
            NewImage<PeridotTalisman>(center + new Vector2(-260, -52), Condition.DownedPlantera
                , GemWeapons.PeridotTalisman, CollectImage.LockIconType.Small, 1.2f);
            NewImage<SapphireHairpin>(center + new Vector2(244, 4), Condition.DownedMartians
                , GemWeapons.SapphireHairpin, CollectImage.LockIconType.Small, 1.2f);
            NewImage<TourmalineMonoclastic>(center + new Vector2(92, 58), Condition.DownedNebulaPillar
                , GemWeapons.TourmalineMonoclastic, CollectImage.LockIconType.Small, 1.1f);
            NewImage<TopazMirror>(center + new Vector2(-100, 228), Condition.DownedMoonLord
                , GemWeapons.TopazMirror, scale: 1.1f);
            NewImage<ZirconGrail>(center + new Vector2(108, 180), Condition.DownedMoonLord
                , GemWeapons.ZirconGrail, scale: 1.35f);
            NewImage<Phosphophyllite>(center + new Vector2(12, 382), CoraliteConditions.DownedNightmarePlantera
                , GemWeapons.Phosphophyllite, CollectImage.LockIconType.Small, 1.3f);

            NewImage<LandOfTheLustrous>(center + new Vector2(-57, 62), CoraliteConditions.DownedNightmarePlantera
                , GemWeapons.LandOfTheLustrous, CollectImage.LockIconType.Big, 2);

            var button = new CollectButton(LandOfTheLustrousCollectButton, LandOfTheLustrousCollectButtonLight
                , new Vector2(0, -10), ItemType<LandOfTheLustrousRelic>(), Unlocks, CoraliteNoteSystem.RewardType.LandOfTheLustrous);
            button.ItemPosOffset = new Vector2(0, -10);
            button.SetCenter(new Vector2(PageWidth / 2, 196));
            Append(button);
        }

        public void NewImage<T>(Vector2 pos, Condition condition, GemWeapons type
            , CollectImage.LockIconType lockType = CollectImage.LockIconType.Middle, float scale = 1f) where T : ModItem
        {
            var button = new CollectImage(ItemType<T>(), condition, Unlocks, (int)type, scale, lockType);
            button.SetCenter(pos);
            Append(button);
        }

        public static void Unlock(GemWeapons type)
            => Unlocks[(int)type] = true;

        public static void Save(TagCompound tag)
        {
            tag.SaveBools(Unlocks, "LandOfTheLustrousUnlock");
        }

        public static void Load(TagCompound tag)
        {
            tag.LoadBools(Unlocks, "LandOfTheLustrousUnlock");
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            LandOfTheLustrousCollectT.Value.QuickCenteredDraw(spriteBatch, Center);

            DrawCollectTip(spriteBatch, Unlocks);

            Vector2 pos = PageTop + new Vector2(0, 132);
            DrawCollectText(spriteBatch, Unlocks, pos + new Vector2(7, -15));
            DrawCollectProgress(spriteBatch, Unlocks, pos + new Vector2(0, 15));
        }
    }
}
