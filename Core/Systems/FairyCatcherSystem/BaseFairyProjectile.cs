using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public abstract class BaseFairyProjectile:ModProjectile
    {
        public BaseFairyItem SelfItem;

        public enum AIStates
        {
            /// <summary>
            /// 刚从仙灵捕手中射出的时候
            /// </summary>
            Shooting,
            /// <summary>
            /// 执行自身的特定AI
            /// </summary>
            Action,
            /// <summary>
            /// 返回自身，此时无敌
            /// </summary>
            Backing,
        }

        public sealed override void AI()
        {
            if (!CheckSelfItem())
                return;


        }

        public bool CheckSelfItem()
        {
            if (SelfItem is null)
            {
                Projectile.Kill();
                return false;
            }

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //自身受伤

        }
    }
}
