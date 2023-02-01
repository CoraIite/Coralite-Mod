using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Coralite.Content.Buffs;
using static Terraria.ModLoader.ModContent;
using Terraria.GameContent;

namespace Coralite.Content.Projectiles.RedJadeProjectiles
{
    public class RedBink:ModProjectile
    {
        public override string Texture => AssetDirectory.RedJadeProjectiles + Name;

        public Player Owner => Main.player[Projectile.owner];
        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("蘑菇幼龙");

            Main.projFrames[Type] = 7;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 25;
            Projectile.alpha = 0;
            Projectile.timeLeft = 300;
            Projectile.minionSlots = 1;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 20;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.minion = true;
            Projectile.usesIDStaticNPCImmunity = true;

            Projectile.DamageType = DamageClass.Summon;
        }

        public override bool MinionContactDamage() => true;

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        #region AI

        public override void AI()
        {
            //1：无敌怪时在玩家周围围成一圈
            //2：找到敌人后不断向敌人位置冲刺
            //3：右键打断当前动作短暂蓄力后向敌人冲刺

            if (!CheckActive(Owner))
                return;

            //添加Buff
            Owner.AddBuff(BuffType<RedBinkBuff>(), 2);

            switch (State)
            {
                default: break;
                case (int)AIStates.idle://回到玩家身边
                    break;

                case (int)AIStates.normalAttack://普通攻击：不断冲向敌人位置
                    break;

                case (int)AIStates.specialAttack://特殊攻击
                    break;
            }




            Projectile.netUpdate = true;
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(BuffType<RedBinkBuff>());
                return false;
            }

            if (owner.HasBuff(BuffType<RedBinkBuff>()))
                Projectile.timeLeft = 2;

            return true;
        }

        #endregion
    }

    public enum AIStates:int
    {
        idle=0,
        normalAttack=1,
        specialAttack=2
    }
}
