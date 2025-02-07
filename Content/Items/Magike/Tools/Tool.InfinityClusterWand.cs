using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

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
    public class InfinityClusterWandProj : RectangleSelectProj
    {
        public override int ItemType => ModContent.ItemType<InfinityClusterWand>();

        public override Color GetDrawColor()
        {
            return (Item.ModItem as InfinityClusterWand).mode == 0 ? Color.Orange : Color.DarkGray;
        }

        public override void Special()
        {
            SearchFromArea(Owner, TargetPoint, BasePosition, FillMagike);
            Helper.PlayPitched("UI/GetSkill", 0.4f, 0, Owner.Center);
        }

        internal void Send_ClusterWand_Data()
        {
            ModPacket modPacket = Coralite.Instance.GetPacket();
            modPacket.Write((byte)CoraliteNetWorkEnum.ClusterWand);
            modPacket.Write(Owner.whoAmI);
            modPacket.WritePoint16(TargetPoint);
            modPacket.WritePoint16(BasePosition);
            modPacket.Send();
        }

        internal static void Hander_ClusterWand(BinaryReader reader, int whoAmI)
        {
            int ownerIndex = reader.ReadInt32();
            Point16 TargetPoint = reader.ReadPoint16();
            Point16 BasePosition = reader.ReadPoint16();
            if (ownerIndex >= 0 && ownerIndex < Main.player.Length)
            {
                Player Owner = Main.player[ownerIndex];
                SearchFromArea(Owner, TargetPoint, BasePosition, FillMagike);
                if (Main.dedServ)
                {
                    ModPacket modPacket = Coralite.Instance.GetPacket();
                    modPacket.Write((byte)CoraliteNetWorkEnum.ClusterWand);
                    modPacket.Write(ownerIndex);
                    modPacket.WritePoint16(TargetPoint);
                    modPacket.WritePoint16(BasePosition);
                    modPacket.Send(-1, whoAmI);
                }
            }
        }

        public static void FillMagike(Player owner, HashSet<Point16> insertPoint, int j, int i)
        {
            Point16? currentTopLeft = MagikeHelper.ToTopLeft(i, j);

            //没有物块就继续往下遍历
            if (!currentTopLeft.HasValue)
                return;

            //把左上角加入hashset中，如果左上角已经出现过那么就跳过
            if (insertPoint.Contains(currentTopLeft.Value))
                return;

            insertPoint.Add(currentTopLeft.Value);

            //尝试根据左上角获取物块实体
            if (!MagikeHelper.TryGetEntityWithComponent(currentTopLeft.Value.X, currentTopLeft.Value.Y
                , MagikeComponentID.MagikeContainer, out MagikeTP entity))
                return;

            int mode = (owner.HeldItem.ModItem as InfinityClusterWand).mode;

            if (mode == 0)
                entity.GetMagikeContainer().FullChargeMagike();
            else
                entity.GetMagikeContainer().ClearMagike();

            if (!VaultUtils.isSinglePlayer)
            {
                entity.SendData();
            }

            MagikeHelper.SpawnLozengeParticle_WithTopLeft(currentTopLeft.Value);
        }
    }
}
