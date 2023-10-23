using Coralite.Content.ModPlayers;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    [AutoloadEquip(EquipType.HandsOn)]
    public class BoneRing : ModItem, INightmareWeapon
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 45;
            Item.reuseDelay = 20;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.DamageType = DamageClass.Ranged;
            Item.rare = RarityType<NightmareRarity>();
            Item.value = Item.sellPrice(2, 0, 0, 0);
            Item.SetWeaponValues(320, 4, 4);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTurn = false;
            Item.shootSpeed = 24;
        }

        public override void HoldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.equippedBoneRing = true;

            player.handon = 25;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
    }

    public class BoneRingDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.HandOnAcc);
        }

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.handon == EquipLoader.GetEquipSlot(Mod, "BoneRing", EquipType.HandsOn);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Vector2 pos = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (drawInfo.drawPlayer.bodyFrame.Width / 2) + (drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f))
                + drawInfo.drawPlayer.bodyPosition
                + drawInfo.drawPlayer.bodyFrame.Size() / 2;
            DrawData item = new DrawData(Request<Texture2D>(AssetDirectory.NightmareItems + "BoneRing_HandsOn").Value,
                pos,
                drawInfo.drawPlayer.bodyFrame,
                Color.White/*drawInfo.colorArmorBody*/,
                drawInfo.drawPlayer.bodyRotation,
                drawInfo.bodyVect, 1f,
                drawInfo.playerEffect);
            item.shader = drawInfo.cHandOn;
            drawInfo.DrawDataCache.Add(item);
        }
    }

}
