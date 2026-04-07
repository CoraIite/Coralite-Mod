using Coralite.Content.CoraliteNotes;
using Coralite.Content.ModPlayers;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Core.Systems.KeySystem;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent;

namespace Coralite.Content.CustomHooks
{
    public class SpecialMouseTextDraw : HookGroup
    {
        public override SafetyLevel Safety => SafetyLevel.Severe;

        const int WidthOff = 14;
        const int HeightOff = 9;
        const int Padding = 8;

        public int ChannelTimer;

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
            cursor.EmitDelegate(DrawSpecialTips);//调用绘制函数
        }

        public void DrawSpecialTips(Vector2 vanillaSize, int x, int y)
        {
            Item i = Main.HoverItem;
            if (i.IsAir)
                return;

            DrawFaiyrText(vanillaSize, x, y, i);
            DrawCoraliteNote(vanillaSize, x, y, i);
        }

        private void DrawFaiyrText(Vector2 vanillaSize, int x, int y, Item i)
        {
            if (i.ModItem is BaseFairyItem bfi && !bfi.DontShowIV)
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
                    FairySkill.TipSize size = fSkill.GetSkillTipTotalSize(Main.LocalPlayer, bfi.FairyIV);
                    if (size.totalSize.X > maxSkillWidth)
                        maxSkillWidth = size.totalSize.X;
                }

                //原版左上角+原版尺寸+原版框右侧+间隔+技能框左侧+技能尺寸+技能框1右侧
                if (topLeft.X + vanillaSize.X + Padding + WidthOff * 2 + WidthOff + maxSkillWidth > Main.screenWidth)
                    topLeft.X -= WidthOff * 2 + Padding + maxSkillWidth;
                else
                    topLeft.X += vanillaSize.X + Padding + WidthOff * 2;

                foreach (var skill in skills)
                {
                    FairySkill fSkill = FairyLoader.GetFairySkill(skill);
                    FairySkill.TipSize size = fSkill.GetSkillTipTotalSize(Main.LocalPlayer, bfi.FairyIV);

                    DrawBack(topLeft, size.totalSize);
                    fSkill.DrawSkillTip(topLeft, Main.LocalPlayer, bfi.FairyIV, size);
                    topLeft.Y += size.totalSize.Y + Padding / 3 + HeightOff * 2;
                }
            }
        }

        public void DrawCoraliteNote(Vector2 vanillaSize, int x, int y, Item i)
        {
            if (Main.LocalPlayer.TryGetModPlayer(out CoralitePlayer cp) && cp.HasEffect(nameof(CoraliteNote))&&KnowledgeSystem.CanConsultInCoraliteNote(i))
            {
                //长按增加
                const float MaxTime = 50;
                if (Core.Loaders.KeybindLoader.ConsultInCoraliteNote.Current)
                {
                    ChannelTimer++;
                }
                else
                {
                    ChannelTimer = 0;
                }

                if (ChannelTimer>MaxTime)
                {
                    ChannelTimer = 0;
                    KnowledgeSystem.ConsultInCoraliteNote(i);
                }

                float f = ChannelTimer / MaxTime;
                Texture2D tex = TextureAssets.Item[ModContent.ItemType<CoraliteNote>()].Value;
                Vector2 pos = new Vector2(x+30, y - 6);
                Vector2 origin = new Vector2(tex.Width / 2, tex.Height);

                Utils.DrawBorderString(Main.spriteBatch, Core.Loaders.KeybindLoader.ConsultInCoraliteNote.GetAssignedKeys()[0], pos, Color.White, 0.75f, 0.5f, 1);
                pos.Y -= 24;

                Main.spriteBatch.Draw(tex, pos, null, Color.White * 0.5f, 0, origin, 1, 0, 0);
                Main.spriteBatch.Draw(tex, pos+new Vector2(0, tex.Height * (1 - f)), new Rectangle(0, (int)(tex.Height * (1 - f)), tex.Width, (int)(tex.Height * f)), Color.White, 0, origin, 1, 0, 0);
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
