using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Items.Materials;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class BrilliantMagikeActivator : ModItem, IMagikeCraftable
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
            Item.GetMagikeItem().magikeAmount = 450;
            Item.channel = true;
        }

        public override bool CanUseItem(Player player)
        {
            Point16 basePoint = Main.MouseWorld.ToTileCoordinates16();
            Projectile.NewProjectile(new EntitySource_ItemUse(Main.LocalPlayer, Main.LocalPlayer.HeldItem), basePoint.ToWorldCoordinates(8, 8),
                Vector2.Zero, ModContent.ProjectileType<BrilliantMagikeActivatorProj>(), 0, 0, Main.myPlayer, basePoint.X, basePoint.Y);

            Helper.PlayPitched("UI/Select", 0.4f, 0, player.Center);

            return true;
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeCraftRecipe.CreateRecipe<MagikeActivator, BrilliantMagikeActivator>(MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 12, 60 * 2))
                .AddIngredient<CrystallineMagike>(5)
                .AddIngredient<Skarn>(20)
                .AddIngredient<LeohtInABottle>()
                .AddCondition(CoraliteConditions.LearnedMagikeAdvance)
                .Register();
        }
    }

    public class BrilliantMagikeActivatorProj : RectangleSelectProj
    {
        public override int ItemType => ModContent.ItemType<BrilliantMagikeActivator>();

        public override void Special()
        {
            Actractive();
        }

        public void Actractive()
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
                    if (!MagikeHelper.TryGetEntityWithComponent<MagikeFactory>(currentTopLeft.Value.X, currentTopLeft.Value.Y, MagikeComponentID.MagikeFactory, out MagikeTP entity))
                        continue;

                    //激活
                    MagikeFactory factory = entity.GetSingleComponent<MagikeFactory>(MagikeComponentID.MagikeFactory);

                    if (factory.Activation(out string text))
                        PopupText.NewText(new AdvancedPopupRequest()
                        {
                            Color = Coralite.MagicCrystalPink,
                            Text = text,
                            DurationInFrames = 60,
                            Velocity = -Vector2.UnitY
                        }, TargetPoint.ToWorldCoordinates());

                    placed = true;
                }

            if (placed)
                Helper.PlayPitched("UI/Activation", 0.4f, 0, Owner.Center);
            else
            {
                Helper.PlayPitched("UI/Error", 0.4f, 0, Owner.Center);

                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Color = Coralite.MagicCrystalPink,
                    Text = MagikeSystem.GetConnectStaffText(MagikeSystem.StaffTextID.FactoryNotFound),
                    DurationInFrames = 60,
                    Velocity = -Vector2.UnitY
                }, TargetPoint.ToWorldCoordinates());
            }
        }
    }
}
