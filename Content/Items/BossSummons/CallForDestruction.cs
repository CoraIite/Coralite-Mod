using Coralite.Core;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.BossSummons
{
    public class CallForDestruction : ModItem
    {
        public override string Texture => AssetDirectory.BossSummons + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("毁灭的呼唤");
            Tooltip.SetDefault("它能招来破坏的化身，但是是用打电话的方式\n在夜晚召唤毁灭者，不消耗");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;

            NPCID.Sets.MPAllowedEnemies[NPCID.TheDestroyer] = true;
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
            Item.rare = ItemRarityID.LightRed;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime && !NPC.AnyNPCs(NPCID.TheDestroyer);
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                int type = NPCID.TheDestroyer;

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
                .AddIngredient(ItemID.RottenChunk, 18)
                .AddRecipeGroup(RecipeGroupID.IronBar, 15)
                .AddIngredient(ItemID.SoulofNight, 18)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                 .AddIngredient(ItemID.Vertebrae, 18)
                 .AddRecipeGroup(RecipeGroupID.IronBar, 15)
                 .AddIngredient(ItemID.SoulofNight, 18)
                 .AddTile(TileID.MythrilAnvil)
                 .Register();
        }
    }
}
