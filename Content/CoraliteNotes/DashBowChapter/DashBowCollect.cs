using Coralite.Content.Items.Icicle;
using Coralite.Content.Items.Thunder;
using Coralite.Content.Items.ThyphionSeries;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using static Coralite.Content.CoraliteNotes.DashBowChapter.DashBowKnowledge;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.CoraliteNotes.DashBowChapter
{
    [VaultLoaden(AssetDirectory.NoteWeapons)]
    public class DashBowCollect : CollectionPage<DashBowKnowledge>
    {
        public static ATex ThyphionCollect { get; private set; }
        public static ATex DashBowCollectButton { get; private set; }
        public static ATex DashBowCollectLight { get; private set; }

        public override void AddImages()
        {
            int Thyphion = ItemType<Thyphion>();
            int RadiantSun = ItemType<RadiantSun>();
            int PlasmaBow = ItemType<PlasmaBow>();
            int HorizonArc = ItemType<HorizonArc>();
            int Solunar = ItemType<Solunar>();
            int Glaciate = ItemType<Glaciate>();

            NewImage<Afterglow>(new Vector2(-240, 126), null
                , DashBows.Afterglow, CollectImage.LockIconType.Small, others: [Thyphion, RadiantSun, Solunar]);
            NewImage<TremblingBow>(new Vector2(-166, 316), null
                , DashBows.TremblingBow, CollectImage.LockIconType.Small, 0.9f, Thyphion, PlasmaBow);
            NewImage<FarAwaySky>(new Vector2(174, 302), Condition.DownedEyeOfCthulhu
                , DashBows.FarAwaySky, CollectImage.LockIconType.Small, others: [Thyphion, HorizonArc]);
            NewImage<IcicleBow>(new Vector2(-240, -88), CoraliteConditions.DownedBabyIceDragon
                , DashBows.IcicleBow, CollectImage.LockIconType.Small, others: [Thyphion, Glaciate]);
            NewImage<Turbulence>(new Vector2(155, 86), Condition.DownedSkeletron
                , DashBows.Turbulence, CollectImage.LockIconType.Small, others: Thyphion);
            NewImage<RadiantSun>(new Vector2(-135, -190), Condition.Hardmode
                , DashBows.RadiantSun, CollectImage.LockIconType.Small, others: [Thyphion, Solunar]);
            NewImage<FullMoon>(new Vector2(-60, -150), Condition.Hardmode
                , DashBows.FullMoon, CollectImage.LockIconType.Small, others: [Thyphion, Solunar]);
            NewImage<PlasmaBow>(new Vector2(24, 260), Condition.DownedSkeletronPrime
                , DashBows.PlasmaBow, CollectImage.LockIconType.Small, others: Thyphion);
            NewImage<HorizonArc>(new Vector2(214, 212), Condition.DownedTwins
                , DashBows.HorizonArc, CollectImage.LockIconType.Small, others: Thyphion);
            NewImage<SeismicWave>(new Vector2(-190, 216), Condition.DownedDestroyer
                , DashBows.SeismicWave, scale: 0.85f, others: Thyphion);
            NewImage<ReverseFlash>(new Vector2(6, 370), CoraliteConditions.DownedThunderveinDragon
                , DashBows.ReversedFlash, CollectImage.LockIconType.Small, 1.1f, others: Thyphion);
            NewImage<Solunar>(new Vector2(28, -100), Condition.DownedPlantera
                , DashBows.Solunar, scale: 0.9f, others: Thyphion);
            NewImage<Glaciate>(new Vector2(-144, -32), Condition.DownedIceQueen
                , DashBows.Glaciate, others: Thyphion);
            NewImage<Aurora>(new Vector2(126, -26), Condition.DownedMoonLord
                , DashBows.Aurora, others: Thyphion);

            var b = new CollectImage(ItemType<Thyphion>(), CoraliteConditions.DownedNightmarePlantera, Knowledge.Collects, (int)DashBows.Thyphion, 1.75f, CollectImage.LockIconType.Big);
            b.SetCenter(new Vector2(PageWidth / 2, PageHeight / 2 - 80) + new Vector2(-2, 103));
            b.LockIconScale = 1.5f;
            Append(b);

            var button2 = new CollectButton(DashBowCollectButton, DashBowCollectLight
                , new Vector2(0, 0), Knowledge)
            {
                ItemPosOffset = new Vector2(0, 0)
            };
            button2.SetCenter(new Vector2(PageWidth / 2 + 158, 100));
            Append(button2);
        }

        public void NewImage<T>(Vector2 pos, Condition condition, DashBows type
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
            ThyphionCollect.Value.QuickCenteredDraw(spriteBatch, Center);

            DrawCollectTip(spriteBatch, Knowledge.Collects);

            DrawCollectText(spriteBatch, Knowledge.Collects, PageTop + new Vector2(-242, 24));
            DrawCollectProgress(spriteBatch, Knowledge.Collects, PageTop + new Vector2(-226, 82));
        }
    }
}
