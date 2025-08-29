using Coralite.Content.Items.HyacinthSeries;
using Coralite.Content.Items.Nightmare;
using Coralite.Content.Items.Thunder;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Content.CoraliteNotes.FlowerGunChapter
{
    [VaultLoaden(AssetDirectory.NoteWeapons)]
    public class FlowerGunCollect : CollectionPage
    {
        public static bool[] Unlocks = new bool[(int)KeyFlowerGuns.Count];

        public static ATex HyacinthCollect { get; private set; }

        public static ATex FlowerGunCollectButton { get; private set; }
        public static ATex FlowerGunCollectButtonLight { get; private set; }

        public enum KeyFlowerGuns
        {
            Wisteria,
            SunflowerGun,
            Floette,
            Arethusa,
            Datura,
            GhostPipe,
            Aloe,
            Rosemary,
            Snowdrop,
            ThunderDukeVine,
            EternalBloom,
            StarsBreath,
            QueenOfNight,
            Lycoris,
            Hyacinth,

            Count,
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
        }

        public override void Recalculate()
        {
            RemoveAllChildren();
            AddImages();

            base.Recalculate();
        }

        public void AddImages()
        {
            Vector2 center = new Vector2(PageWidth / 2, PageHeight / 2);

            NewImage<Wisteria>(center + new Vector2(77, 140), null
                , KeyFlowerGuns.Wisteria, CollectImage.LockIconType.Small, 1.1f);
            NewImage<SunflowerGun>(center + new Vector2(-192, -184), null
                , KeyFlowerGuns.SunflowerGun, CollectImage.LockIconType.Small, 1.1f);
            NewImage<Floette>(center + new Vector2(163, 224), Condition.DownedEyeOfCthulhu
                , KeyFlowerGuns.Floette, CollectImage.LockIconType.Small, 1.1f);

            NewImage<Arethusa>(center + new Vector2(132, -185), Condition.DownedSkeletron
                , KeyFlowerGuns.Arethusa);
            NewImage<Datura>(center + new Vector2(-16, -215), Condition.Hardmode
                , KeyFlowerGuns.Datura);
            NewImage<GhostPipe>(center + new Vector2(-46, 115), Condition.Hardmode
                , KeyFlowerGuns.GhostPipe);

            NewImage<Aloe>(center + new Vector2(170, -65), Condition.DownedQueenSlime
                , KeyFlowerGuns.Aloe);
            NewImage<Rosemary>(center + new Vector2(82, -120), Condition.DownedMechBossAny
                , KeyFlowerGuns.Rosemary);
            NewImage<Snowdrop>(center + new Vector2(-152, -114), Condition.DownedMechBossAll
                , KeyFlowerGuns.Snowdrop);

            NewImage<ThunderDukeVine>(center + new Vector2(118, 60), CoraliteConditions.DownedThunderveinDragon
                , KeyFlowerGuns.ThunderDukeVine);
            NewImage<EternalBloom>(center + new Vector2(188, 148), Condition.DownedPlantera
                , KeyFlowerGuns.EternalBloom);
            NewImage<StarsBreath>(center + new Vector2(-178, -4), Condition.DownedPlantera
                , KeyFlowerGuns.StarsBreath);

            NewImage<QueenOfNight>(center + new Vector2(-32, -145), Condition.DownedEmpressOfLight
                , KeyFlowerGuns.QueenOfNight);
            NewImage<Lycoris>(center + new Vector2(-182, 98), CoraliteConditions.DownedNightmarePlantera
                , KeyFlowerGuns.Lycoris);
            //NewImage<Lycoris>(center + (rot + TwoPiOver3 * 2).ToRotationVector2() * length, null
            //    , KeyFlowerGuns.Lycoris);

            NewImage<Hyacinth>(center + new Vector2(-15, -4), CoraliteConditions.DownedNightmarePlantera
                , KeyFlowerGuns.Hyacinth, CollectImage.LockIconType.Middle, 1.75f);

            var button = new CollectButton(FlowerGunCollectButton, FlowerGunCollectButtonLight
                , new Vector2(0, -20), ModContent.ItemType<HyacinthRelic>(), Unlocks, CoraliteNoteSystem.RewardType.FlowerGun);
            button.SetCenter(new Vector2(PageWidth / 2 - 105, PageHeight - 58));
            Append(button);
        }

        public void NewImage<T>(Vector2 pos, Condition condition, KeyFlowerGuns type
            , CollectImage.LockIconType lockType = CollectImage.LockIconType.Small, float scale = 1f) where T : ModItem
        {
            var button = new CollectImage(ModContent.ItemType<T>(), condition, Unlocks, (int)type, scale, lockType);
            button.SetCenter(pos);
            Append(button);
        }

        public static void Unlock(KeyFlowerGuns type)
            => Unlocks[(int)type] = true;

        public static void Save(TagCompound tag)
        {
            tag.SaveBools(Unlocks, "FlowerGunUnlock");
        }

        public static void Load(TagCompound tag)
        {
            tag.LoadBools(Unlocks, "FlowerGunUnlock");
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            HyacinthCollect.Value.QuickCenteredDraw(spriteBatch, Center);

            DrawCollectTip(spriteBatch, Unlocks);
            Vector2 pos = Bottom + new Vector2(0, -30);
            DrawCollectText(spriteBatch, Unlocks, pos + new Vector2(160, -30));
            DrawCollectProgress(spriteBatch, Unlocks, pos + new Vector2(150, 5));
        }
    }
}
