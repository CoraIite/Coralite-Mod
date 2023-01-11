using Coralite.Content.Tiles.Plants;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items
{
    public class A : BaseSeed
    {
        public override string Texture => "Coralite/Content/Items/A";
        public A() : base("A", "”A“", 999, Item.sellPrice(0, 0, 0, 1), ItemRarityID.White, 15, 15, 0, 0, ModContent.TileType<AgropyronFrozen>(),
            new System.Collections.Generic.Dictionary<int, Core.Systems.BotanicalSystem.CrossBreedData>()
            {
                {ModContent.ItemType<B>(),new Core.Systems.BotanicalSystem.CrossBreedData(ModContent.ItemType<C>(),1)},
            }) { }
    }
}
