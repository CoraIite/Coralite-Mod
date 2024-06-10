using Coralite.Content.Items.FlyingShields;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.Nightmare
{
    public class NightmareHeart : ModItem
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 8));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            //ItemID.Sets.ItemIconPulse[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 30);
            Item.accessory = true;
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.nightmareEnergyMax = 10;
                cp.AddEffect(nameof(NightmareHeart));
                player.GetDamage(DamageClass.Generic) += cp.nightmareEnergy * 0.015f;
            }

            //加8防御 2生命恢复每秒 4%减伤 8%移速 13%近战攻速 4%暴击

            player.statDefense += 8;
            player.lifeRegen += 4;
            player.endurance += 0.04f;
            player.moveSpeed += 0.08f;

            player.GetAttackSpeed(DamageClass.Melee) += 0.13f;
            player.GetCritChance(DamageClass.Generic) += 4;
        }
    }
}
