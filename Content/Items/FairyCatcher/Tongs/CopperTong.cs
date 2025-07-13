using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Tongs
{
    public class CopperTong : BaseTongsItem
    {
        public override string Texture => AssetDirectory.FairyCatcherTong + Name;

        public override int CatchPower => 5;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<CopperTongProj>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 20;
            Item.shootSpeed = 9;
            Item.SetWeaponValues(12, 3);
            Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(0, 0, 20));
            Item.autoReuse = true;
            Item.holdStyle = ItemHoldStyleID.HoldFront;
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.FairyCatcherTong)]
    public class CopperTongProj : BaseTongsProj
    {
        public static ATex CopperTongChain { get; private set; }
        public static ATex CopperTongHandle { get; private set; }

        public override Vector2 TongPosOffset => new Vector2(24, -2);

        public override int MaxFlyLength => 16 * 7;

        public override Vector2 HandelOffset => new Vector2(20, -6);

        public override int ItemType => ModContent.ItemType<CopperTong>();

        public override Texture2D GetHandleTex() => CopperTongHandle.Value;
        public override Texture2D GetLineTex() => CopperTongChain.Value;

        public override Vector2 LineDrawStartPosOffset()
            => -HandleRot.ToRotationVector2() * 4;
    }
}
