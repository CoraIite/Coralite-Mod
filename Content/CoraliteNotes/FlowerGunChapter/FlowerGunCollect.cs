using Coralite.Content.Items.FlyingShields;
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
    [AutoLoadTexture(Path = AssetDirectory.NoteWeapons)]
    public class FlowerGunCollect : CollectionPage
    {
        public static bool[] Unlocks = new bool[(int)KeyFlowerGuns.Count];

        public static ATex Test { get; private set; }

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
            const float TwoPiOver3 = MathHelper.TwoPi / 3;
            const float lengthReduce = 33;
            const float rotAdd = MathHelper.TwoPi / 12 + 0.1f;

            float rot = -MathHelper.PiOver2;
            float length = 265;

            Vector2 center = new Vector2(PageWidth / 2, PageHeight / 2);

            NewImage<Wisteria>(center + rot.ToRotationVector2() * length, null
                , KeyFlowerGuns.Wisteria, CollectImage.LockIconType.Small, 1.1f);
            NewImage<SunflowerGun>(center + (rot + TwoPiOver3).ToRotationVector2() * length, null
                , KeyFlowerGuns.SunflowerGun, CollectImage.LockIconType.Small, 1.1f);
            NewImage<Floette>(center + (rot + TwoPiOver3 * 2).ToRotationVector2() * length, Condition.DownedEyeOfCthulhu
                , KeyFlowerGuns.Floette, CollectImage.LockIconType.Small, 1.1f);

            rot += rotAdd;
            length -= lengthReduce;
            NewImage<Arethusa>(center + rot.ToRotationVector2() * length, Condition.DownedSkeletron
                , KeyFlowerGuns.Wisteria);
            NewImage<Datura>(center + (rot + TwoPiOver3).ToRotationVector2() * length, Condition.Hardmode
                , KeyFlowerGuns.Datura);
            NewImage<GhostPipe>(center + (rot + TwoPiOver3 * 2).ToRotationVector2() * length, Condition.Hardmode
                , KeyFlowerGuns.GhostPipe);

            rot += rotAdd;
            length -= lengthReduce;
            NewImage<Aloe>(center + rot.ToRotationVector2() * length, Condition.DownedQueenSlime
                , KeyFlowerGuns.Aloe);
            NewImage<Rosemary>(center + (rot + TwoPiOver3).ToRotationVector2() * length, Condition.DownedMechBossAny
                , KeyFlowerGuns.Rosemary);
            NewImage<Snowdrop>(center + (rot + TwoPiOver3 * 2).ToRotationVector2() * length, Condition.DownedMechBossAll
                , KeyFlowerGuns.Snowdrop);

            rot += rotAdd;
            length -= lengthReduce;
            NewImage<ThunderDukeVine>(center + rot.ToRotationVector2() * length, CoraliteConditions.DownedThunderveinDragon
                , KeyFlowerGuns.ThunderDukeVine);
            NewImage<EternalBloom>(center + (rot + TwoPiOver3).ToRotationVector2() * length, Condition.DownedPlantera
                , KeyFlowerGuns.EternalBloom);
            NewImage<StarsBreath>(center + (rot + TwoPiOver3 * 2).ToRotationVector2() * length, Condition.DownedPlantera
                , KeyFlowerGuns.StarsBreath);

            rot += rotAdd;
            length -= lengthReduce;
            NewImage<QueenOfNight>(center + rot.ToRotationVector2() * length, Condition.DownedEmpressOfLight
                , KeyFlowerGuns.QueenOfNight);
            NewImage<Lycoris>(center + (rot + TwoPiOver3).ToRotationVector2() * length, CoraliteConditions.DownedNightmarePlantera
                , KeyFlowerGuns.Lycoris);
            //NewImage<Lycoris>(center + (rot + TwoPiOver3 * 2).ToRotationVector2() * length, null
            //    , KeyFlowerGuns.Lycoris);

            NewImage<Hyacinth>(center, CoraliteConditions.DownedNightmarePlantera
                , KeyFlowerGuns.Hyacinth, CollectImage.LockIconType.Middle, 1.75f);

            var button = new CollectButton(ModContent.ItemType<HephaesthRelic>(), Unlocks, CoraliteNoteSystem.RewardType.FlowerGun);
            button.SetCenter(new Vector2(PageWidth / 2, PageHeight - 40));
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
            Test.Value.QuickCenteredDraw(spriteBatch, Center);

            DrawCollectTip(spriteBatch, Unlocks);
            Vector2 pos = Bottom + new Vector2(0, -30);
            DrawCollectText(spriteBatch, Unlocks, pos + new Vector2(-150, 0));
            DrawCollectProgress(spriteBatch, Unlocks, pos + new Vector2(150, 0));
        }
    }
}
