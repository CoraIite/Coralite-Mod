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
    public class FlyingShieldCollect : CollectionPage<FlyingShieldKnowledge>
    {
        public static ATex HephaesthCollect { get; private set; }
        public static ATex FlyingShieldCollectButton { get; private set; }
        public static ATex FlyingShieldCollectButtonLight { get; private set; }

        public override void AddImages()
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
            NewImage<TrashCanLid>(new Vector2(0, y), null, SPFlyingShields.TrashCanLid, Small, 1.1f, Hephaesth);
            NewImage<MeteorFireball>(new Vector2(69, y + 20), null, SPFlyingShields.MeteorFireball, Small, 1.1f, Hephaesth, Leonids, Solleonis);
            NewImage<GlassShield>(new Vector2(-66, y + 20), null, SPFlyingShields.GlassShield, Small, 1.1f, Hephaesth, GlazeBulwark, GemrainAegis);
            NewImage<SilverAngel>(new Vector2(-200, y), Condition.DownedEowOrBoc, SPFlyingShields.SilverAngel, Small, 1.3f, Hephaesth, RoyalAngel, Noctiflair);
            NewImage<GlazeBulwark>(new Vector2(200, y), Condition.DownedEowOrBoc, SPFlyingShields.GlazeBulwark, Small, 1.3f, Hephaesth, GemrainAegis);

            y += 80;

            NewImage<GoldenSamurai>(new Vector2(200, y), Condition.DownedSkeletron
                , SPFlyingShields.GoldenSamurai, others: Hephaesth);
            NewImage<Leonids>( new Vector2(-200, y), Condition.Hardmode
                , SPFlyingShields.Leonids, Middle,1.3f, Hephaesth, Solleonis);

            y += 80;

            NewImage<GemrainAegis>( new Vector2(200, y), Condition.DownedQueenSlime
                , SPFlyingShields.GemrainAegis, others: Hephaesth);
            NewImage<TortoiseshellFortress>( new Vector2(-200, y), Condition.DownedMechBossAll
                , SPFlyingShields.TortoiseshellFortress, others: Hephaesth);

            y += 80;

            NewImage<MechRioter>( new Vector2(200, y), Condition.DownedMechBossAll
                , SPFlyingShields.MechRioter, others: Hephaesth);
            NewImage<RoyalAngel>( new Vector2(-200, y), Condition.DownedPlantera
                , SPFlyingShields.RoyalAngel, Middle,1.3f, Hephaesth, Noctiflair);

            y += 80;

            NewImage<Fishronguard>( new Vector2(160, y), Condition.DownedGolem
                , SPFlyingShields.Fishronguard, others: Hephaesth);
            NewImage<ShanHai>( new Vector2(-160, y), Condition.DownedGolem
                , SPFlyingShields.ShanHai, others: Hephaesth);

            y += 80;

            NewImage<Solleonis>( new Vector2(-100, y), Condition.DownedSolarPillar
                , SPFlyingShields.Solleonis,others: Hephaesth);
            NewImage<ConquerorOfTheSeas>( new Vector2(100, y), Condition.DownedMoonLord
                , SPFlyingShields.ConquerorOfTheSeas, others: Hephaesth);

            y += 80;

            NewImage<Noctiflair>( new Vector2(0, y), Condition.DownedMoonLord
                , SPFlyingShields.Noctiflair, others: Hephaesth);

            NewImage<Hephaesth>(Vector2.Zero, Condition.DownedMoonLord
                , SPFlyingShields.Hephaesth, Big, 3);

            var button = new CollectButton(FlyingShieldCollectButton, FlyingShieldCollectButtonLight, new Vector2(0, -10), Knowledge);
            button.SetCenter(new Vector2(PageWidth / 2, PageHeight - 40));
            Append(button);
        }

        public void NewImage<T>(Vector2 pos, Condition condition, SPFlyingShields type
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
