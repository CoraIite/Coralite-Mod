using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.CursorAIs;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
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
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Rapier;
        }

        public override void ShootFairy(IFairyBottle bottle, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            Item[] fairies = bottle.Fairies;
            bool shooted = false;
            for (int i = 0; i < fairies.Length; i++)
            {
                currentFairyIndex++;
                if (currentFairyIndex > fairies.Length - 1)
                    currentFairyIndex = 0;

                if (bottle.CanShootFairy(currentFairyIndex, out IFairyItem fairyItem))
                {
                    shooted = true;
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<VineLassoSwing>(),
                        damage, knockback, player.whoAmI, fairies[currentFairyIndex].type);
                    break;
                }
            }

            if (!shooted)
            {
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<VineLassoSwing>(),
                    damage, knockback, player.whoAmI);
            }
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

        public override void SetDefs()
        {
            base.SetDefs();
            DrawOriginOffsetX = 8;
            DrawOriginOffsetY = -8;
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

        public override Vector2 GetStringTipPos(Texture2D cursorTex)
        {
            return cursorCenter - new Vector2(cursorScale * cursorTex.Width / 2, 0).RotatedBy(cursorRotation);
        }
    }
}
