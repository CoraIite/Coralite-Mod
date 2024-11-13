using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Particles;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Core.Systems.MagikeSystem.BaseItems
{
    public abstract class FilterItem : ModItem
    {
        public abstract Color FilterColor { get; }

        /// <summary>
        /// 获取滤镜组件
        /// </summary>
        /// <returns></returns>
        public abstract MagikeFilter GetFilterComponent();

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 20;

            Item.maxStack = Item.CommonMaxStack;
            Item.channel = true;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<FilterProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Point16 basePoint = Main.MouseWorld.ToTileCoordinates16();
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, 0, 0, player.whoAmI, basePoint.X, basePoint.Y);

            Helper.PlayPitched("UI/Select", 0.4f, 0, player.Center);
            return false;
        }
    }

    /// <summary>
    /// 使用ai0和ai1传入初始位置
    /// </summary>
    public class FilterProj : BaseHeldProj, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.Blank;
        private bool onspan;

        public Point16 BasePosition
        {
            get => new((int)Projectile.ai[0], (int)Projectile.ai[1]);
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

        public override void AI()
        {
            if (!onspan)
            {
                TargetPoint = BasePosition;
                onspan = true;
            }

            Projectile.Center = Owner.Center;

            if (Owner.HeldItem.ModItem is not FilterItem)
            {
                Projectile.Kill();
                return;
            }

            if (DownLeft)
            {
                Owner.itemTime = Owner.itemAnimation = 7;
                TargetPoint = InMousePos.ToTileCoordinates16();

                //限制范围
                if (Math.Abs(TargetPoint.X - BasePosition.X) > GamePlaySystem.SelectSize)
                    TargetPoint = new Point16(Math.Clamp(TargetPoint.X, BasePosition.X - GamePlaySystem.SelectSize, BasePosition.X + GamePlaySystem.SelectSize), TargetPoint.Y);
                if (Math.Abs(TargetPoint.Y - BasePosition.Y) > GamePlaySystem.SelectSize)
                    TargetPoint = new Point16(TargetPoint.X, Math.Clamp(TargetPoint.Y, BasePosition.Y - GamePlaySystem.SelectSize, BasePosition.Y + GamePlaySystem.SelectSize));
            }
            else
            {
                PlaceFilter();
                Projectile.Kill();
                return;
            }

            //右键直接停止使用
            if (DownRight)
            {
                Projectile.Kill();
                return;
            }
        }

        public void PlaceFilter()
        {
            MagikeFilter filter = (Owner.HeldItem.ModItem as FilterItem).GetFilterComponent();

            bool placed = false;

            int baseX = Math.Min(TargetPoint.X, BasePosition.X);
            int baseY = Math.Min(TargetPoint.Y, BasePosition.Y);

            int xLength = Math.Abs(TargetPoint.X - BasePosition.X) + 1;
            int yLength = Math.Abs(TargetPoint.Y - BasePosition.Y) + 1;

            HashSet<Point16> insertPoint = new();

            //遍历一个矩形区域，并直接检测该位置是否有魔能仪器的物块实体
            for (int j = baseY; j < baseY + yLength; j++)
                for (int i = baseX; i < baseX + xLength; i++)
                {
                    //遍历并获取左上角
                    Point16? currentTopLeft = MagikeHelper.ToTopLeft(i, j);

                    //没有物块就继续往下遍历
                    if (!currentTopLeft.HasValue)
                        continue;

                    //把左上角加入hashset中，如果左上角已经出现过那么就跳过
                    if (insertPoint.Contains(currentTopLeft.Value))
                        continue;

                    insertPoint.Add(currentTopLeft.Value);

                    //尝试根据左上角获取物块实体
                    if (!MagikeHelper.TryGetEntity(currentTopLeft.Value, out MagikeTP entity))
                        continue;

                    //能插入就插，不能就提供失败原因
                    if (filter.CanInsert(entity, out string text))
                    {
                        placed = true;
                        filter.Insert(entity);
                        filter = (Owner.HeldItem.ModItem as FilterItem).GetFilterComponent();

                        //特效部分
                        TileRenewalController.Spawn(currentTopLeft.Value, (Owner.HeldItem.ModItem as FilterItem).FilterColor);

                        //消耗滤镜
                        Owner.HeldItem.stack--;
                        if (Owner.HeldItem.stack <= 0)
                        {
                            Owner.HeldItem.TurnToAir();
                            goto PlaceOver;
                        }
                    }
                    else if (!string.IsNullOrEmpty(text))
                        PopupText.NewText(new AdvancedPopupRequest()
                        {
                            Color = Coralite.MagicCrystalPink,
                            Text = text,
                            DurationInFrames = 60,
                            Velocity = -Vector2.UnitY
                        }, Helper.GetMagikeTileCenter(currentTopLeft.Value));
                }

            PlaceOver:

            if (placed)
            {
                Helper.PlayPitched("UI/GetSkill", 0.4f, 0, Owner.Center);

                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Color = Coralite.MagicCrystalPink,
                    Text = MagikeSystem.GetFilterText(MagikeSystem.FilterID.InsertSuccess),
                    DurationInFrames = 60,
                    Velocity = -Vector2.UnitY
                }, TargetPoint.ToWorldCoordinates());
            }
            else
                Helper.PlayPitched("UI/Error", 0.4f, 0, Owner.Center);
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            if (Owner.HeldItem.ModItem is FilterItem filterItem)
                MagikeHelper.DrawRectangleFrame(spriteBatch, BasePosition, TargetPoint, filterItem.FilterColor);
        }
    }
}
