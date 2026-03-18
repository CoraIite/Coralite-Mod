using Coralite.Content.Items.HyacinthSeries;
using Coralite.Content.Items.Nightmare;
using Coralite.Content.Items.Thunder;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using static Coralite.Content.CoraliteNotes.FlowerGunChapter.FlowerGunKnowledge;

namespace Coralite.Content.CoraliteNotes.FlowerGunChapter
{
    [VaultLoaden(AssetDirectory.NoteWeapons)]
    public class FlowerGunCollect : CollectionPage<FlowerGunKnowledge>
    {
        public static ATex HyacinthCollect { get; private set; }
        public static ATex FlowerGunCollectButton { get; private set; }
        public static ATex FlowerGunCollectButtonLight { get; private set; }

        public override void AddImages()
        {
            int Hyacinth = ModContent.ItemType<Hyacinth>();
            int Arethusa = ModContent.ItemType<Arethusa>();
            int Rosemary = ModContent.ItemType<Rosemary>();
            int EternalBloom = ModContent.ItemType<EternalBloom>();
            int StarsBreath = ModContent.ItemType<StarsBreath>();

            NewImage<Wisteria>(new Vector2(77, 140), null
                , KeyFlowerGuns.Wisteria, CollectImage.LockIconType.Small, 1.1f, Hyacinth);
            NewImage<SunflowerGun>(new Vector2(-192, -184), null
                , KeyFlowerGuns.SunflowerGun, CollectImage.LockIconType.Small, 1.1f, Hyacinth, Arethusa, Rosemary, StarsBreath);
            NewImage<Floette>(new Vector2(163, 224), Condition.DownedEyeOfCthulhu
                , KeyFlowerGuns.Floette, CollectImage.LockIconType.Small, 1.1f, Hyacinth, EternalBloom);

            NewImage<Arethusa>(new Vector2(132, -185), Condition.DownedSkeletron
                , KeyFlowerGuns.Arethusa, others: [ Hyacinth, Rosemary, StarsBreath]);
            NewImage<Datura>(new Vector2(-16, -215), Condition.Hardmode
                , KeyFlowerGuns.Datura, others: [ Hyacinth]);
            NewImage<GhostPipe>(new Vector2(-46, 115), Condition.Hardmode
                , KeyFlowerGuns.GhostPipe, others: [Hyacinth]);

            NewImage<Aloe>(new Vector2(170, -65), Condition.DownedQueenSlime
                , KeyFlowerGuns.Aloe, others: [Hyacinth]);
            NewImage<Rosemary>(new Vector2(82, -120), Condition.DownedMechBossAny
                , KeyFlowerGuns.Rosemary, others: [Hyacinth, StarsBreath]);
            NewImage<Snowdrop>(new Vector2(-152, -114), Condition.DownedMechBossAll
                , KeyFlowerGuns.Snowdrop, others: [Hyacinth, StarsBreath]);

            NewImage<ThunderDukeVine>(new Vector2(118, 60), CoraliteConditions.DownedThunderveinDragon, KeyFlowerGuns.ThunderDukeVine, others: [Hyacinth]);
            NewImage<EternalBloom>(new Vector2(188, 148), Condition.DownedPlantera
                , KeyFlowerGuns.EternalBloom, others: [Hyacinth]);
            NewImage<StarsBreath>(new Vector2(-178, -4), Condition.DownedPlantera
                , KeyFlowerGuns.StarsBreath, others: [Hyacinth]);

            NewImage<QueenOfNight>(new Vector2(-32, -145), Condition.DownedEmpressOfLight
                , KeyFlowerGuns.QueenOfNight, others: [Hyacinth]);
            NewImage<Lycoris>(new Vector2(-182, 98), CoraliteConditions.DownedNightmarePlantera
                , KeyFlowerGuns.Lycoris, others: [Hyacinth]);

            NewImage<Hyacinth>(new Vector2(-15, -4), CoraliteConditions.DownedNightmarePlantera
                , KeyFlowerGuns.Hyacinth, CollectImage.LockIconType.Middle, 1.75f);

            var button = new CollectButton(FlowerGunCollectButton, FlowerGunCollectButtonLight
                , new Vector2(0, -20), Knowledge);
            button.SetCenter(new Vector2(PageWidth / 2 - 105, PageHeight - 58));
            Append(button);
        }

        public void NewImage<T>(Vector2 pos, Condition condition, KeyFlowerGuns type
            , CollectImage.LockIconType lockType = CollectImage.LockIconType.Small, float scale = 1f,params int[] others) where T : ModItem
        {
            Vector2 center = new Vector2(PageWidth / 2, PageHeight / 2);

            var button = new CollectImage(ModContent.ItemType<T>(), condition, Knowledge.Collects, (int)type, scale, lockType);
            button.SetCenter(center + pos);
            if (others != null)
                button.AddOtherItems(others);

            Append(button);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            HyacinthCollect.Value.QuickCenteredDraw(spriteBatch, Center);

            DrawCollectTip(spriteBatch, Knowledge.Collects);
            Vector2 pos = Bottom + new Vector2(0, -30);
            DrawCollectText(spriteBatch, Knowledge.Collects, pos + new Vector2(160, -30));
            DrawCollectProgress(spriteBatch, Knowledge.Collects, pos + new Vector2(150, 5));
        }
    }
}
