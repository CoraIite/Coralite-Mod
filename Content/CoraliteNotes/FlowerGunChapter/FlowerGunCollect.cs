using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.HyacinthSeries;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader.IO;
using static Coralite.Content.Items.HyacinthSeries.HyacinthBullet;

namespace Coralite.Content.CoraliteNotes.FlowerGunChapter
{
    [AutoLoadTexture(Path = AssetDirectory.NoteWeapons)]
    public class FlowerGunCollect : CollectionPage
    {
        public static bool[] Unlocks = new bool[(int)KeyFlowerGuns.Count];

        public static ATex HephaesthCollect { get; private set; }

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

            float rot = TwoPiOver3;
            float length = 350;

            Vector2 center = new Vector2(PageWidth / 2, PageHeight / 2 - 80);

            NewImage<Wisteria>(center + rot.ToRotationVector2() * length, null
                , KeyFlowerGuns.Wisteria, CollectImage.LockIconType.Small, 1.1f);
            NewImage<SunflowerGun>(center + (rot + TwoPiOver3).ToRotationVector2() * length, null
                , KeyFlowerGuns.SunflowerGun, CollectImage.LockIconType.Small, 1.1f);
            NewImage<Floette>(center + (rot + TwoPiOver3*2).ToRotationVector2() * length, Condition.DownedEyeOfCthulhu
                , KeyFlowerGuns.Floette, CollectImage.LockIconType.Small, 1.1f);

            rot += MathHelper.TwoPi / 9;
            length -= 45;
            NewImage<Arethusa>(center + rot.ToRotationVector2() * length, null
                , KeyFlowerGuns.Wisteria, CollectImage.LockIconType.Small, 1.1f);
            NewImage<SunflowerGun>(center + (rot + TwoPiOver3).ToRotationVector2() * length, CoraliteConditions.DownedRediancie
                , KeyFlowerGuns.SunflowerGun, CollectImage.LockIconType.Small, 1.1f);
            NewImage<Floette>(center + (rot + TwoPiOver3 * 2).ToRotationVector2() * length, null
                , KeyFlowerGuns.Floette, CollectImage.LockIconType.Small, 1.1f);

            NewImage<Hyacinth>(center, CoraliteConditions.DownedNightmarePlantera
                , KeyFlowerGuns.Hyacinth, CollectImage.LockIconType.Big, 3);

            var button = new CollectButton(ModContent.ItemType<HephaesthRelic>(), Unlocks, CoraliteNoteSystem.RewardType.FlyingShield);
            button.SetCenter(new Vector2(PageWidth / 2, PageHeight - 40));
            Append(button);
        }

        public void NewImage<T>(Vector2 pos, Condition condition, KeyFlowerGuns type
            , CollectImage.LockIconType lockType = CollectImage.LockIconType.Middle, float scale = 1.3f) where T : ModItem
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
            HephaesthCollect.Value.QuickCenteredDraw(spriteBatch, Center);

            Vector2 pos = Bottom + new Vector2(0, -30);
            DrawCollectText(spriteBatch, Unlocks, pos + new Vector2(-150, 0));
            DrawCollectProgress(spriteBatch, Unlocks, pos + new Vector2(150, 0));
        }
    }
}
