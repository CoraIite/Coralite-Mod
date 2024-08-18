using Coralite.Core.Systems.CoraliteActorComponent;
using System;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class MagikeFactory : Component, ITimerTriggerComponent
    {
        public sealed override int ID => MagikeComponentID.MagikeFactory;

        public bool IsWorking { get; set; }

        public int DelayBase { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float DelayBonus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Timer { get; set ; }

        public override void Update(IEntity entity)
        {

        }



        /// <summary>
        /// 特定的工作
        /// </summary>
        public virtual void Work()
        {

        }

        public override void SaveData(string preName, TagCompound tag)
        {
            base.SaveData(preName, tag);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            base.LoadData(preName, tag);
        }
    }
}
