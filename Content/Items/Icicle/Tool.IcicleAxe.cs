using Coralite.Content.CustomHooks;
using Coralite.Content.Items.GlobalItems;
using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleAxe : ModItem, ISpecialPickaxe
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public bool ModifyPickaxeDamage(ref int damage, Player player, int x, int y, int pickPower, int hitBufferIndex, Tile tileTarget)
        {
            if (TileID.Sets.Snow[tileTarget.TileType] || TileID.Sets.Ices[tileTarget.TileType] || TileID.Sets.IcesSnow[tileTarget.TileType]
                || TileID.Sets.Conversion.Ice[tileTarget.TileType] || TileID.Sets.Conversion.Snow[tileTarget.TileType])
            {
                damage = 100;
                return true;
            }

            return false;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.DeathbringerPickaxe);
            CoraliteGlobalItem.SetColdDamage(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleCrystal>(2)
                .AddIngredient<IcicleScale>()
                .AddTile(TileID.IceMachine)
                .Register();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            int index = Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, DustID.FrostStaff);
            Main.dust[index].noGravity = true;
        }
    }
}
