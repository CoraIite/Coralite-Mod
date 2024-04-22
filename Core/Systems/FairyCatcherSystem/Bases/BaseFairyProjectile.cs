using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class BaseFairyProjectile : ModProjectile, IFairyProjectile
    {
        protected IFairyItem selfItem;

        public IFairyItem FairyItem { get => selfItem; set => selfItem = value; }
        public Player Owner => Main.player[Projectile.owner];

        public int State;
        public int Timer;
        protected int lifeMax;
        protected int life;

        private bool init=true;

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

            if (init)
                Initilize();

            switch (State)
            {
                default:
                    Projectile.Kill(); break;
                case (int)AIStates.Shooting:
                    {
                        Shooting();
                    }
                    break;
                case (int)AIStates.Action:
                    {
                        Action();
                    }
                    break;
                case (int)AIStates.Backing:
                    {
                        Backing();
                    }
                    break;
            }
        }

        public void Initilize()
        {
            init = false;

            OnInitialize();
        }

        public virtual void OnInitialize() { }

        public virtual void Shooting()
        {

        }

        public virtual void Action()
        {

        }

        public virtual void Backing()
        {

        }

        public bool CheckSelfItem()
        {
            if (FairyItem is null)
            {
                Projectile.Kill();
                return false;
            }
            else
            {
                lifeMax = (int)FairyItem.FairyLifeMax;
                life = FairyItem.Life;
            }

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //自身受伤
            if (FairyItem != null)
                if (FairyItem.Hurt(Owner, target, hit, damageDone))
                {
                    Projectile.Kill();
                    return;
                }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawSelf(Main.spriteBatch, Main.screenPosition, lightColor);

            DrawHealthBar();
            return false;
        }

        /// <summary>
        /// 绘制自己
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="screenPos"></param>
        /// <param name="lightColor"></param>
        public virtual void DrawSelf(SpriteBatch spriteBatch,Vector2 screenPos,Color lightColor) { }

        public virtual void DrawHealthBar()
        {
            Main.instance.DrawHealthBar(Projectile.Bottom.X, Projectile.Bottom.Y + 12, life, lifeMax, 1, 1);
        }
    }

    public interface IFairyProjectile
    {
        public IFairyItem FairyItem { get; set; }
    }
}
