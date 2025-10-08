using Coralite.Content.Items.FlyingShields;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.CoraliteNotes.FlyingShieldChapter
{
    [VaultLoaden(AssetDirectory.NoteWeapons)]
    public class FlyingShieldCollect : CollectionPage
    {
        public static bool[] Unlocks = new bool[(int)KeyFlyingShields.Count];

        public static ATex HephaesthCollect { get; private set; }
        public static ATex FlyingShieldCollectButton { get; private set; }
        public static ATex FlyingShieldCollectButtonLight { get; private set; }

        public enum KeyFlyingShields
        {
            TrashCanLid,
            GlassShield,
            MeteorFireball,
            GlazeBulwark,
            GemrainAegis,
            GoldenSamurai,
            TortoiseshellFortress,
            MechRioter,
            Fishronguard,
            ShanHai,
            Leonids,
            Solleonis,
            ConquerorOfTheSeas,
            SilverAngel,
            RoyalAngel,
            Noctiflair,
            Hephaesth,
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
            int y = -200;
            Vector2 center = new Vector2(PageWidth / 2, PageHeight / 2 - 80);
            //最上面一排5个
            NewImage<TrashCanLid>(center + new Vector2(0, y), null
                , KeyFlyingShields.TrashCanLid, CollectImage.LockIconType.Small, 1.1f);
            NewImage<MeteorFireball>(center + new Vector2(69, y + 20), null
                , KeyFlyingShields.MeteorFireball, CollectImage.LockIconType.Small, 1.1f);
            NewImage<GlassShield>(center + new Vector2(-66, y + 20), null
                , KeyFlyingShields.GlassShield, CollectImage.LockIconType.Small, 1.1f);
            NewImage<SilverAngel>(center + new Vector2(-200, y), Condition.DownedEowOrBoc
                , KeyFlyingShields.SilverAngel, CollectImage.LockIconType.Small);
            NewImage<GlazeBulwark>(center + new Vector2(200, y), Condition.DownedEowOrBoc
                , KeyFlyingShields.GlazeBulwark, CollectImage.LockIconType.Small);

            y += 80;
            NewImage<GoldenSamurai>(center + new Vector2(200, y), Condition.DownedSkeletron
                , KeyFlyingShields.GoldenSamurai);
            NewImage<Leonids>(center + new Vector2(-200, y), Condition.Hardmode
                , KeyFlyingShields.Leonids);

            y += 80;
            NewImage<GemrainAegis>(center + new Vector2(200, y), Condition.DownedQueenSlime
                , KeyFlyingShields.GemrainAegis);
            NewImage<TortoiseshellFortress>(center + new Vector2(-200, y), Condition.DownedMechBossAll
                , KeyFlyingShields.TortoiseshellFortress);

            y += 80;
            NewImage<MechRioter>(center + new Vector2(200, y), Condition.DownedMechBossAll
                , KeyFlyingShields.MechRioter);
            NewImage<RoyalAngel>(center + new Vector2(-200, y), Condition.DownedPlantera
                , KeyFlyingShields.RoyalAngel);

            y += 80;
            NewImage<Fishronguard>(center + new Vector2(160, y), Condition.DownedGolem
                , KeyFlyingShields.Fishronguard);
            NewImage<ShanHai>(center + new Vector2(-160, y), Condition.DownedGolem
                , KeyFlyingShields.ShanHai);

            y += 80;
            NewImage<Solleonis>(center + new Vector2(-100, y), Condition.DownedSolarPillar
                , KeyFlyingShields.Solleonis);
            NewImage<ConquerorOfTheSeas>(center + new Vector2(100, y), Condition.DownedMoonLord
                , KeyFlyingShields.ConquerorOfTheSeas);

            y += 80;
            NewImage<Noctiflair>(center + new Vector2(0, y), Condition.DownedMoonLord
                , KeyFlyingShields.Noctiflair);

            NewImage<Hephaesth>(center, Condition.DownedMoonLord
                , KeyFlyingShields.Hephaesth, CollectImage.LockIconType.Big, 3);

            var button = new CollectButton(FlyingShieldCollectButton, FlyingShieldCollectButtonLight
                , new Vector2(0, -10), ItemType<HephaesthRelic>(), Unlocks, CoraliteNoteSystem.RewardType.FlyingShield);
            button.SetCenter(new Vector2(PageWidth / 2, PageHeight - 40));
            Append(button);
        }

        public void NewImage<T>(Vector2 pos, Condition condition, KeyFlyingShields type
            , CollectImage.LockIconType lockType = CollectImage.LockIconType.Middle, float scale = 1.3f) where T : ModItem
        {
            var button = new CollectImage(ItemType<T>(), condition, Unlocks, (int)type, scale, lockType);
            button.SetCenter(pos);
            Append(button);
        }

        public static void Unlock(KeyFlyingShields type)
            => Unlocks[(int)type] = true;

        public static void Save(TagCompound tag)
        {
            tag.SaveBools(Unlocks, "FlyingShieldUnlock");
        }

        public static void Load(TagCompound tag)
        {
            tag.LoadBools(Unlocks, "FlyingShieldUnlock");
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            HephaesthCollect.Value.QuickCenteredDraw(spriteBatch, Center);

            DrawCollectTip(spriteBatch, Unlocks);

            Vector2 pos = Bottom + new Vector2(0, -30);
            DrawCollectText(spriteBatch, Unlocks, pos + new Vector2(-150, 0));
            DrawCollectProgress(spriteBatch, Unlocks, pos + new Vector2(150, 0));
        }
    }
}
