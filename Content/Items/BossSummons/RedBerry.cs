using Coralite.Content.Bosses.Rediancie;
using Coralite.Core;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.BossSummons
{
    public class RedBerry : ModItem
    {
        public override string Texture => AssetDirectory.BossSummons + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("赤色果实");
            Tooltip.SetDefault("吸引一块奇异的赤色石头\n召唤赤玉灵");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;

            NPCID.Sets.MPAllowedEnemies[ModContent.NPCType<Rediancie>()] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 99;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.value = Item.sellPrice(0, 0, 5, 0);
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Blue;
            Item.consumable = true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                int type = ModContent.NPCType<Rediancie>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                else
                    NetMessage.SendData(MessageID.SpawnBoss, number: player.whoAmI, number2: type);
            }

            return true;
        }
    }
}
