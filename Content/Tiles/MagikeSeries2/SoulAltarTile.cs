using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class SoulOfLightAltarTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;
        public static LocalizedText NeedSouls { get; set; }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            NeedSouls = this.GetLocalization(nameof(NeedSouls));
        }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;

            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            MinPick = 110;

            AddMapEntry(new Color(105, 97, 90), CreateMapEntryName());
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool RightClick(int i, int j)
        {
            if (!CoraliteWorld.PlaceLightSoul)
            {
                if (Main.LocalPlayer.ConsumeItem(ItemID.SoulofLight, includeVoidBag: true))
                    CoraliteWorld.PlaceLightSoul = true;
                else
                    CombatText.NewText(new Rectangle(i * 16, j * 16, 1, 1), Coralite.CrystallineMagikePurple, NeedSouls.Value);
            }

            return true;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (drawData.tileFrameX == 0 && drawData.tileFrameY == 0)
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (CoraliteWorld.PlaceLightSoul)
            {
                //绘制光明之魂
                Vector2 offScreen = new(Main.offScreenRange);
                if (Main.drawToScreen)
                    offScreen = Vector2.Zero;

                Point p = new(i, j);
                Tile tile = Main.tile[p.X, p.Y];
                if (tile == null || !tile.HasTile)
                    return;

                Texture2D texture = TextureAssets.Item[ItemID.SoulofLight].Value;
                Vector2 worldPos = p.ToWorldCoordinates();
                Main.instance.LoadItem(ItemID.SoulofLight);
                Main.itemAnimations[ItemID.SoulofLight].Update();
                Color color = Lighting.GetColor(p.X, p.Y);

                Vector2 drawPos = worldPos + offScreen - Main.screenPosition;
                // 绘制主帖图
                Rectangle sourceRectangle = texture.Frame(1, 4, 0, ((int)(Main.timeForVisualEffects % 24) / 6));
                spriteBatch.Draw(texture, drawPos, sourceRectangle, color * 0.6f);
                color.A = 0;
                spriteBatch.Draw(texture, drawPos, sourceRectangle, color * 0.5f);
            }
        }

        public override bool CanExplode(int i, int j) => false;
    }

    public class SoulOfNightAltarTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;
        public static LocalizedText NeedSouls { get; set; }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            NeedSouls = this.GetLocalization(nameof(NeedSouls));
        }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;

            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            MinPick = 110;

            AddMapEntry(new Color(105, 97, 90), CreateMapEntryName());
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool RightClick(int i, int j)
        {
            if (!CoraliteWorld.PlaceNightSoul)
            {
                if (Main.LocalPlayer.ConsumeItem(ItemID.SoulofNight, includeVoidBag: true))
                    CoraliteWorld.PlaceNightSoul = true;
                else
                    CombatText.NewText(new Rectangle(i * 16, j * 16, 1, 1), Coralite.CrystallineMagikePurple, NeedSouls.Value);
            }

            return true;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (drawData.tileFrameX == 0 && drawData.tileFrameY == 0)
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (CoraliteWorld.PlaceNightSoul)
            {
                //绘制暗影之魂
                Vector2 offScreen = new(Main.offScreenRange);
                if (Main.drawToScreen)
                    offScreen = Vector2.Zero;

                Point p = new(i, j);
                Tile tile = Main.tile[p.X, p.Y];
                if (tile == null || !tile.HasTile)
                    return;

                Texture2D texture = TextureAssets.Item[ItemID.SoulofNight].Value;
                Rectangle sourceRectangle = texture.Frame(1, 4, 0, ((int)(Main.timeForVisualEffects % 24) / 6));

                Vector2 worldPos = p.ToWorldCoordinates(2, 6);
                Color color = Lighting.GetColor(p.X, p.Y);

                Vector2 drawPos = worldPos + offScreen - Main.screenPosition;

                // 绘制主帖图
                spriteBatch.Draw(texture, drawPos, sourceRectangle, color * 0.6f);
                color.A = 0;
                spriteBatch.Draw(texture, drawPos, sourceRectangle, color * 0.5f);
            }
        }

        public override bool CanExplode(int i, int j) => false;
    }
}
