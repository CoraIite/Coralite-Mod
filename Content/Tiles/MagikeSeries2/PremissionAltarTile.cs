using Coralite.Content.CoraliteNotes.MagikeInterstitial1;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class PremissionAltarTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public static ATex EXTex;
        public static LocalizedText NeedSouls { get; set; }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            EXTex = ModContent.Request<Texture2D>(Texture + "Extra");
            NeedSouls = this.GetLocalization(nameof(NeedSouls));
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            EXTex = null;
        }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsTileHammeringIfOnTopOfIt[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            MinPick = 110;

            AddMapEntry(new Color(105, 97, 90), CreateMapEntryName());
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.5f;
            g = 0.3f;
            b = 0.55f;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool RightClick(int i, int j)
        {
            if (!CoraliteWorld.HasPermission)
            {
                KnowledgeSystem.CheckForUnlock<MagikeInterstitial1Knowledge>(new Vector2(i, j ) * 16, Coralite.CrystallinePurple);

                if (CoraliteWorld.PlaceNightSoul && CoraliteWorld.PlaceNightSoul)
                {
                    if (!Main.projectile.Any(p => p.active && p.type == ModContent.ProjectileType<PremissionProj>()))
                    {
                        Projectile.NewProjectile(new EntitySource_TileUpdate(i, j)
                            , Helper.GetMagikeTileCenter_NotTopLeft(i, j), Vector2.Zero, ModContent.ProjectileType<PremissionProj>(), 0, 0, Main.myPlayer);

                        Helper.PlayPitched(CoraliteSoundID.IceMagic_Item28, new Vector2(i, j) * 16);
                    }
                }
                else
                    Main.NewText(NeedSouls.Value, Coralite.CrystallinePurple);
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
            if (CoraliteWorld.HasPermission)
            {
                Vector2 offScreen = new(Main.offScreenRange);
                if (Main.drawToScreen)
                    offScreen = Vector2.Zero;

                Point p = new(i, j);
                Tile tile = Main.tile[p.X, p.Y];
                if (tile == null || !tile.HasTile)
                    return;

                Texture2D texture = EXTex.Value;
                Vector2 worldPos = p.ToWorldCoordinates(0, 0);

                Color color = Lighting.GetColor(p.X, p.Y);

                Vector2 drawPos = worldPos + offScreen - Main.screenPosition;

                // 绘制主帖图
                spriteBatch.Draw(texture, drawPos, null, color);
            }
        }

        public override bool CanExplode(int i, int j) => CoraliteWorld.HasPermission;
    }
}
