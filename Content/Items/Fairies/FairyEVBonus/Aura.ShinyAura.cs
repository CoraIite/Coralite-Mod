using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.Fairies.FairyEVBonus
{
    public class ShinyAura() : BaseMaterial(Item.CommonMaxStack, Item.sellPrice(0, 0, 10)
        , ItemRarityID.Yellow, AssetDirectory.FairyEVBonus)
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Type] = true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D tex = TextureAssets.Item[Item.type].Value;

            float rot = Main.GlobalTimeWrappedHourly * 2;
            for (int i = 0; i < 6; i++)
            {
                Vector2 pos = position + rot.ToRotationVector2() * 2;

                Color c = Main.hslToRgb(new Vector3((Main.GlobalTimeWrappedHourly + i / 6f) % 1, 0.8f, 0.8f)) * 0.15f;
                c.A = 0;
                tex.QuickCenteredDraw(spriteBatch, pos, c, 0, scale);

                rot += MathHelper.TwoPi / 6;
            }

            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = TextureAssets.Item[Item.type].Value;

            Vector2 pos2 = Item.Center - Main.screenPosition;
            float rot = Main.GlobalTimeWrappedHourly * 2;
            for (int i = 0; i < 6; i++)
            {
                Vector2 pos = pos2 + rot.ToRotationVector2() * 2;

                Color c = Main.hslToRgb(new Vector3((Main.GlobalTimeWrappedHourly + i / 6f) % 1, 0.8f, 0.8f)) * 0.15f;
                c.A = 0;
                tex.QuickCenteredDraw(spriteBatch, pos, c, rotation, scale);

                rot += MathHelper.TwoPi / 6;
            }

            return true;
        }

    }
}
