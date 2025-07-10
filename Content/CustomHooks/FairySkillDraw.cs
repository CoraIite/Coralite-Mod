using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;

namespace Coralite.Content.CustomHooks
{
    public class FairySkillDraw:HookGroup
    {
        public override void Load()
        {
            IL_Main.MouseText_DrawItemTooltip += IL_Main_MouseText_DrawItemTooltip;
        }

        public override void Unload()
        {
            IL_Main.MouseText_DrawItemTooltip -= IL_Main_MouseText_DrawItemTooltip;
        }

        private void IL_Main_MouseText_DrawItemTooltip(MonoMod.Cil.ILContext il)
        {
            ILCursor cursor = new(il);
            cursor.TryGotoNext(
                 i => i.MatchLdcR4(255)
                , i => i.MatchDiv()
                , i => i.MatchStloc(14)
                , i => i.MatchLdloc(0));

            cursor.Index -= 1;

            cursor.EmitLdloc(17);//拿一下原版物品描述的宽度
            cursor.EmitLdarg(4);//拿一下player
            cursor.EmitLdarg(5);//拿一下player
            cursor.EmitDelegate(DrawFairySkillTip);//调用绘制函数
        }

        public void DrawFairySkillTip(Vector2 vanillaSize, int x, int y)
        {
            Item i = Main.HoverItem;
            if (!i.IsAir && i.ModItem is BaseFairyItem bfi)
            {

            }
        }
    }
}
