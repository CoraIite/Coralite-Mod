﻿using Coralite.Core.Systems.CoraliteActorComponent;

namespace Coralite.Core.Systems.MagikeSystem.Components.Producers
{
    public abstract class MagikeInactiveProducer : MagikeProducer
    {
        public sealed override void Update(IEntity entity) { }
    }
}
