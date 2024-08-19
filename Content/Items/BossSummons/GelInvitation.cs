using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor;
using Coralite.Content.Items.Gels;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace Coralite.Content.Items.BossSummons
{
    public class GelInvitation : ModItem, IMagikeCraftable
    {
        public override string Texture => AssetDirectory.BossSummons + Name;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;

            NPCID.Sets.MPAllowedEnemies[ModContent.NPCType<SlimeEmperor>()] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<SlimeEmperor>());
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                int type = ModContent.NPCType<SlimeEmperor>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
            }

            return true;
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeCraftRecipe.CreateRecipe<GelInvitation>(250)
                .SetMainItem<SymbolOfRoyal>()
                .AddIngredient<GelFiber>(20)
                .AddIngredient(ItemID.Gel, 99)
                .Register();
            MagikeCraftRecipe.CreateRecipe<GelInvitation>(250)
                .SetMainItem(ItemID.SlimeCrown, 3)
                .AddIngredient<GelFiber>(20)
                .AddIngredient(ItemID.Gel, 99)
                .Register();
        }

        //public void AddMagikeRemodelRecipe()
        //{
        //MagikeSystem.AddRemodelRecipe<SymbolOfRoyal, GelInvitation>(400);
        //MagikeSystem.AddRemodelRecipe<GelInvitation>(0f, ItemID.SlimeCrown, 400, selfStack: 3);
        //}
    }
}
