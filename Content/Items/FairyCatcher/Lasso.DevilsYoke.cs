using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.CursorAIs;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher
{
    public class DevilsYoke : BaseLassoItem
    {
        public override string Texture => AssetDirectory.FairyCatcherItems + Name;

        public override int LassoProjType => ModContent.ProjectileType<DevilsYokeSwing>();

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<DevilsYokeCatcher>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 22;
            Item.shootSpeed = 9;
            Item.SetWeaponValues(25, 3);
            Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(0, 1));
            Item.GetGlobalItem<FairyGlobalItem>().CatchPower = 30;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            //CreateRecipe()
            //    .AddIngredient(ItemID.VineRope, 8)
            //    .AddTile(TileID.WorkBenches)
            //    .Register();
        }
    }

    public class DevilsYokeSwing() : BaseLassoSwing(4)
    {
        public override string Texture => AssetDirectory.FairyCatcherItems + "DevilsYokeCatcher";

        public override void SetSwingProperty()
        {
            minDistance = 48;
            maxDistance = 128;

            base.SetSwingProperty();
            DrawOriginOffsetX = 8;
        }

        public override Texture2D GetStringTex() => DevilsYokeCatcher.LineTex.Value;
        public override Color GetStringColor(Vector2 pos) => Lighting.GetColor((int)pos.X / 16, (int)(pos.Y / 16f));

        public override Vector2 GetStringTipPos(Texture2D cursorTex)
        {
            return CursorCenter - new Vector2(cursorTex.Width / 2, 0).RotatedBy(cursorRotation);
        }
    }

    public class DevilsYokeCatcher : BaseFairyCatcherProj
    {
        public static Asset<Texture2D> LineTex;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            LineTex = ModContent.Request<Texture2D>(AssetDirectory.FairyCatcherItems + "PurpleChain");
        }

        public override void Unload()
        {
            LineTex = null;
        }

        public override void SetOtherDefaults()
        {
            CursorWidth = 16;
            CursorHeight = 16;
            cursorMovement = new NormalCursor(5, 0.24f, 0.3f, 0.8f, 16);
            DrawOriginOffsetX = 10;
            DrawOriginOffsetY = -18;
        }

        public override void SetOwnerItemLocation()
        {
            Owner.itemLocation = Owner.Center + new Vector2(Owner.direction * 18, -6);
        }

        public override Texture2D GetStringTex() => LineTex.Value;
        public override Color GetStringColor(Vector2 pos) => Lighting.GetColor((int)pos.X / 16, (int)(pos.Y / 16f));

        public override Vector2 GetStringTipPos(Texture2D cursorTex)
        {
            return cursorCenter - new Vector2(cursorScale * cursorTex.Width / 2, 0).RotatedBy(cursorRotation);
        }
    }
}
