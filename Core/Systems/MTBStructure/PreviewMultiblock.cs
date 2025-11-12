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
        public override void CheckStructure(Point16 origin)
        {
            foreach (var p in Main.projectile.Where(p => p.active && p.friendly && p.type == ModContent.ProjectileType<PreviewMultiblockPeoj>() && p.ai[0] == Type))
                p.Kill();

            Projectile.NewProjectile(new EntitySource_TileUpdate(origin.X, origin.Y), origin.ToWorldCoordinates(0, 0), Vector2.Zero
                , ModContent.ProjectileType<PreviewMultiblockPeoj>(), 0, 0, Main.myPlayer, Type, origin.X, origin.Y);

            base.CheckStructure(origin);
        }

        public override void OnSuccess(Point16 origin)
        {
            foreach (var p in Main.projectile.Where(p => p.active && p.friendly && p.type == ModContent.ProjectileType<PreviewMultiblockPeoj>() && p.ai[0] == Type))
                p.Kill();
        }
    }

    public class PreviewMultiblockPeoj : ModProjectile
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "White32x32";

        public ref float MTBID => ref Projectile.ai[0];

        public Point16 Origin => new Point16((int)Projectile.ai[1], (int)Projectile.ai[2]);

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
            Multiblock m = MultiblockLoader.GetMTBStructure((int)MTBID);

            Point16 origin = Origin;
            foreach (var info in m.Infos)
            {
                Point16 p = origin + info.point;
                Tile tile = Framing.GetTileSafely(p);

                if (info.checkTile)
                    DrawTile(info, p, tile);
                else if (info.wallType> WallID.None)
                    DrawWall(info, p, tile);
                else if (info.liquidAmount>0)
                {
                    //TODO: 添加液体的可视化
                }
            }

            return false;
        }

        private void DrawWall(Multiblock.MultiBlockInfo info, Point16 p, Tile tile)
        {
            if (MultiblockSystem.wallTypeToItemType == null
                || !MultiblockSystem.wallTypeToItemType.TryGetValue(tile.WallType, out int itemType))
                return;

            if (tile.WallType > WallID.None)
            {
                if (tile.WallType != info.wallType)
                {
                    Texture2D tex = Projectile.GetTexture();
                    Vector2 pos = p.ToWorldCoordinates() - Main.screenPosition;

                    Main.spriteBatch.Draw(tex, pos, null, Color.Red * 0.5f
                        , 0, tex.Size() / 2, 0.5f, 0, 0);

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

        private void DrawTile(Multiblock.MultiBlockInfo info, Point16 p, Tile tile)
        {
            int type = info.tileType;
            if (tile.HasTile)
            {
                if (tile.TileType != type)
                {
                    Texture2D tex = Projectile.GetTexture();
                    Vector2 pos = p.ToWorldCoordinates() - Main.screenPosition;

                    tex.QuickCenteredDraw(Main.spriteBatch, pos, Color.Red * 0.5f,scale: 0.5f);

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
    }
}
