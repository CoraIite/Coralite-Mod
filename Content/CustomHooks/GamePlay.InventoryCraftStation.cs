using Coralite.Content.ModPlayers;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;

namespace Coralite.Content.CustomHooks
{
    public class InventoryCraftStation:HookGroup
    {
        public override void Load()
        {
            IL_Player.AdjTiles += IL_Player_AdjTiles;
        }

        public override void Unload()
        {
            IL_Player.AdjTiles -= IL_Player_AdjTiles;
        }

        private void IL_Player_AdjTiles(MonoMod.Cil.ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            cursor.TryGotoNext(
                 i => i.MatchRet()
                , i => i.MatchLdcI4(0)
                , i => i.MatchStloc(4));

            cursor.Index += 3;

            cursor.Emit(OpCodes.Ldarg_0);//拿一下自身player
            cursor.EmitDelegate(UpdateInventoryCraftStation);
        }

        public void UpdateInventoryCraftStation(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                foreach (var i in cp.inventoryCraftStations)
                {
                    i.AdjTiles(player);
                }
            }
        }
    }
}
