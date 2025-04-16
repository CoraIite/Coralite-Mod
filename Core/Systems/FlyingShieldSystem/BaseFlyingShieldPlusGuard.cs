namespace Coralite.Core.Systems.FlyingShieldSystem
{
    public abstract class BaseFlyingShieldPlusGuard : BaseFlyingShieldGuard
    {
        public override void LimitFields()
        {
            if (damageReduce > 0.5f)//减少减伤效果
                damageReduce = 0.5f;

            strongGuard = 1;//必定能格挡弹幕
        }

        public override void Guarding()
        {
            Owner.itemTime = Owner.itemAnimation = 2;

            if (CompletelyHeldUpShield)//玩家左键时取消防御
                if (DownLeft || !DownRight)
                {
                    TurnToDelay();
                    return;
                }

            SetPos();
            OnHoldShield();

            if (DistanceToOwner < GetWidth())
            {
                DistanceToOwner += distanceAdder;
                return;
            }

            CompletelyHeldUpShield = true;
            int which = CheckCollide(out int index);
            if (which > 0)
            {
                UpdateShieldAccessory(accessory => accessory.OnGuard(this));
                OnGuard();
                if (which == (int)GuardType.Projectile)
                    OnGuardProjectile(index);
                else if (which == (int)GuardType.NPC)
                    OnGuardNPC(index);
            }
        }
    }
}
