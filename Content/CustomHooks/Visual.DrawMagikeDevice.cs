using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.CustomHooks
{
    public class DrawMagikeDevice : HookGroup
    {
        public static List<MagikeLinerSender> LinerSenders = new(128);

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
                && LinerSenders.Count > 0)
            {
                Main.spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);

                Texture2D laserTex = MagikeSystem.GetConnectLine();
                Color drawColor = Coralite.MagicCrystalPink * 0.6f;
                var origin = new Vector2(0, laserTex.Height / 2);

                foreach (var linerSender in LinerSenders)
                {
                    //以上是获取线性发送器组件
                    Vector2 selfPos = Helper.GetMagikeTileCenter(linerSender.Entity.Position);
                    Vector2 startPos = selfPos - Main.screenPosition;

                    if (linerSender.Receivers.Count == 0)
                        continue;

                    for (int i = 0; i < linerSender.Receivers.Count; i++)
                    {
                        Vector2 aimPos = Helper.GetMagikeTileCenter(linerSender.Receivers[i]);
                        MagikeSystem.DrawConnectLine(Main.spriteBatch, selfPos, aimPos, Main.screenPosition, drawColor);
                    }
                }

                Main.spriteBatch.End();
                LinerSenders.Clear();
            }
        }
    }
}
