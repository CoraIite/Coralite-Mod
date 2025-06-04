using Coralite.Content.Items.Icicle;
using Coralite.Content.Items.Thunder;
using Coralite.Content.Items.ThyphionSeries;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.CoraliteNotes.DashBowChapter
{
    [AutoLoadTexture(Path = AssetDirectory.NoteWeapons)]
    public class DashBowCollect : CollectionPage
    {
        public static bool[] Unlocks = new bool[(int)DashBows.Count];

        public static ATex Test { get; private set; }
        //public static ATex LandOfTheLustrousCollectButton { get; private set; }
        //public static ATex LandOfTheLustrousCollectButtonLight { get; private set; }

        public enum DashBows
        {
            Afterglow,
            TremblingBow,
            FarAwaySky,
            IcicleBow,
            Turbulence,
            RadiantSun,
            FullMoon,
            PlasmaBow,
            HorizonArc,
            SeismicWave,

            ReversedFlash,
            Glaciate,
            Solunar,
            Aurora,

            Thyphion,
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

            NewImage<Afterglow>(center + new Vector2(-16, 310), null
                , DashBows.Afterglow, CollectImage.LockIconType.Small);
            NewImage<TremblingBow>(center + new Vector2(152, 285), Condition.DownedEyeOfCthulhu
                , DashBows.TremblingBow, CollectImage.LockIconType.Small);
            NewImage<FarAwaySky>(center + new Vector2(-127, 312), Condition.DownedEowOrBoc
                , DashBows.FarAwaySky, CollectImage.LockIconType.Small);
            NewImage<IcicleBow>(center + new Vector2(57, 312), Condition.DownedEowOrBoc
                , DashBows.IcicleBow, CollectImage.LockIconType.Small);
            NewImage<Turbulence>(center + new Vector2(194, 237), Condition.DownedSkeletron
                , DashBows.Turbulence, CollectImage.LockIconType.Small);
            NewImage<RadiantSun>(center + new Vector2(-230, -2), Condition.Hardmode
                , DashBows.RadiantSun, CollectImage.LockIconType.Small);
            NewImage<FullMoon>(center + new Vector2(250, -104), Condition.DownedQueenSlime
                , DashBows.FullMoon, CollectImage.LockIconType.Small);
            NewImage<PlasmaBow>(center + new Vector2(-208, 254), Condition.DownedMechBossAll
                , DashBows.PlasmaBow, CollectImage.LockIconType.Small);
            NewImage<HorizonArc>(center + new Vector2(-160, -52), Condition.DownedPlantera
                , DashBows.HorizonArc, CollectImage.LockIconType.Small);
            NewImage<SeismicWave>(center + new Vector2(244, 4), Condition.DownedMartians
                , DashBows.SeismicWave, CollectImage.LockIconType.Small);
            NewImage<ReverseFlash>(center + new Vector2(92, -40), Condition.DownedNebulaPillar
                , DashBows.ReversedFlash, CollectImage.LockIconType.Small);
            NewImage<Glaciate>(center + new Vector2(92, 58), Condition.DownedNebulaPillar
                , DashBows.Glaciate, CollectImage.LockIconType.Small);
            NewImage<Solunar>(center + new Vector2(-100, 208), Condition.DownedMoonLord
                , DashBows.Solunar);
            NewImage<Aurora>(center + new Vector2(108, 180), Condition.DownedMoonLord
                , DashBows.Aurora);

            NewImage<Thyphion>(center+new Vector2(-57,2), CoraliteConditions.DownedNightmarePlantera
                , DashBows.Thyphion, CollectImage.LockIconType.Big, 1.75f);

            //var button = new CollectButton(LandOfTheLustrousCollectButton, LandOfTheLustrousCollectButtonLight
            //    , new Vector2(0, -10), ItemType<LandOfTheLustrousRelic>(), Unlocks, CoraliteNoteSystem.RewardType.LandOfTheLustrous);
            //button.ItemPosOffset = new Vector2(0, -10);
            //button.SetCenter(new Vector2(PageWidth / 2, 196));
            //Append(button);
        }

        public void NewImage<T>(Vector2 pos, Condition condition, DashBows type
            , CollectImage.LockIconType lockType = CollectImage.LockIconType.Middle, float scale = 1f) where T : ModItem
        {
            var button = new CollectImage(ItemType<T>(), condition, Unlocks, (int)type, scale, lockType);
            button.SetCenter(pos);
            Append(button);
        }

        public static void Unlock(DashBows type)
            => Unlocks[(int)type] = true;

        public static void Save(TagCompound tag)
        {
            tag.SaveBools(Unlocks, "DashBowUnlock");
        }

        public static void Load(TagCompound tag)
        {
            tag.LoadBools(Unlocks, "DashBowUnlock");
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Test.Value.QuickCenteredDraw(spriteBatch, Center);

            DrawCollectTip(spriteBatch, Unlocks);

            Vector2 pos = PageTop + new Vector2(0, 132);
            DrawCollectText(spriteBatch, Unlocks, pos + new Vector2(7, -15));
            DrawCollectProgress(spriteBatch, Unlocks, pos + new Vector2(0, 15));
        }
    }
}
