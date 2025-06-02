using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.CoraliteNotes.LandOfTheLustrousChapter
{
    [AutoLoadTexture(Path = AssetDirectory.NoteWeapons)]
    public class LandOfTheLustrousCollect : CollectionPage
    {
        public static bool[] Unlocks = new bool[(int)GemWeapons.Count];

        public static ATex Test { get; private set; }
        //public static ATex FlyingShieldCollectButton { get; private set; }
        //public static ATex FlyingShieldCollectButtonLight { get; private set; }

        public enum GemWeapons
        {
            PyropeCrown,
            AmethystNecklace,
            AquamarineBracelet,
            PinkDiamondRose,
            ZumurudRing,
            PearlBrooch,
            RubyScepter,
            PeridotTalisman,
            SapphireHairpin,
            TourmalineMonoclastic,
            TopazMirror,
            ZirconGrail,

            Phosphophyllite,

            LandOfTheLustrous,
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

            NewImage<PyropeCrown>(center + new Vector2(-200, 200), null
                , GemWeapons.PyropeCrown, CollectImage.LockIconType.Small,1.2f);
            NewImage<AmethystNecklace>(center + new Vector2(-200, -200), null
                , GemWeapons.AmethystNecklace, CollectImage.LockIconType.Small, 1.2f);
            NewImage<AquamarineBracelet>(center + new Vector2(-100, -200), null
                , GemWeapons.AquamarineBracelet, CollectImage.LockIconType.Small, 1.2f);
            NewImage<PinkDiamondRose>(center + new Vector2(0, -200), null
                , GemWeapons.PinkDiamondRose, CollectImage.LockIconType.Small, 1.2f);
            NewImage<ZumurudRing>(center + new Vector2(100, -200), null
                , GemWeapons.ZumurudRing, CollectImage.LockIconType.Small, 1.3f);
            NewImage<PearlBrooch>(center + new Vector2(0, -100), null
                , GemWeapons.PearlBrooch, CollectImage.LockIconType.Small, 1.2f);
            NewImage<RubyScepter>(center + new Vector2(-100, -100), null
                , GemWeapons.RubyScepter, CollectImage.LockIconType.Small, 1.2f);
            NewImage<PeridotTalisman>(center + new Vector2(-200, -100), null
                , GemWeapons.PeridotTalisman, CollectImage.LockIconType.Small, 1.2f);
            NewImage<SapphireHairpin>(center + new Vector2(200, -100), null
                , GemWeapons.SapphireHairpin, CollectImage.LockIconType.Small, 1.2f);
            NewImage<TourmalineMonoclastic>(center + new Vector2(200, 200), null
                , GemWeapons.TourmalineMonoclastic, CollectImage.LockIconType.Small);
            NewImage<TopazMirror>(center + new Vector2(0, 100), null
                , GemWeapons.TopazMirror);
            NewImage<ZirconGrail>(center + new Vector2(100, 100), null
                , GemWeapons.ZirconGrail);
            NewImage<Phosphophyllite>(center + new Vector2(200, 100), null
                , GemWeapons.Phosphophyllite, CollectImage.LockIconType.Small, 1.3f);

            NewImage<LandOfTheLustrous>(center, null
                , GemWeapons.LandOfTheLustrous, CollectImage.LockIconType.Big, 2);

            //var button = new CollectButton(FlyingShieldCollectButton, FlyingShieldCollectButtonLight
            //    , new Vector2(0, -10), ItemType<HephaesthRelic>(), Unlocks, CoraliteNoteSystem.RewardType.FlyingShield);
            //button.SetCenter(new Vector2(PageWidth / 2, PageHeight - 40));
            //Append(button);
        }

        public void NewImage<T>(Vector2 pos, Condition condition, GemWeapons type
            , CollectImage.LockIconType lockType = CollectImage.LockIconType.Middle, float scale = 1f) where T : ModItem
        {
            var button = new CollectImage(ItemType<T>(), condition, Unlocks, (int)type, scale, lockType);
            button.SetCenter(pos);
            Append(button);
        }

        public static void Unlock(GemWeapons type)
            => Unlocks[(int)type] = true;

        public static void Save(TagCompound tag)
        {
            tag.SaveBools(Unlocks, "LandOfTheLustrousUnlock");
        }

        public static void Load(TagCompound tag)
        {
            tag.LoadBools(Unlocks, "LandOfTheLustrousUnlock");
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
