using Coralite.Content.Items.Misc_Melee;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Coralite.Content.CoraliteNotes.SwordChapter
{
    [AutoLoadTexture(Path = AssetDirectory.NoteWeapons)]
    public class SwordCollect : CollectionPage
    {
        public static bool[] Unlocks = new bool[(int)Swords.Count];

        public static ATex ZenithCollect { get; private set; }
        public static ATex SwordCollectButton { get; private set; }
        public static ATex SwordCollectButtonLight { get; private set; }

        public enum Swords
        {
            CopperShortsword,
            EnchantedSword,
            BeeKeeper,
            Starfury,
            Seedler,
            TheHorsemansBlade,
            InfluxWaver,
            StarWrath,
            Meowmere,

            LightsBane,
            BloodButcherer,
            Volcano,
            Muramasa,
            BladeOfGrass,
            NightsEdge,
            TrueNightsEdge,
            BrokenHeroSword,
            Excalibur,
            TrueExcalibur,
            TerraBlade,

            Zenith,
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
            Vector2 center = new Vector2(PageWidth / 2, PageHeight / 2 - 80);

            NewImage(ItemID.CopperShortsword, center + new Vector2(-200, 146), null
                , Swords.CopperShortsword, CollectImage.LockIconType.Small, 1.2f);
            NewImage(ItemID.EnchantedSword, center + new Vector2(-140, 104), null
                , Swords.EnchantedSword, CollectImage.LockIconType.Small, 1.3f);
            NewImage(ItemID.BeeKeeper, center + new Vector2(16, 296), Condition.DownedQueenBee
                , Swords.BeeKeeper, CollectImage.LockIconType.Small, 1.2f);
            NewImage(ItemID.Starfury, center + new Vector2(106, 262), null
                , Swords.Starfury, CollectImage.LockIconType.Small, 1.1f);
            NewImage(ItemID.Seedler, center + new Vector2(230, 86), Condition.DownedPlantera
                , Swords.Seedler, CollectImage.LockIconType.Small);
            NewImage(ItemID.TheHorsemansBlade, center + new Vector2(180, 326), Condition.DownedPumpking
                , Swords.TheHorsemansBlade, scale: 1.1f);
            NewImage(ItemID.InfluxWaver, center + new Vector2(210, 256), Condition.DownedMartians
                , Swords.InfluxWaver, scale: 1.1f);
            NewImage(ItemID.StarWrath, center + new Vector2(74, -60), Condition.DownedMoonLord
                , Swords.StarWrath, scale: 1.2f);
            NewImage(ItemID.Meowmere, center + new Vector2(156, -50), Condition.DownedMoonLord
                , Swords.Meowmere, scale: 1.2f);
            NewImage(ItemID.LightsBane, center + new Vector2(-192, 280), Condition.DownedEowOrBoc
                , Swords.LightsBane, CollectImage.LockIconType.Small, 1.05f);
            NewImage(ItemID.BloodButcherer, center + new Vector2(-122, 268), Condition.DownedEowOrBoc
                , Swords.BloodButcherer, CollectImage.LockIconType.Small, 1.05f);
            NewImage(ItemID.BladeofGrass, center + new Vector2(-128, 200), null
                , Swords.BladeOfGrass, CollectImage.LockIconType.Small);
            NewImage(ItemID.FieryGreatsword, center + new Vector2(-40, 118), Condition.DownedEowOrBoc
                , Swords.Volcano);
            NewImage(ItemID.Muramasa, center + new Vector2(-50, 206), Condition.DownedSkeletron
                , Swords.Muramasa, CollectImage.LockIconType.Small);
            NewImage(ItemID.NightsEdge, center + new Vector2(20, 128), Condition.DownedSkeletron
                , Swords.NightsEdge, scale: 1.3f);
            NewImage(ItemID.TrueNightsEdge, center + new Vector2(60, 38), Condition.DownedMechBossAll
                , Swords.TrueNightsEdge, scale: 1.3f);
            NewImage(ItemID.BrokenHeroSword, center + new Vector2(146, 116), Condition.DownedPlantera
                , Swords.BrokenHeroSword, CollectImage.LockIconType.Small, 1.2f);
            NewImage(ItemID.Excalibur, center + new Vector2(-180, 14), Condition.DownedMechBossAny
                , Swords.Excalibur, CollectImage.LockIconType.Small, 1.1f);
            NewImage(ItemID.TrueExcalibur, center + new Vector2(-114, -44), Condition.DownedMechBossAll
                , Swords.TrueExcalibur, CollectImage.LockIconType.Small, 1.15f);
            NewImage(ItemID.TerraBlade, center + new Vector2(-4, -8), Condition.DownedPlantera
                , Swords.TerraBlade, scale: 1.2f);

            var b = new CollectImage(ItemID.Zenith, Condition.DownedMoonLord
                , Unlocks, (int)Swords.Zenith, 1.75f, CollectImage.LockIconType.Big);
            b.SetCenter(center + new Vector2(188, -142));
            b.LockIconScale = 0.8f;
            Append(b);

            var button2 = new CollectButton(SwordCollectButton, SwordCollectButtonLight
                , new Vector2(7, -15), ModContent.ItemType<ZenithRelic>(), Unlocks, CoraliteNoteSystem.RewardType.Sword);
            button2.ItemPosOffset = new Vector2(10, 6);
            button2.SetCenter(new Vector2(PageWidth / 2 - 140, 64));
            Append(button2);
        }

        public void NewImage(int itemType, Vector2 pos, Condition condition, Swords type
            , CollectImage.LockIconType lockType = CollectImage.LockIconType.Middle, float scale = 1f)
        {
            var button = new CollectImage(itemType, condition, Unlocks, (int)type, scale, lockType);
            button.SetCenter(pos);
            Append(button);
        }

        public static void Unlock(Swords type)
            => Unlocks[(int)type] = true;

        public static void Save(TagCompound tag)
        {
            tag.SaveBools(Unlocks, "SwordUnlock");
        }

        public static void Load(TagCompound tag)
        {
            tag.LoadBools(Unlocks, "SwordUnlock");
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            ZenithCollect.Value.QuickCenteredDraw(spriteBatch, Center);

            DrawCollectTip(spriteBatch, Unlocks);

            DrawCollectText(spriteBatch, Unlocks, PageTop + new Vector2(45, 40));
            DrawCollectProgress(spriteBatch, Unlocks, PageTop + new Vector2(130, 39));
        }
    }
}
