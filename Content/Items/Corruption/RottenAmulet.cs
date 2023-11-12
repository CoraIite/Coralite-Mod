using Coralite.Content.Items.Crimson;
using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Corruption
{
    public class RottenAmulet:ModItem
    {
        public override string Texture => AssetDirectory.CorruptionItems + Name;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<BloodAmulet>();
        }

        public override void SetDefaults()
        {
            Item.defense = 3;
            Item.height = Item.width = 32;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0, 50);
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out RottenAmuletPlayer rap))
            {
                rap.equippedRottenAmulet = true;
                if (!player.HasBuff<LimbRebirth>())
                    rap.limbRebirthCount = 0;

                player.lifeRegen += rap.limbRebirthCount;
                if (rap.limbRebirthCount >= 6)
                {
                    rap.limbRebirthCount = 0;
                    rap.limbRebirthCD = 60 * 5;
                    player.Heal(25);
                    player.DelBuff(player.FindBuffIndex(ModContent.BuffType<LimbRebirth>()));
                }
            }
        }
    }

    public class LimbRebirth : ModBuff
    {
        public override string Texture => AssetDirectory.CorruptionItems + Name;

        public static int count;

        public LocalizedText Current => this.GetLocalization("Current");

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.armorEffectDrawShadow = true;
            if (player.TryGetModPlayer(out RottenAmuletPlayer rap) && player.whoAmI == Main.myPlayer)
                count = rap.limbRebirthCount;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            tip += $"\n{Current.Value}{count}";
        }
    }

    public class RottenAmuletPlayer:ModPlayer
    {
        public bool equippedRottenAmulet;
        public int limbRebirthCount;
        public int limbRebirthCD;

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            OnHit(hurtInfo);
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            OnHit(hurtInfo);
        }

        public void OnHit(Player.HurtInfo hurtInfo)
        {
            if (!equippedRottenAmulet || limbRebirthCD != 0 || hurtInfo.Damage < 20)//休想骗伤！！！！！！！！！！！！！！！！！！！！！！！！！！！
                return;
            Player.AddBuff(ModContent.BuffType<LimbRebirth>(), 60 * 15);
            limbRebirthCount++;
        }

        public override void ResetEffects()
        {
            if (limbRebirthCount > 6)
                limbRebirthCount = 6;
            if (limbRebirthCD > 0)
                limbRebirthCD--;

            equippedRottenAmulet = false;
        }
    }
}
