using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Content.CustomHooks
{
    public class DrawMagikeDevice : HookGroup
    {
        public static List<Point16> Points = new(128);

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

            if (Main.LocalPlayer.TryGetModPlayer(out CoralitePlayer cp) && cp.HasEffect(nameof(MagikeMonoclastic))
                && Points.Count > 0)
            {
                Main.spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);

                Texture2D laserTex = MagikeSystem.GetConnectLine();
                Color drawColor = Coralite.MagicCrystalPink * 0.6f;
                var origin = new Vector2(0, laserTex.Height / 2);

                foreach (var point in Points)
                {
                    MagikeTileEntity sender = TileEntity.ByPosition[point] as MagikeTileEntity;

                    if (!sender .HasComponent(MagikeComponentID.MagikeSender))
                        continue;

                    Component senderComponent = (Component)sender.Components[MagikeComponentID.MagikeSender];

                    if (senderComponent is not MagikeLinerSender linerSender)
                        continue;

                    //以上是获取线性发送器组件

                    Vector2 selfPos = Helper.GetMagikeTileCenter(point);
                    Vector2 startPos = selfPos - Main.screenPosition;

                    for (int i = 0; i < linerSender.Receivers.Count; i++)
                    {
                        Vector2 aimPos = Helper.GetMagikeTileCenter(linerSender.Receivers[i]);

                        MagikeSystem.DrawConnectLine(Main.spriteBatch, selfPos, aimPos, Main.screenPosition, drawColor);
                    }
                }

                Main.spriteBatch.End();

                Points.Clear();
            }
        }
    }
}
