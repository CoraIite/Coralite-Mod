using Coralite.Content.ModPlayers;
using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.RedJades
{
    public class FlaskOfRedJade : ModItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            ItemID.Sets.DrinkParticleColors[Type] = new Color[1] {
                Coralite.Instance.RedJadeRed
            };
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item3;
            Item.maxStack = 30;
            Item.consumable = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 1);
            Item.buffType = ModContent.BuffType<FlaskOfRedJadeBuff>();
            Item.buffTime = 20 * 60 * 60;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RedJade>(5)
                .AddIngredient(ItemID.BottledWater)
                .AddTile(TileID.ImbuingStation);
        }
    }

    public class FlaskOfRedJadeBuff : ModBuff
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsAFlaskBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.flaskOfRedJade = true;
        }
    }
}
