using Coralite.Content.Items.Icicle;
using Coralite.Content.Items.Thunder;
using Coralite.Content.Items.ThyphionSeries;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.CoraliteNotes.DashBowChapter
{
    [VaultLoaden(AssetDirectory.NoteWeapons)]
    public class DashBowCollect : CollectionPage
    {
        public static bool[] Unlocks = new bool[(int)DashBows.Count];

        public static ATex ThyphionCollect { get; private set; }
        public static ATex DashBowCollectButton { get; private set; }
        public static ATex DashBowCollectLight { get; private set; }

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
            Vector2 center = new Vector2(PageWidth / 2, PageHeight / 2 - 80);

            NewImage<Afterglow>(center + new Vector2(-240, 126), null
                , DashBows.Afterglow, CollectImage.LockIconType.Small);
            NewImage<TremblingBow>(center + new Vector2(-166, 316), null
                , DashBows.TremblingBow, CollectImage.LockIconType.Small, 0.9f);
            NewImage<FarAwaySky>(center + new Vector2(174, 302), Condition.DownedEyeOfCthulhu
                , DashBows.FarAwaySky, CollectImage.LockIconType.Small);
            NewImage<IcicleBow>(center + new Vector2(-240, -88), CoraliteConditions.DownedBabyIceDragon
                , DashBows.IcicleBow, CollectImage.LockIconType.Small);
            NewImage<Turbulence>(center + new Vector2(155, 86), Condition.DownedSkeletron
                , DashBows.Turbulence, CollectImage.LockIconType.Small);
            NewImage<RadiantSun>(center + new Vector2(-135, -190), Condition.Hardmode
                , DashBows.RadiantSun, CollectImage.LockIconType.Small);
            NewImage<FullMoon>(center + new Vector2(-60, -150), Condition.Hardmode
                , DashBows.FullMoon, CollectImage.LockIconType.Small);
            NewImage<PlasmaBow>(center + new Vector2(24, 260), Condition.DownedSkeletronPrime
                , DashBows.PlasmaBow, CollectImage.LockIconType.Small);
            NewImage<HorizonArc>(center + new Vector2(214, 212), Condition.DownedTwins
                , DashBows.HorizonArc, CollectImage.LockIconType.Small);
            NewImage<SeismicWave>(center + new Vector2(-190, 216), Condition.DownedDestroyer
                , DashBows.SeismicWave, scale: 0.85f);
            NewImage<ReverseFlash>(center + new Vector2(6, 370), CoraliteConditions.DownedThunderveinDragon
                , DashBows.ReversedFlash, CollectImage.LockIconType.Small, 1.1f);
            NewImage<Solunar>(center + new Vector2(28, -100), Condition.DownedPlantera
                , DashBows.Solunar, scale: 0.9f);
            NewImage<Glaciate>(center + new Vector2(-144, -32), Condition.DownedIceQueen
                , DashBows.Glaciate);
            NewImage<Aurora>(center + new Vector2(126, -26), Condition.DownedMoonLord
                , DashBows.Aurora);

            var b = new CollectImage(ItemType<Thyphion>(), CoraliteConditions.DownedNightmarePlantera
                , Unlocks, (int)DashBows.Thyphion, 1.75f, CollectImage.LockIconType.Big);
            b.SetCenter(center + new Vector2(-2, 103));
            b.LockIconScale = 1.5f;
            Append(b);

            var button2 = new CollectButton(DashBowCollectButton, DashBowCollectLight
                , new Vector2(0, 0), ItemType<ThyphionRelic>(), Unlocks, CoraliteNoteSystem.RewardType.DashBow);
            button2.ItemPosOffset = new Vector2(0, 0);
            button2.SetCenter(new Vector2(PageWidth / 2 + 158, 100));
            Append(button2);
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
            ThyphionCollect.Value.QuickCenteredDraw(spriteBatch, Center);

            DrawCollectTip(spriteBatch, Unlocks);

            DrawCollectText(spriteBatch, Unlocks, PageTop + new Vector2(-242, 24));
            DrawCollectProgress(spriteBatch, Unlocks, PageTop + new Vector2(-226, 82));
        }
    }
}
