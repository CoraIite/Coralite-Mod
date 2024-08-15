﻿using Coralite.Core.Systems.MagikeSystem.Components;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class BaseActiveProducerTileEntity<TModTile>() : MagikeTileEntity()
        where TModTile : ModTile
    {
        public sealed override ushort TileType => (ushort)ModContent.TileType<TModTile>();

        public override void InitializeBeginningComponent()
        {
            AddComponentDirectly(GetStartContainer());
            AddComponentDirectly(GetStartSender());
            AddComponentDirectly(GetStartProducer());
        }

        public abstract MagikeContainer GetStartContainer();
        public abstract MagikeLinerSender GetStartSender();
        public abstract MagikeActiveProducer GetStartProducer();
    }
}