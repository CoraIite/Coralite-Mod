using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleBreath : BaseMaterial
    {
        public IcicleBreath() : base(Item.CommonMaxStack, Item.sellPrice(0,0,30), ItemRarityID.Green, AssetDirectory.IcicleItems)
        {
        }

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(8, 3, true));
            ItemID.Sets.ItemNoGravity[Type] = true;
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;

            Vector2 frameOrigin = frame.Size() / 2f;
            Vector2 drawPos = position  ;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
                time = 2f - time;

            time = time * 0.5f + 0.5f;

            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(90, 70, 255, 50), 0, frameOrigin, scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 2f).RotatedBy(radians) * time, frame, new Color(140, 120, 255, 77), 0, frameOrigin, scale, SpriteEffects.None, 0);
            }

            return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Rectangle frame;

            if (Main.itemAnimations[Item.type] != null)
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            else
                frame = texture.Frame();

            Vector2 frameOrigin = frame.Size() / 2f;
            Vector2 offset = new Vector2(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
            Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
                time = 2f - time;

            time = time * 0.5f + 0.5f;

            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(90, 70, 255, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(140, 120, 255, 77), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }
            return base.PreDrawInWorld(spriteBatch, lightColor, Color.White, ref rotation, ref scale, whoAmI);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.ReplaceResult(ItemID.VoidLens);
            recipe.AddIngredient(ItemID.Bone, 30);
            recipe.AddIngredient(ItemID.JungleSpores, 15);
            recipe.AddIngredient<IcicleBreath>(10);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();

            recipe = CreateRecipe();
            recipe.ReplaceResult(ItemID.VoidVault);
            recipe.AddIngredient(ItemID.Bone, 15);
            recipe.AddIngredient(ItemID.JungleSpores, 8);
            recipe.AddIngredient<IcicleBreath>(5);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();

            recipe = CreateRecipe();
            recipe.ReplaceResult(ItemID.ObsidianShirt);
            recipe.AddIngredient(ItemID.Silk, 10);
            recipe.AddIngredient(ItemID.Obsidian, 20);
            recipe.AddIngredient<IcicleBreath>(6);
            recipe.AddTile(TileID.Hellforge);
            recipe.Register();
            recipe = CreateRecipe();
            recipe.ReplaceResult(ItemID.ObsidianHelm);
            recipe.AddIngredient(ItemID.Silk, 10);
            recipe.AddIngredient(ItemID.Obsidian, 20);
            recipe.AddIngredient<IcicleBreath>(3);
            recipe.AddTile(TileID.Hellforge);
            recipe.Register();
            recipe = CreateRecipe();
            recipe.ReplaceResult(ItemID.ObsidianPants);
            recipe.AddIngredient(ItemID.Silk, 10);
            recipe.AddIngredient(ItemID.Obsidian, 20);
            recipe.AddIngredient<IcicleBreath>(3);
            recipe.AddTile(TileID.Hellforge);
            recipe.Register();

            recipe = CreateRecipe();
            recipe.ReplaceResult(ItemID.NightmarePickaxe);
            recipe.AddIngredient(ItemID.DemoniteBar, 12);
            recipe.AddIngredient<IcicleBreath>(4);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
