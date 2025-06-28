using static Coralite.Core.AssetDirectory;

namespace Coralite.Core
{
    public partial class CoraliteAssets
    {
        [AutoLoadTexture(NoteReadfragment)]
        public class ReadFragmant
        {
            public static ATex BookName { get; private set; }
        }

        [AutoLoadTexture(NoteMagikeS1)]
        public class MagikeChapter1
        {
            public static ATex KnowledgeCheckButton { get; private set; }

            public static ATex WhatIsMagike { get; private set; }

            public static ATex PlaceFirstLens { get; private set; }
            public static ATex PlacePolarizedFilterArrow { get; private set; }
            public static ATex PlacePolarizedFilter { get; private set; }
            public static ATex UIDescription { get; private set; }
            public static ATex TurnToMagikeProducerUI { get; private set; }
            public static ATex ItemWithMagike { get; private set; }
            public static ATex StoneMakerExample { get; private set; }
            public static ATex CrystalRobot { get; private set; }
            public static ATex ConnectStaff1 { get; private set; }
            public static ATex ConnectStaff2 { get; private set; }
            public static ATex Working { get; private set; }
            public static ATex HarvestStone { get; private set; }
            public static ATex CraftAltarUI { get; private set; }
            public static ATex ConnectToPedestal { get; private set; }
            public static ATex CheckAltar { get; private set; }
            public static ATex AwkwardTime { get; private set; }
            public static ATex Refractors { get; private set; }
            public static ATex Columns { get; private set; }
            public static ATex BigColumn { get; private set; }
            public static ATex PrismLevels { get; private set; }
        }

        [AutoLoadTexture(NoteRedJade)]
        public class RedJade
        {
            public static ATex Rediancie { get; private set; }
        }

        [AutoLoadTexture(NoteIceDragon1)]
        public class IceDragon1
        {
            public static ATex BabyIceDragon { get; private set; }
        }

        [AutoLoadTexture(NoteSlime1)]
        public class Slime1
        {
            public static ATex SlimeEmperor { get; private set; }
        }

        [AutoLoadTexture(NoteThunder1)]
        public class Thunder1
        {
            public static ATex ThunderveidDragon { get; private set; }
        }
    }
}
