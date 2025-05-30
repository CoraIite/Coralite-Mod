﻿using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Thunder
{
    public class FlaskOfThunder : ModItem
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            ItemID.Sets.DrinkParticleColors[Type] = new Color[1] {
                Coralite.ThunderveinYellow
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
            Item.buffType = ModContent.BuffType<FlaskOfThunderBuff>();
            Item.buffTime = 20 * 60 * 60;
        }

        public override void AddRecipes()
        {
            CreateRecipe(3)
                .AddIngredient<ZapCrystal>()
                .AddIngredient(ItemID.BottledWater, 3)
                .AddTile(TileID.ImbuingStation)
                .Register();
        }
    }

    [PlayerEffect]
    public class FlaskOfThunderBuff : ModBuff
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public override void SetStaticDefaults()
        {
            Main.meleeBuff[Type] = true;
            BuffID.Sets.IsAFlaskBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(FlaskOfThunderBuff));
        }
    }
}
