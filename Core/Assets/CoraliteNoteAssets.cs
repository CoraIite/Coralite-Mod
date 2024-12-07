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
        }
    }
}
