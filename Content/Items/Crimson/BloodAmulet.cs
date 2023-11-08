using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Crimson
{
    public class BloodAmulet : ModItem
    {
        public override string Texture => AssetDirectory.CrimsonItems + Name;

        public override void SetDefaults()
        {
            Item.height = Item.width = 32;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0, 50);
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out BloodAmuletPlayer bap))
            {
                bap.equippedBloodAmulet = true;
                if (!player.HasBuff<Bloodthirsty>())
                    bap.bloodthirstyCount = 0;

                player.GetDamage(DamageClass.Generic) += bap.bloodthirstyCount * 0.02f;
                player.moveSpeed += bap.bloodthirstyCount * 0.02f;
            }
        }
    }

    public class Bloodthirsty : ModBuff
    {
        public override string Texture => AssetDirectory.CrimsonItems + Name;

        public static int count;

        public LocalizedText Current => this.GetLocalization("Current");

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.armorEffectDrawShadow = true;
            if (player.TryGetModPlayer(out BloodAmuletPlayer bap)&& player.whoAmI == Main.myPlayer)
                count = bap.bloodthirstyCount;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            tip += $"\n{Current.Value}{count}";
        }
    }

    public class BloodAmuletPlayer : ModPlayer
    {
        public bool equippedBloodAmulet;
        public int bloodthirstyCount;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!equippedBloodAmulet || target.life - damageDone > 0)
                return;

            Player.AddBuff(ModContent.BuffType<Bloodthirsty>(), 60 * 9);
            bloodthirstyCount++;

            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Blood, Scale: Main.rand.NextFloat(1, 2));
                d.velocity = (i * MathHelper.TwoPi / 16).ToRotationVector2() * Main.rand.NextFloat(1, 4);
            }
        }

        public override void ResetEffects()
        {
            if (bloodthirstyCount > 5)
                bloodthirstyCount = 5;

            equippedBloodAmulet = false;
        }
    }
}
