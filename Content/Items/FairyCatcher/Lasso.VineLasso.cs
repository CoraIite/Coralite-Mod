﻿using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher
{
    public class VineLasso : BaseFairyCatcher
    {
        public override string Texture => AssetDirectory.FairyCatcherItems + Name;

        public override int CatchPower => 5;
        public override int RightProjType => ModContent.ProjectileType<VineLassoSwing>();

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<VineLassoSwing>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 24;
            Item.shootSpeed = 8;
            Item.SetWeaponValues(8, 3);
            Item.SetShopValues(ItemRarityColor.White0, Item.sellPrice(0, 0, 20));
        }

        public override void AddRecipes()
        {
            //CreateRecipe()
            //    .AddIngredient(ItemID.VineRope, 8)
            //    .AddTile(TileID.WorkBenches)
            //    .Register();
        }
    }

    public class VineLassoSwing() : BaseLassoSwing(4)
    {
        public override string Texture => AssetDirectory.FairyCatcherItems + "VineLassoCatcher";

        public override void SetSwingProperty()
        {
            base.SetSwingProperty();
            DrawOriginOffsetX = 8;
        }

        public override Color GetStringColor(Vector2 pos)
        {
            Color c = Color.Green;
            c.A = (byte)(c.A * 0.4f);
            c = Lighting.GetColor((int)pos.X / 16, (int)(pos.Y / 16f), c);
            c *= 0.5f;
            return c;
        }

        public override Vector2 GetStringTipPos(Texture2D cursorTex)
        {
            return CursorCenter - new Vector2(cursorTex.Width / 2, 0).RotatedBy(cursorRotation);
        }
    }
}
