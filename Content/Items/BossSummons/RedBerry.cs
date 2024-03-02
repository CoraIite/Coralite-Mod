using Coralite.Content.Bosses.Rediancie;
using Coralite.Content.Items.RedJades;
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
            // DisplayName.SetDefault("赤色果实");
            // Tooltip.SetDefault("偶尔生长在水晶树上，能吸引一块奇异的赤色石头\n召唤赤玉灵");

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

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<Rediancie>());
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
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
            }

            return true;
        }

        public override void AddRecipes()
        {
            //黄玉
            CreateRecipe()
                .AddIngredient(ItemID.GemTreeTopazSeed, 6)
                .Register();
            //紫晶
            CreateRecipe()
                .AddIngredient(ItemID.GemTreeAmethystSeed, 6)
                .Register();
            //蓝玉
            CreateRecipe()
                .AddIngredient(ItemID.GemTreeSapphireSeed, 6)
                .Register();
            //翡翠
            CreateRecipe()
                .AddIngredient(ItemID.GemTreeEmeraldSeed, 6)
                .Register();
            //红玉
            CreateRecipe()
                .AddIngredient(ItemID.GemTreeRubySeed, 6)
                .Register();
            //钻石
            CreateRecipe()
                .AddIngredient(ItemID.GemTreeDiamondSeed, 6)
                .Register();
            //琥珀
            CreateRecipe()
                .AddIngredient(ItemID.GemTreeAmberSeed, 6)
                .Register();
        }
    }
}
