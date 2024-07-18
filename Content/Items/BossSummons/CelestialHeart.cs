using Coralite.Content.Items.Materials;
using Coralite.Core;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace Coralite.Content.Items.BossSummons
{
    public class CelestialHeart : ModItem
    {
        public override string Texture => AssetDirectory.BossSummons + Name;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;

            NPCID.Sets.MPAllowedEnemies[NPCID.MoonLordCore] = true;
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
            return !NPC.TowerActiveNebula && !NPC.TowerActiveSolar&&!NPC.TowerActiveStardust&& !NPC.TowerActiveVortex&& !NPC.LunarApocalypseIsUp&&
                !NPC.AnyNPCs(NPCID.MoonLordCore);
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                WorldGen.StartImpendingDoom(60 * 10);
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Night_luminescentPearl>()
                .AddIngredient(ItemID.TissueSample, 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();

            CreateRecipe()
                .AddIngredient<Night_luminescentPearl>()
                .AddIngredient(ItemID.ShadowScale, 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();

            CreateRecipe()
                .AddIngredient<Night_luminescentPearl>()
                .AddIngredient(ItemID.MoonLordLegs)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
