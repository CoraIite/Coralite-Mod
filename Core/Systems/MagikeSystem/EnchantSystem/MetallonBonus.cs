using Coralite.Content.ModPlayers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class SpecialEnchant_MetallonBonus : SpecialEnchant
    {
        public SpecialEnchant_MetallonBonus(Enchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
        {
        }

        public override void Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.TryGetModPlayer(out MagikePlayer mp) && mp.SpecialEnchantCD <= 0)
            {
                player.AddBuff(ModContent.BuffType<MetallionBuff>(), 60 * 5);
                mp.SpecialEnchantCD = 60 * 12;
            }
        }

        public override string Description => Language.GetOrRegister($"Mods.Coralite.Systems.MagikeSystem.MetallonBonus", () => "刚：攻击时获得防御加成Buff").Value;
    }

    public class MetallionBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;

        public override void Update(Player player, ref int buffIndex)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDustPerfect(player.Center + Main.rand.NextVector2Circular(32, 32), ModContent.DustType<MetallionDust>(), Vector2.Zero, Scale: Main.rand.NextFloat(0.8f, 1.2f));

            player.endurance += 0.08f;//8%伤害减免
        }
    }

    public class MetallionDust : ModDust
    {
        public override string Texture => AssetDirectory.Dusts + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, Main.rand.Next(5) * 24, 14, 24);
            dust.color = Color.White;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            dust.fadeIn++;

            if (dust.fadeIn > 8)
                dust.color *= 0.9f;
            if (dust.fadeIn % 4 == 0)
            {
                dust.frame.Y += 24;
                if (dust.frame.Y > 5 * 24)
                    dust.frame.Y = 0;
            }

            if (dust.fadeIn > 40 || dust.color.A < 10)
            {
                dust.active = false;
            }
            return false;
        }
    }
}
