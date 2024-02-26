using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.FlyingShields
{
    public class MineShield : BaseFlyingShieldItem<MineShieldGuard>
    {
        public MineShield() : base(Item.sellPrice(0, 2), ItemRarityID.LightRed, AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<MineShieldProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 12;
            Item.damage = 40;
        }
    }

    public class MineShieldProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "MineShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 30;
        }

        public override void SetOtherValues()
        {
            flyingTime = 15;
            backTime = 4;
            backSpeed = 12;
            trailCachesLength = 6;
        }
    }

    public class MineShieldGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "MineShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 30;
            Projectile.height = 46;
            Projectile.scale = 1.1f;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.15f;
            distanceAdder=3f;
        }

        public override int CheckCollide()
        {
            Rectangle rect = Projectile.getRect();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active || proj.friendly || proj.whoAmI == Projectile.whoAmI || localProjectileImmunity[i] > 0)
                    continue;

                if (proj.Colliding(proj.getRect(), rect))
                {
                    if (Owner.TryGetModPlayer(out CoralitePlayer cp))
                        cp.Guard(damageReduce);

                    //如果不应该瞎JB乱该这东西的速度那就跳过
                    if (proj.aiStyle == 4 || proj.aiStyle == 38 || proj.aiStyle == 84 || proj.aiStyle == 148 || 
                        (proj.aiStyle == 7 && proj.ai[0] == 2f) || ((proj.type == 440 || proj.type == 449 || 
                        proj.type == 606) && proj.ai[1] == 1f) || (proj.aiStyle == 93 && proj.ai[0] < 0f) || 
                        proj.type == 540 || proj.  type == 756 || proj.type == 818 || proj.type == 856 || 
                        proj.type == 961 || proj.type == 933 || ProjectileID.Sets.IsAGolfBall[proj.type])
                            goto over;

                    if (!ProjectileLoader.ShouldUpdatePosition(proj))
                        goto over;

                    //修改速度
                    proj.velocity *= -1;
                    float angle = proj.velocity.ToRotation();
                    proj.velocity = angle.AngleLerp(Projectile.rotation,0.5f).ToRotationVector2() * proj.velocity.Length();

                    over:
                    float percent = MathHelper.Clamp(StrongGuard, 0, 1);
                    if (Main.rand.NextBool((int)(percent * 100), 100) && proj.penetrate > 0)//削减穿透数
                    {
                        proj.penetrate--;
                        if (proj.penetrate < 1)
                            proj.Kill();
                        OnStrongGuard();
                    }
                    localProjectileImmunity[i] = Projectile.localNPCHitCooldown;
                    return (int)GuardType.Projectile;
                }
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.friendly || npc.immortal || Projectile.localNPCImmunity[i] > 0)
                    continue;

                if (Projectile.Colliding(rect, npc.getRect()))
                {
                    if (Owner.TryGetModPlayer(out CoralitePlayer cp))
                        cp.Guard(damageReduce);

                    Projectile.localNPCImmunity[i] = Projectile.localNPCHitCooldown;
                    if (!npc.dontTakeDamage)
                        npc.SimpleStrikeNPC(Projectile.damage, Projectile.direction, knockBack: Projectile.knockBack, damageType: DamageClass.Melee);

                    return (int)GuardType.NPC;
                }
            }

            return (int)GuardType.notGuard;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            Helper.PlayPitched("Misc/ShieldGuard2", 0.4f, 0, Projectile.Center);
        }
    }
}
