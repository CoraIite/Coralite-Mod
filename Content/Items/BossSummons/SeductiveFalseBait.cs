using Coralite.Core;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.BossSummons
{
    public class SeductiveFalseBait : ModItem
    {
        public override string Texture => AssetDirectory.BossSummons + Name;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;

            NPCID.Sets.MPAllowedEnemies[NPCID.DukeFishron] = true;
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
            return player.ZoneBeach && !player.ZoneRockLayerHeight && !NPC.AnyNPCs(NPCID.DukeFishron);
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                int type = NPCID.DukeFishron;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int num = NPC.NewNPC(NPC.GetBossSpawnSource(player.whoAmI), (int)player.Center.X + (player.direction * 200), (int)player.Center.Y + 100, type);
                    string typeName = Main.npc[num].TypeName;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText(Language.GetTextValue("Announcement.HasAwoken", typeName), 175, 75);
                    else if (VaultUtils.isServer)
                        ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", Main.npc[num].GetTypeNetName()), new Color(175, 75, 255));

                }
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TruffleWorm)
                .AddIngredient(ItemID.ShroomiteBar, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
