using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Terraria;

namespace Coralite.Content.CustomHooks
{
    public class CheckForPlace : HookGroup
    {

        public override void Load()
        {
            On_Player.PlaceThing_Tiles_PlaceIt += On_Player_PlaceThing_Tiles_PlaceIt; ;
        }

        public override void Unload()
        {
            On_Player.PlaceThing_Tiles_PlaceIt -= On_Player_PlaceThing_Tiles_PlaceIt;
        }

        private TileObject On_Player_PlaceThing_Tiles_PlaceIt(On_Player.orig_PlaceThing_Tiles_PlaceIt orig, Player self, bool newObjectType, TileObject data, int tileToCreate)
        {
            Item item = self.inventory[self.selectedItem];
            if (item.ModItem is IMagikeContainerItemPlaceable checker)
            {
                if (checker.CanPlace(self))
                    return orig.Invoke(self, newObjectType, data, tileToCreate);

                return data;
            }

            return orig.Invoke(self, newObjectType, data, tileToCreate);
        }

    }
}
