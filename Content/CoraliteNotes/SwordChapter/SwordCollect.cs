using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using static Coralite.Content.CoraliteNotes.SwordChapter.SwordKnowledge;

namespace Coralite.Content.CoraliteNotes.SwordChapter
{
    [VaultLoaden(AssetDirectory.NoteWeapons)]
    public class SwordCollect : CollectionPage<SwordKnowledge>
    {
        public static ATex ZenithCollect { get; private set; }
        public static ATex SwordCollectButton { get; private set; }
        public static ATex SwordCollectButtonLight { get; private set; }

        public override void AddImages()
        {
            NewImage(ItemID.CopperShortsword, new Vector2(-200, 146), null
                , Swords.CopperShortsword, CollectImage.LockIconType.Small, 1.2f, ItemID.Zenith);
            NewImage(ItemID.EnchantedSword, new Vector2(-140, 104), null
                , Swords.EnchantedSword, CollectImage.LockIconType.Small, 1.3f, ItemID.Zenith);
            NewImage(ItemID.BeeKeeper, new Vector2(16, 296), Condition.DownedQueenBee
                , Swords.BeeKeeper, CollectImage.LockIconType.Small, 1.2f, ItemID.Zenith);
            NewImage(ItemID.Starfury, new Vector2(106, 262), null
                , Swords.Starfury, CollectImage.LockIconType.Small, 1.1f, ItemID.Zenith);
            NewImage(ItemID.Seedler, new Vector2(230, 86), Condition.DownedPlantera
                , Swords.Seedler, CollectImage.LockIconType.Small, others: ItemID.Zenith);
            NewImage(ItemID.TheHorsemansBlade, new Vector2(180, 326), Condition.DownedPumpking
                , Swords.TheHorsemansBlade, scale: 1.1f, others: ItemID.Zenith);
            NewImage(ItemID.InfluxWaver, new Vector2(210, 256), Condition.DownedMartians
                , Swords.InfluxWaver, scale: 1.1f, others: ItemID.Zenith);
            NewImage(ItemID.StarWrath, new Vector2(74, -60), Condition.DownedMoonLord
                , Swords.StarWrath, scale: 1.2f, others: ItemID.Zenith);
            NewImage(ItemID.Meowmere, new Vector2(156, -50), Condition.DownedMoonLord
                , Swords.Meowmere, scale: 1.2f, others: ItemID.Zenith);

            NewImage(ItemID.LightsBane, new Vector2(-192, 280), Condition.DownedEowOrBoc
                , Swords.LightsBane, CollectImage.LockIconType.Small, 1.05f, ItemID.Zenith, ItemID.TerraBlade, ItemID.TrueNightsEdge, ItemID.NightsEdge);
            NewImage(ItemID.BloodButcherer, new Vector2(-122, 268), Condition.DownedEowOrBoc
                , Swords.BloodButcherer, CollectImage.LockIconType.Small, 1.05f, ItemID.Zenith, ItemID.TerraBlade, ItemID.TrueNightsEdge, ItemID.NightsEdge);
            NewImage(ItemID.BladeofGrass, new Vector2(-128, 200), null
                , Swords.BladeOfGrass, CollectImage.LockIconType.Small, others: [ItemID.Zenith, ItemID.TerraBlade, ItemID.TrueNightsEdge, ItemID.NightsEdge]);
            NewImage(ItemID.FieryGreatsword, new Vector2(-40, 118), Condition.DownedEowOrBoc
                , Swords.Volcano, others: [ItemID.Zenith, ItemID.TerraBlade, ItemID.TrueNightsEdge, ItemID.NightsEdge]);
            NewImage(ItemID.Muramasa, new Vector2(-50, 206), Condition.DownedSkeletron
                , Swords.Muramasa, CollectImage.LockIconType.Small, others: [ItemID.Zenith, ItemID.TerraBlade, ItemID.TrueNightsEdge, ItemID.NightsEdge]);
            NewImage(ItemID.NightsEdge, new Vector2(20, 128), Condition.DownedSkeletron
                , Swords.NightsEdge, scale: 1.3f, others: [ItemID.Zenith, ItemID.TerraBlade, ItemID.TrueNightsEdge]);
            NewImage(ItemID.TrueNightsEdge, new Vector2(60, 38), Condition.DownedMechBossAll
                , Swords.TrueNightsEdge, scale: 1.3f, others: [ItemID.Zenith, ItemID.TerraBlade]);
            NewImage(ItemID.BrokenHeroSword, new Vector2(146, 116), Condition.DownedPlantera
                , Swords.BrokenHeroSword, CollectImage.LockIconType.Small, 1.2f, ItemID.Zenith, ItemID.TerraBlade);
            NewImage(ItemID.Excalibur, new Vector2(-180, 14), Condition.DownedMechBossAny
                , Swords.Excalibur, CollectImage.LockIconType.Small, 1.1f, ItemID.Zenith, ItemID.TerraBlade, ItemID.TrueExcalibur);
            NewImage(ItemID.TrueExcalibur, new Vector2(-114, -44), Condition.DownedMechBossAll
                , Swords.TrueExcalibur, CollectImage.LockIconType.Small, 1.15f, ItemID.Zenith, ItemID.TerraBlade);
            NewImage(ItemID.TerraBlade, new Vector2(-4, -8), Condition.DownedPlantera
                , Swords.TerraBlade, scale: 1.2f, others: ItemID.Zenith);

            var b = new CollectImage(ItemID.Zenith, Condition.DownedMoonLord
                , Knowledge.Collects, (int)Swords.Zenith, 1.75f, CollectImage.LockIconType.Big);
            b.SetCenter(new Vector2(PageWidth / 2, PageHeight / 2 - 80) + new Vector2(188, -142));
            b.LockIconScale = 0.8f;
            Append(b);

            var button2 = new CollectButton(SwordCollectButton, SwordCollectButtonLight
                , new Vector2(7, -15), Knowledge)
            {
                ItemPosOffset = new Vector2(10, 6)
            };
            button2.SetCenter(new Vector2(PageWidth / 2 - 140, 64));
            Append(button2);
        }

        public void NewImage(int itemType, Vector2 pos, Condition condition, Swords type
            , CollectImage.LockIconType lockType = CollectImage.LockIconType.Middle, float scale = 1f, params int[] others)
        {
            Vector2 center = new Vector2(PageWidth / 2, PageHeight / 2 - 80);

            var button = new CollectImage(itemType, condition, Knowledge.Collects, (int)type, scale, lockType);
            button.SetCenter(center + pos);
            if (others != null)
                button.AddOtherItems(others);

            Append(button);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            ZenithCollect.Value.QuickCenteredDraw(spriteBatch, Center);

            DrawCollectTip(spriteBatch, Knowledge.Collects);

            DrawCollectText(spriteBatch, Knowledge.Collects, PageTop + new Vector2(45, 40));
            DrawCollectProgress(spriteBatch, Knowledge.Collects, PageTop + new Vector2(130, 39));
        }
    }
}
