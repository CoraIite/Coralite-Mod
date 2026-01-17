using Coralite.Core;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.AlchorthentSeries
{
    public class RhombicMirror : BaseAlchorthentItem
    {
        public override void SetOtherDefaults()
        {
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 30;
            //Item.shoot = ModContent.ProjectileType<FaintEagleProj>();

            Item.SetWeaponValues(24, 4);
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(0, 0, 50));

            Item.useTurn = false;
        }

        public override void Summon(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Projectile.NewProjectile(source, player.Center + new Vector2(player.direction * 20, 0), new Vector2(player.direction * 4, -8), type, damage, knockback, player.whoAmI, 1);

            //Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<FaintEagleHeldProj>(), damage, knockback, player.whoAmI, 0);

            //player.AddBuff(ModContent.BuffType<FaintEagleBuff>(), 60);

            //Helper.PlayPitched(CoraliteSoundID.SummonStaff_Item44, player.Center);
            //Helper.PlayPitched(CoraliteSoundID.FireBallExplosion_DD2_BetsyFireballImpact, player.Center, pitchAdjust: 0.4f);
        }

        public override void MinionAim(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<FaintEagleHeldProj>(), damage, knockback, player.whoAmI, 0);
        }

        public override void SpecialAttack(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //if (!player.CheckMana(1, true, true))
            //    return;

            //player.manaRegenDelay = 40;

            //Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<FaintEagleHeldProj>(), damage, knockback, player.whoAmI, 1);
        }

        public override void AddRecipes()
        {
            //CreateRecipe()
            //    .AddIngredient(ItemID.CopperBar, 12)
            //    .AddIngredient<MagicalPowder>(3)
            //    .AddIngredient(ItemID.VilePowder, 12)
            //    .AddTile<MagicCraftStation>()
            //    .Register();
            //CreateRecipe()
            //    .AddIngredient(ItemID.CopperBar, 12)
            //    .AddIngredient<MagicalPowder>(3)
            //    .AddIngredient(ItemID.ViciousPowder, 12)
            //    .AddTile<MagicCraftStation>()
            //    .Register();

            //CreateRecipe()
            //    .AddIngredient(ItemID.TinBar, 12)
            //    .AddIngredient<MagicalPowder>(3)
            //    .AddIngredient(ItemID.VilePowder, 12)
            //    .AddTile<MagicCraftStation>()
            //    .Register();
            //CreateRecipe()
            //    .AddIngredient(ItemID.TinBar, 12)
            //    .AddIngredient<MagicalPowder>(3)
            //    .AddIngredient(ItemID.ViciousPowder, 12)
            //    .AddTile<MagicCraftStation>()
            //    .Register();
        }
    }

    public class RhombicMirrorBuff : BaseAlchorthentBuff<RhombicMirrorProj>
    {
        public override string Texture => AssetDirectory.MinionBuffs + "DefaultAlchorthentSeries";
    }

    /// <summary>
    /// 召唤和右键时出现的手持弹幕<br></br>
    /// ai0传入类型，0表示召唤，1表示锁定
    /// </summary>
    public class RhombicMirrorHeldProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + "RhombicMirror";

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = Projectile.height = 16;
        }

        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            if (Owner.dead || Owner.HeldItem.type != ModContent.ItemType<RhombicMirror>())
            {
                Projectile.Kill();
                return;
            }

            SetHeld();

            if (State == 0)
                Summon();
            else
                AimTarget();
        }

        public void Summon()
        {
            /*
             * 光效展开后召唤物在人物背后缓慢旋转出现
             */
            Projectile.Center = Owner.Center + new Vector2(Owner.direction * 12, 0);
            Owner.itemTime = Owner.itemAnimation = 2;

            if (Timer == 0)//生成光效
            {

            }

            if (Timer>45)
            {
                Projectile.Kill();
            }

            Timer++;
        }

        public void AimTarget()
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }

    /// <summary>
    /// 可以变成扇形的激光粒子
    /// </summary>
    public abstract class RhombicMirrorLaserParticle : Particle
    {
        //public int OwnerProjIndex;
        /// <summary>
        /// 激光长度
        /// </summary>
        public float LaserLength;
        /// <summary>
        /// 激光的扇形张角
        /// </summary>
        public float LaserAngleOffset;
        /// <summary>
        /// 激光宽度，建议不变
        /// </summary>
        public float LaserWidth;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            CoraliteSystem.InitBars();

            Texture2D Texture = TexValue;

            Vector2 pos = Position - Main.screenPosition;
            Vector2 normal = (Rotation - MathHelper.PiOver2).ToRotationVector2();
            pos -= normal * LaserWidth;

            for (int i = 0; i <= 12; i++)
            {
                float factor = (float)i / 12;

                Vector2 dir = Helper.Lerp(LaserAngleOffset, -LaserAngleOffset, factor).ToRotationVector2();
                Vector2 Top = pos + normal * LaserWidth * 2 / 12f;
                Vector2 Bottom = Top + dir * LaserLength;

                CoraliteSystem.Vertexes.Add(new(Top, Color, new Vector3(0, factor,  0)));
                CoraliteSystem.Vertexes.Add(new(Bottom, Color, new Vector3(1, factor,  0)));
            }

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, CoraliteSystem.Vertexes.ToArray(), 0, CoraliteSystem.Vertexes.Count - 2);

            return false;
        }
    }

    /// <summary>
    /// 菱花镜召唤物，ai0控制是否强化形态
    /// </summary>
    [VaultLoaden(AssetDirectory.AlchorthentSeriesItems)]
    public class RhombicMirrorProj : BaseAlchorthentMinion<RhombicMirrorBuff>
    {
        /*
         * 神秘身体部分贴图
         * 1~17帧为伴随着攻击变得生锈，18~37帧为特殊攻击清除生锈
         */
        /// <summary> 下面的头，上面的尾巴，右上后腿，左上后腿 </summary>
        public static ATex RhombicMirrorProjPart1 { get; set; }
        /// <summary> 右下前腿，左下前腿 </summary>
        public static ATex RhombicMirrorProjPart2 { get; set; }

        /// <summary>
        /// 攻击次数
        /// </summary>
        public short attackCount;

        private enum AIStates : byte
        {
            /// <summary> 刚召唤出来 </summary>
            OnSummon,
            /// <summary> 飞回玩家的过程 </summary>
            BackToOwner,
            /// <summary> 在玩家身边 </summary>
            Idle,
            /// <summary> 特殊静止状态 </summary>
            SPIdle,
            /// <summary> 射光束 </summary>
            Shoot,
            /// <summary> 腐蚀光束 </summary>
            CorruptedShoot,
        }

        public override void SetOtherDefault()
        {
            Projectile.tileCollide = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1;
            Projectile.width = Projectile.height = 46;
            Projectile.decidesManualFallThrough = true;
            Projectile.localNPCHitCooldown = 10;
        }

        #region AI

        public override void Initialize()
        {

        }

        public override void AIMoves()
        {

        }

        public void OnSummon()
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

    public class AlchSymbolCopper
    {

    }

    public class AlchSymbolCorruption
    {

    }
}
