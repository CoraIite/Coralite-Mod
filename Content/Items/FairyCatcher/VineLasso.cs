using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.CursorAIs;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher
{
    public class VineLasso : BaseFairyCatcher
    {
        public override string Texture => AssetDirectory.FairyCatcherItems + Name;

        public override void SetOtherDefaults()
        { 
            Item.shoot = ModContent.ProjectileType<VineLassoCatcher>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 24;
            Item.shootSpeed = 12;
            Item.SetWeaponValues(8, 3);
            Item.SetShopValues(ItemRarityColor.White0, Item.sellPrice(0, 0, 20));
            Item.GetGlobalItem<FairyGlobalItem>().CatchPower = 10;
        }

        public override void ModifyShootFairyStyle(Player player)
        {
            Item.noUseGraphic = false;
            Item.useStyle = ItemUseStyleID.Swing;
        }
    }

    public class VineLassoCatcher : BaseFairyCatcherProj
    {
        public override void SetOtherDefaults()
        {
            CursorWidth = 16;
            CursorHeight = 16;
            cursorMovement = new NormalCursor(3, 0.15f, 0.2f, 0.85f, 20);
            DrawOriginOffsetX = 8;
            DrawOriginOffsetY = -8;
        }

        public override void SetOwnerItemLocation()
        {
            Owner.itemLocation = Owner.Center + new Vector2(Owner.direction * 15, 0);
        }

        public override Color GetStringColor(Vector2 pos)
        {
            Color c = Color.Green;
            c.A = (byte)(c.A * 0.4f);
            c = Lighting.GetColor((int)pos.X / 16, (int)(pos.Y / 16f), c);
            c *= 0.5f;
            return c;
        }

        public override void DrawHandle(Texture2D HandleTex)
        {
            Main.spriteBatch.Draw(HandleTex, Owner.itemLocation - Main.screenPosition, null,
                Lighting.GetColor(Owner.Center.ToTileCoordinates()), 0, HandleTex.Size() / 2, 1f, Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        }

        public override Vector2 GetStringTipPos(Texture2D cursorTex)
        {
            return cursorCenter - new Vector2(cursorScale * cursorTex.Width / 2, 0).RotatedBy(cursorRotation);
        }
    }
}
