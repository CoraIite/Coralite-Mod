using Coralite.Content.Items.FlyingShields;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using static Coralite.Content.CoraliteNotes.FlyingShieldChapter.FlyingShieldKnowledge;
using static Terraria.ModLoader.ModContent;
using static Coralite.Content.CoraliteNotes.CollectImage.LockIconType;

namespace Coralite.Content.CoraliteNotes.FlyingShieldChapter
{
    [VaultLoaden(AssetDirectory.NoteWeapons)]
    public class FlyingShieldCollect : CollectionPage
    {
        public static ATex HephaesthCollect { get; private set; }
        public static ATex FlyingShieldCollectButton { get; private set; }
        public static ATex FlyingShieldCollectButtonLight { get; private set; }

        private FlyingShieldKnowledge _knowledge;
        public FlyingShieldKnowledge Knowledge
        {
            get
            {
                _knowledge ??= (FlyingShieldKnowledge)CoraliteContent.GetKnowledge<FlyingShieldKnowledge>();
                return _knowledge;
            }
        }

        public override void Recalculate()
        {
            RemoveAllChildren();
            AddImages();

            base.Recalculate();
        }

        public void AddImages()
        {
            int y = -200;

            int GlazeBulwark = ItemType<GlazeBulwark>();
            int Hephaesth = ItemType<Hephaesth>();
            int Leonids = ItemType<Hephaesth>();
            int GemrainAegis = ItemType<GemrainAegis>();
            int RoyalAngel = ItemType<RoyalAngel>();
            int Solleonis = ItemType<Solleonis>();
            int Noctiflair = ItemType<Noctiflair>();

            //最上面一排5个
            NewImage<TrashCanLid>(new Vector2(0, y), null, KeyFlyingShields.TrashCanLid, Small, 1.1f, Hephaesth);
            NewImage<MeteorFireball>(new Vector2(69, y + 20), null, KeyFlyingShields.MeteorFireball, Small, 1.1f, Hephaesth, Leonids, Solleonis);
            NewImage<GlassShield>(new Vector2(-66, y + 20), null, KeyFlyingShields.GlassShield, Small, 1.1f, Hephaesth, GlazeBulwark, GemrainAegis);
            NewImage<SilverAngel>(new Vector2(-200, y), Condition.DownedEowOrBoc, KeyFlyingShields.SilverAngel, Small, 1.3f, Hephaesth, RoyalAngel, Noctiflair);
            NewImage<GlazeBulwark>(new Vector2(200, y), Condition.DownedEowOrBoc, KeyFlyingShields.GlazeBulwark, Small, 1.3f, Hephaesth, GemrainAegis);

            y += 80;

            NewImage<GoldenSamurai>(new Vector2(200, y), Condition.DownedSkeletron
                , KeyFlyingShields.GoldenSamurai, others: Hephaesth);
            NewImage<Leonids>( new Vector2(-200, y), Condition.Hardmode
                , KeyFlyingShields.Leonids, Middle,1.3f, Hephaesth, Solleonis);

            y += 80;

            NewImage<GemrainAegis>( new Vector2(200, y), Condition.DownedQueenSlime
                , KeyFlyingShields.GemrainAegis, others: Hephaesth);
            NewImage<TortoiseshellFortress>( new Vector2(-200, y), Condition.DownedMechBossAll
                , KeyFlyingShields.TortoiseshellFortress, others: Hephaesth);

            y += 80;

            NewImage<MechRioter>( new Vector2(200, y), Condition.DownedMechBossAll
                , KeyFlyingShields.MechRioter, others: Hephaesth);
            NewImage<RoyalAngel>( new Vector2(-200, y), Condition.DownedPlantera
                , KeyFlyingShields.RoyalAngel, Middle,1.3f, Hephaesth, Noctiflair);

            y += 80;

            NewImage<Fishronguard>( new Vector2(160, y), Condition.DownedGolem
                , KeyFlyingShields.Fishronguard, others: Hephaesth);
            NewImage<ShanHai>( new Vector2(-160, y), Condition.DownedGolem
                , KeyFlyingShields.ShanHai, others: Hephaesth);

            y += 80;

            NewImage<Solleonis>( new Vector2(-100, y), Condition.DownedSolarPillar
                , KeyFlyingShields.Solleonis,others: Hephaesth);
            NewImage<ConquerorOfTheSeas>( new Vector2(100, y), Condition.DownedMoonLord
                , KeyFlyingShields.ConquerorOfTheSeas, others: Hephaesth);

            y += 80;

            NewImage<Noctiflair>( new Vector2(0, y), Condition.DownedMoonLord
                , KeyFlyingShields.Noctiflair, others: Hephaesth);

            NewImage<Hephaesth>(Vector2.Zero, Condition.DownedMoonLord
                , KeyFlyingShields.Hephaesth, Big, 3);

            var button = new CollectButton(FlyingShieldCollectButton, FlyingShieldCollectButtonLight
                , new Vector2(0, -10), Knowledge);
            button.SetCenter(new Vector2(PageWidth / 2, PageHeight - 40));
            Append(button);
        }

        public void NewImage<T>(Vector2 pos, Condition condition, KeyFlyingShields type
            , CollectImage.LockIconType lockType = Middle, float scale = 1.3f, params int[] others) where T : ModItem
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
            HephaesthCollect.Value.QuickCenteredDraw(spriteBatch, Center);

            DrawCollectTip(spriteBatch, Knowledge.Collects);

            Vector2 pos = Bottom + new Vector2(0, -30);
            DrawCollectText(spriteBatch, Knowledge.Collects, pos + new Vector2(-150, 0));
            DrawCollectProgress(spriteBatch, Knowledge.Collects, pos + new Vector2(150, 0));
        }
    }
}
