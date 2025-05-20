using Coralite.Core.Attributes;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem
{
    [AutoLoadTexture(Path = AssetDirectory.MagikeUI)]
    public partial class MagikeItem : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => true;
        public override bool InstancePerEntity => true;

        public static int MagikeBarTimer;

        public static ATex MagikeBar { get; set; }

        /// <summary>
        /// 物品中存储的魔能最大值
        /// </summary>
        public int MagikeMax { get; set; } = -1;
        /// <summary>
        /// 物品中当前存储的魔能量
        /// </summary>
        public int Magike { get; set; }
        /// <summary>
        /// 可以发送魔能给其他物品
        /// </summary>
        public bool magikeSendable;

        /// <summary>
        /// 物品自身的魔能含量，设置了这个就能让物品被普通透镜转化成魔能
        /// </summary>
        public int magikeAmount = -1;


        public override GlobalItem Clone(Item from, Item to)
        {
            if (to.TryGetGlobalItem(out MagikeItem mItem) && from.TryGetGlobalItem(out MagikeItem fromItem))
            {
                mItem.Enchant = fromItem.enchant;
                mItem.Magike = fromItem.Magike;
            }
            return base.Clone(from, to);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            Magike = reader.ReadInt32();
        }

        public override void NetSend(Item item, BinaryWriter writer)
        {
            writer.Write(Magike);
        }

        #region 数据存储

        public override void SaveData(Item item, TagCompound tag)
        {
            //SaveEnchant(tag);

            if (MagikeMax >= 0)
            {
                tag.Add("magike", Magike);
            }
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            //LoadEnchant(tag);

            Magike = tag.GetInt("magike");
            if (MagikeMax != -1)
                Magike = Math.Clamp(Magike, 0, MagikeMax);
        }

        #endregion

        public void ReduceMagike(int amount)
        {
            Magike -= amount;
            LimitMagike();
        }

        public void AddMagike(int amount)
        {
            Magike += amount;
            LimitMagike();
        }

        public void ClearMagike()
        {
            Magike = 0;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            //if (enchant != null)
            //{
            //    for (int i = 0; i < 3; i++)
            //    {
            //        if (enchant.datas[i] != null)
            //        {
            //            TooltipLine line = new(Mod, "enchant" + i.ToString(), enchant.datas[i].Description);
            //            line.OverrideColor = GetColor(enchant.datas[i].level);
            //            tooltips.Add(line);
            //        }
            //    }
            //}

            if (MagikeSystem.MagikeCraftRecipesFrozen.ContainsKey(item.type))
                tooltips.Add(new TooltipLine(Mod, "CanMagikeCraft", MagikeSystem.CanMagikeCraft.Value));

            if (magikeAmount > 0)
            {
                string magikeAmount = MagikeSystem.ItemMagikeAmount.Format(this.magikeAmount);
                TooltipLine line = new(Mod, "MagikeAmount", magikeAmount);
                if (this.magikeAmount < 440)
                    line.OverrideColor = Coralite.MagicCrystalPink;
                else if (this.magikeAmount < 4900)
                    line.OverrideColor = Coralite.CrystallinePurple;
                else if (this.magikeAmount < 50_0000)
                    line.OverrideColor = Coralite.SplendorMagicoreLightBlue;
                else
                    line.OverrideColor = Color.Orange;

                tooltips.Add(line);
            }

            if (MagikeMax >= 0)
            {
                //if (Main.keyState.PressingShift())//显示条
                //{
                //    if (MagikeBarTimer < 20)
                //        MagikeBarTimer++;
                //}
                //else if (MagikeBarTimer > 0)
                //    MagikeBarTimer--;

                //if (MagikeBarTimer > 0)
                //{
                TooltipLine line = new(Mod, "MagikeBar", ".                        ");
                tooltips.Add(line);
                //}
                //else
                //{
                //    TooltipLine line = new(Mod, "magikeFactory", $"{MagikeSystem.ItemContainsMagike.Value}：{Magike} / {MagikeMax}");
                //    tooltips.Add(line);
                //    line = new(Mod, "PressShift", MagikeSystem.PressShiftToShowMore.Value);
                //    tooltips.Add(line);
                //}
            }
        }

        public override void PostDrawTooltipLine(Item item, DrawableTooltipLine line)
        {
            if (MagikeMax >= 0/* && MagikeBarTimer > 0*/ && line.Name == "MagikeBar")
            {
                Texture2D barTex = MagikeBar.Value;

                float scale = 1.5f;

                Rectangle frameBox = barTex.Frame(1, 2, 0, 1);
                Vector2 pos = new(line.OriginalX, line.OriginalY + frameBox.Y / 2);

                //float factor = Helper.BezierEase(MagikeBarTimer, 20);
                //绘制底部条
                //frameBox.Width = (int)(frameBox.Width * factor);
                Main.spriteBatch.Draw(barTex, pos, frameBox, Color.White, 0, new Vector2(0, frameBox.Height / 2), scale, 0, 0);

                //绘制顶部条
                frameBox = barTex.Frame(1, 2, 0, 0);
                frameBox.Width = (int)(frameBox.Width /** factor*/* (Magike / (float)MagikeMax));
                Main.spriteBatch.Draw(barTex, pos, frameBox, Color.White, 0, new Vector2(0, frameBox.Height / 2), scale, 0, 0);

                //绘制文字
                //if (MagikeBarTimer > 10)
                Utils.DrawBorderString(Main.spriteBatch, $"{Magike} / {MagikeMax}", pos + new Vector2(barTex.Width / 2 * scale/**factor*/, 4), Color.White, 1, 0.5f, 0.5f);
            }
        }

        //public static Color GetColor(Enchant.Level level)
        //{
        //    return level switch
        //    {
        //        Enchant.Level.Nothing => Color.Gray,
        //        Enchant.Level.One => Color.White,
        //        Enchant.Level.Two => Color.CornflowerBlue,
        //        Enchant.Level.Three => Color.LightSeaGreen,
        //        Enchant.Level.Four => Color.Yellow,
        //        Enchant.Level.Five => Color.Pink,
        //        _ => Color.Orange
        //    };
        //}

        public void LimitMagike()
        {
            Magike = Math.Clamp(Magike, 0, MagikeMax);
        }

        /// <summary>
        /// 给物品充能，应该只传入正值，消耗魔能请使用<see cref="Cosume(int)"/>
        /// </summary>
        /// <param name="howManyMagike"></param>
        /// <returns></returns>
        public bool Charge(int howManyMagike)
        {
            if (FillUp)
                return false;

            Magike += howManyMagike;
            LimitMagike();

            return true;
        }

        /// <summary>
        /// 消耗物品中的魔能，请传入正值
        /// </summary>
        /// <param name="howManyMagike"></param>
        /// <returns></returns>
        public bool Cosume(int howManyMagike)
        {
            if (Magike < howManyMagike)
                return false;

            Magike -= howManyMagike;
            return true;
        }

        /// <summary>
        /// 魔能是否满了
        /// </summary>
        public bool FillUp => Magike >= MagikeMax;
    }
}
