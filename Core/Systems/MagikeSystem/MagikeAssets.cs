namespace Coralite.Core.Systems.MagikeSystem
{
    [VaultLoaden(AssetDirectory.MagikeUI)]
    public class MagikeAssets
    {
        [VaultLoaden(AssetDirectory.Misc)]
        public static ATex SelectFrame { get; private set; }

        public static ATex MagikeContainerBar { get; private set; }
        public static ATex CraftArrow { get; private set; }
        public static ATex FilterRemoveButton { get; private set; }

        public static ATex CraftSelectButton { get; private set; }
        public static ATex CraftAutoSelectButton { get; private set; }
        public static ATex CraftShowButton { get; private set; }
        public static ATex CraftItemSpawnButton { get; private set; }
        public static ATex CraftShowRecipeButton { get; private set; }
        public static ATex ChargerItemButton { get; private set; }
        public static ATex ChargerPlayerButton { get; private set; }
        public static ATex CanCraftShow { get; private set; }

        [VaultLoaden(AssetDirectory.UI)]
        public static ATex AlphaBar { get; private set; }
        public static ATex CraftMagikeBar { get; private set; }

        [VaultLoaden(AssetDirectory.Misc)]
        public static ATex Mabird { get; private set; }

        public static ATex MabirdStateButton { get; private set; }
        public static ATex MabirdSlot { get; private set; }
        public static ATex RouteDrawButton { get; private set; }
        public static ATex CopyRouteButton { get; private set; }
        public static ATex PasteRouteButton { get; private set; }
    }
}
