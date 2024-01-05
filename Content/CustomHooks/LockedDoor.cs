using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.CustomHooks
{
    public class LockedDoor : HookGroup
    {
        public override void Load()
        {
            On_WorldGen.OpenDoor += LockDoor;
        }

        private bool LockDoor(On_WorldGen.orig_OpenDoor orig, int i, int j, int direction)
        {
            Tile tile = Main.tile[i, j];
            ModTile mt = TileLoader.GetTile(tile.TileType);
            if (mt != null && mt is ILockableDoor ld && !ld.Unlock(i, j))
                return false;

            return orig.Invoke(i, j, direction);
        }
    }

    public interface ILockableDoor
    {
        bool Unlock(int i, int j);
    }
}
