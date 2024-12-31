using Coralite.Core.Loaders;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Core.Systems.MTBStructure
{
    public abstract class PreviewMultiblock : Multiblock
    {
        public override void CheckStructure(Point origin)
        {
            foreach (var p in Main.projectile.Where(p => p.active && p.friendly && p.type == ModContent.ProjectileType<PreviewMultiblockPeoj>() && p.ai[0]==Type))
                p.Kill();

            Projectile.NewProjectile(new EntitySource_TileUpdate(origin.X, origin.Y), origin.ToWorldCoordinates(0, 0), Vector2.Zero
                , ModContent.ProjectileType<PreviewMultiblockPeoj>(), 0, 0, Main.myPlayer, Type, origin.X, origin.Y);

            base.CheckStructure(origin);
        }

        public override void OnSuccess(Point origin)
        {
            foreach (var p in Main.projectile.Where(p => p.active && p.friendly && p.type == ModContent.ProjectileType<PreviewMultiblockPeoj>() && p.ai[0] == Type))
                p.Kill();
        }
    }

    public class PreviewMultiblockPeoj : ModProjectile
    {
        public override string Texture => AssetDirectory.OtherProjectiles+ "White32x32";

        public ref float MTBID => ref Projectile.ai[0];

        public Point Origin => new Point((int)Projectile.ai[1], (int)Projectile.ai[2]);

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.timeLeft = 60 * 60 * 10;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
            => false;

        public override bool? CanDamage()
            => false;

        public override bool ShouldUpdatePosition()
            => false;


        public override bool PreDraw(ref Color lightColor)
        {
            int[,] structureTile = MultiblockLoader.GetMTBStructure((int)MTBID).StructureTile;
            int width = structureTile.GetLength(1);
            int height = structureTile.GetLength(0);

            Point topleft = Origin - new Point(width / 2, height / 2);

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    Point p = new Point(topleft.X + j, topleft.Y + i);
                    int type = structureTile[i, j];

                    if (type == -1)
                        continue;

                    Tile tile = Framing.GetTileSafely(p);

                    if (tile.HasTile)
                    {
                        if (tile.TileType != type)
                        {
                            Texture2D tex = Projectile.GetTexture();
                            Vector2 pos = p.ToWorldCoordinates() - Main.screenPosition;

                            Main.spriteBatch.Draw(tex, pos, null, Color.Red * 0.5f
                                , 0, tex.Size() / 2, 0.5f, 0, 0);

                            int itemType = TileLoader.GetItemDropFromTypeAndStyle(type);

                            Main.instance.LoadItem(itemType);
                            Texture2D itemTex = TextureAssets.Item[itemType].Value;

                            float colorA = 0.5f;
                            float scale = 0.9f;
                            if (Helper.MouseScreenInRect(new Rectangle((int)pos.X - 8, (int)pos.Y - 8, 16, 16)))
                            {
                                colorA = 1f;
                                scale = 1.2f;

                                Utils.DrawBorderString(Main.spriteBatch, ContentSamples.ItemsByType[itemType].Name, pos - new Vector2(0, 16)
                                    , Color.White, anchorx: 0.5f, anchory: 0.5f);
                            }

                            Main.spriteBatch.Draw(itemTex, pos, null, Color.White * colorA, 0, itemTex.Size() / 2, scale, 0, 0);
                        }
                    }
                    else
                    {
                        int itemType = TileLoader.GetItemDropFromTypeAndStyle(type);

                        Main.instance.LoadItem(itemType);
                        Texture2D itemTex = TextureAssets.Item[itemType].Value;

                        Vector2 pos = p.ToWorldCoordinates() - Main.screenPosition;
                        float colorA = 0.5f;
                        float scale = 0.9f;
                        if (Helper.MouseScreenInRect(new Rectangle((int)pos.X - 8, (int)pos.Y - 8, 16, 16)))
                        {
                            colorA = 1f;
                            scale = 1.2f;

                            Utils.DrawBorderString(Main.spriteBatch, ContentSamples.ItemsByType[itemType].Name, pos - new Vector2(0, 16)
                                , Color.White, anchorx: 0.5f, anchory: 0.5f);
                        }

                        Main.spriteBatch.Draw(itemTex, pos, null, Color.White * colorA, 0, itemTex.Size() / 2, scale, 0, 0);
                    }
                }

            return false;
        }
    }
}
