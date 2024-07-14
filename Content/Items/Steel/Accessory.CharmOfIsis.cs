using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Steel
{
    //[AutoloadEquip(EquipType.Head)]
    public class CharmOfIsis : BaseAccessory
    {
        public override string Texture => AssetDirectory.SteelItems + Name;

        public CharmOfIsis() : base(ItemRarityID.Pink, Item.sellPrice(0, 6, 0, 0))
        {
        }

        public override void AutoDefaults() { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.lifeRegen = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddEffect(nameof(CharmOfIsis));
                //if (!hideVisual)
                //    cp.AddEffect(nameof(CharmOfIsis)+"Vanity");
            }

            player.pStone = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MedalOfLife>()
                .AddIngredient(ItemID.CharmofMyths)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
