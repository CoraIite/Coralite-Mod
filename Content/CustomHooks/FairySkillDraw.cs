using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using MonoMod.Cil;
using Terraria;

namespace Coralite.Content.CustomHooks
{
    public class FairySkillDraw : HookGroup
    {
        const int WidthOff = 14;
        const int HeightOff = 9;
        const int Padding = 8;

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
            if (!i.IsAir && i.ModItem is BaseFairyItem bfi && !bfi.DontShowIV)
            {
                int[] skills = bfi.GetFairySkills();
                if (skills == null || skills.Length < 1)
                    return;

                Vector2 topLeft = new Vector2(x, y);

                //获得最大尺寸，决定绘制在哪里
                float maxSkillWidth = 0;

                foreach (var skill in skills)
                {
                    FairySkill fSkill = FairyLoader.GetFairySkill(skill);
                    Vector2 size = fSkill.GetSkillTipTotalSize(Main.LocalPlayer, bfi.FairyIV, out _);
                    if (size.X > maxSkillWidth)
                        maxSkillWidth = size.X;
                }

                //原版左上角+原版尺寸+原版框右侧+间隔+技能框左侧+技能尺寸+技能框1右侧
                if (topLeft.X + vanillaSize.X + Padding + WidthOff * 2 + WidthOff + maxSkillWidth > Main.screenWidth)
                    topLeft.X -= WidthOff * 2 + Padding + maxSkillWidth;
                else
                    topLeft.X += vanillaSize.X + Padding + WidthOff * 2;

                foreach (var skill in skills)
                {
                    FairySkill fSkill = FairyLoader.GetFairySkill(skill);
                    Vector2 size = fSkill.GetSkillTipTotalSize(Main.LocalPlayer, bfi.FairyIV, out Vector2 nameSize);

                    DrawBack(topLeft, size);
                    fSkill.DrawSkillTip(topLeft, Main.LocalPlayer, bfi.FairyIV, size, nameSize);
                    topLeft.Y += size.Y;
                }
            }
        }

        private void DrawBack(Vector2 pos, Vector2 size)
        {
            Utils.DrawInvBG(Main.spriteBatch
                , new Rectangle((int)pos.X - WidthOff, (int)pos.Y - HeightOff
                , (int)size.X + WidthOff * 2, (int)size.Y + HeightOff + HeightOff / 2)
                , new Color(23, 25, 81, 255) * 0.925f);
        }
    }
}
