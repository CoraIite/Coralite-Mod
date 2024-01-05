using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Coralite.Content.Items.Magike
{
    public class MagikeNote1 : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public int whichToUnlock = 1;

        public LocalizedText ExtraDescription
        {
            get
            {
                return this.GetLocalization("ExtraDescription", () => "使用以解锁魔能辞典第二章中“发现魔力水晶洞”部分的第 {0} 条知识").WithFormatArgs(whichToUnlock);
            }
        }

        public override void SetDefaults()
        {
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = CoraliteSoundID.MagicStaff_Item8;
            Item.shoot = 10;
            Item.consumable = true;
        }

        public void RandomKnowledge(int which)
        {
            if (which > 0 && which < 7)
            {
                whichToUnlock = which;
                return;
            }
            whichToUnlock = WorldGen.genRand.Next(1, 7);
        }

        public override bool CanUseItem(Player player)
        {
            switch (whichToUnlock)
            {
                default:
                case 1:
                    UseNote(ref MagikeSystem.MagikeCave_1);
                    break;
                case 2:
                    UseNote(ref MagikeSystem.MagikeCave_2);
                    break;
                case 3:
                    UseNote(ref MagikeSystem.MagikeCave_3);
                    break;
                case 4:
                    UseNote(ref MagikeSystem.MagikeCave_4);
                    break;
                case 5:
                    UseNote(ref MagikeSystem.MagikeCave_5);
                    break;
                case 6:
                    UseNote(ref MagikeSystem.MagikeCave_6);
                    break;
            }
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "ExtraDescription", ExtraDescription.Value));
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("whichToUnlock", whichToUnlock);
        }

        public override void LoadData(TagCompound tag)
        {
            whichToUnlock = tag.GetInt("whichToUnlock");
        }

        public void UseNote(ref bool @lock)
        {
            if (!@lock)
            {
                Main.NewText(MagikeSystem.NewKnowledgeUnlocked, Coralite.Instance.MagicCrystalPink);
            }

            @lock = true;
        }
    }
}
