using Coralite.Content.Items.Gels;
using Coralite.Content.ModPlayers;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;

namespace Coralite.Content.CustomHooks
{
    public class ModifyPlayerShadow : HookGroup
    {
        public override void Load()
        {
            IL_LegacyPlayerRenderer.DrawPlayerFull += IL_LegacyPlayerRenderer_DrawPlayerFull;
        }

        private void IL_LegacyPlayerRenderer_DrawPlayerFull(ILContext il)
        {
            ILCursor cursor = new(il);
            cursor.TryGotoNext(
                 i => i.MatchLdarg(2)
                , i => i.MatchLdfld<Vector2>("position")
                , i => i.MatchStloc(4));

            //cursor.Index += 3;

            cursor.Emit(OpCodes.Ldarg_1);//拿一下camera
            cursor.Emit(OpCodes.Ldarg_2);//拿一下player
            cursor.EmitDelegate(SpecialPlayerShadowDraw);//调用绘制函数
        }

        private void SpecialPlayerShadowDraw(Camera camera, Player drawPlayer)
        {
            if (!drawPlayer.TryGetModPlayer(out CoralitePlayer cp))
                return;

            if (cp.EmperorDefence > 0 && cp.HasEffect(EmperorSlimeBoots.DefenceSet))
            {
                cp.SlimeDraw = true;
                for (int i = 0; i < 4; i++)
                {
                    Vector2 offset = (i * MathHelper.PiOver2 + 3 * Main.GlobalTimeWrappedHourly).ToRotationVector2();
                    offset *= 2 + Math.Clamp(4 * cp.EmperorDefence / (float)CoralitePlayer.EmperorDefenctMax, 1, 4);
                    Main.PlayerRenderer.DrawPlayer(camera, drawPlayer, drawPlayer.position + offset + new Vector2(0, drawPlayer.gfxOffY)
                        , drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, 0.8f);
                }

                cp.SlimeDraw = false;
            }
        }

        public override void Unload()
        {
            IL_LegacyPlayerRenderer.DrawPlayerFull -= IL_LegacyPlayerRenderer_DrawPlayerFull;
        }
    }
}
