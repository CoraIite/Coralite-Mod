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
                    if (entity.Value is not BaseMagikeTileEntity sender)
                        continue;

                    if (!(sender as IEntity).HasComponent(MagikeComponentID.MagikeSender))
                        continue;

                    List<Component> senderComponents = sender.Components[MagikeComponentID.MagikeSender];

                    for (int k = 0; k < senderComponents.Count; k++)
                    {
                        Component c = senderComponents[k];
                        if (c is not MagikeLinerSender linerSender) 
                            continue;

                        Vector2 selfPos = Helper.GetTileCenter(entity.Key);
                        Vector2 startPos = selfPos - Main.screenPosition;

                        if (Helper.OnScreen(startPos))
                        {
                            for (int i = 0; i < linerSender.Receivers.Count; i++)
                            {
                                Vector2 aimPos = Helper.GetTileCenter(linerSender.Receivers[i]);

                                MagikeSystem.DrawConnectLine(Main.spriteBatch, selfPos, aimPos, Main.screenPosition, drawColor);
                            }

                            continue;
                        }

                        for (int i = 0; i < linerSender.Receivers.Count; i++)
                        {
                            Vector2 aimPos = Helper.GetTileCenter(linerSender.Receivers[i]);

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
