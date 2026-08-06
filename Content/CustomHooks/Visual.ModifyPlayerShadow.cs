using Coralite.Content.Items.Gels;
using Coralite.Content.Items.Steel;
using Coralite.Content.ModPlayers;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.ID;

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

            //由于原版的某些意义不明的操作导致玩家绘制里会遍历弹幕然后绘制激光
            //所以暂时用这个跳过那部分激光的绘制，不然就会闪退
            bool cart = drawPlayer.UsingSuperCart;
            drawPlayer.UsingSuperCart = false;
            
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
            else if (cp.HasEffect(nameof(SteelBreastplate)))
            {
                cp.SteelDraw = true;

                for (int i = 0; i < 6; i++)
                {
                    Vector2 offset = (i * MathHelper.Pi / 3 + 2 * Main.GlobalTimeWrappedHourly).ToRotationVector2();
                    offset *= 3;
                    
                    Main.PlayerRenderer.DrawPlayer(camera, drawPlayer, drawPlayer.position + offset + new Vector2(0, drawPlayer.gfxOffY), drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, 0.8f);
                }

                cp.SteelDraw = false;
            }

            drawPlayer.UsingSuperCart = cart;
        }

        public override void Unload()
        {
            IL_LegacyPlayerRenderer.DrawPlayerFull -= IL_LegacyPlayerRenderer_DrawPlayerFull;
        }
    }
}
