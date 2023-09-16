using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.BossSummons
{
    public class NightmareHarp:ModItem,IMagikeRemodelable
    {
        public override string Texture => AssetDirectory.BossSummons + Name;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;

            NPCID.Sets.MPAllowedEnemies[ModContent.NPCType<NightmarePlantera>()] = true;
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
            return !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<NightmarePlantera>());
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                int type = ModContent.NPCType<NightmarePlantera>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
            }

            return true;
        }

        public void AddMagikeRemodelRecipe()
        {
            MagikeSystem.AddRemodelRecipe<NightmareHarp>(0f, ItemID.Harp, 10000);
        }
    }
}
