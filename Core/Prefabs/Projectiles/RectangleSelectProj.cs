using Coralite.Content.Items.Magike.Tools;
using Coralite.Core.Configs;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Prefabs.Projectiles
{
    public abstract class RectangleSelectProj : BaseHeldProj, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.Blank;

        public Point16 BasePosition
        {
            get => new((int)Projectile.ai[0], (int)Projectile.ai[1]);
        }

        public override bool CanFire => true;
        protected bool onspan;

        public Point16 TargetPoint { get; set; }

        public virtual int ItemType => -1;

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
                Projectile.ai[0] = InMousePos.ToTileCoordinates16().X;
                Projectile.ai[1] = InMousePos.ToTileCoordinates16().Y;
                TargetPoint = BasePosition;
                onspan = true;
            }

            Projectile.Center = Owner.Center;

            if (ItemType != -1 && Owner.HeldItem.type != ItemType)
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
                if (Projectile.IsOwnedByLocalPlayer())
                {
                    Special();
                }

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

        /// <summary>
        /// 选择完松手后的特殊操作
        /// </summary>
        public virtual void Special()
        {

        }

        public static void SearchFromArea(Player owner, Point16 targetPoint, Point16 basePosition, Action<Player, HashSet<Point16>, int, int> tileCheck)
        {
            int baseX = Math.Min(targetPoint.X, basePosition.X);
            int baseY = Math.Min(targetPoint.Y, basePosition.Y);

            int xLength = Math.Abs(targetPoint.X - basePosition.X) + 1;
            int yLength = Math.Abs(targetPoint.Y - basePosition.Y) + 1;

            HashSet<Point16> insertPoint = new();
            for (int j = baseY; j < baseY + yLength; j++)
                for (int i = baseX; i < baseX + xLength; i++)
                {
                    tileCheck(owner, insertPoint, j, i);
                }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            if (Owner.HeldItem.ModItem is InfinityClusterWand wand)
                MagikeHelper.DrawRectangleFrame(spriteBatch, BasePosition, TargetPoint, wand.mode == 0 ? Color.Orange : Color.DarkGray);
        }

    }
}
