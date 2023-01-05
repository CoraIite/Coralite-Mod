using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Accessories
{
    public class CoralStone : ModItem
    {
        //设置物品名称
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coral Stone");
            DisplayName.AddTranslation(7, "珊瑚石");

            Tooltip.SetDefault("add 100 maxmana");
            Tooltip.AddTranslation(7, "增加100点最大魔力值");
        }
        //设置物品属性
        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = 8;
            Item.expert = true;

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statManaMax2 += 100;
            player.statLifeMax2 += 100;
        }
        //设置合成表
        public override void AddRecipes()
        {
            
        }
    }
}
