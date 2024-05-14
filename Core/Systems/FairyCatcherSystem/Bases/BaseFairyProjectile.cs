using Coralite.Content.Items.Fairies;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class BaseFairyProjectile : ModProjectile, IFairyProjectile
    {
        protected IFairyItem selfItem;

        public IFairyItem FairyItem { get => selfItem; set => selfItem = value; }
        public Player Owner => Main.player[Projectile.owner];

        public int State;
        public int Timer;
        protected int LifeMax => (int)FairyItem.FairyLifeMax;
        protected int Life => FairyItem.Life;
        protected virtual string SkillName => "";
        protected virtual int FrameX => 1;
        protected virtual int FrameY => 1;
        /// <summary>
        /// 能够开始攻击的距离
        /// </summary>
        protected float AttackDistance=400;

        private bool init = true;
        protected bool canDamage;

        public LocalizedText SkillText;

        public override void Load()
        {
            SkillText = this.GetLocalization("SkillText", () => SkillName);
        }

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

            Projectile.width = (int)(Projectile.width * Projectile.scale);
            Projectile.height = (int)(Projectile.height * Projectile.scale);

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

        public virtual void Backing_LerpToOwner()
        {
            Projectile.velocity = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 2;
            Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.Center, Timer * 0.0005f);

            if (Vector2.Distance(Projectile.Center, Owner.Center) < 24)
                Projectile.Kill();
        }

        public virtual void SpawnSkillText(Color color)
        {
            ModProjectile m = ModContent.GetModProjectile(Projectile.type);
            CombatText.NewText(Projectile.getRect(), color,
                (m as BaseFairyProjectile).SkillText.Value);
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

        public void UpdateFrameY(int spacing)
        {
            if (++Projectile.frameCounter > spacing)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= FrameY)
                    Projectile.frame = 0;
            }
        }

        public void SetDirectionNormally()
        {
            Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
        }

        public override bool? CanDamage()
        {
            if (canDamage)
                return null;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //自身受伤
            if (FairyItem != null)
                if (FairyItem.Hurt(Owner, target, hit, damageDone))
                {
                    Projectile.Kill();
                    OnKillByNPC(target);
                    return;
                }
        }

        public virtual void ExchangeToBack()
        {
            Timer = 0;
            State = (int)AIStates.Backing;
            canDamage = false;
            Projectile.timeLeft = 1200;
            Projectile.tileCollide = false;
        }

        public virtual void ExchangeToAction()
        {
            if (Helper.TryFindClosestEnemy(Projectile.Center, AttackDistance, n => n.CanBeChasedBy() && Collision.CanHit(Projectile, n), out NPC target))
            {
                State = (int)AIStates.Action;
                OnExchangeToAction(target);
                Timer = 0;
                canDamage = true;
            }
            else
                ExchangeToBack();
        }

        public virtual void RestartAction()
        {
            if (Helper.TryFindClosestEnemy(Projectile.Center, AttackDistance, n => n.CanBeChasedBy() && Collision.CanHit(Projectile, n), out NPC target))
            {
                State = (int)AIStates.Action;
                OnExchangeToAction(target);
                Timer = 0;
                canDamage = true;
            }
        }

        public virtual void OnExchangeToAction(NPC target) { }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (State == (int)AIStates.Shooting)
                ExchangeToAction();
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            FairyItem.IsOut = false;
        }

        /// <summary>
        /// 在仙灵血量减为0后执行，用于生成死亡效果
        /// </summary>
        /// <param name="target"></param>
        public virtual void OnKillByNPC(NPC target) { }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawSelf(Main.spriteBatch, Main.screenPosition, lightColor);

            if (canDamage)
                DrawHealthBar();
            return false;
        }

        /// <summary>
        /// 绘制自己
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="screenPos"></param>
        /// <param name="lightColor"></param>
        public virtual void DrawSelf(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var frame = mainTex.Frame(FrameX, FrameY, 0, Projectile.frame);

            spriteBatch.Draw(mainTex, Projectile.Center - screenPos, frame, lightColor, Projectile.rotation, frame.Size() / 2,
                Projectile.scale, Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        }

        public virtual void DrawHealthBar()
        {
            Main.instance.DrawHealthBar(Projectile.Bottom.X, Projectile.Bottom.Y + 12, Life, LifeMax, 1, 1);
        }
    }

    public interface IFairyProjectile
    {
        public IFairyItem FairyItem { get; set; }
    }
}
