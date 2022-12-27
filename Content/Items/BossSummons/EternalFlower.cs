using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.Audio;
using Coralite.Core;

namespace Coralite.Content.Items.BossSummons
{
    public class EternalFlower:ModItem
    {
        public override string Texture => AssetDirectory.BossSummons + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("永恒的花朵");
            Tooltip.SetDefault("象征着永恒的花朵，无论过去多少个世纪它都不会凋零\n在地下丛林召唤世纪之花，不消耗");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;

            NPCID.Sets.MPAllowedEnemies[NPCID.Plantera] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneJungle && player.ZoneRockLayerHeight && !NPC.AnyNPCs(NPCID.Plantera);
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                int type = NPCID.Plantera;

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
                .AddIngredient(ItemID.Ectoplasm,33)
                .AddIngredient(ItemID.Vine, 3)
                .AddIngredient(ItemID.JungleSpores,9)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
