using System.Collections.Generic;
using Terraria;

namespace Coralite.Core.Systems.DotSystem
{
    public class DotGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        private List<Dot> _dots;

        /// <summary>
        /// 当前NPC持有的附着，每种类型存储一个独立实例
        /// </summary>
        public List<Dot> Dots => _dots ??= [];

        public bool HasDots => _dots is { Count: > 0 };

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (_dots == null)
                return;

            foreach (var dot in _dots)
                dot.ModifyNPCHit(npc, ref modifiers);
        }

        public override bool PreAI(NPC npc)
        {
            if (_dots == null)
                return base.PreAI(npc);

            _dots.RemoveAll(static dot => dot.NotActive);

            int count = _dots.Count;
            for (int i = 0; i < count; i++)
                _dots[i].Update(npc);

            return base.PreAI(npc);
        }
    }
}
