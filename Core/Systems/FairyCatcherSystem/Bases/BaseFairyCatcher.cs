using Coralite.Content.DamageClasses;
using Coralite.Content.ModPlayers;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class BaseFairyCatcher : ModItem
    {
        public override string Texture => AssetDirectory.FairyCatcherItems + Name;

        /// <summary>
        /// 捕捉力
        /// </summary>
        public abstract int CatchPower { get; }

        /// <summary>
        /// 前缀的捕捉力加成
        /// </summary>
        public float CatchPowerMult { get; set; } = 1;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = true;
        }

        public sealed override void SetDefaults()
        {
            Item.DamageType = FairyDamage.Instance;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            SetOtherDefaults();
        }

        public virtual void SetOtherDefaults() { }

        #region 攻击部分

        /// <summary> 右键的弹幕ID </summary>
        public abstract int RightProjType { get; }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp) && cp.useSpecialAttack)
            {
                ShootCircle(player, source, position);
                return false;
            }

            if (player.altFunctionUse == 2)
            {
                ShootCatcher(player, source, position, velocity, RightProjType);
                return false;
            }


            return false;
        }

        /// <summary>
        /// 生成捕捉器弹幕
        /// </summary>
        /// <param name="player"></param>
        /// <param name="source"></param>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        /// <param name="type"></param>
        public virtual void ShootCircle(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position)
        {
            int type = ModContent.ProjectileType<FairyCatcherProj>();
            if (player.ownedProjectileCounts[type] > 0)
            {
                foreach (var proj in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == type))
                    (proj.ModProjectile as FairyCatcherProj).TrunToBacking();

                return;
            }

            Projectile.NewProjectile(source, position, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero) * 16
                , type, 0, 0, player.whoAmI);
        }

        /// <summary>
        /// 右键使用时发射捕捉器的弹幕
        /// </summary>
        /// <param name="player"></param>
        /// <param name="source"></param>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        /// <param name="type"></param>
        public virtual void ShootCatcher(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type)
        {
            Projectile.NewProjectile(source, position, velocity, type, 0, 0, player.whoAmI);
        }

        /// <summary>
        /// 自定义仙灵的攻击方式
        /// </summary>
        public virtual void ModifyFairyProjectile(BaseFairyProjectile fairyProjectile)
        {

        }

        #endregion

        #region IO

        public override void SaveData(TagCompound tag)
        {
            tag.Add(nameof(CatchPowerMult), CatchPowerMult);
        }

        public override void LoadData(TagCompound tag)
        {
            CatchPowerMult = tag.GetFloat(nameof(CatchPowerMult));
        }

        #endregion

    }
}
