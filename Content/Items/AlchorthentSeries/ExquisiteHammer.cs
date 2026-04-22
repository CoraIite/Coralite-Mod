using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.AlchorthentSeries
{
    public class ExquisiteHammer : BaseAlchorthentItem
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems+ "ExquisiteHammerItem";

        public override void SetOtherDefaults()
        {
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 30;
            Item.shoot = ModContent.ProjectileType<ExquisiteHammerHeldProj>();

            Item.SetWeaponValues(24, 4);
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(0, 2));
        }

        public override void Summon(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
        }

        public override void MinionAim(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            
        }

        public override void SpecialAttack(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            
        }

        public override void AddRecipes()
        {
            
        }
    }

    public class ExquisiteHammerHeldProj() : BaseSwingProj(1,40)
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + nameof(ExquisiteHammer);

        public override void SetSwingProperty()
        {

        }
    }

    public class ExquisiteAwlBuff : BaseAlchorthentBuff<ExquisiteAwl>
    {
        public override string Texture => AssetDirectory.Buffs + "Buff";
    }

    [VaultLoaden(AssetDirectory.AlchorthentSeriesItems)]
    public class ExquisiteAwl : BaseAlchorthentMinion<FaintEagleBuff>
    {
        public ref float TexType => ref Projectile.ai[0];

        public ref float Recorder => ref Projectile.ai[1];
        public ref float Recorder2 => ref Projectile.ai[2];
        public ref float Recorder3 => ref Projectile.localAI[1];
        public ref float Recorder4 => ref Projectile.localAI[2];

        /// <summary>
        /// 翅膀帧图
        /// </summary>
        public static ATex ExquisiteWing { get; private set; }

        public const int IdleFrame = 14;
        public const int FlyFrame = IdleFrame + 15;

        public const int AwlFrameMax = FlyFrame + 1;
        public const int WimgFrameMax = 14;

        private enum AIStates : byte
        {
            /// <summary> 刚召唤出来 </summary>
            OnSummon,
            /// <summary> 飞回玩家的过程 </summary>
            BackToOwner,
            /// <summary> 待机 </summary>
            Idle,
            /// <summary> 特殊待机 </summary>
            SpIdle,
            /// <summary> 戳刺攻击 </summary>
            SpikeAttackHeavy,
            /// <summary> 强化版冲刺攻击 </summary>
            SpikeAttackWeak,
            /// <summary> 撞碎后重组自身 </summary>
            SpikeAttackSpecial
        }

        public override void SetOtherDefault()
        {
            Projectile.tileCollide = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1;
            Projectile.width = Projectile.height = 32;
            Projectile.localNPCHitCooldown = 20;
        }

        #region AI

        public override void AIMoves()
        {
            Timer++;

            switch (State)
            {
                default:
                case (int)AIStates.OnSummon:
                    break;
                case (int)AIStates.BackToOwner:
                    break;
                case (int)AIStates.Idle:
                    break;
                case (int)AIStates.SpIdle:
                    break;
            }
        }

        public void OnSummon()
        {

        }

        public void BackToOwner()
        {

        }

        public void Idle()
        {

        }

        public void SpIdle()
        {

        }

        /// <summary>
        /// 被击打出去
        /// </summary>
        /// <param name="velocity"></param>
        public void BoostShoot(Vector2 velocity)
        {

        }

        #endregion

        #region Draw

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        #endregion
    }
}
