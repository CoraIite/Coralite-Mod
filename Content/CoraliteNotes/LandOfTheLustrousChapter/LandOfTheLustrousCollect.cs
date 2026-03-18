using Coralite.Content.Items.Donator;
using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using static Coralite.Content.CoraliteNotes.LandOfTheLustrousChapter.LandOfTheLustrousKnowledge;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.CoraliteNotes.LandOfTheLustrousChapter
{
    [VaultLoaden(AssetDirectory.NoteWeapons)]
    public class LandOfTheLustrousCollect : CollectionPage<LandOfTheLustrousKnowledge>
    {
        public static ATex LandOfTheLustrousCollectT { get; private set; }
        public static ATex LandOfTheLustrousCollectButton { get; private set; }
        public static ATex LandOfTheLustrousCollectButtonLight { get; private set; }

        public override void AddImages()
        {
            int LandOfTheLustrous = ItemType<LandOfTheLustrous>();
            int DiamondFlower = ItemType<DiamondFlower>();
            int AmberEardrop = ItemType<AmberEardrop>();

            NewImage<PyropeCrown>( new Vector2(-16, 310), null
                , GemWeapons.PyropeCrown, CollectImage.LockIconType.Small, 1.25f, LandOfTheLustrous);
            NewImage<AmethystNecklace>(new Vector2(152, 285), Condition.DownedEyeOfCthulhu
                , GemWeapons.AmethystNecklace, CollectImage.LockIconType.Small, 1.3f, LandOfTheLustrous);
            NewImage<AquamarineBracelet>(new Vector2(-127, 312), Condition.DownedEowOrBoc
                , GemWeapons.AquamarineBracelet, CollectImage.LockIconType.Small, 1.3f, LandOfTheLustrous, AmberEardrop);
            NewImage<PinkDiamondRose>(new Vector2(194, 237), Condition.DownedSkeletron
                , GemWeapons.PinkDiamondRose, CollectImage.LockIconType.Small, 1.3f, LandOfTheLustrous, DiamondFlower);
            NewImage<ZumurudRing>( new Vector2(-230, -2), Condition.Hardmode
                , GemWeapons.ZumurudRing, CollectImage.LockIconType.Small, 1.3f, LandOfTheLustrous);
            NewImage<PearlBrooch>( new Vector2(250, -54), Condition.DownedQueenSlime
                , GemWeapons.PearlBrooch, CollectImage.LockIconType.Small, 1.3f, LandOfTheLustrous);
            NewImage<RubyScepter>( new Vector2(-208, 254), Condition.DownedMechBossAll
                , GemWeapons.RubyScepter, CollectImage.LockIconType.Small, 1.2f, LandOfTheLustrous);
            NewImage<PeridotTalisman>( new Vector2(-260, -52), Condition.DownedPlantera
                , GemWeapons.PeridotTalisman, CollectImage.LockIconType.Small, 1.2f, LandOfTheLustrous);
            NewImage<SapphireHairpin>(new Vector2(244, 4), Condition.DownedMartians
                , GemWeapons.SapphireHairpin, CollectImage.LockIconType.Small, 1.2f, LandOfTheLustrous);
            NewImage<TourmalineMonoclastic>(new Vector2(92, 58), Condition.DownedNebulaPillar
                , GemWeapons.TourmalineMonoclastic, CollectImage.LockIconType.Small, 1.1f, LandOfTheLustrous);
            NewImage<TopazMirror>(new Vector2(-100, 228), Condition.DownedMoonLord
                , GemWeapons.TopazMirror, scale: 1.1f, others: LandOfTheLustrous);
            NewImage<ZirconGrail>(new Vector2(108, 180), Condition.DownedMoonLord
                , GemWeapons.ZirconGrail, scale: 1.35f, others: LandOfTheLustrous);
            NewImage<Phosphophyllite>(new Vector2(12, 382), CoraliteConditions.DownedNightmarePlantera
                , GemWeapons.Phosphophyllite, CollectImage.LockIconType.Small, 1.3f, LandOfTheLustrous);

            NewImage<LandOfTheLustrous>(new Vector2(-57, 62), CoraliteConditions.DownedNightmarePlantera
                , GemWeapons.LandOfTheLustrous, CollectImage.LockIconType.Big, 2);

            var button = new CollectButton(LandOfTheLustrousCollectButton, LandOfTheLustrousCollectButtonLight, new Vector2(0, -10), Knowledge);
            button.ItemPosOffset = new Vector2(0, -10);
            button.SetCenter(new Vector2(PageWidth / 2, 196));
            Append(button);
        }

        public void NewImage<T>(Vector2 pos, Condition condition, GemWeapons type
            , CollectImage.LockIconType lockType = CollectImage.LockIconType.Middle, float scale = 1f, params int[] others) where T : ModItem
        {
            Vector2 center = new Vector2(PageWidth / 2, PageHeight / 2 - 80);
            var button = new CollectImage(ItemType<T>(), condition, Knowledge.Collects, (int)type, scale, lockType);
            button.SetCenter(center + pos);
            if (others != null)
                button.AddOtherItems(others);

            Append(button);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            LandOfTheLustrousCollectT.Value.QuickCenteredDraw(spriteBatch, Center);

            DrawCollectTip(spriteBatch, Knowledge.Collects);

            Vector2 pos = PageTop + new Vector2(0, 132);
            DrawCollectText(spriteBatch, Knowledge.Collects, pos + new Vector2(7, -15));
            DrawCollectProgress(spriteBatch, Knowledge.Collects, pos + new Vector2(0, 15));
        }
    }
}
