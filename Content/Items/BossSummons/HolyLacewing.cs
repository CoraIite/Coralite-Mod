using Coralite.Core;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace Coralite.Content.Items.BossSummons
{
    public class HolyLacewing : ModItem
    {
        public override string Texture => AssetDirectory.BossSummons + Name;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;

            NPCID.Sets.MPAllowedEnemies[NPCID.HallowBoss] = true;
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
            Item.rare = ItemRarityID.Pink;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneHallow && !NPC.AnyNPCs(NPCID.HallowBoss);
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                int type = NPCID.HallowBoss;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.EmpressButterfly, 3)
                .AddIngredient(ItemID.SoulofLight, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.LightningBug, 3)
                .AddIngredient(ItemID.ButterflyDust)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
