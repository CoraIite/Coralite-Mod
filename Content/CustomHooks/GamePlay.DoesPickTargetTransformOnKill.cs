using Coralite.Core;
using Terraria;

namespace Coralite.Content.CustomHooks
{
    public class DoesPickTargetTransformOnKill : HookGroup
    {
        public override void Load()
        {
            On_Player.DoesPickTargetTransformOnKill += On_Player_DoesPickTargetTransformOnKill;
        }

        private bool On_Player_DoesPickTargetTransformOnKill(On_Player.orig_DoesPickTargetTransformOnKill orig, Player self, HitTile hitCounter, int damage, int x, int y, int pickPower, int bufferIndex, Tile tileTarget)
        {
            if (hitCounter.AddDamage(bufferIndex, damage, updateAmount: false) >= 100 && CoraliteSets.TileSpecialMoss[tileTarget.TileType])
                return true;

            return orig.Invoke(self, hitCounter, damage, x, y, pickPower, bufferIndex, tileTarget);
        }

        public override void Unload()
        {
            On_Player.DoesPickTargetTransformOnKill -= On_Player_DoesPickTargetTransformOnKill;
        }
    }
}
