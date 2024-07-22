using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Coralite.Content.CustomHooks
{
    public class DrawMagikeDevice : HookGroup
    {
        // 应该不会干涉任何东西
        public override SafetyLevel Safety => SafetyLevel.Safe;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            On_Main.DrawDust += DrawMagikeDevices;
        }

        private void DrawMagikeDevices(On_Main.orig_DrawDust orig, Main self)
        {
            orig.Invoke(self);

            if (Main.gameMenu)
                return;

            if (Main.LocalPlayer.TryGetModPlayer(out CoralitePlayer cp) && cp.HasEffect(nameof(MagikeMonoclastic)))
            {
                Main.spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);
                Texture2D laserTex = MagikeSystem.GetConnectLine();
                Color drawColor = Coralite.Instance.MagicCrystalPink * 0.6f;
                var origin = new Vector2(0, laserTex.Height / 2);

                foreach (var entity in TileEntity.ByPosition)
                {
                    if (entity.Value is IMagikeSender_Line sender)
                    {
                        Tile tile = Framing.GetTileSafely(entity.Key);
                        TileObjectData data = TileObjectData.GetTileData(tile);
                        int x = data == null ? 8 : data.Width * 16 / 2;
                        int y = data == null ? 8 : data.Height * 16 / 2;

                        Vector2 selfPos = entity.Value.Position.ToWorldCoordinates(x, y);
                        Vector2 startPos = selfPos - Main.screenPosition;

                        if (Helper.OnScreen(startPos))
                        {
                            for (int i = 0; i < sender.receiverPoints.Length; i++)
                            {
                                if (sender.receiverPoints[i] == Point16.NegativeOne)
                                    continue;
                                Tile tile2 = Framing.GetTileSafely(sender.receiverPoints[i]);
                                TileObjectData data2 = TileObjectData.GetTileData(tile2);
                                int x2 = data2 == null ? 8 : data2.Width * 16 / 2;
                                int y2 = data2 == null ? 8 : data2.Height * 16 / 2;
                                Vector2 aimPos = sender.receiverPoints[i].ToWorldCoordinates(x2, y2);

                                MagikeSystem.DrawConnectLine(Main.spriteBatch, selfPos, aimPos, Main.screenPosition, drawColor);
                            }
                            continue;
                        }

                        for (int i = 0; i < sender.receiverPoints.Length; i++)
                        {
                            if (sender.receiverPoints[i] == Point16.NegativeOne)
                                continue;

                            Tile tile2 = Framing.GetTileSafely(sender.receiverPoints[i]);
                            TileObjectData data2 = TileObjectData.GetTileData(tile2);
                            int x2 = data2 == null ? 8 : data2.Width * 16 / 2;
                            int y2 = data2 == null ? 8 : data2.Height * 16 / 2;
                            Vector2 aimPos = sender.receiverPoints[i].ToWorldCoordinates(x2, y2);

                            if (!Helper.OnScreen(aimPos - Main.screenPosition))
                                continue;

                            MagikeSystem.DrawConnectLine(Main.spriteBatch, selfPos, aimPos, Main.screenPosition, drawColor);
                        }
                    }
                }

                Main.spriteBatch.End();
            }
        }
    }
}
