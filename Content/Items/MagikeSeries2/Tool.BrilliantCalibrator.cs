using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Items.Materials;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class BrilliantCalibrator : ModItem, IMagikeCraftable
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = Item.useTime = 10;
            Item.useTurn = true;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
            Item.channel = true;
        }

        public override bool CanUseItem(Player player)
        {
            Point16 basePoint = Main.MouseWorld.ToTileCoordinates16();
            Projectile.NewProjectile(new EntitySource_ItemUse(Main.LocalPlayer, Main.LocalPlayer.HeldItem), basePoint.ToWorldCoordinates(8, 8),
                Vector2.Zero, ModContent.ProjectileType<BrilliantCalibratorProj>(), 0, 0, Main.myPlayer, basePoint.X, basePoint.Y);

            Helper.PlayPitched("UI/Select", 0.4f, 0, player.Center);

            return true;
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<OpticalPathCalibrator, BrilliantCalibrator>(MagikeHelper.CalculateMagikeCost<BrilliantLevel>( 12, 60 * 2))
                .AddIngredient<CrystallineMagike>(5)
                .AddIngredient<Skarn>(20)
                .AddIngredient<MutatusInABottle>()
                .AddCondition(CoraliteConditions.LearnedMagikeAdvance)
                .Register();
        }
    }

    public class BrilliantCalibratorProj : BaseHeldProj, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.Blank;

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

        public override void Initialize()
        {
            TargetPoint = BasePosition;
        }

        public override void AI()
        {
            Projectile.Center = Owner.Center;

            if (Item.ModItem is not BrilliantCalibrator)
            {
                Projectile.Kill();
                return;
            }

            if (Owner.channel)
            {
                Owner.itemTime = Owner.itemAnimation = 5;
                TargetPoint = Main.MouseWorld.ToTileCoordinates16();

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
                    if (!MagikeHelper.TryGetEntityWithTopLeft(currentTopLeft.Value, out MagikeTP entity))
                        continue;

                    //能插入就插，不能就提供失败原因
                    foreach (var component in entity.ComponentsCache)
                    {
                        if (component is ITimerTriggerComponent timer && timer.TimeResetable)
                        {
                            timer.Timer = timer.Delay;
                            placed = true;
                            PopupText.NewText(new AdvancedPopupRequest()
                            {
                                Color = Coralite.MagicCrystalPink,
                                Text = OpticalPathCalibrator.TimerSynced.Value,
                                DurationInFrames = 60,
                                Velocity = -Vector2.UnitY
                            }, currentTopLeft.Value.ToWorldCoordinates());
                        }
                    }
                }

            if (placed)
                Helper.PlayPitched("Fairy/FairyBottleClick2", 0.4f, 0, Owner.Center);
            else
            {
                Helper.PlayPitched("UI/Error", 0.4f, 0, Owner.Center);

                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Color = Coralite.MagicCrystalPink,
                    Text = OpticalPathCalibrator.TimerNotFound.Value,
                    DurationInFrames = 60,
                    Velocity = -Vector2.UnitY
                }, TargetPoint.ToWorldCoordinates());
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            MagikeHelper.DrawRectangleFrame(spriteBatch, BasePosition, TargetPoint, Coralite.CrystallinePurple);
        }
    }
}
