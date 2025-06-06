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

        public static ATex Test { get; private set; }
        //public static ATex DashBowCollectButton { get; private set; }
        //public static ATex DashBowCollectLight { get; private set; }

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

            NewImage(ItemID.CopperShortsword,center + new Vector2(-240, 126), null
                , Swords.CopperShortsword, CollectImage.LockIconType.Small);
            NewImage(ItemID.EnchantedSword, center + new Vector2(-166, 316), null
                , Swords.EnchantedSword, CollectImage.LockIconType.Small);
            NewImage(ItemID.BeeKeeper, center + new Vector2(174, 302), Condition.DownedEyeOfCthulhu
                , Swords.BeeKeeper, CollectImage.LockIconType.Small);
            NewImage(ItemID.Starfury, center + new Vector2(-240, -88), CoraliteConditions.DownedBabyIceDragon
                , Swords.Starfury, CollectImage.LockIconType.Small);
            NewImage(ItemID.Seedler, center + new Vector2(155, 86), Condition.DownedSkeletron
                , Swords.Seedler, CollectImage.LockIconType.Small);
            NewImage(ItemID.TheHorsemansBlade, center + new Vector2(-135, -190), Condition.Hardmode
                , Swords.TheHorsemansBlade);
            NewImage(ItemID.InfluxWaver, center + new Vector2(-60, -150), Condition.Hardmode
                , Swords.InfluxWaver, CollectImage.LockIconType.Small);
            NewImage(ItemID.StarWrath, center + new Vector2(24, 260), Condition.DownedSkeletronPrime
                , Swords.StarWrath, CollectImage.LockIconType.Small);
            NewImage(ItemID.Meowmere, center + new Vector2(214, 212), Condition.DownedTwins
                , Swords.Meowmere, CollectImage.LockIconType.Small);
            NewImage(ItemID.LightsBane, center + new Vector2(-190, 216), Condition.DownedDestroyer
                , Swords.LightsBane);
            NewImage(ItemID.BloodButcherer, center + new Vector2(6, 370), CoraliteConditions.DownedThunderveinDragon
                , Swords.BloodButcherer, CollectImage.LockIconType.Small,1.1f);
            NewImage(ItemID.FieryGreatsword, center + new Vector2(128, -100), Condition.DownedPlantera
                , Swords.Volcano);
            NewImage(ItemID.Muramasa, center + new Vector2(-144, -32), Condition.DownedIceQueen
                , Swords.Muramasa);
            NewImage(ItemID.BladeofGrass, center + new Vector2(126, -26), Condition.DownedMoonLord
                , Swords.BladeOfGrass);
            NewImage(ItemID.NightsEdge, center + new Vector2(26, -26), Condition.DownedMoonLord
                , Swords.NightsEdge);
            NewImage(ItemID.TrueNightsEdge, center + new Vector2(26, -126), Condition.DownedMoonLord
                , Swords.TrueNightsEdge);
            NewImage(ItemID.BrokenHeroSword, center + new Vector2(26, -226), Condition.DownedMoonLord
                , Swords.BrokenHeroSword);
            NewImage(ItemID.Excalibur, center + new Vector2(126, -226), Condition.DownedMoonLord
                , Swords.Excalibur);
            NewImage(ItemID.TrueExcalibur, center + new Vector2(226, -226), Condition.DownedMoonLord
                , Swords.TrueExcalibur);
            NewImage(ItemID.TerraBlade, center + new Vector2(226, -326), Condition.DownedMoonLord
                , Swords.TerraBlade);

            var b = new CollectImage(ItemID.Zenith, CoraliteConditions.DownedNightmarePlantera
                , Unlocks, (int)Swords.Zenith, 2f, CollectImage.LockIconType.Big);
            b.SetCenter(center + new Vector2(-2, 103));
            b.LockIconScale = 1.5f;
            Append(b);

            //var button2 = new CollectButton(DashBowCollectButton, DashBowCollectLight
            //    , new Vector2(0, 0), ItemType<ThyphionRelic>(), Unlocks, CoraliteNoteSystem.RewardType.Sword);
            //button2.ItemPosOffset = new Vector2(0, 0);
            //button2.SetCenter(new Vector2(PageWidth / 2+158, 100));
            //Append(button2);
        }

        public void NewImage(int itemType,Vector2 pos, Condition condition, Swords type
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
            Test.Value.QuickCenteredDraw(spriteBatch, Center);

            DrawCollectTip(spriteBatch, Unlocks);

            //DrawCollectText(spriteBatch, Unlocks, PageTop + new Vector2(115, 40));
            //DrawCollectProgress(spriteBatch, Unlocks, PageTop + new Vector2(210, 39));
        }
    }
}
