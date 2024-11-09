using Coralite.Core.Systems.MagikeSystem.Components;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.GameContent;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class Pedestal<TModTile>() : MagikeTP()
        where TModTile : ModTile
    {
        public sealed override int TargetTileID => (ushort)ModContent.TileType<TModTile>();

        public override void InitializeBeginningComponent()
        {
            AddComponent(GetStartItemContainer());
        }

        public abstract ItemContainer GetStartItemContainer();
    }
}
