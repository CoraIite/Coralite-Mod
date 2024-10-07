﻿using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Items.Materials;
using Coralite.Content.Raritys;
using Coralite.Content.UI;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class BrilliantConnectStaff : ModItem,IMagikeCraftable
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public static List<MagikeLinerSender> Senders = new();

        public int DisconnectState;

        public static LocalizedText CurrentState { get; private set; }
        public static LocalizedText UIDisconnect { get; private set; }
        public static LocalizedText DisconnectAll { get; private set; }
        public static LocalizedText DisconnectSuccess { get; private set; }

        public override void Load()
        {
            CurrentState = this.GetLocalization(nameof(CurrentState));
            UIDisconnect = this.GetLocalization(nameof(UIDisconnect));
            DisconnectAll = this.GetLocalization(nameof(DisconnectAll));
            DisconnectSuccess = this.GetLocalization(nameof(DisconnectSuccess));
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = Item.useTime = 10;
            Item.useTurn = true;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0, 50);
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
            Item.GetMagikeItem().magikeAmount = 450;
            Item.channel = true;
        }

        public override bool AltFunctionUse(Player player) => true;
        public override bool CanRightClick() => true;
        public override bool ConsumeItem(Player player) => false;

        public override void RightClick(Player player)
        {
            DisconnectState = DisconnectState switch
            {
                0 => 1,
                _ => 0
            };
        }

        public override bool CanUseItem(Player player)
        {
            Point16 pos = Main.MouseWorld.ToTileCoordinates16();

            //左键寻找发送器
            Point16 topLeft = Main.MouseWorld.ToTileCoordinates16();

            //右键打开UI
            if (player.altFunctionUse == 2)
            {
                switch (DisconnectState)
                {
                    default:
                    case 0:
                        if (MagikeConnectUI.visible)
                            MagikeConnectUI.visible = false;
                        else if (MagikeHelper.TryGetEntityWithComponent<MagikeLinerSender>(pos.X, pos.Y, MagikeComponentID.MagikeSender, out MagikeTileEntity entity1))    //找到了
                        {
                            MagikeLinerSender senderComponent = entity1.GetSingleComponent<MagikeLinerSender>(MagikeComponentID.MagikeSender);

                            MagikeConnectUI.sender = senderComponent;
                            UILoader.GetUIState<MagikeConnectUI>()?.Recalculate();
                            MagikeConnectUI.visible = true;
                            Helper.PlayPitched("UI/Tick", 0.4f, 0, player.Center);
                        }
                        break;
                    case 1:
                        Projectile.NewProjectile(new EntitySource_ItemUse(Main.LocalPlayer, Main.LocalPlayer.HeldItem), topLeft.ToWorldCoordinates(8, 8),
                            Vector2.Zero, ModContent.ProjectileType<DisconnectProj>(), 0, 0, Main.myPlayer, topLeft.X, topLeft.Y);
                        break;
                }

                return true;
            }


            Senders.Clear();
            Projectile.NewProjectile(new EntitySource_ItemUse(Main.LocalPlayer, Main.LocalPlayer.HeldItem), topLeft.ToWorldCoordinates(8, 8),
                Vector2.Zero, ModContent.ProjectileType<BrilliantConnectStaffProj>(), 0, 0, Main.myPlayer, topLeft.X, topLeft.Y);

            Helper.PlayPitched("Fairy/FairyBottleClick2", 0.4f, 0, player.Center);

            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Coralite DisconnectState", CurrentState.Value + (DisconnectState == 1 ? DisconnectAll.Value : UIDisconnect.Value)));
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add(nameof(DisconnectState), DisconnectState);
        }

        public override void LoadData(TagCompound tag)
        {
            DisconnectState = tag.GetInt(nameof(DisconnectState));
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeCraftRecipe.CreateRecipe<MagConnectStaff, BrilliantConnectStaff>(MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 12, 60 * 2))
                .AddIngredient<CrystallineMagike>(5)
                .AddIngredient<Skarn>(20)
                .AddIngredient<TalantosInABottle>()
                .AddIngredient<ConcileInABottle>()
                .AddCondition(CoraliteConditions.LearnedMagikeAdvance)
                .Register();
        }
    }

    /// <summary>
    /// 使用ai0和ai1传入初始位置<br></br>
    /// ai2传入使用状态
    /// </summary>
    public class BrilliantConnectStaffProj : BaseHeldProj, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.Blank;

        public Point16 BasePosition
        {
            get => new((int)Projectile.ai[0], (int)Projectile.ai[1]);
            set
            {
                Projectile.ai[0] = value.X;
                Projectile.ai[1] = value.Y;
            }
        }

        public ref float State => ref Projectile.ai[2];
        public bool CanDrawFrame = true;

        public Point16 TargetPoint { get; set; }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 1200;
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = false;
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

            if (Owner.HeldItem.ModItem is not BrilliantConnectStaff)
            {
                Projectile.Kill();
                return;
            }

            switch (State)
            {
                default:
                case 0://选择连接
                    ChooseSenders();
                    break;
                case 1://选择接收者
                    ChooseReceivers();
                    break;
            }

        }

        public void ChooseSenders()
        {
            if (Owner.channel)
            {
                LockOwnerItemTime(5);
                Projectile.timeLeft = 2;

                TargetPoint = Main.MouseWorld.ToTileCoordinates16();

                //限制范围
                if (Math.Abs(TargetPoint.X - BasePosition.X) > GamePlaySystem.SelectSize)
                    TargetPoint = new Point16(Math.Clamp(TargetPoint.X, BasePosition.X - GamePlaySystem.SelectSize, BasePosition.X + GamePlaySystem.SelectSize), TargetPoint.Y);
                if (Math.Abs(TargetPoint.Y - BasePosition.Y) > GamePlaySystem.SelectSize)
                    TargetPoint = new Point16(TargetPoint.X, Math.Clamp(TargetPoint.Y, BasePosition.Y - GamePlaySystem.SelectSize, BasePosition.Y + GamePlaySystem.SelectSize));
            }
            else
            {
                if (FindSender())
                {
                    State = 1;
                    Projectile.timeLeft = 60 * 10;
                    CanDrawFrame = false;
                }
                else
                {
                    Projectile.Kill();
                }
                return;
            }

        }

        public void ChooseReceivers()
        {
            LockOwnerItemTime(5);

            if (Owner.controlUseItem)
            {
                if (Projectile.localAI[0] == 0)
                {
                    CanDrawFrame = true;
                    BasePosition = Main.MouseWorld.ToTileCoordinates16();
                    TargetPoint = BasePosition;
                    Projectile.localAI[0] = 1;
                    Helper.PlayPitched("Fairy/FairyBottleClick2", 0.4f, 0, Owner.Center);
                }

                Projectile.timeLeft = 2;

                TargetPoint = Main.MouseWorld.ToTileCoordinates16();

                //限制范围
                if (Math.Abs(TargetPoint.X - BasePosition.X) > GamePlaySystem.SelectSize)
                    TargetPoint = new Point16(Math.Clamp(TargetPoint.X, BasePosition.X - GamePlaySystem.SelectSize, BasePosition.X + GamePlaySystem.SelectSize), TargetPoint.Y);
                if (Math.Abs(TargetPoint.Y - BasePosition.Y) > GamePlaySystem.SelectSize)
                    TargetPoint = new Point16(TargetPoint.X, Math.Clamp(TargetPoint.Y, BasePosition.Y - GamePlaySystem.SelectSize, BasePosition.Y + GamePlaySystem.SelectSize));
            }
            else if (Projectile.localAI[0] == 1)
            {
                FindReceiverAndConnect();
                Projectile.Kill();
            }
        }

        public bool FindSender()
        {
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
                    if (!MagikeHelper.TryGetEntityWithComponent<MagikeLinerSender>(currentTopLeft.Value.X, currentTopLeft.Value.Y, MagikeComponentID.MagikeSender, out MagikeTileEntity entity))
                        continue;

                    //直接加入
                    BrilliantConnectStaff.Senders.Add(entity.GetSingleComponent<MagikeLinerSender>(MagikeComponentID.MagikeSender));
                    MagikeHelper.SpawnLozengeParticle_WithTopLeft(currentTopLeft.Value);
                }

            if (BrilliantConnectStaff.Senders.Count > 0)
            {
                Helper.PlayPitched("Fairy/CursorExpand", 0.4f, 0, Owner.Center);

                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Color = Coralite.MagicCrystalPink,
                    Text = MagikeSystem.GetConnectStaffText(MagikeSystem.StaffTextID.ChooseSender_Found),
                    DurationInFrames = 60,
                    Velocity = -Vector2.UnitY
                }, TargetPoint.ToWorldCoordinates());

                return true;
            }

            Helper.PlayPitched("UI/Error", 0.4f, 0, Owner.Center);

            PopupText.NewText(new AdvancedPopupRequest()
            {
                Color = Coralite.MagicCrystalPink,
                Text = MagikeSystem.GetConnectStaffText(MagikeSystem.StaffTextID.ChooseSender_NotFound),
                DurationInFrames = 60,
                Velocity = -Vector2.UnitY
            }, Main.MouseWorld - (Vector2.UnitY * 32));

            return false;
        }

        public void FindReceiverAndConnect()
        {
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
                    if (!MagikeHelper.TryGetEntity(currentTopLeft.Value, out MagikeTileEntity entity))
                        continue;

                    //能连接就连一下，不能就提供失败原因
                    foreach (var senderComponent in BrilliantConnectStaff.Senders)
                    {
                        Point16 pos = (senderComponent.Entity as MagikeTileEntity).Position;
                        if (!senderComponent.CanConnect(currentTopLeft.Value, out string failText))
                        {
                            PopupText.NewText(new AdvancedPopupRequest()
                            {
                                Color = Coralite.MagicCrystalPink,
                                Text = failText,
                                DurationInFrames = 60,
                                Velocity = -Vector2.UnitY
                            }, pos.ToWorldCoordinates() - (Vector2.UnitY * 32));
                        }
                        else
                        {
                            senderComponent.Connect(entity.Position);
                            PopupText.NewText(new AdvancedPopupRequest()
                            {
                                Color = Coralite.MagicCrystalPink,
                                Text = MagikeSystem.GetConnectStaffText(MagikeSystem.StaffTextID.Connect_Success),
                                DurationInFrames = 60,
                                Velocity = -Vector2.UnitY
                            }, pos.ToWorldCoordinates() - (Vector2.UnitY * 32));

                            MagikeHelper.SpawnLozengeParticle_WithTopLeft(pos);
                            MagikeHelper.SpawnLozengeParticle_WithTopLeft(entity.Position);
                            MagikeHelper.SpawnDustOnSend(pos, entity.Position);
                        }
                    }
                }

            Helper.PlayPitched("Fairy/CursorExpand", 0.4f, 0, Owner.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (BrilliantConnectStaff.Senders.Count > 0)
            {
                spriteBatch.End();
                spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);

                Point16 currentPoint = new((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
                Vector2 aimPos = currentPoint.ToWorldCoordinates();

                MagikeHelper.TryGetEntity(currentPoint.X, currentPoint.Y, out MagikeTileEntity receiver);

                if (receiver != null)
                {
                    currentPoint = receiver.Position;
                    aimPos = Helper.GetMagikeTileCenter(currentPoint);
                }

                foreach (var sender in BrilliantConnectStaff.Senders)
                {
                    Point16 pos = (sender.Entity as MagikeTileEntity).Position;

                    bool canConnect = sender.CanConnect(currentPoint, out _);
                    Color c = canConnect ? Color.GreenYellow : Color.MediumVioletRed;

                    MagikeSystem.DrawConnectLine(spriteBatch, Helper.GetMagikeTileCenter(pos), aimPos, Main.screenPosition, c);
                }

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, DepthStencilState.None, spriteBatch.GraphicsDevice.RasterizerState, null, Main.GameViewMatrix.TransformationMatrix);
            }

            return false;
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            if (CanDrawFrame)
                MagikeHelper.DrawRectangleFrame(spriteBatch, BasePosition, TargetPoint, Coralite.CrystallineMagikePurple);
        }
    }

    public class DisconnectProj : BaseHeldProj, IDrawNonPremultiplied
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

        public override void OnSpawn(IEntitySource source)
        {
            TargetPoint = BasePosition;
        }

        public override void AI()
        {
            Projectile.Center = Owner.Center;

            if (Owner.HeldItem.ModItem is not BrilliantConnectStaff)
            {
                Projectile.Kill();
                return;
            }

            if (Main.mouseRight)
            {
                LockOwnerItemTime(5);
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
                    if (!MagikeHelper.TryGetEntityWithComponent<MagikeLinerSender>(currentTopLeft.Value.X, currentTopLeft.Value.Y, MagikeComponentID.MagikeSender, out MagikeTileEntity entity))
                        continue;

                    //能插入就插，不能就提供失败原因
                    MagikeLinerSender sender = entity.GetSingleComponent<MagikeLinerSender>(MagikeComponentID.MagikeSender);
                    sender.Receivers.Clear();

                    PopupText.NewText(new AdvancedPopupRequest()
                    {
                        Color = Coralite.MagicCrystalPink,
                        Text = BrilliantConnectStaff.DisconnectSuccess.Value,
                        DurationInFrames = 60,
                        Velocity = -Vector2.UnitY
                    }, currentTopLeft.Value.ToWorldCoordinates());

                    placed = true;
                }

            if (placed)
                Helper.PlayPitched("Fairy/CursorExpand", 0.4f, 0, Owner.Center);
            else
                Helper.PlayPitched("UI/Error", 0.4f, 0, Owner.Center);
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
                MagikeHelper.DrawRectangleFrame(spriteBatch, BasePosition, TargetPoint, Coralite.CrystallineMagikePurple);
        }
    }
}