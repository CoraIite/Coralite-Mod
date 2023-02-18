using Coralite.Core;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.BossSummons
{
    public class EndlessDeliciousness : ModItem
    {
        public override string Texture => AssetDirectory.BossSummons + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("无尽的美味");
            Tooltip.SetDefault("注入了魔法的腐烂食物，重要的是，它可以无限吃！\n在腐化地形召唤世界吞噬怪，不消耗");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;

            NPCID.Sets.MPAllowedEnemies[NPCID.EaterofWorldsHead] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0, 1, 0);
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Green;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneCorrupt && !NPC.AnyNPCs(NPCID.EaterofWorldsHead);
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                int type = NPCID.EaterofWorldsHead;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                else
                    NetMessage.SendData(MessageID.SpawnBoss, number: player.whoAmI, number2: type);
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.VilePowder, 40)
                .AddIngredient(ItemID.RottenChunk, 45)
                .AddIngredient(ItemID.ManaCrystal)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
