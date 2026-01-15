using Coralite.Content.Items.Materials;
using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
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
            CreateRecipe()
                .AddIngredient(ItemID.CopperBar, 12)
                .AddIngredient<MagicalPowder>(3)
                .AddIngredient(ItemID.VilePowder, 12)
                .AddTile<MagicCraftStation>()
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.CopperBar, 12)
                .AddIngredient<MagicalPowder>(3)
                .AddIngredient(ItemID.ViciousPowder, 12)
                .AddTile<MagicCraftStation>()
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.TinBar, 12)
                .AddIngredient<MagicalPowder>(3)
                .AddIngredient(ItemID.VilePowder, 12)
                .AddTile<MagicCraftStation>()
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.TinBar, 12)
                .AddIngredient<MagicalPowder>(3)
                .AddIngredient(ItemID.ViciousPowder, 12)
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class RhombicMirrorBuff : BaseAlchorthentBuff<RhombicMirrorProj>
    {
        public override string Texture => AssetDirectory.MinionBuffs+ "DefaultAlchorthentSeries";
    }

    /// <summary>
    /// 菱花镜召唤物，ai0控制是否强化形态
    /// </summary>
    public class RhombicMirrorProj : BaseAlchorthentMinion<RhombicMirrorBuff>
    {
        public override string Texture => AssetDirectory.Blank;

        /*
         * 神秘身体部分贴图
         */



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

    public class RhombicMirrorHeldProj
    {

    }

    public class AlchSymbolCopper
    {

    }

    public class AlchSymbolCorruption
    {

    }
}
