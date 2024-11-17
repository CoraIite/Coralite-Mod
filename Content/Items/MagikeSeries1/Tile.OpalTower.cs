using Coralite.Content.ModPlayers;
using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class OpalTower : ModItem, IMagikeCraftable
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public void AddMagikeCraftRecipe()
        {
            MagikeCraftRecipe.CreateRecipe<MagikeMonoclastic, OpalTower>(MagikeHelper.CalculateMagikeCost(MALevel.MagicCrystal, 6, 60 * 2))
                .AddIngredient<Basalt>(12)
                .AddIngredient(ItemID.Lens)
                .Register();
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<OpalTowerTile>());
            Item.value = Item.sellPrice(0, 0, 50);
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
        }
    }

    public class OpalBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(MagikeMonoclastic));
        }
    }
}
