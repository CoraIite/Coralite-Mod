using Terraria;

namespace Coralite.Core.Systems.FlyingShieldSystem
{
    public interface IFlyingShieldAccessory: IFlyingShieldAccessory_Fly, IFlyingShieldAccessory_Guard
    {

    }

    public interface IFlyingShieldAccessory_Fly
    {
        virtual void OnInitialize(BaseFlyingShield projectile) { }
        virtual void PostInitialize(BaseFlyingShield projectile) { }

        virtual void OnTileCollide(BaseFlyingShield projectile) { }

        virtual void OnJustHited(BaseFlyingShield projectile) { }

        virtual void OnHitNPC(BaseFlyingShield projectile, NPC target, NPC.HitInfo hit, int damageDone) { }
    }

    public interface IFlyingShieldAccessory_Guard
    {
        virtual void OnGuardInitialize(BaseFlyingShieldGuard projectile) { }

        virtual bool OnParry(BaseFlyingShieldGuard projectile) => false;
        virtual void OnGuard(BaseFlyingShieldGuard projectile) { }
        virtual void OnStartDashing(BaseFlyingShieldGuard projectile) { }
        virtual void OnDashing(BaseFlyingShieldGuard projectile) { }
        virtual void OnDashHit(BaseFlyingShieldGuard projectile, NPC target, ref NPC.HitModifiers modifiers) { }
        virtual void OnDashOver(BaseFlyingShieldGuard projectile) { }
    }
}
