using Terraria;

namespace Coralite.Core.Systems.FlyingShieldSystem
{
    public interface IFlyingShieldAccessory
    {
        virtual void OnInitialize(BaseFlyingShield projectile) { }
        virtual void PostInitialize(BaseFlyingShield projectile) { }
        virtual void OnGuardInitialize(BaseFlyingShieldGuard projectile) { }

        virtual void OnTileCollide(BaseFlyingShield projectile) { }

        virtual void OnJustHited(BaseFlyingShield projectile) { }

        virtual bool OnParry(BaseFlyingShieldGuard projectile) => false;
        virtual void OnGuard(BaseFlyingShieldGuard projectile) { }
        virtual void OnStartDashing(BaseFlyingShieldGuard projectile) { }
        virtual void OnDashing(BaseFlyingShieldGuard projectile) { }
        virtual void OnDashHit(BaseFlyingShieldGuard projectile, NPC target, ref NPC.HitModifiers modifiers) { }
        virtual void OnDashOver(BaseFlyingShieldGuard projectile) { }

        virtual void OnHitNPC(BaseFlyingShield projectile, NPC target, NPC.HitInfo hit, int damageDone) { }
    }
}
