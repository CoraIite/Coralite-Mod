using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace Coralite.Content.Items.GlobalItems
{
    public class CoraliteGlobalItem : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            switch (item.type)
            {
                default: break;
                case ItemID.PhoenixBlaster: //削弱凤凰爆破枪，目的是为了给幽兰让路
                    item.damage = 30;
                    break;
                case ItemID.IceRod:     //削弱冰雪魔仗，因为它现在在肉前就能获得
                    item.damage = (int)(item.damage * 0.45f);
                    item.value = Item.sellPrice(0, 0, 10, 0);
                    item.mana = 9;
                    item.rare = ItemRarityID.Orange;
                    item.useTime = item.useAnimation = 8;
                    break;
                case ItemID.Coal:
                    item.maxStack = Item.CommonMaxStack;
                    break;
            }
        }


    }
}