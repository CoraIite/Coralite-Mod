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

        public override void SetDefaults()
        {
            Item.DamageType = ModContent.GetInstance<FairyDamage>();


        }

        public override bool CanRightClick() => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
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
