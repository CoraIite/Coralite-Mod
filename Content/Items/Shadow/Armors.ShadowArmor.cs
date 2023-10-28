using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Shadow
{
    [AutoloadEquip(EquipType.Head)]
    public class ShadowHead : ModItem, IControllableArmorBonus
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 18;
            Item.value = Item.sellPrice(0, 0, 80, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<ShadowBreastplate>() && legs.type == ItemType<ShadowLegs>();
        }

        public override void UpdateEquip(Player player)
        {
            player.manaCost -= 0.10f;
            player.statManaMax2 += 60;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "影铠法环守护着你" +
                "\n使用物品时根据使用时间为法环充能" +
                "\n能量不足时按下套装奖励键，无事发生" +
                "\n能量较少时按下套装奖励键进行较弱的攻击" +
                "\n能量充足时按下套装奖励键根据当前使用武器类型发射不同弹幕攻击" +
                "\n套装奖励键可在模组配置中更改";
            if (Main.myPlayer == player.whoAmI && player.ownedProjectileCounts[ProjectileType<ShadowCircle>()] < 1)
            {
                //生成弹幕
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Microsoft.Xna.Framework.Vector2.Zero,
                    ProjectileType<ShadowCircle>(), 50, 0, player.whoAmI);
            }
        }

        public void UseArmorBonus(Player player)
        {
            foreach (var proj in Main.projectile)
            {
                if (proj.active && proj.owner == player.whoAmI && proj.type == ProjectileType<ShadowCircle>())
                {
                    //设置特殊行动
                    (proj.ModProjectile as ShadowCircle).StartAttack();
                    break;
                }
            }
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class ShadowBreastplate : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 18;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 8;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.10f;
            player.GetCritChance(DamageClass.Generic) += 0.06f;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class ShadowLegs : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 18;
            Item.value = Item.sellPrice(0, 0, 80, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 6;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed(DamageClass.Generic) += 0.08f;
            player.GetKnockback(DamageClass.Generic) += 0.06f;
        }
    }
}
