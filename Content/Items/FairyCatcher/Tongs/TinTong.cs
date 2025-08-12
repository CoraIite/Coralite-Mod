using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Tongs
{
    public class TinTong : BaseTongsItem
    {
        public override string Texture => AssetDirectory.FairyCatcherTong + Name;

        public override int CatchPower => 5;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<TinTongProj>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 20;
            Item.shootSpeed = 10;
            Item.SetWeaponValues(12, 3);
            Item.SetShopValues(ItemRarityColor.Blue1, Item.sellPrice(0, 0, 20));
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TinBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.FairyCatcherTong)]
    public class TinTongProj : BaseTongsProj
    {
        public static ATex CopperTongChain { get; private set; }
        public static ATex TinTongHandle { get; private set; }
        public static ATex CopperTongHandleHead { get; private set; }

        public ref float HandleHeadRot => ref Projectile.localAI[2];

        public override Vector2 TongPosOffset => new Vector2(24, -2);

        public override int MaxFlyLength => 16 * 7;

        public override Vector2 HandelOffset => new Vector2(20, -6);

        public override int ItemType => ModContent.ItemType<TinTong>();

        public override void Flying()
        {
            base.Flying();

            HandleHeadRot += MathF.Sign(Projectile.velocity.X) * 0.66f;
        }

        public override void Backing()
        {
            base.Backing();

            HandleHeadRot -= MathF.Sign(Projectile.velocity.X) * 0.66f;
        }

        public override Texture2D GetHandleTex() => TinTongHandle.Value;
        public override Texture2D GetLineTex() => CopperTongChain.Value;

        public override Vector2 LineDrawStartPosOffset()
            => -HandleRot.ToRotationVector2() * 4;

        public override void DrawHandle(Texture2D handleTex, Vector2 pos, Color lightColor)
        {
            base.DrawHandle(handleTex, pos, lightColor);

            Vector2 offset = -HandleRot.ToRotationVector2() * 11 + (HandleRot - Owner.direction * MathHelper.PiOver2).ToRotationVector2() * 4;

            Main.spriteBatch.Draw(CopperTongHandleHead.Value, pos - Main.screenPosition + offset
                , null, lightColor, HandleRot + HandleHeadRot + (Owner.direction > 0 ? 0 : MathHelper.Pi), new Vector2(CopperTongHandleHead.Width() / 2, CopperTongHandleHead.Height())
                , Projectile.scale, Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        }
    }
}
