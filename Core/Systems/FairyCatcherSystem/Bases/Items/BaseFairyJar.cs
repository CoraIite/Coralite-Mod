using Coralite.Content.Items.FairyCatcher.Accessories;
using Coralite.Content.ModPlayers;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases.Items
{
    public abstract class BaseFairyJar : BaseFairyCatcher
    {
        public enum ChannelSpeeds : byte
        {
            VerySlow,
            Slow,
            Middle,
            Fast
        }

        public abstract ChannelSpeeds ChannelSpeed { get; }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = tooltips.FindIndex(t => t.Mod == "Terraria" && t.Name == "Knockback");
            if (index != -1)
            {
                ChannelSpeeds s = ChannelSpeed;
                if (Main.LocalPlayer.TryGetModPlayer(out CoralitePlayer cp)
                    && cp.HasEffect(nameof(EnergyDrink)))
                {
                    s++;
                    if (s > ChannelSpeeds.Fast)
                        s = ChannelSpeeds.Fast;
                }

                tooltips.Insert(index + 1, new TooltipLine(Mod, "FairyJar", FairySystem.FairyJar.Value));

                string text = s switch
                {
                    ChannelSpeeds.VerySlow => FairySystem.JarChannelSpeedVerySlow.Value,
                    ChannelSpeeds.Slow => FairySystem.JarChannelSpeedSlow.Value,
                    ChannelSpeeds.Middle => FairySystem.JarChannelSpeedMiddle.Value,
                    ChannelSpeeds.Fast => FairySystem.JarChannelSpeedFast.Value,
                    _ => FairySystem.JarChannelSpeedSlow.Value
                };

                tooltips.Insert(index + 2, new TooltipLine(Mod, "FairyJarChannelSpeed", text));
            }
        }
    }
}
