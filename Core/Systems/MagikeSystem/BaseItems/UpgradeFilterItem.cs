using Coralite.Core.Prefabs.Projectiles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Core.Systems.MagikeSystem.BaseItems
{
    public abstract class UpgradeFilterItem( int value, int rare, string texturePath = AssetDirectory.MagikeItems, bool pathHasName = false) : ModItem
    {
        public override string Texture => string.IsNullOrEmpty(texturePath) ? base.Texture : texturePath + (pathHasName ? string.Empty : Name);

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 20;
            Item.value = value;
            Item.rare = rare;

            Item.channel = true;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<FilterProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            return false;
       }
    }

    /// <summary>
    /// 使用ai0和ai1传入初始位置
    /// </summary>
    public class FilterProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.Blank;

        public Point16 BasePosition
        {
            get => new Point16((int)Projectile.ai[0], (int)Projectile.ai[1]);
        }

        public Point16 TargetPoint { get; set; }
        
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
        }

        public override bool? CanDamage() => false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

        public override void OnSpawn(IEntitySource source)
        {
            TargetPoint = BasePosition;
        }

        public override void AI()
        {
            Projectile.Center = Owner.Center;

            if (Owner.channel)
            {
                Owner.itemTime = Owner.itemAnimation = 5;
                TargetPoint = Main.MouseWorld.ToTileCoordinates16();

                if (Math.Abs(TargetPoint.X - BasePosition.X) > 10)
                    TargetPoint = new Point16(Math.Clamp(TargetPoint.X, BasePosition.X - 10, BasePosition.X + 10), TargetPoint.Y);
                if (Math.Abs(TargetPoint.Y - BasePosition.Y) > 10)
                    TargetPoint = new Point16(TargetPoint.X, Math.Clamp(TargetPoint.Y, BasePosition.Y - 10, BasePosition.Y + 10));
            }
            else
            {
                PlaceFilter();
                Projectile.Kill();
                return;
            }

            //右键直接停止使用
            if (Main.mouseRight)
            {
                Projectile.Kill();
                return;
            }
        }

        public void PlaceFilter()
        {
            bool placed = false;

            //当只有一格时，才会获取物块左上角
            if (BasePosition == TargetPoint)
            {
                return;
            }

            int baseX=Math.Min(TargetPoint.X, BasePosition.X);
            int baseY=Math.Min(TargetPoint.Y, BasePosition.Y);

            int xLength = Math.Abs(TargetPoint.X - BasePosition.X);
            int yLength = Math.Abs(TargetPoint.Y - BasePosition.Y);

            //遍历一个矩形区域，并直接检测该位置是否有魔能仪器的物块实体
            for (int j = 0; j < yLength; j++)
                for (int i = 0; i < xLength; i++)
                {

                }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}
