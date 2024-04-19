using Coralite.Content.DamageClasses;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class BaseFairyCatcher : ModItem
    {
        public override string Texture => AssetDirectory.FairyCatcherItems + Name;

        /// <summary>
        /// 捕捉力
        /// </summary>
        public int catchPower;

        /// <summary>
        /// 上一次射出的仙灵的index
        /// </summary>
        public int currentFairyIndex;

        public sealed override void SetDefaults()
        {
            Item.DamageType = ModContent.GetInstance<FairyDamage>();


        }

        public virtual void SetOtherDefaults() { }

        public override bool CanRightClick() => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse==2)
            {
                ShootCatcherProjectile(player, source, position, velocity, type);
                return false;
            }

            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
            {
                if (fcp.FairyShoot_GetFairyBottle(out IFairyBottle bottle))
                {
                    ShootFairy(bottle, source, position, velocity);
                    return false;
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// 右键使用时发射捕捉器的弹幕
        /// </summary>
        /// <param name="player"></param>
        /// <param name="source"></param>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        /// <param name="type"></param>
        public virtual void ShootCatcherProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type)
        {
            Projectile.NewProjectile(source, position, velocity, type, 0, 0, player.whoAmI);
        }

        /// <summary>
        /// 左键使用时依次发射
        /// </summary>
        /// <param name="bottle"></param>
        /// <param name="source"></param>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        public virtual void ShootFairy(IFairyBottle bottle, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity)
        {
            Item[] fairies= bottle.Fairies;

            for (int i = 0; i < fairies.Length; i++)
            {
                Item item = fairies[currentFairyIndex];
                if (item.ModItem is IFairyItem)
                {

                }

                currentFairyIndex++;
                if (currentFairyIndex > fairies.Length - 1)
                {
                    currentFairyIndex = 0;
                }
            }
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
        }

        public override void Load()
        {
            base.Load();
        }
    }
}
