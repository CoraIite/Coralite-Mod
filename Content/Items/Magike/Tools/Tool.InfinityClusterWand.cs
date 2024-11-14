using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using InnoVault;
using System.IO;

namespace Coralite.Content.Items.Magike.Tools
{
    public class InfinityClusterWand : ModItem
    {
        public override string Texture => AssetDirectory.MagikeTools + Name;

        public static LocalizedText ChargeMode { get; private set; }
        public static LocalizedText ClearMode { get; private set; }

        public int mode;

        public override void Load()
        {
            ChargeMode = this.GetLocalization("ChargeMode", () => "充能模式");
            ClearMode = this.GetLocalization("ClearMode", () => "清空模式");
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = Item.useTime = 20;
            Item.useTurn = true;
            Item.shoot = ModContent.ProjectileType<InfinityClusterWandProj>();
            Item.value = Item.sellPrice(1);
            Item.rare = ItemRarityID.Red;

            Item.channel = true;
            Item.autoReuse = false;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                mode++;
                if (mode > 1)
                    mode = 0;

                string text = mode switch
                {
                    0 => ChargeMode.Value,
                    _ => ClearMode.Value
                };

                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Color = Color.Orange,
                    Text = text,
                    DurationInFrames = 60,
                    Velocity = -Vector2.UnitY
                }, Main.MouseWorld - (Vector2.UnitY * 32));

                return false;
            }

            Point16 basePoint = Main.MouseWorld.ToTileCoordinates16();
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, 0, 0, player.whoAmI, basePoint.X, basePoint.Y);

            Helper.PlayPitched("UI/Select", 0.4f, 0, player.Center);
            Main.NewText(basePoint);

            return false;
        }
    }

    /// <summary>
    /// 使用ai0和ai1传入初始位置
    /// </summary>
    public class InfinityClusterWandProj : BaseHeldProj, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.Blank;

        public Point16 BasePosition
        {
            get => new((int)Projectile.ai[0], (int)Projectile.ai[1]);
        }
        public override bool CanFire => true;
        private bool onspan;

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
                Projectile.ai[0] = InMousePos.ToTileCoordinates16().X;
                Projectile.ai[1] = InMousePos.ToTileCoordinates16().Y;
                TargetPoint = BasePosition;
                onspan = true;
            }

            Projectile.Center = Owner.Center;

            if (Owner.HeldItem.type != ModContent.ItemType<InfinityClusterWand>())
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
                    Charge(Owner, TargetPoint, BasePosition, -1);
                    if (VaultUtils.isClient)
                    {
                        //Send_ClusterWand_Data();
                    }
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

        internal void Send_ClusterWand_Data()
        {
            ModPacket modPacket = Coralite.Instance.GetPacket();
            modPacket.Write((byte)CLNetWorkEnum.ClusterWand);
            modPacket.Write(Owner.whoAmI);
            modPacket.WritePoint16(TargetPoint);
            modPacket.WritePoint16(BasePosition);
            modPacket.Write(-1);
            modPacket.Send();
        }

        internal static void Hander_ClusterWand(BinaryReader reader, int whoAmI)
        {
            int ownerIndex = reader.ReadInt32();
            Point16 TargetPoint = reader.ReadPoint16();
            Point16 BasePosition = reader.ReadPoint16();
            int amount = reader.ReadInt32();
            if (ownerIndex >= 0 && ownerIndex < Main.player.Length)
            {
                Player Owner = Main.player[ownerIndex];
                Charge(Owner, TargetPoint, BasePosition, amount);
                if (Main.dedServ)
                {
                    ModPacket modPacket = Coralite.Instance.GetPacket();
                    modPacket.Write((byte)CLNetWorkEnum.ClusterWand);
                    modPacket.Write(ownerIndex);
                    modPacket.WritePoint16(TargetPoint);
                    modPacket.WritePoint16(BasePosition);
                    modPacket.Write(amount);
                    modPacket.Send(-1, whoAmI);
                }
            }
        }

        public static void Charge(Player Owner, Point16 TargetPoint, Point16 BasePosition, int amount)
        {
            int mode = (Owner.HeldItem.ModItem as InfinityClusterWand).mode;

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
                    if (!MagikeHelper.TryGetEntityWithComponent(currentTopLeft.Value.X, currentTopLeft.Value.Y
                        , MagikeComponentID.MagikeContainer, out MagikeTP entity))
                        continue;

                    if (mode == 0)
                    {
                        if (amount == -1)
                        {
                            entity.GetMagikeContainer().FullChargeMagike();
                        }
                        else
                        {
                            entity.GetMagikeContainer().AddMagike(amount);
                        }
                    }
                    else
                    {
                        entity.GetMagikeContainer().ClearMagike();
                    }

                    if (!VaultUtils.isSinglePlayer)
                    {
                        entity.SendData();
                    }

                    MagikeHelper.SpawnLozengeParticle_WithTopLeft(currentTopLeft.Value);
                }

            Helper.PlayPitched("UI/GetSkill", 0.4f, 0, Owner.Center);
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            if (Owner.HeldItem.ModItem is InfinityClusterWand wand)
                MagikeHelper.DrawRectangleFrame(spriteBatch, BasePosition, TargetPoint, wand.mode == 0 ? Color.Orange : Color.DarkGray);
        }
    }
}
