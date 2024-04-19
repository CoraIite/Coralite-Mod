using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class BaseFairyProjectile : ModProjectile, IFairyProjectile
    {
        protected IFairyItem selfItem;

        public IFairyItem FairyItem { get => selfItem; set => selfItem = value; }

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
            if (FairyItem is null)
            {
                Projectile.Kill();
                return false;
            }

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //自身受伤
            FairyItem?.Hurt();
        }
    }

    public interface IFairyProjectile
    {
        public IFairyItem FairyItem { get; set; }
    }
}
