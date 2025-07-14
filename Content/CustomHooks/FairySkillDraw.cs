using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using MonoMod.Cil;
using Terraria;

namespace Coralite.Content.CustomHooks
{
    public class FairySkillDraw : HookGroup
    {
        public override void Load()
        {
            IL_Main.MouseText_DrawItemTooltip += IL_Main_MouseText_DrawItemTooltip;
        }

        public override void Unload()
        {
            IL_Main.MouseText_DrawItemTooltip -= IL_Main_MouseText_DrawItemTooltip;
        }

        private void IL_Main_MouseText_DrawItemTooltip(ILContext il)
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
                int[] skills = bfi.GetFairySkills();
                if (skills == null || skills.Length < 1)
                    return;

                //获得最大尺寸，决定绘制在哪里
                bool drawLeft = false;

                Vector2 topLeft = new Vector2(x, y);

                foreach ( var skill in skills )
                {

                }
            }
        }

        private void DrawBack(Vector2 pos,Vector2 size)
        {
            int widthoff = 14;
            int heightoff = 9;
            Utils.DrawInvBG(Main.spriteBatch
                , new Rectangle((int)pos.X - widthoff, (int)pos.Y - heightoff
                , (int)size.X + widthoff * 2, (int)size.Y + heightoff + heightoff / 2)
                , new Color(23, 25, 81, 255) * 0.925f);
        }
    }
}
