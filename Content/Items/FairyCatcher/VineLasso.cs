using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.CursorAIs;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher
{
    public class VineLasso : BaseFairyCatcher
    {
        public override string Texture => AssetDirectory.FairyCatcherItems+Name;

        public override void SetOtherDefaults()
        {
            Item.damage = 20;
            Item.shoot = ModContent.ProjectileType<VineLassoCatcher>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 24;
            Item.shootSpeed = 12;
        }
    }

    public class VineLassoCatcher : BaseFairyCatcherProj
    {
        public override void SetOtherDefaults()
        {
            CursorWidth = 16;
            CursorHeight = 16;
            cursorMovement = new NormalCursor(3, 0.15f, 0.2f, 0.85f, 20);
        }

        public override void SetOwnerItemLocation()
        {
            Owner.itemLocation = Owner.Center+new Vector2(Owner.direction * 15, 0);
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
            return cursorCenter - new Vector2(cursorScale * cursorTex.Width / 2, 0).RotatedBy(cursorRotation);
        }
    }
}
