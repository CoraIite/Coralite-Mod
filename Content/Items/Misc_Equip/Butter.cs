using Coralite.Content.GlobalNPCs;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Misc_Equip
{
    [PlayerEffect()]
    [AutoloadEquip(EquipType.Head)]
    public class Butter : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Equip + Name;

        public override void SetStaticDefaults()
        {
            int id = EquipLoader.GetEquipSlot(Mod, nameof(Butter), EquipType.Head);
            ArmorIDs.Head.Sets.DrawFullHair[id] = true;
        }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 11;
        }

        public override void UpdateEquip(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddEffect(nameof(Butter));
                player.GetCritChance(DamageClass.Ranged) += 5;
            }
        }
    }

    public class ButterDebuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + "Buff";

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.NextBool(2))
            {
                int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.YellowStarDust, SpeedY: -1.5f, Scale: Main.rand.NextFloat(1, 2f));
                Main.dust[d].noGravity = true;
            }

            if (npc.buffTime[buffIndex] < 3)
                return;

            if (npc.TryGetGlobalNPC(out CoraliteGlobalNPC cgnpc))
            {
                cgnpc.StopHitPlayer = true;
                cgnpc.SlowDownPercent = 0.75f;
            }

            if (npc.realLife != -1 && Main.npc[npc.realLife].TryGetGlobalNPC(out CoraliteGlobalNPC cgnpc2))
            {
                cgnpc2.StopHitPlayer = true;
                cgnpc2.SlowDownPercent = 0.75f;
            }
        }
    }
}
